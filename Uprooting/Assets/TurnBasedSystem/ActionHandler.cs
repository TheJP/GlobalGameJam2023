using System.Collections.Generic;

namespace TurnBasedSystem {
    public class ActionHandler {
        public static readonly Dictionary<ActionType, int> ActionCosts = new Dictionary<ActionType, int> {
            {ActionType.Dig, 2},
            {ActionType.Eat, 3},
        };
    }
    
    public enum ActionType {
        Dig,
        Eat,
    }
}