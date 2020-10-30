using UnityEngine;
using UnityEngine.UI;

namespace Puzzle.Actions {
    /**
     * Stack of performed actions. Manages undoing an redoing.
     */
    public class ActionStack : MonoBehaviour {
        [SerializeField] private int bufferSize = 50;
        private RingStack<IAction> actions;

#pragma warning disable 0649
        [SerializeField] private Button undoButton;
        [SerializeField] private Button redoButton;
#pragma warning restore 0649

        private void Awake() {
            actions = new RingStack<IAction>(bufferSize);
        }

        public void Clear() {
            actions.Clear();
        }

        public void PerformAction(IAction action) {
            actions.Push(action);
            action.Perform();
            redoButton.interactable = false;
            undoButton.interactable = true;
        }

        public void RedoAction() {
            actions.Advance().Perform();
            
            undoButton.interactable = true;
            if (actions.AtEnd) {
                redoButton.interactable = false;
            }
        }

        public void UndoAction() {
            actions.Pop().Undo();
            
            redoButton.interactable = true;
            if (actions.Empty) {
                undoButton.interactable = false;
            }
        }
    }
}