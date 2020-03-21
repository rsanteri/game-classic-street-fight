module Stage.Render

open Microsoft.Xna.Framework
open Microsoft.Xna.Framework.Graphics
open Entity.Types
open AI.Types
open Stage.Types

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
