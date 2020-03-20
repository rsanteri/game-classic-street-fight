module Entity

open Microsoft.Xna.Framework
open Microsoft.Xna.Framework.Graphics

open AITypes
open EntityTypes

let mutable entityIndex = 0

let DefaulBodyPart (pos: EntityPosition) (distance: int) (size: int): EntityBodyPart =
    { position = pos
      basePosition = pos
      velocity =
          { x = 0
            y = 0 }
      distance = distance
      size =
          { width = size
            height = size } }

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
      speed = 10
      properties = { health = 10 }
      body =
          { hand =
                DefaulBodyPart
                    { x = 25
                      y = 0 } 25 10
            lleg =
                DefaulBodyPart
                    { x = 5
                      y = 20 } 10 15
            rleg =
                DefaulBodyPart
                    { x = 30
                      y = 20 } 10 15 }
      facing = Right
      walking = NotWalking
      action = NoOp }

///
/// Right now hitbox should always be bottom part of entity so if sprite is bigger than hitbox, we need
/// to move it up
///
let GetSpritePosition(entity: Entity): EntityPosition =
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
let DrawEntity (spriteBatch: SpriteBatch) (entity: RenderableEntity) (toCameraPos: Rectangle -> Rectangle) =
    let (isPlayer, entityProps) =
        match entity with
        | PlayerEntity ent -> (true, ent)
        | NPCEntity ent -> (false, ent)

    // Draw Sprite
    let sritePosition = GetSpritePosition entityProps
    let defaultSprite = ResourceManager.getSprite "default"

    spriteBatch.Draw
        (defaultSprite,
         toCameraPos
             (Rectangle(sritePosition.x, sritePosition.y, entityProps.size.sprite.width, entityProps.size.sprite.height)),
         (if isPlayer then Color.White else Color.Red))

    // Draw hitbox
    spriteBatch.Draw
        (defaultSprite,
         toCameraPos
             (Rectangle
                 (entityProps.position.x, entityProps.position.y, entityProps.size.original.width,
                  entityProps.size.original.height)), Color.LightBlue * 0.5f)

    // Draw body parts
    let handPos = BodyPartPosition entityProps entityProps.body.hand
    spriteBatch.Draw
        (defaultSprite,
         toCameraPos
             (Rectangle(handPos.x, handPos.y, entityProps.body.hand.size.width, entityProps.body.hand.size.height)),
         Color.DarkBlue)

    let llegPos = BodyPartPosition entityProps entityProps.body.lleg
    let rlegPos = BodyPartPosition entityProps entityProps.body.rleg

    spriteBatch.Draw
        (defaultSprite,
         toCameraPos
             (Rectangle(llegPos.x, llegPos.y, entityProps.body.lleg.size.width, entityProps.body.lleg.size.height)),
         Color.SandyBrown)

    spriteBatch.Draw
        (defaultSprite,
         toCameraPos
             (Rectangle(rlegPos.x, rlegPos.y, entityProps.body.rleg.size.width, entityProps.body.rleg.size.height)),
         Color.SandyBrown)

    // Draw health
    for i in 0 .. entityProps.properties.health do
        spriteBatch.Draw
            (defaultSprite, toCameraPos (Rectangle(entityProps.position.x + (i * 5), sritePosition.y - 15, 4, 10)),
             Color.LightGreen)
