using System.Collections.Generic;

namespace TurnBasedSystem {
    public class MovementHandler {
        public static readonly Dictionary<MovementType, int> MovementCosts = new Dictionary<MovementType, int> {
            {MovementType.Walk, 1},
            {MovementType.Climb, 2},
        };
    }
    
    public enum MovementType {
        Walk,
        Climb,
    }
}