using UnityEngine;

namespace Puzzle.Scriptable {
    public abstract class BaseElement : ScriptableObject {
        public abstract override int GetHashCode();
    }
}