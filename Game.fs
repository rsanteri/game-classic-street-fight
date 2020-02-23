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

type GameState = 
    | Playing
    | Paused
    | OnMenu

type Game1() as self =
    inherit Game()

    do self.Content.RootDirectory <- "Content"
    let graphics = new GraphicsDeviceManager(self)
    let mutable spriteBatch = Unchecked.defaultof<SpriteBatch>
    let mutable cursor = { x = 0; y = 0 }
    let mutable gamestate = Playing

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

    override self.LoadContent() = 
        ResourceManager.loadResources self.Content

    override self.Update(gameTime) =
        let keyboard = Keyboard.GetState()
        let mouse = Mouse.GetState()

        cursor <- { x = mouse.X; y=mouse.Y }

        /// Helper to find input actions
        let ioactions = makeIOActionState io { keys = keyboard; mouse = mouse }
        /// Gameplay updates will only happen if we are not in menu or paused.
        match gamestate with
        | Playing ->
            ///
            /// Handle input
            /// 
            
            /// gamestate changes. Set to pause or to menu
            if ioactions.pressed Keys.P then
                gamestate <- Paused
            else if ioactions.pressed Keys.Escape then
                gamestate <- OnMenu

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
                // Update entity position
                MoveEntity entity otherEntities
                // Update possible entity action
                HandleEntityAction entity otherEntities
                // Update walking animation
                UpdateEntityWalking entity
            
            /// Clean up the dead
            npcs <- List.filter (fun npc -> npc.properties.health > 0) npcs
        
        | Paused ->
            if ioactions.pressed Keys.P then
                gamestate <- Playing
        | OnMenu ->
            if ioactions.pressed Keys.Escape then
                gamestate <- Playing

        /// Save io state for next frame
        io <- { keys = keyboard; mouse = mouse }
            

    override self.Draw(gameTime) =
        do self.GraphicsDevice.Clear Color.DarkSlateBlue

        let entities = OrderEntities npcs player

        spriteBatch.Begin()
        // BG
        spriteBatch.Draw(ResourceManager.getSprite "default", Rectangle(0, 418, 1024, 350), Color.DarkGray)
        // Entities
        for entity in entities do
            DrawEntity spriteBatch entity
        // Cursor. Probably not needed in real game
        spriteBatch.Draw(ResourceManager.getSprite "default", Rectangle(cursor.x, cursor.y, 5, 5), Color.Red)
        // Render message about pause or menu state
        match gamestate with
        | Paused ->
            spriteBatch.DrawString
                (ResourceManager.getFont "default", "PAUSED", Vector2(100.0f, 100.0f), Color.White)
        | OnMenu ->
            spriteBatch.DrawString
                (ResourceManager.getFont "default", "MENU", Vector2(100.0f, 100.0f), Color.White)
        | Playing -> ()

        spriteBatch.End()
