module Global

type Resolution =
    { width: int
      height: int }

type ApplicationState =
    | InGame of (Stage.Types.Stage * Stage.Types.StageController)
    | InMap
    | Loading

type GlobalState =
    { mutable cursor: UI.Types.CursorState
      mutable resolution: Resolution
      mutable console: bool
      mutable state: ApplicationState
      mutable io: InputHelper.IOState }

let createState (state: ApplicationState) =
    { cursor =
          { x = 0
            y = 0 }
      resolution =
          { width = 1280
            height = 768 }
      console = false
      state = state
      io =
          { keys = Unchecked.defaultof<Microsoft.Xna.Framework.Input.KeyboardState>
            mouse = Unchecked.defaultof<Microsoft.Xna.Framework.Input.MouseState> } }
