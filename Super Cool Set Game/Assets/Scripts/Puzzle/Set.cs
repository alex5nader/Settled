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
            return Elements
                .Select(b => b.GetHashCode())
                .OrderBy(h => h)
                .Aggregate(19, (hs, h) => hs * 31 + h);
        }

        public override string ToString() {
            return $"{{{string.Join(", ", Elements.Select(e => e.ToString()))}}}";
        }
    }
}
