module Entity

open Microsoft.Xna.Framework
open Microsoft.Xna.Framework.Graphics

type EntityPosition =
    { mutable x: int
      mutable y: int }

type EntityVelocity =
    { mutable x: int
      mutable y: int }

type Size =
    { width: int
      height: int }

type EntitySize =
    { original: Size // Hitbox size
      sprite: Size } // Sprite size

type EntityProperties =
    { mutable health: int }

type EntityBodyPart =
    { mutable position: EntityPosition
      basePosition: EntityPosition
      velocity: EntityVelocity
      distance: int
      size: Size }

type EntityBodyParts =
    { hand: EntityBodyPart }

type EntityAction =
    | NoOp
    | Hit
    | HitRecovery

type Facing = 
    | Left
    | Right

type Entity =
    { id: int
      mutable position: EntityPosition
      // Original (hitbox) size and sprite size.
      size: EntitySize
      // Value between -100 and 100 for x and y axis to get movement speed
      velocity: EntityVelocity
      // Speed how fast velocity accumulates. for example
      // - velocitySpeed = 10 would take 10 frames to reach maximum speed. 
      // - velocitySpeed = 100 would take 1 frames to reach maximum speed. 
      velocitySpeed: int
      // Maximum speed`
      speed: int
      // Game attributes
      properties: EntityProperties
      // How should i do this?
      body: EntityBodyParts
      mutable facing : Facing
      // Active action. Only one at a time.
      mutable action: EntityAction }

type RenderableEntity =
    | PlayerEntity of Entity
    | NPCEntity of Entity

let mutable entityIndex = 0

let DefaulBodyPart pos =
    { position = pos
      basePosition = pos
      velocity =
          { x = 0
            y = 0 }
      distance = 30
      size =
          { width = 10
            height = 10 } }

let DefaultHumanoid(pos: EntityPosition): Entity =
    entityIndex <- entityIndex + 1
    { id = entityIndex
      position = pos
      size =
          { original =
                { width = 50
                  height = 25 }
            sprite =
                { width = 50
                  height = 50 } }
      velocity =
          { x = 0
            y = 0 }
      velocitySpeed = 10
      speed = 15
      properties = { health = 10 }
      body =
          { hand =
                DefaulBodyPart
                    { x = 25
                      y = 0 } }
      facing = Right
      action = NoOp }


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

let GetSpritePosition(entity: Entity) =
    if entity.size.original = entity.size.sprite then
        entity.position
    else
        { x = entity.position.x + (entity.size.original.width - entity.size.sprite.width)
          y = entity.position.y + (entity.size.original.height - entity.size.sprite.height) }


let BodyPartPosition (entity: Entity) (part: EntityBodyPart): EntityPosition =
    { x = entity.position.x + part.position.x
      y = entity.position.y + part.position.y }

///
/// Draw entity
///
let DrawEntity (spriteBatch: SpriteBatch) (entity: RenderableEntity) (texture: Texture2D) =
    let (isPlayer, entityProps) =
        match entity with
        | PlayerEntity ent -> (true, ent)
        | NPCEntity ent -> (false, ent)

    // Draw Sprite
    let sritePosition = GetSpritePosition entityProps

    spriteBatch.Draw
        (texture,
         Rectangle(sritePosition.x, sritePosition.y, entityProps.size.sprite.width, entityProps.size.sprite.height),
         (if isPlayer then Color.White else Color.Red))

    // Draw hitbox
    spriteBatch.Draw
        (texture,
         Rectangle
             (entityProps.position.x, entityProps.position.y, entityProps.size.original.width,
              entityProps.size.original.height), Color.LightBlue * 0.5f)

    // Draw body parts
    let handPos = BodyPartPosition entityProps entityProps.body.hand
    spriteBatch.Draw
        (texture, Rectangle(handPos.x, handPos.y, entityProps.body.hand.size.width, entityProps.body.hand.size.height),
         Color.DarkBlue)

    // Draw health
    for i in 0 .. entityProps.properties.health do
        spriteBatch.Draw
            (texture, Rectangle(entityProps.position.x + (i * 5), sritePosition.y - 15, 4, 10), Color.LightGreen)
