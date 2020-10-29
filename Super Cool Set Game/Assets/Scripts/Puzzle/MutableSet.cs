using UnityEngine;

namespace Puzzle {
    public sealed class MutableSet : Set {
        public void Add(BaseElement element) {
            element.transform.SetParent(transform, true);
            Elements.Add(element);
            OnContentsChanged();
        }

        public void Remove(BaseElement element, Transform newParent) {
            element.transform.SetParent(newParent, true);
            Elements.Remove(element);
            OnContentsChanged();
        }
        
        public delegate void ContentChangeEvent();

        public event ContentChangeEvent ContentsChanged;

        private void OnContentsChanged() {
            ContentsChanged?.Invoke();
        }
    }
}