import { PrismaClient } from "@prisma/client";
import express from 'express'

const prisma = new PrismaClient()
const app = express()
const playerController = require("./controllers/playerController")

app.use(express.json())

app.post('/Player', playerController.createPlayerAsync)
app.put('/Player', playerController.updatePlayerAsync)
app.delete('/Player', playerController.deletePlayerAsync)
app.get('/Player', playerController.getPlayersAsync)
app.get('/Player/:Id', playerController.getPlayerByIdAsync)
app.get('/Player/NoCountry', playerController.getPlayersWithoutCountryAsync)
app.get('/Player/Percentage', playerController.getPercentageByCountryAsync)


//npx ts-node index.ts
async function main() {
    const allPlayers = await prisma.player.findMany()
    console.log(allPlayers)
}

main()
    .catch((e) => {
        throw e
    })
    .finally(async () => {
        await prisma.$disconnect()
    })