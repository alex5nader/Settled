using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Puzzle {
    [RequireComponent(typeof(RectTransform), typeof(BaseElement))]
    public class Draggable : MonoBehaviour, IPointerDownHandler, IPointerUpHandler {
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
            offset = Input.mousePosition - transform.position;

            transform.SetParent(holder, true);
        }

        public void OnPointerUp(PointerEventData eventData) {
            offset = null;

            var list = new List<RaycastResult>();
            EventSystem.current.RaycastAll(eventData, list);

            var hoveredSet = list
                .Select(res => res.gameObject.GetComponent<MutableSet>())
                .FirstOrDefault(s => s);

            if (parentSet) {
                parentSet.Remove(element, elementsTray);
            }

            if (hoveredSet != null) {
                hoveredSet.Add(element);
                parentSet = hoveredSet;
            } else {
                transform.SetParent(elementsTray, true);
            }
        }
    }
}