using UnityEngine;

namespace Puzzle {
    public sealed class MutableSet : Set {
        public void Add(BaseElement element) {
            element.transform.SetParent(transform, true);
            Elements.Add(element);
            OnContentsChanged();
        }

        public void AddAll(Set other) {
            foreach (var el in other) {
                if (!Elements.Contains(el)) {
                    Add(el);
                }
            }
        }

        public void Remove(BaseElement element, Transform newParent) {
            element.transform.SetParent(newParent, true);
            Remove(element);
        }

        public void Remove(BaseElement element) {
            Elements.Remove(element);
            OnContentsChanged();
        }
        
        public delegate void ContentChangeEvent();

        public event ContentChangeEvent ContentsChanged;

        private void OnContentsChanged() {
            ContentsChanged?.Invoke();
        }

        public void AddUnionTo(GameObject target, MutableSet other) {
            var union = target.AddComponent<MutableSet>();
            union.Elements.UnionWith(Elements);
            union.Elements.UnionWith(other.Elements);
        }
    }
}