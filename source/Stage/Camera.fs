module Camera

open Microsoft.Xna.Framework


let ToCameraPosition (camera: Stage.Types.XCamera) (rectangle: Rectangle) =
    Rectangle(rectangle.X - camera.x, rectangle.Y, rectangle.Width, rectangle.Height)

let ToParallaxPosition (camera: Stage.Types.XCamera) (rectangle: Rectangle) (distance: int) =
    Rectangle(rectangle.X - (camera.x * distance / 100), rectangle.Y, rectangle.Width, rectangle.Height)

let MoveCamera (resolution: Global.Resolution) (xcamera: Stage.Types.XCamera) (x: int) (mapsize: int) =
    if xcamera.locked then
        ()
    else
        let newValue =
            // If is on left move to left
            if x < xcamera.x + 400 then
                if x - 400 < 0 then 0 else x - 400
            // If is on right move to right
            else if x > xcamera.x + (resolution.width - 400) then
                if xcamera.x > mapsize - resolution.width then xcamera.x else x - resolution.width + 400
            else
                xcamera.x

        xcamera.x <- newValue
