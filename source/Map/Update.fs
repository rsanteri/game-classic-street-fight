module Map.Update

type MapSprite = string


type MapStage =
    { position: EntityTypes.EntityPosition
      size: EntityTypes.Size
      sprite: MapSprite
      available: bool }

type MapCamera =
    { x: int
      y: int }

type MapState =
    { sprites: StageTypes.StaticSprite list
      stages: MapStage list
      camera: MapCamera }

let update() = ()
