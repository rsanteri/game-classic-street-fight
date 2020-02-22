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
    brain.decision <- MoveTo (0, 0)

///
/// Operates AI controlled units
/// If there is no decision, make one and act by that decision
/// 
let OperateNPC (entityController: EntityController) (entities: Entity list) (frameTime: int) =
    let brain = entityController.brain
    let entity = entityController.entity

    if ShouldMakeDecision brain frameTime then
        MakeDecision brain entity entities

    match brain.decision with
    | Slack -> ignore 0
    | MoveTo (x, y) ->
        let radians = atan2 (float (x - entity.position.x)) (float (entity.position.y - y))
        let degrees = (radians + 6.2831853071795865) * 57.2957795130823209;
        
        entity.velocity.x <- AddVelocity entity.velocity.x  (int (float entity.velocitySpeed *  cos degrees))
        entity.velocity.y <- AddVelocity entity.velocity.y (int (float entity.velocitySpeed * sin degrees))
    | MoveNextTo ent ->
        ignore 0