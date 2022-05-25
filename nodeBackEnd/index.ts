import express from 'express'
import { prisma, PrismaClient } from '@prisma/client'

const app = express()
const playerController = require("./controllers/playerController")

app.use(express.json())

app.post('/Player', playerController.createPlayerAsync)
app.put('/Player', playerController.updatePlayerAsync)
app.delete('/Player', playerController.deletePlayerAsync)
app.get('/Player', playerController.getPlayersAsync)
app.get('/Player/Id', playerController.getPlayerByIdAsync)
app.get('/Player/NoCountry', playerController.getPlayersWithoutCountryAsync)
app.get('/Player/Percentage', playerController.getPercentageByCountryAsync)


//npx ts-node index.ts
const server = app.listen(3000, () => {
    console.log(`server ready at: http://localhost:3000`)
})