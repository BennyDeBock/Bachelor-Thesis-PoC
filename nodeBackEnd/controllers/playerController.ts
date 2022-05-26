import { PrismaClient } from '@prisma/client'
import { Request, Response } from 'express'
import * as service from '../services/playerService'
import { PlayerDTOCreate, PlayerDTOUpdate } from "../Models/playerDTO";

module.exports.getPlayersAsync = async (req:any, res:any) => {
    try {
        let players = await service.getAllPlayers()
        res.send(players)
    } catch {
        res.status(400).send()
    }
}

module.exports.getPlayersWithoutCountryAsync = async (req:any, res:any) => {
    try {
        let players = await service.getAllPlayersWithoutCountry()
        res.send(players)
    } catch {
        res.status(400).send()
    }
}

module.exports.getPlayerByIdAsync = async (req:Request<{}, {}, {}>, res:Response) => {
    try {
        const id = req.query.id as string
        const result = await service.getPlayerById(id)
        if (result === null)
        {
            throw("Something went wrong")
        }
        res.send(result)
    } catch {
        res.status(400).send()
    }
}

module.exports.getPercentageByCountryAsync = async (req:any, res:any) => {
    try {
        let percentages = await service.getPercentageByCountry()
        res.send(Array.from(percentages))
    } catch {
        res.status(400).send()
    }
}

module.exports.createPlayerAsync = async (req:any, res:any) => {
    try {
        let newPlayer: PlayerDTOCreate = {
            playerId: req.body.PlayerID,
            name: req.body.Name,
            gender: req.body.Gender,
            active: req.body.Active,
            birthyear: req.body.Birthyear,
            playHand: req.body.PlayHand,
            playStyle: req.body.PlayStyle,
            grip: req.body.Grip,
            countryCode: req.body.CountryCode
        }        
        let player = await service.createPlayer(newPlayer)
        res.send(player)
    } catch {
        res.status(400).send()
    }
}

module.exports.updatePlayerAsync = async (req:any, res:any) => {
    try {
        let updatePlayer: PlayerDTOUpdate = {
            id: req.body.Id,
            name: req.body.Name,
            active: req.body.active
        }
        let success = await service.updatePlayer(updatePlayer)
        res.send(success)
    } catch {
        res.status(400).send(false)
    }
}

module.exports.deletePlayerAsync = async (req:any, res:any) => {
    try {
        const result = service.deletePlayer(req.body.Id)

        res.json(result)
    } catch (err) {
        res.status(400).send(err)
    }
}
