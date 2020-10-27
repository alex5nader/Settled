using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Puzzle {
    [RequireComponent(typeof(RectTransform), typeof(BoxCollider2D), typeof(BaseElement))]
    public class Draggable : MonoBehaviour, IPointerDownHandler, IPointerUpHandler {
        private new RectTransform transform;
        private new BoxCollider2D collider;

        private Camera mainCamera;

        private BaseElement element;

        private MutableSet parentSet;

        public Transform elementsTray;
        public Transform holder;

        // private MutableSet hoveredSet;
        private Vector3? offset;

        private void Awake() {
            mainCamera = Camera.main;

            transform = GetComponent<RectTransform>();
            collider = GetComponent<BoxCollider2D>();
            element = GetComponent<BaseElement>();
        }

        private void Update() {
            if (offset is Vector3 o) {
                var pos = Input.mousePosition - o;
                transform.position = new Vector3(pos.x, pos.y, transform.position.z);
            }
        }

        // private void OnTriggerStay2D(Collider2D other) {
        //     var inside = other.Contains(collider);
        //     var alreadyTracked = hoveredSet != null && other.gameObject == hoveredSet.gameObject;
        //     if (!alreadyTracked && inside) {
        //         hoveredSet = other.GetComponent<MutableSet>();
        //     } else if (alreadyTracked && !inside) {
        //         hoveredSet = null;
        //     }
        // }
        //
        // private void OnTriggerExit2D(Collider2D other) {
        //     if (hoveredSet && hoveredSet.gameObject == other.gameObject) {
        //         hoveredSet = null;
        //     }
        // }

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
                .First(s => s);

            if (parentSet) {
                parentSet.Remove(element, elementsTray);
            }

            if (hoveredSet) {
                Debug.Log($"adding to {hoveredSet.name}", hoveredSet);
                hoveredSet.Add(element);

                parentSet = hoveredSet;
            } else {
                transform.SetParent(elementsTray, true);
            }
        }
    }
}