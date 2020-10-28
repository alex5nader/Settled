using System.Collections.Generic;
using System.Linq;

namespace Puzzle {
    public class Set : BaseElement {
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
    }
}
