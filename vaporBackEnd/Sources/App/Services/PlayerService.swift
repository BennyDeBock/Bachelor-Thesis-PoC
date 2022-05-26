//
//  File.swift
//  
//
//  Created by Benny De Bock on 25/05/2022.
//

import Foundation
import PostgresNIO

struct PlayerService {
    let eventLoop: EventLoop
    let logger: Logger
    let config = PostgresConnection.Configuration(
        connection: .init(
            host: "bachelorproef.cot79hcuq61g.eu-west-2.rds.amazonaws.com",
            port: 5432
        ), authentication: .init(
            username: "benny",
            database: "bachelorproef",
            password: "school123"
        ),
        tls: .disable
    )
    
    let connection: PostgresConnection
    
    init(eventloop: EventLoop, logger: Logger) async throws {
        self.logger = logger
        self.eventLoop = eventloop
        connection = try await PostgresConnection.connect(
            on: self.eventLoop.next(),
            configuration: config,
            id: 1,
            logger: self.logger
        )
    }
    
    func getAllPlayers() async throws -> [Player] {
        var players: [Player] = []
        let rows = try await connection.query(
            """
                SELECT p."Id", "PlayerID", "Name", "Gender", "Active", "Birthyear", "PlayHand", "PlayStyle", "Grip", c."Id", "Code"
                FROM "Player" as p
                JOIN "Country" as c ON p."CountryId" = c."Id"
            """, logger: self.logger
        )
        
        for try await (Id, PlayerID, Name, Gender, Active, Birthyear,PlayHand, PlayStyle, Grip, CountryId, Code) in rows.decode((UUID, Int, String, Bool, Bool, Int?, String?, String?, String?, UUID, String).self, context: .default) {
            let Country = Country(Id: CountryId, Code: Code)
            players.append(
                Player(
                    Id: Id,
                    PlayerID: PlayerID,
                    Name: Name,
                    Gender: Gender,
                    Active: Active,
                    Birthyear: Birthyear,
                    PlayHand: PlayHand,
                    PlayStyle: PlayStyle,
                    Grip: Grip,
                    Country: Country))
        }
        
        return players
        
    }
    
    func getAllPlayersWithoutCountries() async throws -> [Player] {
        var players: [Player] = []
        let rows = try await connection.query(
            """
                SELECT "Id", "PlayerID", "Name", "Gender", "Active", "Birthyear", "PlayHand", "PlayStyle", "Grip"
                FROM "Player"
            """, logger: self.logger
        )
        
        for try await (Id, PlayerID, Name, Gender, Active, Birthyear,PlayHand, PlayStyle, Grip) in rows.decode((UUID, Int, String, Bool, Bool, Int?, String?, String?, String?).self, context: .default) {
            players.append(
                Player(
                    Id: Id,
                    PlayerID: PlayerID,
                    Name: Name,
                    Gender: Gender,
                    Active: Active,
                    Birthyear: Birthyear,
                    PlayHand: PlayHand,
                    PlayStyle: PlayStyle,
                    Grip: Grip,
                    Country: nil))
        }
        
        return players
    }
    
    func getPlayerById(id: UUID) async throws -> Player {
        var player: Player = Player(Id: UUID(), PlayerID: 0, Name: "", Gender: false, Active: false, Birthyear: 1999, PlayHand: nil, PlayStyle: nil, Grip: nil, Country: nil)
        
        print(id.uuidString.lowercased())
        let rows = try await connection.query(
            """
                SELECT p."Id", "PlayerID", "Name", "Gender", "Active", "Birthyear", "PlayHand", "PlayStyle", "Grip", c."Id", "Code"
                FROM "Player" as p
                JOIN "Country" as c ON p."CountryId" = c."Id"
                WHERE p."Id"::text='\(unescaped: id.uuidString.lowercased())';
            """, logger: self.logger
        )
        
        for try await (Id, PlayerID, Name, Gender, Active, Birthyear,PlayHand, PlayStyle, Grip, CountryId, Code) in rows.decode((UUID, Int, String, Bool, Bool, Int?, String?, String?, String?, UUID, String).self, context: .default) {
            let Country = Country(Id: CountryId, Code: Code)
            player = Player(
                Id: Id,
                PlayerID: PlayerID,
                Name: Name,
                Gender: Gender,
                Active: Active,
                Birthyear: Birthyear,
                PlayHand: PlayHand,
                PlayStyle: PlayStyle,
                Grip: Grip,
                Country: Country)
        }
        
        return player

    }
    
    func getPercentagePerCountry() async throws -> [String: Double] {
        var dictionary: [String: Double] = [:]
        
        let rows = try await connection.query(
            """
                SELECT "Code", COUNT(*)
                FROM "Player" as p
                JOIN "Country" as c ON p."CountryId" = c."Id"
                GROUP BY "Code"
                ORDER BY COUNT(*) DESC
            """, logger: logger
        )
        
        let amountOfRows = try await getTotalAmountOfPlayers()
        
        for try await (Code, count) in rows.decode((String, Int).self, context: .default) {
            dictionary["\(Code)"] = Double(count) / Double(amountOfRows) * 100
        }
        
        return dictionary
    }
    
    private func getTotalAmountOfPlayers() async throws -> Int {
        let count = try await connection.query(
            """
                SELECT Count(*)
                FROM "Player";
            """, logger: logger)
        
        var amountOfRows = 0
        for try await (amount) in count.decode((Int).self, context: .default) {
            amountOfRows = amount
        }
        
        return amountOfRows
    }
    
    func deletePlayer(id: UUID) async throws {
        try await connection.query(
            """
                DELETE FROM "Player"
                WHERE "Id"::text='\(id)';
            """, logger: logger)
    }
    
    func insertPlayer(player: PlayerDTO.Create) async throws -> Player {
        let rows = try await connection.query(
            """
                SELECT "Id"
                FROM "Country"
                WHERE "Code"::text = '\(unescaped: player.CountryCode)'
            """, logger: logger)
        var CountryId: UUID = UUID()
        
        for try await (Id) in rows.decode((UUID).self, context: .default) {
            CountryId = Id
        }
        
        let newId = UUID()
        try await connection.query(
            """
                INSERT INTO "Player"("Id", "PlayerID", "Name", "Gender", "Active", "Birthyear", "PlayHand", "PlayStyle", "Grip", "CountryId")
                VALUES ('\(unescaped: newId.uuidString.lowercased())', \(player.PlayerID), '\(unescaped: player.Name)', \(player.Gender), \(player.Active), \(player.Birthyear), \(player.PlayHand), \(player.PlayStyle), \(player.Grip), '\(unescaped: CountryId.uuidString.lowercased())');
            """, logger: logger
        )
        
        return try await getPlayerById(id: newId)
    }
    
    func updatePlayer(player: PlayerDTO.Update) async throws -> Bool {
        let playerExists = try await self.getPlayerById(id: player.Id)
        
        if(playerExists.Name == "") {
            return false
        }
        
        try await connection.query(
            """
                UPDATE "Player"
                SET "Name"='\(unescaped: player.Name)', "Active"=\(player.Active)
                WHERE "Id"='\(unescaped: player.Id.uuidString.lowercased())'
            """, logger: logger)
        return true
    }
}
