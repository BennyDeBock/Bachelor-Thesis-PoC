//
//  File.swift
//  
//
//  Created by Benny De Bock on 26/05/2022.
//

import Foundation
import Vapor

struct PlayerDTO {
    struct GetById: Content {
        let id: UUID?
    }
    struct Delete: Content {
        let Id: UUID
    }
    
    struct Create: Content {
        let PlayerID: Int
        let Name: String
        let Gender: Bool
        let Active: Bool
        let Birthyear: Int?
        let PlayHand: String?
        let PlayStyle: String?
        let Grip: String?
        let CountryCode: String
    }
    
    struct Update: Content {
        let Id: UUID
        let Name: String
        let Active: Bool
    }
}
