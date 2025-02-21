module GamePlay.Update

open Microsoft.Xna.Framework
open Microsoft.Xna.Framework.Input

open Global
open Stage.Types
open Stage.Update

let update
    (gameTime: GameTime)
    (state: Global.GlobalState)
    (ioactions: InputHelper.IOActionState)
    (map: Stage)
    (mapController: StageController)
    =

    let player = mapController.player
    /// Gameplay updates will only happen if we are not in menu or paused.
    match (mapController.gameState, mapController.stageState) with
    | (Playing, StageState.Normal) ->
        SoundManager.soundHelper.getRelativePosition <-
            Camera.getRelativePositionToScreen mapController.camera.x state.resolution
        ///
        /// Handle input
        ///

        /// gamestate changes. Set to pause or to menu
        if state.console then ()
        else if ioactions.pressed Keys.P then mapController.gameState <- Paused
        else if ioactions.pressed Keys.Escape then mapController.gameState <- OnMenu

        /// Update player velocity according input
        if not state.console then
            Player.HandlePlayerMovementInput state.io.keys player

            if player.action = Entity.Types.EntityAction.NoOp && state.io.keys.IsKeyDown(Keys.Space) then
                player.action <- Entity.Types.Hit

        ///
        /// Make ai decisions
        ///
        let allEntities =
            List.append [ player ] (List.map (fun (i: AI.Types.EntityController) -> i.entity) mapController.entities)

        for controlledEntity in mapController.entities do
            if not controlledEntity.brain.dormant then
                AI.Update.OperateNPC controlledEntity allEntities gameTime.ElapsedGameTime.Milliseconds

        ///
        /// Update Movement & Action State
        ///

        for entity in allEntities do
            let otherEntities = List.filter (fun (item: Entity.Types.Entity) -> item.id <> entity.id) allEntities
            // Update entity position
            EntityMovement.MoveEntity entity otherEntities
            // Update possible entity action
            EntityActions.HandleEntityAction entity otherEntities
            // Update walking animation
            EntityActions.UpdateEntityWalking entity

        /// Update triggers after movements
        ApplyTrigers map mapController
        /// Clean up the dead
        mapController.entities <- List.filter (fun npc -> npc.entity.properties.health > 0) mapController.entities

    | (Playing, stgState) ->
        /// Update transition animation
        updateStageTransition mapController
        /// Move player somewhere
        let target =
            match stgState with
            | Entering _ -> 100
            | Exiting(_, etarget, _) -> etarget
            | Normal -> player.position.x
            | ExitNow nextStage ->
                if nextStage = Map
                then state.state <- InMap(Map.Update.initMap())
                else state.state <- InGame(Areas.init nextStage)

                player.position.x

        AI.Update.MoveAI mapController.player (target, player.position.y) |> ignore
        player.position <- EntityMovement.CalculatePosition player.velocity player.position player.speed

    | (Paused, _) ->
        if ioactions.pressed Keys.P then mapController.gameState <- Playing
    | (OnMenu, _) ->
        if ioactions.pressed Keys.Escape then mapController.gameState <- Playing

    /// Move camera after everything
    Camera.MoveCamera state.resolution mapController.camera player.position.x map.size
