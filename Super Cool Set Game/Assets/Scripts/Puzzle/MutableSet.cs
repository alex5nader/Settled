using UnityEngine;

namespace Puzzle {
    [RequireComponent(typeof(BoxCollider2D))]
    public sealed class MutableSet : Set {
        public void Add(BaseElement element) {
            element.transform.SetParent(transform, true);
            Elements.Add(element);
            OnContentsChanged();
            Debug.Log($"after adding set is now {this}");
        }

        public void Remove(BaseElement element, Transform newParent) {
            element.transform.SetParent(newParent, true);
            Elements.Remove(element);
            OnContentsChanged();
            Debug.Log($"after removing set is now {this}");
        }
        
        public delegate void ContentChangeEvent();

        public event ContentChangeEvent ContentsChanged;

        private void OnContentsChanged() {
            ContentsChanged?.Invoke();
        }
    }
}