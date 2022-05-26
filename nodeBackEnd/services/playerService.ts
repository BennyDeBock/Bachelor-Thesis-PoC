import { Prisma, PrismaClient } from "@prisma/client";
import { Player } from "../Models/player";
import { v4 as uuidv4 } from "uuid";
import { info } from "console";
import { PlayerDTOCreate, PlayerDTOUpdate } from "../Models/playerDTO";



export const getAllPlayers = async () => { 
    const prisma = new PrismaClient()
    const players =  await prisma.player.findMany({
        include: {
            Country: true
        },
    })

    return players
}

export const getAllPlayersWithoutCountry = async () => {
    const prisma = new PrismaClient()
    return await prisma.player.findMany()
} 

export const getPlayerById = async (playerId: string) => {    
    const prisma = new PrismaClient()
    const player = await prisma.player.findUnique({
        //include: { Country: true },
        where: { id: `${playerId}` },
    })
    
    return player
}

export const getPercentageByCountry = async() => {
    const prisma = new PrismaClient()
    const players = await prisma.player.findMany({
        include: { Country: true },
    })

    const percentages = new Map()
    players.map((player) => {
        if(percentages.has(player.Country?.code)) {
            let oldValue = percentages.get(player.Country?.code) as number
            percentages.set(player.Country?.code, oldValue + 1)
        }
        else {
            percentages.set(player.Country?.code, 1)
        }
    })

    for (let entry of Array.from(percentages.entries())) {
        percentages.set(entry[0], entry[1]/players.length * 100)
    }

    return percentages
}

export const updatePlayer = async(player: PlayerDTOUpdate) => {
    const prisma = new PrismaClient()
    let updatedPlayer = await prisma.player.update({
        where: { id: `${player.id}` },
        data: {
            name: `${player.name}`,
            active: player.active
        }
    })

    return updatedPlayer !== null
}

export const createPlayer = async(player: PlayerDTOCreate) => {
    const prisma = new PrismaClient()
    const existingPlayer = await prisma.player.findFirst({
        include: { Country: true },
        where: { 
            name: player.name,
            Country: {
                code: player.countryCode
            }
        }
    })

    if (existingPlayer === null) {
        let newPlayer: Player = {
            id: uuidv4(),
            playerId: player.playerId,
            name: player.name,
            gender: player.gender,
            active: player.active,
            birthyear: player.birthyear,
            playHand: player.playHand,
            playStyle: player.playStyle,
            grip: player.grip
        }

        const existingCountry = await prisma.country.findFirst({
            where: {
                code: player.countryCode
            }
        })

        let countryCode = player.countryCode
        if (existingCountry === null) {

            let newCountry = await prisma.country.create({
                data: {
                    id: uuidv4(),
                    code: countryCode
                },
                select: {
                    id: true,
                    code: true
                }
            })

            newPlayer.country = {
                id: newCountry.id,
                code: newCountry.code ?? undefined
            }
        } else {
            newPlayer.country = {
                id: existingCountry.id,
                code: existingCountry.code ?? undefined
            }
        }

        return await prisma.player.create({
            data: {
                id: newPlayer.id!,
                playerId: newPlayer.playerId,
                name: newPlayer.name,
                gender: newPlayer.gender,
                active: newPlayer.active,
                birthyear: newPlayer.birthyear,
                playHand: newPlayer.playHand,
                playStyle: newPlayer.playStyle,
                grip: newPlayer.grip,
                countryId: newPlayer.country.id
            }
        })
    }

    
}

export const deletePlayer = async(playerId: string) => {
    try {
        const prisma = new PrismaClient()
        let player = await prisma.player.findFirst({
            where: { id: playerId }
        })

        if(player !== null) {
            return await prisma.player.delete({
                where: { id: playerId }
            })
        }

        //throw 'The given user does not exist'
    } catch (e) {
        if (e instanceof Prisma.PrismaClientKnownRequestError) {
            if (e.code == 'P2025') {
                console.log("The record to delete does not exist")
            }
        }

        throw e
    }
    
}

const playerWithCountry = Prisma.validator<Prisma.PlayerArgs>()({
    include: { Country: true }
})
type PlayerWithCountry = Prisma.PlayerGetPayload<typeof playerWithCountry>
// npx prisma generate
