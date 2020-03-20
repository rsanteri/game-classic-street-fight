module StageTypes

open EntityTypes

///
/// Camera
///

type XCamera =
    { mutable x: int
      mutable locked: bool }

///
/// Stage
///

type Area =
    | Street
    | Map

type TriggerAction =
    | LockCamera of int
    | UnlockCamera
    | AddEntity of Entity
    | Exit of int * Area

type Trigger =
    { x: int * int
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

type StageState =
    | Entering of int
    | Normal
    /// Payload progress * target
    | Exiting of int * int
    | ExitNow

type StageController =
    { mutable gameState: GameState
      mutable stageState: StageState
      camera: XCamera
      player: Entity
      mutable entities: AITypes.EntityController list }
