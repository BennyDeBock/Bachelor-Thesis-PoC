import { PrismaClient } from "@prisma/client";
import express from 'express'

const prisma = new PrismaClient()
const app = express()

app.use(express.json())

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