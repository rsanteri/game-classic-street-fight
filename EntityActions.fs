module EntityActions

open EntityTypes
open Entity
open EntityMovement

///
/// Frame update for entity action effects.
/// 

///
/// Frame update for hit. Move hand forward and check if there is collisions
/// 
let HitAction entity entities =
    let hand = entity.body.hand 
    if abs (hand.position.x - hand.basePosition.x) > hand.distance then 
        entity.action <- HitRecovery
    else 
        hand.velocity.x <- hand.velocity.x + (if entity.facing = Right then 10 else -10)
    
    hand.position <- CalculatePosition hand.velocity hand.position 30
    let handWithPos = { hand with position = BodyPartPosition entity entity.body.hand}
    let colliding = List.tryFind (fun npc -> IsBodyPartColliding handWithPos npc) entities
    
    match colliding with
    | Some npc ->
        npc.properties.health <- npc.properties.health - 1
    | None ->
        ignore 0

///
/// Frame update for hit recovery. Return hand to base position
/// 
let HitRecoveryAction entity =
    let hand = entity.body.hand 
    // if (entity.facing = Right && hand.position.x <= hand.basePosition.x) || (entity.facing = Left && hand.position.x >= hand.basePosition.x) then
    if abs (hand.position.x - hand.basePosition.x) < 10 then
        hand.position.x <- hand.basePosition.x
        hand.velocity.x <- 0
        entity.action <- NoOp
    else
        hand.velocity.x <- hand.velocity.x + (if hand.position.x > hand.basePosition.x then -10 else 10)
    
    hand.position <- CalculatePosition hand.velocity hand.position 10

///
/// Passive body part recovery for situations where bodyparts are not where they should be for some reason.
/// 
let BodyPartRecovery entity =
    let hand = entity.body.hand
    if hand.position.x <> hand.basePosition.x then
        HitRecoveryAction entity

///
/// Handle frame update for action if one exists
/// 
let HandleEntityAction entity entities =
    match entity.action with
    | NoOp -> 
        BodyPartRecovery entity
    | Hit ->
        HitAction entity entities
    | HitRecovery -> HitRecoveryAction entity
