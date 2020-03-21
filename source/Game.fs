module Game

open Microsoft.Xna.Framework
open Microsoft.Xna.Framework.Graphics
open Microsoft.Xna.Framework.Input
open System

open InputHelper
open Stage
open Global


type Game1() as self =
    inherit Game()

    do self.Content.RootDirectory <- "Content"
    let graphics = new GraphicsDeviceManager(self)
    let mutable spriteBatch = Unchecked.defaultof<SpriteBatch>
    let mutable state = Global.createState (Global.InGame(Street.Area1.init()))
    let mutable consoleCommands: Console.ConsoleCommand list = []

    override self.Initialize() =
        do spriteBatch <- new SpriteBatch(self.GraphicsDevice)
        graphics.PreferredBackBufferWidth <- state.resolution.width
        graphics.PreferredBackBufferHeight <- state.resolution.height
        graphics.ApplyChanges()
        do base.Initialize()
        ()

    override self.LoadContent() =
        ResourceManager.loadResources self.Content
        /// Console text handler
        self.Window.TextInput.AddHandler(fun a b ->
            let commands = Console.update state b
            consoleCommands <- List.concat [ commands; consoleCommands ])

    override self.Update(gameTime) =
        let keyboard = Keyboard.GetState()
        let mouse = Mouse.GetState()

        Cursor.updateCursor state mouse

        /// Helper to find input actions
        let ioactions =
            makeIOActionState state.io
                { keys = keyboard
                  mouse = mouse }

        if ioactions.pressed Keys.F1 then state.console <- not state.console

        match state.state with
        | Loading -> ()
        | InMap -> Map.Update.update()
        | InGame(map, mapController) -> GamePlay.Update.update gameTime state ioactions map mapController

        /// Save io state for next frame
        state.io <-
            { keys = keyboard
              mouse = mouse }

        ///
        /// Run commands called from console
        ///
        for command in consoleCommands do
            match command with
            | Console.ConsoleCommand.SetResolution(width, height) ->
                graphics.PreferredBackBufferWidth <- width
                graphics.PreferredBackBufferHeight <- height
                graphics.ApplyChanges()
            | Console.ConsoleCommand.Restart ->
                match state.state with
                | InGame(map, _) -> state.state <- InGame(Areas.init map.area)
                | InMap -> ()
                | Loading -> ()
            | Console.ConsoleCommand.ExitApp -> exit 0

        consoleCommands <- []

    override self.Draw(gameTime) =
        do self.GraphicsDevice.Clear Color.DarkSlateBlue

        match state.state with
        | Loading -> ()
        | InMap -> Map.Render.render state spriteBatch
        | InGame(map, mapController) -> GamePlay.Render.renderGamePlay state spriteBatch mapController map

        spriteBatch.Begin()
        if state.console then Console.DrawConsoleLog state spriteBatch
        spriteBatch.End()
