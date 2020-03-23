module Console

open Microsoft.Xna.Framework
open Microsoft.Xna.Framework.Input

type ConsoleItemType =
    | Basic
    | ConsoleError

type ConsoleItem =
    { itemType: ConsoleItemType
      message: string
      time: System.DateTime }

type ConsoleModel =
    { mutable text: string
      mutable log: ConsoleItem list }

type ConsoleCommand =
    | SetResolution of int * int
    | GoTo of string
    | Restart
    | ExitApp

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

let AddErrorItem msg =
    console.log <-
        List.append console.log
            [ { itemType = ConsoleError
                message = msg
                time = System.DateTime.Now } ]

let AddToText char = console.text <- console.text + char

let RemoveFromText() =
    if console.text.Length > 0 then console.text <- console.text.Remove(console.text.Length - 1, 1)

let EnterCommand(): ConsoleCommand list =
    AddItem console.text

    let command =
        match Seq.toList (console.text.Split(" ")) with
        | [ "goto"; target ] -> [ GoTo target ]
        | [ "restart" ] -> [ Restart ]
        | [ "setresolution"; w; h ] -> [ SetResolution(int w, int h) ]
        | [ "exit" ] -> [ ExitApp ]
        | _ -> []

    if List.isEmpty command then AddErrorItem(console.text + " is not recognized command.")

    console.text <- ""
    command

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

    let text =
        match item.itemType with
        | Basic -> item.message
        | ConsoleError -> "[ERROR] " + item.message

    "[" + timestamp + "]: " + text


///
/// Render
///

let DrawConsoleLog (state: Global.GlobalState) (spriteBatch: Graphics.SpriteBatch) =
    /// Console background
    spriteBatch.Draw
        (ResourceManager.getSprite "box", Rectangle(0, 0, state.resolution.width, 500), (Color.Brown * 0.5f))
    /// Input field
    spriteBatch.Draw
        (ResourceManager.getSprite "box", Rectangle(0, 15 * 32, state.resolution.width, 20), (Color.Green * 0.5f))

    let mutable i = 0
    /// Draw messages
    for log in (if List.length console.log > 32 then TakeLast 32 console.log else console.log) do
        let color =
            match log.itemType with
            | Basic -> Color.White
            | ConsoleError -> Color.Red

        spriteBatch.DrawString
            (ResourceManager.getFont "console", datedText log, Vector2(25.0f, 15.0f * float32 i), color)
        i <- i + 1

    /// Draw input
    spriteBatch.DrawString(ResourceManager.getFont "console", console.text, Vector2(25.0f, 15.0f * 32.0f), Color.White)

    /// Draw input cursor
    spriteBatch.Draw
        (ResourceManager.getSprite "box", Rectangle(25 + (console.text.Length * 8), 15 * 32, 3, 20), Color.White)

///
/// Update
///

let update (state: Global.GlobalState) (b: TextInputEventArgs): ConsoleCommand list =
    if state.console then
        if System.Char.IsLetterOrDigit b.Character then
            AddToText(string b.Character)
            []
        else if Keys.Back = b.Key then
            RemoveFromText()
            []
        else if Keys.Space = b.Key then
            AddToText " "
            []
        else if Keys.Enter = b.Key then
            EnterCommand()
        else
            []
    else
        []
