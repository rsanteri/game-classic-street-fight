module Areas

open Stage.Types

let init (area: Area): Stage * StageController =
    match area with
    | Map -> (Street.Area1.init())
    | Street1 -> (Street.Area1.init())
    | Street2 -> (Street.Area2.init())
