module Camera
open Microsoft.Xna.Framework


type XCamera =
    { mutable x: int; mutable locked: bool }

let ToCameraPosition (camera: XCamera) (rectangle: Rectangle) =
    Rectangle(rectangle.X - camera.x, rectangle.Y, rectangle.Width, rectangle.Height)

let ToParallaxPosition (camera: XCamera) (rectangle: Rectangle) (distance: int) = 
    Rectangle(rectangle.X - (camera.x * distance / 100), rectangle.Y, rectangle.Width, rectangle.Height)

let MoveCamera (xcamera: XCamera) (x: int) (mapsize: int) =
    if xcamera.locked then
        ()
    // If is on left move to left
    else if x < xcamera.x + 200 then
        xcamera.x <- if x - 200 < 0 then 0 else x - 200
    // If is on right move to right
    else if x > xcamera.x + (1024 - 200) then
        xcamera.x <- if xcamera.x > mapsize - 1024 then xcamera.x else x - 1024 + 200