module Stage

open Microsoft.Xna.Framework
open Microsoft.Xna.Framework.Graphics
open EntityTypes
open AITypes

type TriggerAction =
    | LockCamera of int
    | UnlockCamera
    | AddEntity of Entity

type Trigger =
    { x: int
      mutable active: bool
      action: TriggerAction }

type StaticSprite =
    { sprite: string
      position: EntityPosition
      size: Size }

type DrawLayer =
    { sprites: StaticSprite list
      distance: int }

type StageLayers =
    { bg1: DrawLayer
      bg2: DrawLayer
      street: StaticSprite list
      foreground: StaticSprite list }

///
/// Stage should present just the static map. Where to hold ai?
///
type Stage =
    { /// Define length of stage
      size: int
      /// Define list of triggers.
      triggers: Trigger list
      /// Define graphics for different layers of stage
      layers: StageLayers }

type GameState =
    | Playing
    | Paused
    | OnMenu

type StageController =
    { mutable state: GameState
      camera: Camera.XCamera
      player: Entity
      mutable entities: EntityController list }

///
/// Default values
///

let defaultMap() =
    { size = 2000
      triggers =
          [ { x = 300
              active = true
              action =
                  AddEntity
                      (Entity.DefaultHumanoid
                          { x = 1024
                            y = 500 }) }
            
            { x = 1000
              active = true
              action =
                  AddEntity
                      (Entity.DefaultHumanoid
                          { x = 0
                            y = 500 }) } ]
      layers =
          { bg1 =
                { sprites =
                      [ { sprite = "citybg"
                          position =
                              { x = 0
                                y = 0 }
                          size =
                              { width = 1361
                                height = 600 } } ]
                  distance = 10 }
            bg2 =
                { sprites =
                      [ { sprite = "citybuildings"
                          position =
                              { x = 0
                                y = -100 }
                          size =
                              { width = 1361
                                height = 600 } } ]
                  distance = 30 }
            street = []
            foreground = [] } }

let defaultMapController() =
    { state = Playing
      camera =
          { x = 0
            locked = false }
      player =
          Entity.DefaultHumanoid
              { x = 200
                y = 500 }
      entities = [] }

///
/// Order entities by position.y so those that are more down are rendered ON the entities more up
///
let OrderEntities(map: StageController) =
    let entities = List.map (fun i -> i.entity) map.entities

    entities
    |> List.map NPCEntity
    |> List.append [ PlayerEntity map.player ]
    |> List.sortWith (fun a b ->
        let ae =
            match a with
            | PlayerEntity ent -> ent
            | NPCEntity ent -> ent

        let be =
            match b with
            | PlayerEntity ent -> ent
            | NPCEntity ent -> ent

        ae.position.y - be.position.y)

///
/// Stage drawing
///

let drawLayer (spriteBatch: SpriteBatch) (layer: DrawLayer) (camera: Camera.XCamera) =
    for sprite in layer.sprites do
        spriteBatch.Draw
            (ResourceManager.getSprite sprite.sprite,
             Camera.ToParallaxPosition camera
                 (Rectangle(sprite.position.x, sprite.position.y, sprite.size.width, sprite.size.height))
                 layer.distance, Color.White)

let renderStage (spriteBatch: SpriteBatch) (stage: Stage) (camera: Camera.XCamera) =
    drawLayer spriteBatch stage.layers.bg1 camera
    drawLayer spriteBatch stage.layers.bg2 camera
    // Street
    spriteBatch.Draw
        (ResourceManager.getSprite "default", Camera.ToCameraPosition camera (Rectangle(0, 418, stage.size * 2, 350)),
         Color.DarkGray)


///
/// Stage events
///

let ApplyTrigers (map: Stage) (mapcontroller: StageController) =
    for trigger in map.triggers do
        if trigger.active && mapcontroller.player.position.x > trigger.x then
            match trigger.action with
            | AddEntity entity ->
                // Add new entity to mapcontroller
                mapcontroller.entities <-
                    List.append mapcontroller.entities
                        [ { entity = entity
                            brain =
                                { dormant = false
                                  decision = MoveNextTo mapcontroller.player
                                  nextDecision = 5000 } } ]
                
                trigger.active <- false

            | LockCamera x -> ()
            | UnlockCamera -> ()
        else
            ()
