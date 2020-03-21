module Street.Area1

open StageTypes

let initialMap(): Stage =
    { area = Street1
      size = 2000
      triggers =
          [ { x = (300, 400)
              active = true
              action =
                  AddEntity
                      (Entity.DefaultHumanoid
                          { x = 1024
                            y = 500 }) }

            { x = (1000, 1100)
              active = true
              action =
                  AddEntity
                      (Entity.DefaultHumanoid
                          { x = 0
                            y = 500 }) }
            /// Exit to back
            { x = (0, 50)
              active = true
              action = Exit(-100, Area.Map) }
            /// Exit to forward
            { x = (1900, 2000)
              active = true
              action = Exit(2100, Area.Street2) } ]
      layers =
          { bg =
                [ { sprites =
                        [ { sprite = "citybg"
                            position =
                                { x = 0
                                  y = 0 }
                            size =
                                { width = 1361
                                  height = 600 } } ]
                    distance = 10 }

                  { sprites =
                        [ { sprite = "citybuildings"
                            position =
                                { x = 0
                                  y = -100 }
                            size =
                                { width = 1361
                                  height = 600 } } ]
                    distance = 30 } ]
            street = []
            foreground = [] } }

let initialMapController(): StageController =
    { gameState = Playing
      stageState = Entering 100
      camera =
          { x = 0
            locked = false }
      player =
          Entity.DefaultHumanoid
              { x = -100
                y = 500 }
      entities = [] }

let init() = (initialMap(), initialMapController())
