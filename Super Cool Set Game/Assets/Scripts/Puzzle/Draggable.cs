using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Puzzle {
    [RequireComponent(typeof(RectTransform), typeof(BaseElement))]
    public class Draggable : MonoBehaviour, IPointerDownHandler, IPointerUpHandler {
        public delegate void ResizeEventHandler(Vector2 parentCellSize);

        public event ResizeEventHandler Resize;
        
        private new RectTransform transform;

        private BaseElement element;

        private MutableSet parentSet;

        public Transform elementsTray;
        public Transform holder;

        private Vector3? offset;

        private void Awake() {
            transform = GetComponent<RectTransform>();
            element = GetComponent<BaseElement>();
        }

        private void Update() {
            if (offset is Vector3 o) {
                var pos = Input.mousePosition - o;
                transform.position = new Vector3(pos.x, pos.y, transform.position.z);
            }
        }

        public void OnPointerDown(PointerEventData eventData) {
            var newCellSize = holder.GetComponent<GridLayoutGroup>().cellSize;
            offset = (Input.mousePosition - transform.position) * newCellSize.Min() / transform.rect.size.Min();

            transform.SetParent(holder, true);
            OnResize(newCellSize);
        }

        public void OnPointerUp(PointerEventData eventData) {
            offset = null;

            var list = new List<RaycastResult>();
            EventSystem.current.RaycastAll(eventData, list);

            var hoveredSet = list
                .Select(res => res.gameObject.GetComponent<MutableSet>())
                .FirstOrDefault(s => s);

            if (parentSet) {
                parentSet.Remove(element, elementsTray.transform);
            }

            if (hoveredSet != null) {
                hoveredSet.Add(element);
                parentSet = hoveredSet;
                
                OnResize(parentSet.GetComponent<GridLayoutGroup>().cellSize);
            } else {
                transform.SetParent(elementsTray.transform, true);
                parentSet = null;
                
                OnResize(elementsTray.GetComponent<GridLayoutGroup>().cellSize);
            }
        }

        protected virtual void OnResize(Vector2 parentCellSize) {
            Resize?.Invoke(parentCellSize);
        }
    }
}