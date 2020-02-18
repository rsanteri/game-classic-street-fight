module Game

open Microsoft.Xna.Framework
open Microsoft.Xna.Framework.Graphics
open Microsoft.Xna.Framework.Input
open System

type EntityPosition =
    { mutable x: int
      mutable y: int }

type EntityVelocity =
    { mutable x: int
      mutable y: int }

let AddVelocity (current: int) (add: int) =
    let newVelocity = current + add

    if newVelocity < -100 then -100
    else if newVelocity > 100 then 100
    else newVelocity

let NaturalSlowVelocity(current: int): int =
    if current = 0 then 0
    else if current > 0 then if current - 10 < 0 then 0 else current - 10
    else if current + 10 > 0 then 0
    else current + 10


let CalculatePosition (velocity: EntityVelocity) (position: EntityPosition) (speed: int): EntityPosition =
    let x = float32 position.x + (float32 (velocity.x) / 100.0f * float32 speed)
    let y = float32 position.y + (float32 (velocity.y) / 100.0f * (float32 speed / 2.0f))

    { x = int x
      y = int y }

let WithinBoundary(pos: EntityPosition): EntityPosition =
    let x =
        if pos.x < 0 then 0
        else if pos.x > 1024 - 50 then 1024 - 50
        else pos.x

    let y =
        if pos.y < 418 - 45 then 418 - 45
        else if pos.y > 768 - 50 then 768 - 50
        else pos.y

    { x = x
      y = y }



type Entity =
    { mutable position: EntityPosition
      velocity: EntityVelocity
      velocitySpeed: int
      speed: int }

type RenderableEntity =
    | PlayerEntity of Entity
    | NPCEntity of Entity

let IsColliding (a: Entity) (b: Entity) =
    let x =  a.position.x < b.position.x + 50
    let x2 = a.position.x + 50 > b.position.x
    let y = a.position.y < b.position.y + 50
    let y2 = a.position.y + 50 > b.position.y

    x && x2 && y && y2

let CollisionDirection a b =
    let angle = (Math.Atan2 (float (b.position.y - a.position.y), float (b.position.x - a.position.x))) * (180.0 / Math.PI)

    if angle < 45.0 && angle > -45.0 then
        "Right"
    else if angle < -45.0 && angle > -135.0 then
        "Top"
    else if angle < 135.0 && angle > 45.0 then
        "Bottom"
    else
        "Left"

let OrderEntities (entitylist: Entity list) (player: Entity) =
    entitylist
    |> List.map NPCEntity
    |> List.append [ PlayerEntity player ]
    |> List.sortWith (fun a b ->
        let ae =
            match a with
            | PlayerEntity ent -> ent
            | NPCEntity ent -> ent

        let be =
            match b with
            | PlayerEntity ent -> ent
            | NPCEntity ent -> ent

        ae.position.y - be.position.y)

type Game1() as self =
    inherit Game()

    do self.Content.RootDirectory <- "Content"
    let graphics = new GraphicsDeviceManager(self)
    let mutable spriteBatch = Unchecked.defaultof<SpriteBatch>
    let mutable randomTexture = Unchecked.defaultof<Texture2D>

    let player =
        { position =
              { x = 100
                y = 500 }
          velocity =
              { x = 0
                y = 0 }
          velocitySpeed = 10
          speed = 20 }

    let npcs =
        [ { position =
                { x = 300
                  y = 500 }
            velocity =
                { x = 0
                  y = 0 }
            velocitySpeed = 10
            speed = 20 } ]


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

        if keyboard.IsKeyDown(Keys.W) || keyboard.IsKeyDown(Keys.S) then
            let dir =
                if keyboard.IsKeyDown(Keys.W) then -player.velocitySpeed else player.velocitySpeed
            player.velocity.y <- AddVelocity player.velocity.y dir
        else
            player.velocity.y <- NaturalSlowVelocity player.velocity.y

        if keyboard.IsKeyDown(Keys.A) || keyboard.IsKeyDown(Keys.D) then
            let dir =
                if keyboard.IsKeyDown(Keys.A) then -player.velocitySpeed else player.velocitySpeed
            player.velocity.x <- AddVelocity player.velocity.x dir
        else
            player.velocity.x <- NaturalSlowVelocity player.velocity.x

        //
        // Update State
        //
        player.position <- WithinBoundary(CalculatePosition player.velocity player.position player.speed)

        let isCollidingWithAny =
            npcs |>
                List.tryFind (fun item -> IsColliding player item)

        match isCollidingWithAny with
        | Some ent -> 
            let dir = CollisionDirection player ent
            if (dir = "Top" && player.velocity.y < 0) || (dir = "Bottom" && player.velocity.y > 0) then
                player.position.y <- if dir = "Top" then ent.position.y + 50 else ent.position.y - 50
                player.velocity.y <- 0
            else if (dir = "Left" && player.velocity.x < 0) || (dir = "Right" && player.velocity.x > 0) then
                player.position.x <- if dir = "Left" then ent.position.x + 50 else ent.position.x - 50
                player.velocity.x <- 0
        | None ->
            ignore 0


    override self.Draw(gameTime) =
        do self.GraphicsDevice.Clear Color.DarkSlateBlue

        let entities = OrderEntities npcs player

        spriteBatch.Begin()
        // BG
        spriteBatch.Draw(randomTexture, Rectangle(0, 418, 1024, 350), Color.DarkGray)
        // Player
        for entity in entities do
            let (isPlayer, entityProps) =
                match entity with
                | PlayerEntity ent -> (true, ent)
                | NPCEntity ent -> (false, ent)

            spriteBatch.Draw
                (randomTexture, Rectangle(entityProps.position.x, entityProps.position.y, 50, 50),
                 (if isPlayer then Color.White else Color.Red))

        spriteBatch.End()
