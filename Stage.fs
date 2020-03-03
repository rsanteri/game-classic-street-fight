module Stage

open Microsoft.Xna.Framework.Graphics
open EntityTypes
open AITypes

type TriggerAction =
    | LockCamera of int
    | UnlockCamera
    | AddEntity of Entity

type Trigger =
    { x : int; mutable active : bool; action: TriggerAction }

type StaticSprite =
    { sprite : Texture2D; position : EntityPosition; size: Size }

type DrawLayer = 
    { sprites : StaticSprite list; distance : int }

type StageLayers = 
    { bg1 : DrawLayer; street : StaticSprite list; foreground : StaticSprite list }

///
/// Stage should present just the static map. Where to hold ai?
/// 
type Stage =
    { /// Define length of stage
      size : int
      /// Define list of triggers.
      triggers : Trigger list
      /// Define graphics for different layers of stage
      layers : StageLayers }

type GameState = 
    | Playing
    | Paused
    | OnMenu

type StageController = 
    { mutable state : GameState
      camera : Camera.XCamera
      player : Entity
      mutable entities : EntityController list }

let defaultMap() =
    {
        size = 2000
        triggers = []
        layers = {
            bg1 = { sprites = []; distance = 50}
            street = []
            foreground = []
        }
    }

let defaultMapController() =
    { state = Playing; camera = { x = 0; locked = false }; player = Entity.DefaultHumanoid { x = 200; y = 500 }; entities = [] }

///
/// Order entities by position.y so those that are more down are rendered ON the entities more up
///
let OrderEntities (map: StageController) =
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