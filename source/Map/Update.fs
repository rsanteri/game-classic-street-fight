module Map.Update

open Map.Types

let private stages: MapStage list =
    [ { id = 0
        position =
            { x = 100
              y = 100 }
        size =
            { width = 100
              height = 100 }
        sprite = "default"
        available = true
        area = Stage.Types.Area.Street1 }
      { id = 2
        position =
            { x = 300
              y = 100 }
        size =
            { width = 100
              height = 100 }
        sprite = "default"
        available = true
        area = Stage.Types.Area.Street2 } ]

let initMap() =
    { sprites = []
      stages = stages
      camera =
          { x = 0
            y = 0 }
      activeStage = 0 }

let update (mapState: MapState) = ()
