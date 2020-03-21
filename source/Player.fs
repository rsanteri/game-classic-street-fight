module Player

open Microsoft.Xna.Framework.Input
open Entity.Types
open EntityMovement

///
/// Update player velocity and facing by player input. Doesnt care about collisions
///
let HandlePlayerMovementInput (keyboard: KeyboardState) player =
    // Handle vertical movement
    if keyboard.IsKeyDown(Keys.W) || keyboard.IsKeyDown(Keys.S) then
        let dir =
            if keyboard.IsKeyDown(Keys.W) then -player.velocitySpeed else player.velocitySpeed
        player.velocity.y <- AddVelocity player.velocity.y dir
    else
        player.velocity.y <- NaturalSlowVelocity player.velocity.y

    // Handle horizontal movement
    if keyboard.IsKeyDown(Keys.A) || keyboard.IsKeyDown(Keys.D) then
        let dir =
            if keyboard.IsKeyDown(Keys.A) then -player.velocitySpeed else player.velocitySpeed
        player.velocity.x <- AddVelocity player.velocity.x dir
        // Update player facing
        player.facing <- if dir < 0 then Left else Right
        // Flip position of hand according the facing
        player.body.hand.basePosition.x <- if dir < 0 then 10 else 30
    else
        player.velocity.x <- NaturalSlowVelocity player.velocity.x
