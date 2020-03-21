module GamePlay.Render

open Microsoft.Xna.Framework

open Stage.Types

let renderGamePlay
    (state: Global.GlobalState)
    (spriteBatch: Graphics.SpriteBatch)
    (mapController: StageController)
    (map: Stage)
    =
    let entities = Stage.Render.OrderEntities mapController
    let cameraPosition = Camera.ToCameraPosition mapController.camera

    spriteBatch.Begin()

    Stage.Render.renderStage spriteBatch map mapController.camera

    // Entities
    for entity in entities do
        Entity.Render.DrawEntity spriteBatch entity cameraPosition

    // Cursor. Probably not needed in real game
    spriteBatch.Draw(ResourceManager.getSprite "default", Rectangle(state.cursor.x, state.cursor.y, 5, 5), Color.Red)

    /// Render possible transition overlay
    Stage.Render.renderTransitionOverlay spriteBatch map mapController

    // Render message about pause or menu state
    match mapController.gameState with
    | Paused ->
        spriteBatch.DrawString(ResourceManager.getFont "default", "PAUSED", Vector2(100.0f, 100.0f), Color.White)
    | OnMenu -> spriteBatch.DrawString(ResourceManager.getFont "default", "MENU", Vector2(100.0f, 100.0f), Color.White)
    | Playing -> ()

    spriteBatch.DrawString(ResourceManager.getFont "default", string map.area, Vector2(5.0f, 5.0f), Color.White)

    spriteBatch.End()
