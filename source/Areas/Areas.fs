module Areas

open StageTypes

let init (area: Area): StageTypes.Stage * StageTypes.StageController =
    match area with
    | Map -> (Street.Area1.init())
    | Street1 -> (Street.Area1.init())
    | Street2 -> (Street.Area2.init())
