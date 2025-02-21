module AI.Types

open Entity.Types

type Decision =
    /// No operation
    | Slack
    /// Move to static point in map (x,y)
    | MoveTo of (int * int)
    /// Try moving next to entity
    | MoveNextTo of Entity

type Brain =
    { mutable dormant: bool
      /// Decision to be operated
      mutable decision: Decision
      /// Time to next decision (There should be something else deciding that as well)
      mutable nextDecision: int }

type EntityController =
    { brain: Brain
      entity: Entity }
