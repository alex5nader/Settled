using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Puzzle {
    [RequireComponent(typeof(BoxCollider2D))]
    public class MutableSet : MonoBehaviour {
        public delegate void ContentChangeEvent();
    
        public Text contentsText;
    
        private new BoxCollider2D collider;

        // private readonly List<GameObject> elements = new List<GameObject>();

        public event ContentChangeEvent ContentsChanged;

#if UNITY_EDITOR
        private void Reset() {
            if (gameObject.layer != LayerMask.NameToLayer("Editable Set")) {
                Debug.LogError($"{GetType().FullName} must be on the Editable Set layer.");
            }
        }
#endif // UNITY_EDITOR

        private void Awake() {
            collider = GetComponent<BoxCollider2D>();
#if UNITY_EDITOR
            if (!contentsText) {
                Debug.LogWarning("No contents text defined; set will not display its contents.", gameObject);
            }
#endif
        }

        private bool sized;

        private void Update() {
            // ReSharper disable once Unity.PerformanceCriticalCodeInvocation
            // TODO: make this less god awful
            // problem: want to set collider size during init, but parent rect does not size this rect until animation update
            // temp solution: set size after first animation update and never again
            if (!sized) {
                var rect = ((RectTransform) transform).rect;
                collider.size = new Vector2(rect.width, rect.height);
                sized = true;
            }
        }

//         private void OnTriggerStay2D(Collider2D other) {
// #if UNITY_EDITOR
//             if (other.gameObject.layer != LayerMask.NameToLayer("Element")) {
//                 Debug.LogError($"{other.gameObject.name} must be on the Element layer.");
//             }
// #endif // UNITY_EDITOR
//             Debug.Log("colliding with me", other);
//             var alreadyTracked = elements.Contains(other.gameObject); 
//             var inside = collider.Contains(other);
//             if (!alreadyTracked && inside) {
//                 elements.Add(other.gameObject);
//                 OnContentsChanged();
//             } else if (alreadyTracked && !inside) {
//                 elements.Remove(other.gameObject);
//                 OnContentsChanged();
//             }
//         }
//
//         private void OnTriggerExit2D(Collider2D other) {
//             elements.Remove(other.gameObject);
//             OnContentsChanged();
//         }

        // protected virtual void OnContentsChanged() {
        //     ContentsChanged?.Invoke();
        // }

        // public override string ToString() {
        //     IEnumerable<string> Rec(GameObject go) {
        //         var count = go.transform.childCount;
        //         if (count == 0) {
        //             yield return go.name;
        //         } else {
        //             foreach (var child in go.transform.Children()) {
        //                 foreach (var it in Rec(child.gameObject)) {
        //                     yield return it;
        //                 }
        //             }
        //         }
        //     }
        //
        //     return string.Join("\n", elements.SelectMany(Rec));
        // }
    }
}