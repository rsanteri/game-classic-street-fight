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
    let mutable state = Global.createState (Global.InGame(defaultMap(), defaultMapController()))


    override self.Initialize() =
        do spriteBatch <- new SpriteBatch(self.GraphicsDevice)
        graphics.PreferredBackBufferWidth <- 1024
        graphics.PreferredBackBufferHeight <- 768
        graphics.ApplyChanges()
        do base.Initialize()
        ()

    override self.LoadContent() =
        ResourceManager.loadResources self.Content
        /// Console text handler
        self.Window.TextInput.AddHandler(fun a b -> Console.update state b)

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
        | InGame(map, mapController) -> GamePlay.Update.update gameTime state ioactions map mapController

        /// Save io state for next frame
        state.io <-
            { keys = keyboard
              mouse = mouse }


    override self.Draw(gameTime) =
        do self.GraphicsDevice.Clear Color.DarkSlateBlue

        match state.state with
        | Loading -> ()
        | InGame(map, mapController) -> GamePlay.Render.renderGamePlay state spriteBatch mapController map
