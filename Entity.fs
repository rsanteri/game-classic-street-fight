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
    { health: int }

type Entity =
    { id: int
      mutable position: EntityPosition
      size: EntitySize
      velocity: EntityVelocity
      velocitySpeed: int
      speed: int
      properties: EntityProperties }

type RenderableEntity =
    | PlayerEntity of Entity
    | NPCEntity of Entity

let mutable entityIndex = 0

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
      properties = { health = 10 } }


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

    // Draw health
    for i in 0 .. entityProps.properties.health do
        spriteBatch.Draw
            (texture,
             Rectangle
                 (entityProps.position.x + (i * 4), entityProps.position.y + entityProps.size.original.height + 5, 3, 10),
             Color.LightGreen)