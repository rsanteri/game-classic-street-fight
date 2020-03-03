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
open Stage




type ApplicationState =
    | InGame of (Stage * StageController)
    | Loading

type Game1() as self =
    inherit Game()

    do self.Content.RootDirectory <- "Content"
    let graphics = new GraphicsDeviceManager(self)
    let mutable spriteBatch = Unchecked.defaultof<SpriteBatch>
    let mutable cursor = { x = 0; y = 0 }
    let mutable state =
        InGame (defaultMap(), defaultMapController() )

    let mutable io = 
        { keys = Unchecked.defaultof<KeyboardState>
          mouse = Unchecked.defaultof<MouseState> }

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

        match state with
        | Loading -> ()
        | InGame (map, mapController) ->
            let player = mapController.player
            /// Gameplay updates will only happen if we are not in menu or paused.
            match mapController.state with
            | Playing ->
                ///
                /// Handle input
                /// 
                
                /// gamestate changes. Set to pause or to menu
                if ioactions.pressed Keys.P then
                    mapController.state <- Paused
                else if ioactions.pressed Keys.Escape then
                    mapController.state <- OnMenu

                /// Update player velocity according input
                HandlePlayerMovementInput keyboard player
                /// Update action
                if player.action = NoOp && keyboard.IsKeyDown(Keys.Space) then
                    player.action <- Hit

                ///
                /// Make ai decisions
                /// 
                /// 
                let allEntities = List.append [player] (List.map (fun i -> i.entity) mapController.entities)
                
                for controlledEntity in mapController.entities do
                    if not controlledEntity.brain.dormant then
                        OperateNPC controlledEntity allEntities gameTime.ElapsedGameTime.Milliseconds

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
                mapController.entities <- List.filter (fun npc -> npc.entity.properties.health > 0) mapController.entities
            
            | Paused ->
                if ioactions.pressed Keys.P then
                    mapController.state <- Playing
            | OnMenu ->
                if ioactions.pressed Keys.Escape then
                    mapController.state <- Playing

            /// Move camera after everything
            Camera.MoveCamera mapController.camera player.position.x map.size

        /// Save io state for next frame
        io <- { keys = keyboard; mouse = mouse }
            

    override self.Draw(gameTime) =
        do self.GraphicsDevice.Clear Color.DarkSlateBlue

        match state with
        | Loading -> ()
        | InGame (map, mapController) ->
            let entities = OrderEntities mapController
            let cameraPosition = Camera.ToCameraPosition mapController.camera

            spriteBatch.Begin()

            spriteBatch.Draw(ResourceManager.getSprite "cityview", Camera.ToParallaxPosition mapController.camera (Rectangle(0, 0, 1361, 600)) 10, Color.White)
            // Street
            spriteBatch.Draw(ResourceManager.getSprite "default", cameraPosition (Rectangle(0, 418, map.size * 2, 350)), Color.DarkGray)
            // Entities
            for entity in entities do
                DrawEntity spriteBatch entity cameraPosition
            // Cursor. Probably not needed in real game
            spriteBatch.Draw(ResourceManager.getSprite "default", Rectangle(cursor.x, cursor.y, 5, 5), Color.Red)
            // Render message about pause or menu state
            match mapController.state with
            | Paused ->
                spriteBatch.DrawString
                    (ResourceManager.getFont "default", "PAUSED", Vector2(100.0f, 100.0f), Color.White)
            | OnMenu ->
                spriteBatch.DrawString
                    (ResourceManager.getFont "default", "MENU", Vector2(100.0f, 100.0f), Color.White)
            | Playing -> ()

            spriteBatch.End()
