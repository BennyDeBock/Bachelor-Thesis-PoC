import Foundation
import Vapor

struct PlayerController: RouteCollection {
    let eventloop: EventLoop
    let logger: Logger
    let service: PlayerService
    
    init(eventloop: EventLoop, logger: Logger) async throws {
        self.eventloop = eventloop
        self.logger = logger
        
        self.service = try await PlayerService(eventloop: self.eventloop, logger: self.logger)
    }
    
    func boot(routes: RoutesBuilder) throws {
        let players = routes.grouped("Player")
        players.get(use: getAll)
        players.get("NoCountry", use: getAllWithoutCountry)
        players.get("Id", use: getPlayer)
        players.get("Percentage", use: getPercentage)
        players.put(use: update)
        players.post(use: create)
        players.delete(use: delete)
    }
    
    func getAll(req: Request) async throws -> [Player] {
        return try await service.getAllPlayers()
    }
    
    func getAllWithoutCountry(req: Request) async throws -> [Player] {
        return try await service.getAllPlayersWithoutCountries()
    }
    
    func getPlayer(req: Request) async throws -> Player {
        let id = try req.query.decode(PlayerDTO.GetById.self)
        print(id)
        return try await service.getPlayerById(id: id.id!)
    }
    
    func getPercentage(req: Request) async throws -> PlayerDictionary {
        let dict = try await service.getPercentagePerCountry()
        return PlayerDictionary(dict: dict)
    }
    
    func create(req: Request) async throws -> Player {
        let newPlayer = try req.content.decode(PlayerDTO.Create.self)
                
        return try await service.insertPlayer(player: newPlayer)
    }
    
    func update(req: Request) async throws -> HTTPStatus {
        do {
            let playerToUpdate = try req.content.decode(PlayerDTO.Update.self)
            
            let success = try await service.updatePlayer(player: playerToUpdate)
            if !success {
                return .notFound
            }
            
            return .ok
        } catch let error {
            print(error)
            return .badRequest
        }
    }
    
    func delete(req: Request) async throws -> HTTPStatus {
        do {
            let delete = try req.content.decode(PlayerDTO.Delete.self)
            try await service.deletePlayer(id: delete.Id)
            return .ok
        } catch let error {
            print(error)
            return .badRequest
        }
    }
}
