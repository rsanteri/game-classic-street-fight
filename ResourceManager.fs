module ResourceManager

open Microsoft.Xna.Framework.Content
open Microsoft.Xna.Framework.Graphics
open Microsoft.Xna.Framework.Audio

// Fonts

type FontBank =
    { mutable fdefault: SpriteFont }

let fontBank: FontBank = { fdefault = Unchecked.defaultof<SpriteFont> }

let getFont a =
    match a with
    | "default" -> fontBank.fdefault
    | _ -> fontBank.fdefault

// Sprites

type SpriteBank =
    { mutable box: Texture2D }

let spriteBank: SpriteBank =
    { box = Unchecked.defaultof<Texture2D> }

let getSprite a =
    match a with
    | "box" -> spriteBank.box
    | _ -> spriteBank.box

// Sounds
(* 
type SoundBank =
    { mutable music: SoundEffect
      mutable move: SoundEffect
      mutable goal: SoundEffect
      mutable error: SoundEffect
      mutable pressSwitch: SoundEffect }

let soundBank: SoundBank =
    { music = Unchecked.defaultof<SoundEffect>
      goal = Unchecked.defaultof<SoundEffect>
      move = Unchecked.defaultof<SoundEffect>
      error = Unchecked.defaultof<SoundEffect>
      pressSwitch = Unchecked.defaultof<SoundEffect> }

let getSound a =
    match a with
    | "music" -> soundBank.music
    | "move" -> soundBank.move
    | "goal" -> soundBank.goal
    | "error" -> soundBank.error
    | "pressSwitch" -> soundBank.pressSwitch
    | _ -> soundBank.error

let playSound (a: string) = (getSound a).Play() |> ignore
 *)
// Resource loading

let loadResources (contentManager: ContentManager) =
    spriteBank.box <- contentManager.Load<Texture2D>("floor")

    fontBank.fdefault <- contentManager.Load<SpriteFont>("OdibeeSans")

(*     soundBank.music <- contentManager.Load<SoundEffect>("beat")
    soundBank.move <- contentManager.Load<SoundEffect>("move_click")
    soundBank.goal <- contentManager.Load<SoundEffect>("goal_sound")
    soundBank.error <- contentManager.Load<SoundEffect>("move_error")
    soundBank.pressSwitch <- contentManager.Load<SoundEffect>("press") *)
