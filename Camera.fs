module Camera
open Microsoft.Xna.Framework


type XCamera =
    { mutable x: int }

let ToCameraPosition (camera: XCamera) (rectangle: Rectangle) =
    Rectangle(rectangle.X - camera.x, rectangle.Y, rectangle.Width, rectangle.Height)

let ToParallaxPosition (camera: XCamera) (rectangle: Rectangle) (distance: int) = 
    Rectangle(rectangle.X - (camera.x * distance / 100), rectangle.Y, rectangle.Width, rectangle.Height)

let MoveCamera (xcamera: XCamera) (x: int) =
    // If is on left move to left
    if x < xcamera.x + 200 then
        xcamera.x <- if x - 200 < 0 then 0 else x - 200
    // If is on right move to right
    else if x > xcamera.x + (1024 - 200) then
        xcamera.x <- x - 1024 + 200