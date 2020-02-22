module Game

open Microsoft.Xna.Framework
open Microsoft.Xna.Framework.Graphics
open Microsoft.Xna.Framework.Input
open System
open System.Diagnostics

open Entity
open EntityTypes
open EntityMovement
open EntityActions
open Player
open AI
open AITypes
open InputHelper

type Game1() as self =
    inherit Game()

    do self.Content.RootDirectory <- "Content"
    let graphics = new GraphicsDeviceManager(self)
    let mutable spriteBatch = Unchecked.defaultof<SpriteBatch>
    let mutable randomTexture = Unchecked.defaultof<Texture2D>
    let mutable cursor = { x = 0; y = 0 }

    let mutable io = 
        { keys = Unchecked.defaultof<KeyboardState>
          mouse = Unchecked.defaultof<MouseState> }

    let player = DefaultHumanoid { x = 100; y = 500 }

    let mutable npcs =
        [ DefaultHumanoid { x = 300; y = 500 }; DefaultHumanoid { x = 350; y = 525 } ]

    let mutable brains: EntityController list =
        List.map (fun npc -> { brain = { decision = MoveNextTo player; nextDecision = 1000 }; entity = npc }) npcs

    override self.Initialize() =
        do spriteBatch <- new SpriteBatch(self.GraphicsDevice)
        graphics.PreferredBackBufferWidth <- 1024
        graphics.PreferredBackBufferHeight <- 768
        graphics.ApplyChanges()
        do base.Initialize()
        ()

    override self.LoadContent() = randomTexture <- self.Content.Load<Texture2D>("floor")

    override self.Update(gameTime) =
        let keyboard = Keyboard.GetState()
        let mouse = Mouse.GetState()

        cursor <- { x = mouse.X; y=mouse.Y }

        /// Helper to find input actions
        let ioactions = makeIOActionState io { keys = keyboard; mouse = mouse }

        ///
        /// Handle input
        /// 

        /// Update player velocity according input
        HandlePlayerMovementInput keyboard player
        /// Update action
        if player.action = NoOp && keyboard.IsKeyDown(Keys.Space) then
            player.action <- Hit

        ///
        /// Make ai decisions
        /// 
        /// 
        let allEntities = List.append [player] npcs
        for brain in brains do
            OperateNPC brain allEntities gameTime.ElapsedGameTime.Milliseconds

        if ioactions.leftClicked then
            for controller in brains do
                controller.brain.decision <- MoveTo (mouse.X, mouse.Y)

        ///
        /// Update Movement & Action State
        ///

        for entity in allEntities do
            let otherEntities = List.filter (fun item -> item.id <> entity.id) allEntities
            MoveEntity entity otherEntities
            HandleEntityAction entity otherEntities
        
        /// Clean up the dead
        npcs <- List.filter (fun npc -> npc.properties.health > 0) npcs

        /// Save io state for next frame
        io <- { keys = keyboard; mouse = mouse }
            

    override self.Draw(gameTime) =
        do self.GraphicsDevice.Clear Color.DarkSlateBlue

        let entities = OrderEntities npcs player

        spriteBatch.Begin()
        // BG
        spriteBatch.Draw(randomTexture, Rectangle(0, 418, 1024, 350), Color.DarkGray)
        // Player
        for entity in entities do
            DrawEntity spriteBatch entity randomTexture

        spriteBatch.Draw(randomTexture, Rectangle(cursor.x, cursor.y, 5, 5), Color.Red)

        spriteBatch.End()
