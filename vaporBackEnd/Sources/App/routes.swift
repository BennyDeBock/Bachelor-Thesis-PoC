import Vapor

func routes(_ app: Application) async throws {
    app.get { req in
        return "It works!"
    }

    try await app.register(collection: PlayerController(eventloop: app.eventLoopGroup.next(), logger: app.logger))
}
