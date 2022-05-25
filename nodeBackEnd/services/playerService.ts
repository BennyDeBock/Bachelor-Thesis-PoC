import { Prisma, PrismaClient } from "@prisma/client";
import { Player } from "../Models/player";
import { v4 as uuidv4 } from "uuid";
import { info } from "console";



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
    try {
        const prisma = new PrismaClient()
        const player = await prisma.player.findUnique({
            include: { Country: true },
            where: { id: playerId },
        })
        return player
    } catch (err) {
        console.log(err)
    }
    
}

export const getPercentageByCountry = async() => {
    const prisma = new PrismaClient()
    const players = await prisma.player.findMany({
        include: { Country: true },
    })

    const percentages = new Map()
    players.map((player) => {
        if(percentages.has(player.Country?.code)) {
            percentages.get(player.Country?.code).val++
        }
        else {
            percentages.set(player.Country?.code, {val: 1})
        }
    })

    for (let entry of Array.from(percentages.entries())) {
        percentages.set(entry[0], entry[1]/percentages.size * 100)
    }

    return percentages
}

export const updatePlayer = async(player: Player) => {
    const prisma = new PrismaClient()
    return await prisma.player.update({
        where: { id: player.id },
        data: {
            name: player.name,
            active: player.active
        }
    })
}

export const createPlayer = async(player: Player) => {
    const prisma = new PrismaClient()
    const existingPlayer = await prisma.player.findFirst({
        include: { Country: true },
        where: { 
            name: player.name,
            Country: {
                code: player.country?.code
            }
        }
    })

    if (existingPlayer === null) {
        const existingCountry = await prisma.country.findFirst({
            where: {
                code: player.country?.code
            }
        })

        let countryCode = player.country!.code
        if (existingCountry === null) {

            let newCountry = await prisma.country.create({
                data: {
                    id: uuidv4(),
                    code: countryCode
                },
                select: {
                    id: true
                }
            })

            player.country!.id = newCountry.id
        } else {
            player.country!.id = existingCountry.id
        }

        return await prisma.player.create({
            data: {
                id: uuidv4(),
                playerId: player.playerId,
                name: player.name,
                gender: player.gender,
                active: player.active,
                birthyear: player.birthyear,
                playHand: player.playHand,
                playStyle: player.playStyle,
                grip: player.grip,
                countryId: player.country?.id
            }
        })
    }

    
}

export const deletePlayer = async(id: string) => {
    const prisma = new PrismaClient()
    return await prisma.player.delete({
        where: { id: id }
    })
}

const playerWithCountry = Prisma.validator<Prisma.PlayerArgs>()({
    include: { Country: true }
})
type PlayerWithCountry = Prisma.PlayerGetPayload<typeof playerWithCountry>
// npx prisma generate
