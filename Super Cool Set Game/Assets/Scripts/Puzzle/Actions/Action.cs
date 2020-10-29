using UnityEngine;
using UnityEngine.UI;

namespace Puzzle.Actions {
    public interface IAction {
        void Perform();
        void Undo();
    }

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

    public readonly struct UnionSets : IAction {
        private readonly RectTransform input;
        private readonly MutableSet a, b;
        private readonly GameObject aGo, bGo;

        public UnionSets(RectTransform input, MutableSet a, MutableSet b) {
            this.input = input;
            this.a = a;
            this.b = b;
            aGo = a.gameObject;
            bGo = b.gameObject;
        }

        public void Perform() {
            var unionGo = new GameObject();
            unionGo.transform.SetParent(input, false);
            a.AddUnionTo(unionGo, b);
            Object.Destroy(aGo);
            Object.Destroy(bGo);
        }

        public void Undo() {
            Object.Instantiate(aGo, input, false);
            Object.Instantiate(bGo, input, false);
        }
    }
}