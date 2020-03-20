module AI

open AITypes
open EntityTypes
open EntityMovement
open System.Diagnostics

///
/// Checks if should make new decision.
///
let ShouldMakeDecision (brain: Brain) (frameTime: int): bool =
    if brain.nextDecision - frameTime < 0 then
        true
    else
        brain.nextDecision <- brain.nextDecision - frameTime
        false

///
/// Makes decision
///
let MakeDecision (brain: Brain) (entity: Entity) (entities: Entity list) =
    brain.nextDecision <- 1000
    brain.decision <- MoveTo(0, 0)

let MoveAI (entity: Entity) (x, y) =
    let radians = atan2 (float (y - entity.position.y)) (float (x - entity.position.x))
    let degrees = radians % 360.0

    let isCloseEnoughX = abs (x - entity.position.x) < 10
    let isCloseEnoughY = abs (y - entity.position.y) < 10

    entity.velocity.x <-
        if isCloseEnoughX
        then 0
        else AddVelocity entity.velocity.x (int (float entity.velocitySpeed * cos degrees))
    entity.velocity.y <-
        if isCloseEnoughY
        then 0
        else AddVelocity entity.velocity.y (int (float entity.velocitySpeed * sin degrees))

    if entity.velocity.x > 0 && entity.facing = Left
    then entity.facing <- Right
    else if entity.velocity.x < 0 && entity.facing = Right
    then entity.facing <- Left

    isCloseEnoughX && isCloseEnoughY

///
/// Operates AI controlled units
/// If there is no decision, make one and act by that decision
///
let OperateNPC (entityController: EntityController) (entities: Entity list) (frameTime: int) =
    let brain = entityController.brain
    let entity = entityController.entity

    // if ShouldMakeDecision brain frameTime then
    //  MakeDecision brain entity entities

    match brain.decision with
    | Slack -> ignore 0
    | MoveTo target ->
        let closeEnough = MoveAI entity target
        if closeEnough then brain.decision <- Slack
    | MoveNextTo ent ->
        let targetAtLeftSide = ent.position.x < entity.position.x

        let target =
            if targetAtLeftSide
            then (ent.position.x + ent.size.original.width + 5, ent.position.y)
            else (ent.position.x - entity.size.original.width, ent.position.y)
        MoveAI entity target |> ignore

        if entity.action = NoOp then entity.action <- Hit
        ()
