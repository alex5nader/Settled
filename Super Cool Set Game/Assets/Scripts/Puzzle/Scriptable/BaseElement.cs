using UnityEngine;

namespace Puzzle.Scriptable {
    /**
     * Scriptable object base for elements.
     */
    public abstract class BaseElement : ScriptableObject {
        public abstract override int GetHashCode();
    }
}