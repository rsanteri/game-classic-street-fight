module Cursor

open Microsoft.Xna.Framework.Input

let updateCursor (appState: Global.GlobalState) (mouse: MouseState) =
    appState.cursor <-
        { x = mouse.X
          y = mouse.Y }
