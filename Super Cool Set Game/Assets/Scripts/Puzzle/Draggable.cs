using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Puzzle {
    [RequireComponent(typeof(BoxCollider2D))]
    public class Draggable : MonoBehaviour, IPointerDownHandler, IPointerUpHandler {
        private new Transform transform;
        private new BoxCollider2D collider;

        public Transform elementsTray;
        public Transform holder;

        private GameObject hovered;

        private Vector3? offset;

        private void Awake() {
            transform = GetComponent<Transform>();
            collider = GetComponent<BoxCollider2D>();
        }

        private void Update() {
            if (offset is Vector3 o) {
                var pos = Input.mousePosition - o;
                transform.position = new Vector3(pos.x, pos.y, transform.position.z);
            }
        }

        private void OnTriggerStay2D(Collider2D other) {
            Debug.Log($"colliding with {other.name}", other);
            var inside = other.Contains(collider);
            var alreadyTracked = hovered != null && other.gameObject == hovered.gameObject;
            if (!alreadyTracked && inside) {
                hovered = other.gameObject;
            } else if (alreadyTracked && !inside) {
                hovered = null;
            }
        }

        private void OnTriggerExit2D(Collider2D other) {
            if (hovered == other.gameObject) {
                hovered = null;
            }
        }

        public void OnPointerDown(PointerEventData eventData) {
            Debug.Log("clicked on me", gameObject);
            offset = Input.mousePosition - transform.position;
            transform.SetParent(holder, true);
        }

        public void OnPointerUp(PointerEventData eventData) {
            offset = null;
            transform.SetParent(hovered ? hovered.transform : elementsTray, true);
        }
    }
}