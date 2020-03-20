module EntityTypes

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
    { /// Real position relative to entity
      mutable position: EntityPosition
      /// Position where this body part is attached.
      mutable basePosition: EntityPosition
      /// Velocity for part movement
      velocity: EntityVelocity
      /// Maximum distance from base position
      distance: int
      /// Size. Body parts doesnt have separated sprite size (maybe they should)
      size: Size }

type EntityBodyParts =
    { hand: EntityBodyPart
      lleg: EntityBodyPart
      rleg: EntityBodyPart }

type EntityAction =
    | NoOp
    | Hit
    | HitRecovery

type EntityWalking =
    | NotWalking
    | LeftLegUp
    | RightLegUp

type Facing =
    | Left
    | Right

type Entity =
    { id: int
      mutable position: EntityPosition
      /// Original (hitbox) size and sprite size.
      size: EntitySize
      /// Value between -100 and 100 for x and y axis to get movement speed
      velocity: EntityVelocity
      /// Speed how fast velocity accumulates. for example
      /// - velocitySpeed = 10 would take 10 frames to reach maximum speed.
      /// - velocitySpeed = 100 would take 1 frames to reach maximum speed.
      velocitySpeed: int
      /// Maximum speed`
      speed: int
      /// Game attributes
      properties: EntityProperties
      /// How should i do this?
      body: EntityBodyParts
      mutable walking: EntityWalking
      /// Which way entity is facing?
      mutable facing: Facing
      /// Active action. Only one at a time.
      mutable action: EntityAction }

type RenderableEntity =
    | PlayerEntity of Entity
    | NPCEntity of Entity
