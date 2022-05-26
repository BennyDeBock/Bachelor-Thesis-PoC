// interface Player {
//     id: String,
//     playerId: number,
//     name: String,
//     gender: boolean,
//     active: boolean,
//     birthyear?: number
//     playHand?: String,
//     playStyle?: String,
//     grip?: String,
//     country?: Country
// }

// interface Country {
//     id: String,
//     code: String
// }

export declare type Player = {
    id?: string,
    playerId: number,
    name: string,
    gender: boolean,
    active: boolean,
    birthyear?: number
    playHand?: string,
    playStyle?: string,
    grip?: string,
    country?: Country
}

export declare type Country = {
    id: string,
    code?: string
}