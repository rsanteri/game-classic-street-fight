module Map.Render

open Microsoft.Xna.Framework

let render (state: Global.GlobalState) (spriteBatch: Graphics.SpriteBatch) =
    spriteBatch.Begin()

    /// background
    spriteBatch.Draw(ResourceManager.getSprite "default", Rectangle(0, 0, 2000, 2000), Color.DarkBlue)
    /// stage 1
    spriteBatch.Draw(ResourceManager.getSprite "default", Rectangle(100, 100, 100, 100), Color.LightBlue)
    /// stage 2
    spriteBatch.Draw(ResourceManager.getSprite "default", Rectangle(300, 100, 100, 100), Color.LightBlue)

    spriteBatch.End()
