using System.Collections.Generic;

namespace TurnBasedSystem {
    public static class MovementHandler {
        public static readonly Dictionary<MovementType, int> MovementCosts = new() {
            { MovementType.None, 0},
            {MovementType.Walk, 1},
            {MovementType.Climb, 2},
            {MovementType.EasyClimb, 1},
            {MovementType.DigMovement, 1},
            {MovementType.FallDownMovement, 0},
        };
    }
    
    public enum MovementType {
        None,
        Walk,
        Climb,
        EasyClimb,
        DigMovement,
        FallDownMovement
    }
}