module EntityMovement

open Entity
open System.Diagnostics

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


let IsLeftCollision a b = a.position.x <= b.position.x + b.size.original.width
let IsRightCollision a b = a.position.x + a.size.original.width >= b.position.x
let IsTopCollision a b = a.position.y <= b.position.y + b.size.original.height
let IsBottomCollision a b = a.position.y + a.size.original.height >= b.position.y

let IsColliding (a: Entity) (b: Entity) =
    IsLeftCollision a b && IsRightCollision a b && IsTopCollision a b && IsBottomCollision a b

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


let TopLeft a = (a.position.x, a.position.y)
let TopRight a = (a.position.x + a.size.original.width, a.position.y)
let BottomLeft a = (a.position.x, a.position.y + a.size.original.height)
let BottomRight a = (a.position.x + a.size.original.width, a.position.y + a.size.original.height)

let IsWithin (x , y) entity =
    x > entity.position.x && 
    x < entity.position.x + entity.size.original.width &&
    y > entity.position.y &&
    y < entity.position.y + entity.size.original.height

// let IsInArea (x, y) entity =


let AnotherColliderCalculation a b =
    let ( aTLx, aTLy) = (a.position.x, a.position.y)
    let ( aBRx, aBRy ) = (a.position.x + a.size.original.width, a.position.y + a.size.original.height)

    let ( bTLx, bTLy) = (b.position.x, b.position.y)
    let ( bBRx, bBRy ) = (b.position.x + b.size.original.width, b.position.y + b.size.original.height)

    if aTLx > bTLx && aTLy > bTLy && aTLx < bBRx && aTLy < bBRy then
        if aTLx - bBRx > aTLy - bBRy then "Left" else "Top"
    else 
        if bTLx - aBRx > bTLy - aBRy then "Right" else "Bottom"


///
/// Update function
///

let MoveEntity (entity: Entity) (entities: Entity list) =
    let newPosition = WithinBoundary(CalculatePosition entity.velocity entity.position entity.speed)
    let newEntity = { entity with position = newPosition }

    let collisions = entities |> List.filter (fun item -> IsColliding newEntity item)

    entity.position <- newPosition

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
