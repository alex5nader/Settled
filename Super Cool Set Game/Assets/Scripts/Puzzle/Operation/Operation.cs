using Puzzle.Actions;
using UnityEngine;

namespace Puzzle.Operation {
    [RequireComponent(typeof(ActionStack))]
    public abstract class Operation : MonoBehaviour {
        protected ActionStack ActionStack;

        private void Awake() {
            ActionStack = GetComponent<ActionStack>();
        }
        
        [SerializeField] protected RectTransform input;
    }
}