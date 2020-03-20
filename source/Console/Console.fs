module Console

open Microsoft.Xna.Framework
open Microsoft.Xna.Framework.Input

type ConsoleItemType = | Basic

type ConsoleItem =
    { itemType: ConsoleItemType
      message: string
      time: System.DateTime }

type ConsoleModel =
    { mutable text: string
      mutable log: ConsoleItem list }

let console =
    { text = ""
      log =
          [ { itemType = Basic
              message = "System started"
              time = System.DateTime.Now } ] }

///
/// Mutators
///

let AddItem msg =
    console.log <-
        List.append console.log
            [ { itemType = Basic
                message = msg
                time = System.DateTime.Now } ]

let AddToText char = console.text <- console.text + char

let RemoveFromText() =
    if console.text.Length > 0 then console.text <- console.text.Remove(console.text.Length - 1, 1)

let EnterCommand() =
    AddItem console.text
    /// TODO: Add command checker here?
    console.text <- ""

///
/// Render helpers
///

let TakeLast int list =
    list
    |> List.rev
    |> List.take int
    |> List.rev

let datedText (item: ConsoleItem) =
    let timestamp = item.time.ToString("HH:mm:ss")
    "[" + timestamp + "]: " + item.message


///
/// Render
///

let DrawConsoleLog(spriteBatch: Graphics.SpriteBatch) =
    spriteBatch.Draw(ResourceManager.getSprite "box", Rectangle(0, 0, 1024, 500), (Color.Brown * 0.5f))
    spriteBatch.Draw(ResourceManager.getSprite "box", Rectangle(0, 15 * 32, 1024, 20), (Color.Green * 0.5f))

    let mutable i = 0
    /// Draw messages
    for log in (if List.length console.log > 32 then TakeLast 32 console.log else console.log) do
        spriteBatch.DrawString
            (ResourceManager.getFont "console", datedText log, Vector2(25.0f, 15.0f * float32 i), Color.White)
        i <- i + 1

    /// Draw input
    spriteBatch.DrawString(ResourceManager.getFont "console", console.text, Vector2(25.0f, 15.0f * 32.0f), Color.White)

    /// Draw input cursor
    spriteBatch.Draw
        (ResourceManager.getSprite "box", Rectangle(25 + (console.text.Length * 8), 15 * 32, 3, 20), Color.White)

///
/// Update
///

let update (state: Global.GlobalState) (b: TextInputEventArgs) =
    if state.console then
        if System.Char.IsLetterOrDigit b.Character then AddToText(string b.Character)
        else if Keys.Back = b.Key then RemoveFromText()
        else if Keys.Space = b.Key then AddToText " "
        else if Keys.Enter = b.Key then EnterCommand()
