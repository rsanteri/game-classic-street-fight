module ResourceManager

open Microsoft.Xna.Framework.Content
open Microsoft.Xna.Framework.Graphics
open Microsoft.Xna.Framework.Audio

// Fonts

type FontBank =
    { mutable fdefault: SpriteFont
      mutable console: SpriteFont }

let private fontBank: FontBank =
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

let private spriteBank: SpriteBank =
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

type SoundBank =
    { mutable footstep: SoundEffect }

let private soundBank: SoundBank = { footstep = Unchecked.defaultof<SoundEffect> }

let getSound a =
    match a with
    | "footstep" -> soundBank.footstep
    | _ -> soundBank.footstep

// Resource loading

let loadResources (contentManager: ContentManager) =
    spriteBank.box <- contentManager.Load<Texture2D>("floor")
    spriteBank.citybg <- contentManager.Load<Texture2D>("city_bg")
    spriteBank.citybuildings <- contentManager.Load<Texture2D>("city_buildings")
    spriteBank.cityview <- contentManager.Load<Texture2D>("0_cityview_crop")

    fontBank.fdefault <- contentManager.Load<SpriteFont>("OdibeeSans")
    fontBank.console <- contentManager.Load<SpriteFont>("Inconsolata")

    soundBank.footstep <- contentManager.Load<SoundEffect>("footstep1")
