using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Puzzle {
    [RequireComponent(typeof(Collider2D))]
    public class MutableSet : MonoBehaviour {
        public delegate void ContentChangeEvent();
    
        public Text contentsText;
    
        private Collider2D _collider;

        private readonly List<GameObject> _elements = new List<GameObject>();

        public event ContentChangeEvent ContentsChanged;

        private void UpdateContentsText() {
            contentsText.text = ToString();
        }

#if UNITY_EDITOR
        private void Reset() {
            if (gameObject.layer != LayerMask.NameToLayer("Editable Set")) {
                Debug.LogError($"{GetType().FullName} must be on the Editable Set layer.");
            }
        }
#endif // UNITY_EDITOR

        private void Awake() {
            _collider = GetComponent<Collider2D>();
#if UNITY_EDITOR
            if (!contentsText) {
                Debug.LogWarning("No contents text defined; set will not display its contents.", gameObject);
            }
#endif
        }

        private void OnTriggerStay2D(Collider2D other) {
#if UNITY_EDITOR
            if (other.gameObject.layer != LayerMask.NameToLayer("Element")) {
                Debug.LogError($"{other.gameObject.name} must be on the Element layer.");
            }
#endif // UNITY_EDITOR
            var alreadyTracked = _elements.Contains(other.gameObject); 
            var inside = _collider.Contains(other);
            if (!alreadyTracked && inside) {
                _elements.Add(other.gameObject);
                OnContentsChanged();
            } else if (alreadyTracked && !inside) {
                _elements.Remove(other.gameObject);
                OnContentsChanged();
            }
        }

        private void OnTriggerExit2D(Collider2D other) {
            _elements.Remove(other.gameObject);
            OnContentsChanged();
        }

        protected virtual void OnContentsChanged() {
            UpdateContentsText();
            ContentsChanged?.Invoke();
        }

        public override string ToString() {
            IEnumerable<string> Rec(GameObject go) {
                var count = go.transform.childCount;
                if (count == 0) {
                    yield return go.name;
                } else {
                    for (var i = 0; i < count; i++) {
                        var go2 = go.transform.GetChild(i).gameObject;
                        foreach (var it in Rec(go2)) {
                            yield return it;
                        }
                    }
                }
            }

            return string.Join("\n", _elements.SelectMany(Rec));
        }
    }
}