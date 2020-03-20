module GamePlay.Render

open Microsoft.Xna.Framework

open StageTypes

let renderGamePlay
    (appState: Global.GlobalState)
    (spriteBatch: Graphics.SpriteBatch)
    (mapController: StageController)
    (map: Stage)
    =
    let entities = Stage.OrderEntities mapController
    let cameraPosition = Camera.ToCameraPosition mapController.camera

    spriteBatch.Begin()

    Stage.renderStage spriteBatch map mapController.camera

    // Entities
    for entity in entities do
        Entity.DrawEntity spriteBatch entity cameraPosition

    // Cursor. Probably not needed in real game
    spriteBatch.Draw
        (ResourceManager.getSprite "default", Rectangle(appState.cursor.x, appState.cursor.y, 5, 5), Color.Red)

    /// Render possible transition overlay
    Stage.renderTransitionOverlay spriteBatch map mapController

    // Render message about pause or menu state
    match mapController.gameState with
    | Paused ->
        spriteBatch.DrawString(ResourceManager.getFont "default", "PAUSED", Vector2(100.0f, 100.0f), Color.White)
    | OnMenu -> spriteBatch.DrawString(ResourceManager.getFont "default", "MENU", Vector2(100.0f, 100.0f), Color.White)
    | Playing -> ()

    if appState.console then Console.DrawConsoleLog spriteBatch

    spriteBatch.End()
