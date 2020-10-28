using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Puzzle {
    public class Set : BaseElement, IPointerDownHandler {
        protected readonly HashSet<BaseElement> Elements = new HashSet<BaseElement>();
        
        public void RecalculateElements() {
            Elements.Clear();
            
            foreach (var tr in transform.Children()) {
                if (tr.GetComponent<BaseElement>() is BaseElement e) {
                    Elements.Add(e);
                }
            }
        }

        public override bool Equals(BaseElement other) {
            if (!(other is Set otherSet)) {
                Debug.Log("not even a set smh", other);
                return false;
            }

            return Elements.SetEquals(otherSet.Elements);
        }

        public override int GetHashCode() {
            return Elements.GetHashCode();
        }

        protected override IEnumerable<BaseElement> Children() {
            return Elements;
        }

        public override string ToString() {
            return $"{{{string.Join(", ", Elements.Select(e => e.ToString()))}}}";
        }

        public void OnPointerDown(PointerEventData eventData) {
            Debug.Log(this, this);
        }
    }
}
