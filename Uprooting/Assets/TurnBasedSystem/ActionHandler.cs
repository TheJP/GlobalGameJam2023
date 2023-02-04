using System.Collections.Generic;

namespace TurnBasedSystem {
    public static class ActionHandler {
        public static readonly Dictionary<ActionType, int> ActionCosts = new() {
            {ActionType.Dig, 2},
            {ActionType.Eat, 3},
        };
    }
    
    public enum ActionType {
        Dig,
        Eat,
    }
}