module Map.Types

type MapSprite = string

type MapStage =
    { id: int
      position: Entity.Types.EntityPosition
      size: Entity.Types.Size
      sprite: MapSprite
      available: bool
      area: Stage.Types.Area }

type MapCamera =
    { x: int
      y: int }

type MapState =
    { sprites: Stage.Types.StaticSprite list
      stages: MapStage list
      camera: MapCamera
      activeStage: int }
