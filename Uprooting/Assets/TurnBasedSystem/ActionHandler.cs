using System.Collections.Generic;

namespace TurnBasedSystem {
    public static class ActionHandler {
        public static readonly Dictionary<ActionType, int> ActionCosts = new() {
            {ActionType.Dig, 1},
            {ActionType.GrabPlant, 1},
        };
    }
    
    public enum ActionType {
        Dig,
        GrabPlant,
    }
}