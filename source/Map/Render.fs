module Map.Render

open Microsoft.Xna.Framework

let render (state: Global.GlobalState) (spriteBatch: Graphics.SpriteBatch) =
    spriteBatch.Begin()

    spriteBatch.Draw(ResourceManager.getSprite "default", Rectangle(0, 0, 2000, 2000), Color.DarkBlue)

    spriteBatch.End()
