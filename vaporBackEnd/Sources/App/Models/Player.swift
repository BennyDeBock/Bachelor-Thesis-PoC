//
//  File.swift
//  
//
//  Created by Benny De Bock on 18/05/2022.
//

import Foundation
import Vapor

struct PlayerDB: Content {
    
}

struct Player: Content {
    let Id: UUID
    let PlayerID: Int
    let Name: String
    let Gender: Bool
    let Active: Bool
    let Birthyear: Int?
    let PlayHand: String?
    let PlayStyle: String?
    let Grip: String?
    let Country: Country?
}

struct Country: Content {
    let Id: UUID
    let Code: String
}

struct PlayerDictionary: Content {
    let dict: [String: Double]
}
