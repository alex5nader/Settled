using System.Collections.Generic;
using System.Linq;
using Puzzle.Scriptable;
using UnityEngine;
using UnityEngine.UI;

namespace Puzzle.Actions {
    /**
     * A revertible action.
     */
    public interface IAction {
        void Perform();
        void Undo();
    }

    /**
     * Represents moving an element from one set to another
     */
    public readonly struct MoveElement : IAction {
        private readonly Draggable elementDrag;
        private readonly BaseElement element;
        private readonly MutableSet from;
        private readonly MutableSet to;

        public MoveElement(Draggable elementDrag, BaseElement element, MutableSet from, MutableSet to) {
            this.elementDrag = elementDrag;
            this.element = element;
            this.from = from;
            this.to = to;
        }

        public void Perform() {
            Debug.Log("moving");
            from.Remove(element);
            to.Add(element);
            elementDrag.ParentSet = to;
            elementDrag.TriggerResize(to.GetComponent<GridLayoutGroup>().cellSize);
        }

        public void Undo() {
            to.Remove(element);
            from.Add(element);
            elementDrag.ParentSet = from;
            elementDrag.TriggerResize(from.GetComponent<GridLayoutGroup>().cellSize);
        }
    }

    /**
     * Represents adding an element to a set from outside of a set (for instance, from the elements tray).
     */
    public readonly struct AddElement : IAction {
        private readonly Draggable elementDrag;
        private readonly BaseElement element;
        private readonly Transform from;
        private readonly MutableSet to;

        public AddElement(Draggable elementDrag, BaseElement element, Transform from, MutableSet to) {
            this.elementDrag = elementDrag;
            this.element = element;
            this.from = from;
            this.to = to;
        }

        public void Perform() {
            Debug.Log("adding");
            to.Add(element);
            elementDrag.ParentSet = to;
            elementDrag.TriggerResize(to.GetComponent<GridLayoutGroup>().cellSize);
        }

        public void Undo() {
            to.Remove(element, from);
            elementDrag.ParentSet = null;
            elementDrag.TriggerResize(from.GetComponent<GridLayoutGroup>().cellSize);
        }
    }

    /**
     * Represents moving an element from a set into outside of a set (for instance, into the elements tray).
     */
    public readonly struct RemoveElement : IAction {
        private readonly Draggable elementDrag;
        private readonly BaseElement element;
        private readonly MutableSet from;
        private readonly Transform to;

        public RemoveElement(Draggable elementDrag, BaseElement element, MutableSet from, Transform to) {
            this.elementDrag = elementDrag;
            this.element = element;
            this.from = from;
            this.to = to;
        }

        public void Perform() {
            Debug.Log("removing");
            from.Remove(element, to);
            elementDrag.ParentSet = null;
            elementDrag.TriggerResize(to.GetComponent<GridLayoutGroup>().cellSize);
        }

        public void Undo() {
            from.Add(element);
            elementDrag.ParentSet = from;
            elementDrag.TriggerResize(from.GetComponent<GridLayoutGroup>().cellSize);
        }
    }

    /**
     * Represents unioning two sets together.
     */
    public struct UnionSets : IAction {
        private readonly GridLayoutGroup input;
        
        private readonly GameObject aGo, bGo;
        private readonly GameObject newGo;

        private readonly Vector2 oldCellSize;
        private readonly Vector2 newCellSize;

        // ReSharper disable twice SuggestBaseTypeForParameter
        public UnionSets(PuzzleLoader puzzleLoader, Set a, Set b, ActionStack actionStack) {
            input = puzzleLoader.input;
            var inputTr = input.transform;

            aGo = a.gameObject;
            bGo = b.gameObject;

            var count = inputTr.ActiveChildCount();
            var delta = a.RemovalDelta + b.RemovalDelta + 1;
            oldCellSize = puzzleLoader.inputGrids[count-1].cellSize;
            newCellSize = puzzleLoader.inputGrids[count-1-delta].cellSize;

            var newSo = ScriptableObject.CreateInstance<SetElement>();
            var newElements = new HashSet<Scriptable.BaseElement>(a.Select(s => s.Scriptable));
            newElements.UnionWith(b.Select(s => s.Scriptable));
            newSo.elements = newElements.ToList();
            
            newGo = Scriptable.Puzzle.MakeFixedSet(puzzleLoader.setBackground, newSo, input.transform, true, puzzleLoader.dragHolder, puzzleLoader.elementsTray, actionStack);
            
            puzzleLoader.SubscribeToChanges(newGo.GetComponent<MutableSet>());
        }
    
        public void Perform() {
            input.cellSize = newCellSize;
            newGo.SetActive(true);
            aGo.SetActive(false);
            bGo.SetActive(false);
        }
    
        public void Undo() {
            input.cellSize = oldCellSize;
            newGo.SetActive(false);
            aGo.SetActive(true);
            bGo.SetActive(true);
        }
    }

    /**
     * Represents intersection-ing two sets together.
     */
    public struct IntersectSets : IAction {
        private readonly GridLayoutGroup input;
        
        private readonly GameObject aGo, bGo;
        private readonly GameObject newGo;

        private readonly Vector2 oldCellSize;
        private readonly Vector2 newCellSize;

        // ReSharper disable twice SuggestBaseTypeForParameter
        public IntersectSets(PuzzleLoader puzzleLoader, Set a, Set b, ActionStack actionStack) {
            input = puzzleLoader.input;
            var inputTr = input.transform;

            aGo = a.gameObject;
            bGo = b.gameObject;

            var count = inputTr.ActiveChildCount();
            var delta = a.RemovalDelta + b.RemovalDelta + 1;
            oldCellSize = puzzleLoader.inputGrids[count-1].cellSize;
            newCellSize = puzzleLoader.inputGrids[count-1-delta].cellSize;

            var newSo = ScriptableObject.CreateInstance<SetElement>();
            var newElements = new HashSet<Scriptable.BaseElement>(a.Select(s => s.Scriptable));
            newElements.IntersectWith(b.Select(s => s.Scriptable));
            newSo.elements = newElements.ToList();
            
            newGo = Scriptable.Puzzle.MakeFixedSet(puzzleLoader.setBackground, newSo, input.transform, true, puzzleLoader.dragHolder, puzzleLoader.elementsTray, actionStack);
            
            puzzleLoader.SubscribeToChanges(newGo.GetComponent<MutableSet>());
        }
    
        public void Perform() {
            input.cellSize = newCellSize;
            newGo.SetActive(true);
            aGo.SetActive(false);
            bGo.SetActive(false);
        }
    
        public void Undo() {
            input.cellSize = oldCellSize;
            newGo.SetActive(false);
            aGo.SetActive(true);
            bGo.SetActive(true);
        }
    }

    /**
     * Represents intersection-ing two sets together.
     */
    public struct SubtractSets : IAction {
        private readonly GridLayoutGroup input;
        
        private readonly GameObject aGo, bGo;
        private readonly GameObject newGo;

        private readonly Vector2 oldCellSize;
        private readonly Vector2 newCellSize;

        // ReSharper disable twice SuggestBaseTypeForParameter
        public SubtractSets(PuzzleLoader puzzleLoader, Set a, Set b, ActionStack actionStack) {
            input = puzzleLoader.input;
            var inputTr = input.transform;

            aGo = a.gameObject;
            bGo = b.gameObject;

            var count = inputTr.ActiveChildCount();
            var delta = a.RemovalDelta + b.RemovalDelta + 1;
            oldCellSize = puzzleLoader.inputGrids[count-1].cellSize;
            newCellSize = puzzleLoader.inputGrids[count-1-delta].cellSize;

            var newSo = ScriptableObject.CreateInstance<SetElement>();
            var newElements = new HashSet<Scriptable.BaseElement>(a.Select(s => s.Scriptable));
            newElements.ExceptWith(b.Select(s => s.Scriptable));
            newSo.elements = newElements.ToList();
            
            newGo = Scriptable.Puzzle.MakeFixedSet(puzzleLoader.setBackground, newSo, input.transform, true, puzzleLoader.dragHolder, puzzleLoader.elementsTray, actionStack);
            
            puzzleLoader.SubscribeToChanges(newGo.GetComponent<MutableSet>());
        }
    
        public void Perform() {
            input.cellSize = newCellSize;
            newGo.SetActive(true);
            aGo.SetActive(false);
            bGo.SetActive(false);
        }
    
        public void Undo() {
            input.cellSize = oldCellSize;
            newGo.SetActive(false);
            aGo.SetActive(true);
            bGo.SetActive(true);
        }
    }

    public readonly struct MakePowerset : IAction {
        private readonly GameObject orig;
        private readonly GameObject powerset;

        // ReSharper disable twice SuggestBaseTypeForParameter
        public MakePowerset(PuzzleLoader puzzleLoader, Set original, ActionStack actionStack) {
            var inputTr = puzzleLoader.input.transform;

            orig = original.gameObject;

            var elements = original.ToList();

            var subsets = new List<Scriptable.BaseElement>();

            for (var i = (1 << original.Count) - 1; i >= 0; i--) {
                var subset = new List<Scriptable.BaseElement>();

                for (var j = 0; j < original.Count; j++) {
                    if ((i & (1 << j)) != 0) {
                        subset.Add(elements[j].Scriptable);
                    }
                }

                var scriptable = ScriptableObject.CreateInstance<SetElement>();
                scriptable.elements = subset;
                
                subsets.Add(scriptable);
            }

            var powersetScriptable = ScriptableObject.CreateInstance<SetElement>();
            powersetScriptable.elements = subsets;

            powerset = Scriptable.Puzzle.MakeFixedSet(puzzleLoader.setBackground, powersetScriptable, inputTr, true, puzzleLoader.dragHolder, puzzleLoader.elementsTray, actionStack);
            
            puzzleLoader.SubscribeToChanges(powerset.GetComponent<MutableSet>());
        }
    
        public void Perform() {
            powerset.SetActive(true);
            orig.SetActive(false);
        }
    
        public void Undo() {
            powerset.SetActive(false);
            orig.SetActive(true);
        }
    }
}