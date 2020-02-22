module Game

open Microsoft.Xna.Framework
open Microsoft.Xna.Framework.Graphics
open Microsoft.Xna.Framework.Input
open System
open System.Diagnostics

open Entity
open EntityMovement
open EntityActions
open Player

type Game1() as self =
    inherit Game()

    do self.Content.RootDirectory <- "Content"
    let graphics = new GraphicsDeviceManager(self)
    let mutable spriteBatch = Unchecked.defaultof<SpriteBatch>
    let mutable randomTexture = Unchecked.defaultof<Texture2D>

    let player = DefaultHumanoid { x = 100; y = 500 }

    let mutable npcs =
        [ DefaultHumanoid { x = 300; y = 500 }; DefaultHumanoid { x = 350; y = 525 } ]


    override self.Initialize() =
        do spriteBatch <- new SpriteBatch(self.GraphicsDevice)
        graphics.PreferredBackBufferWidth <- 1024
        graphics.PreferredBackBufferHeight <- 768
        graphics.ApplyChanges()
        do base.Initialize()
        // TODO: Add your initialization logic here

        ()

    override self.LoadContent() = randomTexture <- self.Content.Load<Texture2D>("floor")

    override self.Update(gameTime) =

        //
        // Handle input
        //

        let keyboard = Keyboard.GetState()
        // Update player velocity according input
        HandlePlayerMovementInput keyboard player
        // Update action
        if player.action = NoOp && keyboard.IsKeyDown(Keys.Space) then
            player.action <- Hit

        //
        // Update Movement & Action State
        //
        
        MoveEntity player npcs
        HandleEntityAction player npcs

        let allEntities = List.append npcs [player]
        for npc in npcs do
            MoveEntity npc allEntities
            HandleEntityAction npc allEntities
        
        /// Clean up the dead
        npcs <- List.filter (fun npc -> npc.properties.health > 0) npcs
            

    override self.Draw(gameTime) =
        do self.GraphicsDevice.Clear Color.DarkSlateBlue

        let entities = OrderEntities npcs player

        spriteBatch.Begin()
        // BG
        spriteBatch.Draw(randomTexture, Rectangle(0, 418, 1024, 350), Color.DarkGray)
        // Player
        for entity in entities do
            DrawEntity spriteBatch entity randomTexture

        spriteBatch.End()
