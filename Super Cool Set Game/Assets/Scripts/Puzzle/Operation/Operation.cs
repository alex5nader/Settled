using Puzzle.Actions;
using UnityEngine;

namespace Puzzle.Operation {
    [RequireComponent(typeof(ActionStack), typeof(PuzzleLoader))]
    public abstract class Operation : MonoBehaviour {
        protected ActionStack ActionStack;
        protected PuzzleLoader PuzzleLoader;

        private void Awake() {
            ActionStack = GetComponent<ActionStack>();
            PuzzleLoader = GetComponent<PuzzleLoader>();
        }
        
    }
}