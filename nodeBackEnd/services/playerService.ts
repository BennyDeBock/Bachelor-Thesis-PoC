import { Prisma, PrismaClient } from "@prisma/client";
import { Player } from "../Models/player";

const prisma = new PrismaClient()

const getAllPlayers = async () => { 
    return await prisma.player.findMany({
        include: {
            Country: true
        },
    })
}

const getAllPlayersWithoutCountry = async () => {
    return await prisma.player.findMany()
} 

const getPlayerById = async (playerId: number) => {
    return await prisma.player.findUnique({
        include: { Country: true },
        where: { id: `${playerId}` }
    })
}

const getPercentageByCountry = async() => {
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

const updatePlayer = async(player: Player) => {
    return await prisma.player.update({
        where: { id: player.id },
        data: {
            name: player.name,
            active: player.active
        }
    })
}

const createPlayer = async(player: Player) => {
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
                    code: countryCode
                },
                select: {
                    id: true
                }
            })
        }

        return await prisma.player.create({
            where: { id: "42"},
            data: {
                playerId: player.playerId,
                name: player.name,
                gender: player.gender,
                active: player.active,
                birthyear: player.birthyear,
                playHand: player.playHand,
                playStyle: player.playStyle,
                grip: player.grip
            }
        })
    }

    
}

const deletePlayer = async(id: string) => {
    return await prisma.player.delete({
        where: { id: id }
    })
}

const playerWithCountry = Prisma.validator<Prisma.PlayerArgs>()({
    include: { Country: true },
    select: { playerId: true }
})
type PlayerWithCountry = Prisma.PlayerGetPayload<typeof playerWithCountry>
// npx prisma generate
