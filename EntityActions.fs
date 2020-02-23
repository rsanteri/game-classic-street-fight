module EntityActions

open EntityTypes
open Entity
open EntityMovement

///
/// Helpers
///

// Todo: Fix this to be radial check instead of box
let IsBodyPartTooFar (bodypart: EntityBodyPart) =
    abs (bodypart.position.x - bodypart.basePosition.x) > bodypart.distance ||
    abs (bodypart.position.y - bodypart.basePosition.y) > bodypart.distance

let IsBasePosition (bodypart: EntityBodyPart) =
    bodypart.position = bodypart.basePosition

///
/// Frame update for hit. Move hand forward and check if there is collisions
///

let HitAction entity entities =
    let hand = entity.body.hand 
    
    if IsBodyPartTooFar hand then 
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

let UpdateLegMovement entity isLeftLegUp =
    let lleg = entity.body.lleg
    let rleg = entity.body.rleg

    let leftTooFar = IsBodyPartTooFar entity.body.lleg
    let rightTooFar = IsBodyPartTooFar entity.body.rleg

    if leftTooFar then
        lleg.velocity.y <- 0
    else 
        lleg.velocity.y <- lleg.velocity.y + (if isLeftLegUp then -3 else 3)

    if rightTooFar then
        rleg.velocity.y <- 0
    else
        rleg.velocity.y <- rleg.velocity.y + (if isLeftLegUp then 3 else -3)
    
    if leftTooFar && rightTooFar then
        lleg.position.y <- lleg.basePosition.y + if isLeftLegUp then -lleg.distance else lleg.distance
        rleg.position.y <- rleg.basePosition.y + if isLeftLegUp then rleg.distance else -rleg.distance
        // Set another leg to go up
        entity.walking <- if isLeftLegUp then RightLegUp else LeftLegUp
    else
        // Update position
        lleg.position <- CalculatePosition lleg.velocity lleg.position 10
        rleg.position <- CalculatePosition rleg.velocity rleg.position 10

let UpdateEntityWalking (entity: Entity) =
    let shouldBeWalking = entity.velocity.x <> 0 || entity.velocity.y <> 0

    /// Animate / start animation when we have some velocity
    if shouldBeWalking then
        match entity.walking with
        | NotWalking ->
            entity.walking <- LeftLegUp
        | LeftLegUp ->
            UpdateLegMovement entity true
        | RightLegUp ->
            UpdateLegMovement entity false
    else
        entity.walking <- NotWalking

        /// Return left leg to base position
        if not (IsBasePosition entity.body.lleg) then
            entity.body.lleg.position <- entity.body.lleg.basePosition
        
        /// Return right leg to base position
        if not (IsBasePosition entity.body.rleg) then
            entity.body.rleg.position <- entity.body.rleg.basePosition
        
    ()