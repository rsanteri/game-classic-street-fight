module ResourceManager

open Microsoft.Xna.Framework.Content
open Microsoft.Xna.Framework.Graphics
open Microsoft.Xna.Framework.Audio

// Fonts

type FontBank =
    { mutable fdefault: SpriteFont
      mutable console: SpriteFont }

let fontBank: FontBank =
    { fdefault = Unchecked.defaultof<SpriteFont>
      console = Unchecked.defaultof<SpriteFont> }

let getFont a =
    match a with
    | "default" -> fontBank.fdefault
    | "console" -> fontBank.console
    | _ -> fontBank.fdefault

// Sprites

type SpriteBank =
    { mutable box: Texture2D
      mutable citybg: Texture2D
      mutable citybuildings: Texture2D
      mutable cityview: Texture2D }

let spriteBank: SpriteBank =
    { box = Unchecked.defaultof<Texture2D>
      citybg = Unchecked.defaultof<Texture2D>
      citybuildings = Unchecked.defaultof<Texture2D>
      cityview = Unchecked.defaultof<Texture2D> }

let getSprite (a) =
    match a with
    | "box" -> spriteBank.box

    | "citybg" -> spriteBank.citybg
    | "citybuildings" -> spriteBank.citybuildings
    | "cityview" -> spriteBank.cityview
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
    spriteBank.citybg <- contentManager.Load<Texture2D>("city_bg")
    spriteBank.citybuildings <- contentManager.Load<Texture2D>("city_buildings")
    spriteBank.cityview <- contentManager.Load<Texture2D>("0_cityview_crop")

    fontBank.fdefault <- contentManager.Load<SpriteFont>("OdibeeSans")
    fontBank.console <- contentManager.Load<SpriteFont>("Inconsolata")
(*     soundBank.music <- contentManager.Load<SoundEffect>("beat")
    soundBank.move <- contentManager.Load<SoundEffect>("move_click")
    soundBank.goal <- contentManager.Load<SoundEffect>("goal_sound")
    soundBank.error <- contentManager.Load<SoundEffect>("move_error")
    soundBank.pressSwitch <- contentManager.Load<SoundEffect>("press") *)

