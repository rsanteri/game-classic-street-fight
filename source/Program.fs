﻿module Program

open Game

[<EntryPoint>]
let main argv =
    use g = new Game1()
    g.Run()
    0
