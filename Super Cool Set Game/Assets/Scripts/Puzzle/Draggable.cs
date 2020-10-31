using System.Collections.Generic;
using System.Linq;
using Puzzle.Actions;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Puzzle {
    /**
     * Component enabling the element to be dragged into, out of, and between sets.
     */
    [RequireComponent(typeof(RectTransform), typeof(BaseElement))]
    public class Draggable : MonoBehaviour, IPointerDownHandler, IPointerUpHandler {
        public delegate void ResizeEventHandler(Vector2 parentCellSize);

        public event ResizeEventHandler SizeChanged;
        
        private new RectTransform transform;

        private BaseElement element;

        public MutableSet ParentSet { private get; set; }

        public ActionStack actionStack;
        public Transform elementsTray;
        public Transform holder;

        private Vector3? offset;

        private void Awake() {
            transform = GetComponent<RectTransform>();
            element = GetComponent<BaseElement>();

            var set = transform.parent.GetComponent<MutableSet>();
            if (set) {
                ParentSet = set;
            }
        }

        private void Update() {
            if (offset is Vector3 o) {
                var pos = Input.mousePosition - o;
                transform.position = new Vector3(pos.x, pos.y, transform.position.z);
            }
        }

        public void OnPointerDown(PointerEventData eventData) {
            var newCellSize = holder.GetComponent<GridLayoutGroup>().cellSize;
            offset = (Input.mousePosition - transform.position) * newCellSize.MinComponent() / transform.rect.size.MinComponent();

            transform.SetParent(holder, true);
            TriggerResize(newCellSize);
        }

        public void OnPointerUp(PointerEventData eventData) {
            offset = null;

            var list = new List<RaycastResult>();
            EventSystem.current.RaycastAll(eventData, list);

            var hoveredSet = list
                .Select(res => res.gameObject.GetComponent<MutableSet>())
                .FirstOrDefault(s => s);

            var hoveringOverSet = (bool) hoveredSet;
            var inTray = !ParentSet;

            if (inTray && hoveringOverSet) {
                actionStack.PerformAction(new AddElement(this, element, elementsTray, hoveredSet));
            } else if (!inTray && hoveringOverSet) {
                actionStack.PerformAction(new MoveElement(this, element, ParentSet, hoveredSet));
            } else if (!inTray /* && !hoveringOverSet */) {
                actionStack.PerformAction(new RemoveElement(this, element, ParentSet, elementsTray));
            } /* else if (inTray && !hoveringOverSet) */ // nothing should be done

            if (ParentSet) {
                ParentSet.Remove(element, elementsTray.transform);
            }
            
            if (hoveredSet != null) {
                hoveredSet.Add(element);
                ParentSet = hoveredSet;
                
                TriggerResize(ParentSet.GetComponent<GridLayoutGroup>().cellSize);
            } else {
                
                transform.SetParent(elementsTray.transform, true);
                ParentSet = null;
                
                TriggerResize(elementsTray.GetComponent<GridLayoutGroup>().cellSize);
            }
        }

        public virtual void TriggerResize(Vector2 parentCellSize) {
            SizeChanged?.Invoke(parentCellSize);
        }
    }
}