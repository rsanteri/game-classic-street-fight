module Stage

open Microsoft.Xna.Framework.Graphics
open EntityTypes

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

type Stage =
    { /// Define length of stage
      size : int
      /// Store active entities (NPCs)
      mutable entities : Entity list
      /// Define list of triggers.
      triggers : Trigger list
      /// Define graphics for different layers of stage
      layers : StageLayers }