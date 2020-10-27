using System.Collections.Generic;
using System.Linq;

namespace Puzzle {
    public class Set : BaseElement {
        protected readonly List<BaseElement> Elements = new List<BaseElement>();

        private void OnEnable() {
            RecalculateElements();
        }
        
        public void RecalculateElements() {
            Elements.Clear();
            
            foreach (var tr in transform.Children()) {
                Elements.Add(tr.GetComponent<BaseElement>());
            }
        }

        public override bool Equals(BaseElement other) {
            if (!(other is Set otherSet)) {
                return false;
            }

            return Elements.SequenceEqual(otherSet.Elements);
        }

        protected override IEnumerable<BaseElement> Children() {
            return Elements;
        }

        public override string ToString() {
            return $"{{{string.Join(", ", Elements.Select(e => e.ToString()))}}}";
        }
    }
}
