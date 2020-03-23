module Map.Types

type MapSprite = string

type MapMovementDirection =
    | Left
    | Right
    | Top
    | Down

type MovementTarget = MapMovementDirection * int

type MapStage =
    { id: int
      position: Entity.Types.EntityPosition
      size: Entity.Types.Size
      sprite: MapSprite
      available: bool
      area: Stage.Types.Area
      movements: MovementTarget list }

type MapCamera =
    { x: int
      y: int }

type MapState =
    { sprites: Stage.Types.StaticSprite list
      stages: MapStage list
      camera: MapCamera
      mutable activeStage: int }
