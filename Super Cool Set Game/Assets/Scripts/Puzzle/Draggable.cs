using UnityEngine;

namespace Puzzle {
    public class Draggable : MonoBehaviour {
        [SerializeField] private Camera mainCamera;

        private Transform _transform;

        private Vector3? _offset;

        private void Awake() {
            if (!mainCamera) {
                mainCamera = Camera.main;
            }
            _transform = GetComponent<Transform>();
        }

        private void Update() {
            if (_offset is Vector3 offset) {
                var pos = mainCamera.ScreenToWorldPoint(Input.mousePosition) - offset;
                _transform.position = new Vector3(pos.x, pos.y, _transform.position.z);
            }
        }

        private void OnMouseDown() {
            _offset = mainCamera.ScreenToWorldPoint(Input.mousePosition) - _transform.position;
        }

        private void OnMouseUp() {
            _offset = null;
        }
    }
}