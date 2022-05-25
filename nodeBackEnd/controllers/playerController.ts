import { Request, Response } from 'express'
import * as service from '../services/playerService'

module.exports.getPlayersAsync = async (req:any, res:any) => {
    try {
        service.getAllPlayers()
    } catch {
        res.status(400).send()
    }
}

module.exports.getPlayersWithoutCountryAsync = async (req:any, res:any) => {
    console.log(req)
    console.log(res)
}

module.exports.getPlayerByIdAsync = async (req:Request<{}, {}, {}>, res:Response) => {
    try {
        console.log(req.query.id)
        const query  = req.query.id as string
        const result = service.getPlayerById(query)

        res.json(result)
    } catch {
        res.status(400).send()
    }
}

module.exports.getPercentageByCountryAsync = async (req:any, res:any) => {
    console.log(req)
    console.log(res)
}

module.exports.createPlayerAsync = async (req:any, res:any) => {
    console.log(req)
    console.log(res)
}

module.exports.updatePlayerAsync = async (req:any, res:any) => {
    console.log(req)
    console.log(res)
}

module.exports.deletePlayerAsync = async (req:any, res:any) => {
    console.log(req)
    console.log(res)
}
