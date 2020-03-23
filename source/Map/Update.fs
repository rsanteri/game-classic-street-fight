module Map.Update

open Microsoft.Xna.Framework.Input

open Map.Types

let baseStage id position area movements: MapStage =
    { id = id
      position = position
      size =
          { width = 100
            height = 100 }
      sprite = "default"
      available = true
      area = area
      movements = movements }

let private stages: MapStage list =
    [ baseStage 0
          { x = 100
            y = 100 } Stage.Types.Area.Street1 [ (Right, 1) ]
      baseStage 1
          { x = 300
            y = 100 } Stage.Types.Area.Street2 [ (Left, 0) ] ]

let initMap() =
    { sprites = []
      stages = stages
      camera =
          { x = 0
            y = 0 }
      activeStage = 0 }

let mapKeyToDir key: MapMovementDirection =
    match key with
    | Keys.A -> Left
    | Keys.D -> Right
    | Keys.W -> Top
    | Keys.S -> Down
    | _ -> Left

let update (mapState: MapState) (state: Global.GlobalState) (ioactions: InputHelper.IOActionState) =
    ///
    /// Handle possible movement in map (click left, right, up, down)
    ///
    let directionClick = ioactions.pressedAnyOf [ Keys.A; Keys.W; Keys.D; Keys.S ]
    match directionClick with
    | Some key ->
        let activeStage = List.find (fun stage -> stage.id = mapState.activeStage) mapState.stages
        let direction = mapKeyToDir key
        let next = List.tryFind (fun (dir, _) -> dir = direction) activeStage.movements

        match next with
        | Some(_, id) -> mapState.activeStage <- id
        | None -> ()
    | None -> ()
    ///
    /// Handle selection of stage
    ///
    if ioactions.pressed Keys.Enter then
        let activeStage = List.find (fun stage -> stage.id = mapState.activeStage) mapState.stages

        state.state <- Global.ApplicationState.InGame(Areas.init activeStage.area)
