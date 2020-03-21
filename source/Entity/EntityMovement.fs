module EntityMovement

open Entity.Types
open System.Diagnostics

///
/// Add velocity within the limit.
/// NOTE: Need to think is this kind of limitation good. For example for impacts we might want yuge velocity
///
let AddVelocity (current: int) (add: int) =
    let newVelocity = current + add

    if newVelocity < -100 then -100
    else if newVelocity > 100 then 100
    else newVelocity

///
/// Slow down velocity when there is no input
///
let NaturalSlowVelocity(current: int): int =
    if current = 0 then 0
    else if current > 0 then if current - 10 < 0 then 0 else current - 10
    else if current + 10 > 0 then 0
    else current + 10

///
/// Calculate position according position, velocity and speed
/// newposition = position + ( velocity * speed )
///
let CalculatePosition (velocity: EntityVelocity) (position: EntityPosition) (speed: int): EntityPosition =
    let x = float32 position.x + (float32 (velocity.x) / 100.0f * float32 speed)
    let y = float32 position.y + (float32 (velocity.y) / 100.0f * (float32 speed / 2.0f))

    { x = int x
      y = int y }

///
/// Check that position is within playground.
/// NOTE: This wont work with moving camera and values should come from some global state
/// management
///
let WithinBoundary(pos: EntityPosition): EntityPosition =
    let x =
        if pos.x < 0 then 0
        else if pos.x > 2000 - 50 then 2000 - 50
        else pos.x

    let y =
        if pos.y < 418 then 418
        else if pos.y > 768 - 25 then 768 - 25
        else pos.y

    { x = x
      y = y }


let IsLeftCollision (a: EntityPosition) (b: EntityPosition) (bsize: Size) = a.x <= b.x + bsize.width
let IsRightCollision (a: EntityPosition) (b: EntityPosition) (asize: Size) = a.x + asize.width >= b.x
let IsTopCollision (a: EntityPosition) (b: EntityPosition) (bsize: Size) = a.y <= b.y + bsize.height
let IsBottomCollision (a: EntityPosition) (b: EntityPosition) (asize: Size) = a.y + asize.height >= b.y

///
/// Check collision between two entities
///
let IsColliding (a: Entity) (b: Entity) =
    IsLeftCollision a.position b.position b.size.original && IsRightCollision a.position b.position a.size.original
    && IsTopCollision a.position b.position b.size.original
    && IsBottomCollision a.position b.position a.size.original

///
/// Check collision between bodypart and entity
///
let IsBodyPartColliding (a: EntityBodyPart) (b: Entity) =
    IsLeftCollision a.position b.position b.size.original && IsRightCollision a.position b.position a.size
    && IsTopCollision a.position b.position b.size.original && IsBottomCollision a.position b.position a.size

///
/// Try to find out collision direction (Where did it come from)
/// Now using overlapping amount as comparing value.
///
let CollisionDirection a b =
    let leftCollision = a.position.x - (b.position.x + b.size.original.width)
    let rightCollision = b.position.x - (a.position.x + a.size.original.width)
    let topCollision = a.position.y - (b.position.y + b.size.original.height)
    let bottomCollision = b.position.y - (a.position.y + a.size.original.height)

    let (_, dir) =
        [ (leftCollision, "Left")
          (rightCollision, "Right")
          (topCollision, "Top")
          (bottomCollision, "Bottom") ]
        |> List.sortWith (fun (valA, _) (valB, _) -> valB - valA)
        |> List.head

    dir

///
/// Frame update function for entity movement updates position (and velocity if there was collision);
///
let MoveEntity (entity: Entity) (entities: Entity list) =
    entity.position <- WithinBoundary(CalculatePosition entity.velocity entity.position entity.speed)

    let collisions = entities |> List.filter (fun item -> IsColliding entity item)

    for collision in collisions do
        let dir = CollisionDirection entity collision
        if (dir = "Top" && entity.velocity.y < 0) || (dir = "Bottom" && entity.velocity.y > 0) then
            entity.position.y <-
                if dir = "Top"
                then collision.position.y + collision.size.original.height
                else collision.position.y - collision.size.original.height
            entity.velocity.y <- 0
        else if (dir = "Left" && entity.velocity.x < 0) || (dir = "Right" && entity.velocity.x > 0) then
            entity.position.x <-
                if dir = "Left"
                then collision.position.x + collision.size.original.width
                else collision.position.x - collision.size.original.width
            entity.velocity.x <- 0
