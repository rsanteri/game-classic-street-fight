module Entity

open Microsoft.Xna.Framework
open Microsoft.Xna.Framework.Graphics

open AITypes
open EntityTypes

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
      action = NoOp}

///
/// Order entities by position.y so those that are more down are rendered ON the entities more up
/// 
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

///
/// Right now hitbox should always be bottom part of entity so if sprite is bigger than hitbox, we need 
/// to move it up
/// 
let GetSpritePosition(entity: Entity) =
    if entity.size.original = entity.size.sprite then
        entity.position
    else
        { x = entity.position.x + (entity.size.original.width - entity.size.sprite.width)
          y = entity.position.y + (entity.size.original.height - entity.size.sprite.height) }

///
/// Body part position is relative to its parent entity so realy position is entity.position + part.position 
/// 
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
