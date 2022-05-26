export declare type PlayerDTOCreate = {
    playerId: number,
    name: string,
    gender: boolean,
    active: boolean,
    birthyear?: number
    playHand?: string,
    playStyle?: string,
    grip?: string,
    countryCode: string
}

export declare type PlayerDTOUpdate = {
    id: string,
    name: string,
    active: boolean,
}

