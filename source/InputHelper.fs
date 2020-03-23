module InputHelper

open Microsoft.Xna.Framework.Input

type IOState =
    { mutable keys: KeyboardState
      mutable mouse: MouseState }

type IOActionState =
    { pressed: Keys -> bool
      pressedAnyOf: Keys list -> Keys option
      leftClicked: bool
      rightClicked: bool }



let pressed (old: KeyboardState) (current: KeyboardState) (key: Keys): bool = old.IsKeyUp(key) && current.IsKeyDown(key)

let pressedAnyOf (old: KeyboardState) (current: KeyboardState) (keys: Keys list) =
    List.tryFind (fun key -> pressed old current key) keys

let leftClicked (old: MouseState) (current: MouseState): bool =
    old.LeftButton = ButtonState.Released && current.LeftButton = ButtonState.Pressed

let rightClicked (old: MouseState) (current: MouseState): bool =
    old.RightButton = ButtonState.Released && current.RightButton = ButtonState.Pressed

let makeIOActionState oldio newio =
    { pressed = pressed oldio.keys newio.keys
      pressedAnyOf = pressedAnyOf oldio.keys newio.keys
      leftClicked = leftClicked oldio.mouse newio.mouse
      rightClicked = rightClicked oldio.mouse newio.mouse }
