module Stage

open Microsoft.Xna.Framework
open Microsoft.Xna.Framework.Graphics
open EntityTypes
open AITypes
open StageTypes

///
/// Order entities by position.y so those that are more down are rendered ON the entities more up
///
let OrderEntities(map: StageController): RenderableEntity list =
    let entities = List.map (fun i -> i.entity) map.entities

    entities
    |> List.map NPCEntity
    |> List.append [ PlayerEntity map.player ]
    |> List.sortWith (fun a b ->
        let ae =
            match a with
            | PlayerEntity ent -> ent
            | NPCEntity ent -> ent

        let be =
            match b with
            | PlayerEntity ent -> ent
            | NPCEntity ent -> ent

        ae.position.y - be.position.y)

///
/// Stage drawing
///

let drawLayer (spriteBatch: SpriteBatch) (layer: DrawLayer) (camera: XCamera) =
    for sprite in layer.sprites do
        spriteBatch.Draw
            (ResourceManager.getSprite sprite.sprite,
             Camera.ToParallaxPosition camera
                 (Rectangle(sprite.position.x, sprite.position.y, sprite.size.width, sprite.size.height))
                 layer.distance, Color.White)

let renderStage (spriteBatch: SpriteBatch) (stage: Stage) (camera: XCamera) =
    for layer in stage.layers.bg do
        drawLayer spriteBatch layer camera

    // Street
    spriteBatch.Draw
        (ResourceManager.getSprite "default", Camera.ToCameraPosition camera (Rectangle(0, 418, stage.size * 2, 350)),
         Color.DarkGray)


let renderTransitionOverlay (spriteBatch: SpriteBatch) (stage: Stage) (stageController: StageController) =
    let drawOverlay progress =
        spriteBatch.Draw
            (ResourceManager.getSprite "default", Rectangle(0, 0, stage.size, 768),
             Color.Black * (float32 progress / 100.0f))
    /// Enter and leave animation
    match stageController.stageState with
    | Entering progress -> drawOverlay progress
    | Normal -> ()
    | Exiting(progress, _, _) -> drawOverlay progress
    | ExitNow _ -> drawOverlay 100

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
