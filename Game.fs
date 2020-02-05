module Game

open Microsoft.Xna.Framework
open Microsoft.Xna.Framework.Graphics
open Microsoft.Xna.Framework.Input
open System

type PlayerPosition = { mutable x: int; mutable y: int; }
type PlayerVelocity = { mutable x: int; mutable y: int; }

let AddVelocity (current: int) (add: int) =
    let newVelocity = current + add
    
    if newVelocity < -100 then
        -100
    else if newVelocity > 100 then
        100
    else
        newVelocity

let NaturalSlowVelocity (current: int): int =
    if current = 0 then 
        0
    else if current > 0 then
        if current - 10 < 0 then 0 else current - 10
    else 
        if current + 10 > 0 then 0 else current + 10


let CalculatePosition (velocity: PlayerVelocity) (position: PlayerPosition) (speed: int): PlayerPosition =
    let x = float32 position.x + (float32 (velocity.x) / 100.0f * float32 speed)
    let y = float32 position.y + (float32 (velocity.y) / 100.0f * (float32 speed/2.0f))
    System.Diagnostics.Debug.Print ("x: " + x.ToString())
    System.Diagnostics.Debug.Print ("y: " + y.ToString())
    { x = int x; y = int y }

let WithinBoundary (pos: PlayerPosition): PlayerPosition =
    let x = if pos.x < 0 then 0 else if pos.x > 1024 - 50 then 1024 - 50 else pos.x
    let y = if pos.y < 0 then 0 else if pos.y > 768 - 50 then 768 - 50 else pos.y

    { x = x; y = y }

type Game1() as self =
    inherit Game()

    do self.Content.RootDirectory <- "Content"
    let graphics = new GraphicsDeviceManager(self)
    let mutable spriteBatch = Unchecked.defaultof<SpriteBatch>
    let mutable randomTexture = Unchecked.defaultof<Texture2D>
    let mutable playerPosition: PlayerPosition = { x = 0; y = 0 }
    let playerVelocity: PlayerVelocity = { x = 0; y = 0 }
    let velocitySpeed = 10
    let playerSpeed = 20

    override self.Initialize() =
        do spriteBatch <- new SpriteBatch(self.GraphicsDevice)
        graphics.PreferredBackBufferWidth <- 1024
        graphics.PreferredBackBufferHeight <- 768
        graphics.ApplyChanges()
        do base.Initialize()
        // TODO: Add your initialization logic here

        ()

    override self.LoadContent() =
        randomTexture <- self.Content.Load<Texture2D>("floor")

    override self.Update(gameTime) =
        let keyboard = Keyboard.GetState()

        if keyboard.IsKeyDown(Keys.W) || keyboard.IsKeyDown(Keys.S) then
            let dir = if keyboard.IsKeyDown(Keys.W) then -velocitySpeed else velocitySpeed
            playerVelocity.y <- AddVelocity playerVelocity.y dir
        else 
            playerVelocity.y <- NaturalSlowVelocity playerVelocity.y
        
        if keyboard.IsKeyDown(Keys.A) || keyboard.IsKeyDown(Keys.D) then
            let dir = if keyboard.IsKeyDown(Keys.A) then -velocitySpeed else velocitySpeed
            playerVelocity.x <- AddVelocity playerVelocity.x dir
        else 
            playerVelocity.x <- NaturalSlowVelocity playerVelocity.x
    
        // System.Diagnostics.Debug.Print ("Velocity: " + playerVelocity.ToString())

        playerPosition <- WithinBoundary (CalculatePosition playerVelocity playerPosition playerSpeed)

        // System.Diagnostics.Debug.Print ("Position: " + playerPosition.ToString())

    override self.Draw(gameTime) =
        do self.GraphicsDevice.Clear Color.Black
        spriteBatch.Begin()
        spriteBatch.Draw(randomTexture, Rectangle(playerPosition.x, playerPosition.y, 50, 50), Color.White)
        spriteBatch.End()
