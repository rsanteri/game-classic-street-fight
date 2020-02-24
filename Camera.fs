module Camera
open Microsoft.Xna.Framework


type XCamera =
    { mutable x: int }

let ToCameraPosition (camera: XCamera) (rectangle: Rectangle) =
    Rectangle(rectangle.X - camera.x, rectangle.Y, rectangle.Width, rectangle.Height)

let MoveCamera (xcamera: XCamera) (x: int) =
    if x < xcamera.x + 200 then
        xcamera.x <- x - 200
    else if x > xcamera.x + (1024 - 200) then
        xcamera.x <- x - 1024 + 200