using System.Collections.Generic;

namespace TurnBasedSystem {
    public static class MovementHandler {
        public static readonly Dictionary<MovementType, int> MovementCosts = new() {
            { MovementType.None, 0},
            {MovementType.Walk, 1},
            {MovementType.Climb, 2},
            {MovementType.EasyClimb, 1},
        };
    }
    
    public enum MovementType {
        None,
        Walk,
        Climb,
        EasyClimb
    }
}