module Map.Render

open Microsoft.Xna.Framework
open Map.Types

let render (state: Global.GlobalState) (spriteBatch: Graphics.SpriteBatch) (mapState: MapState) =
    spriteBatch.Begin()

    /// background
    spriteBatch.Draw(ResourceManager.getSprite "default", Rectangle(0, 0, 2000, 2000), Color.DarkBlue)

    for stage in mapState.stages do
        if mapState.activeStage = stage.id then
            spriteBatch.Draw
                (ResourceManager.getSprite stage.sprite,
                 Rectangle(stage.position.x - 5, stage.position.y - 5, stage.size.width + 10, stage.size.height + 10),
                 Color.Yellow)

        spriteBatch.Draw
            (ResourceManager.getSprite stage.sprite,
             Rectangle(stage.position.x, stage.position.y, stage.size.width, stage.size.height), Color.White)

    spriteBatch.End()
