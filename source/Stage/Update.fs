module Stage.Update

open Entity.Types
open AI.Types
open Stage.Types

///
/// Stage events
///

let updateStageTransition (stgController: StageController) =
    match stgController.stageState with
    | Entering progress ->
        if progress <= 0
        then stgController.stageState <- Normal
        else stgController.stageState <- Entering(progress - 1)
    | Normal -> ()
    | Exiting(progress, target, exitTo) ->
        if progress >= 100
        then stgController.stageState <- ExitNow exitTo
        else stgController.stageState <- Exiting(progress + 1, target, exitTo)
    | ExitNow _ -> ()

let ApplyTrigers (map: Stage) (mapcontroller: StageController) =
    for trigger in map.triggers do
        if trigger.active && mapcontroller.player.position.x > (fst trigger.x)
           && mapcontroller.player.position.x < (snd trigger.x) then
            match trigger.action with
            | AddEntity entity ->
                // Add new entity to mapcontroller
                mapcontroller.entities <-
                    List.append mapcontroller.entities
                        [ { entity = entity
                            brain =
                                { dormant = false
                                  decision = MoveNextTo mapcontroller.player
                                  nextDecision = 5000 } } ]

                trigger.active <- false

            | LockCamera x -> ()
            | UnlockCamera -> ()
            | Exit(exitTo, exitArea) ->
                if mapcontroller.stageState = Normal then
                    mapcontroller.stageState <- Exiting(0, exitTo, exitArea)
                    trigger.active <- false

        else
            ()
