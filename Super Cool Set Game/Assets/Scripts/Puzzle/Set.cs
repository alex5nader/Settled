using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Puzzle {
    /**
     * Component for set elements.
     */
    [RequireComponent(typeof(GridLayoutGroup))]
    public class Set : BaseElement, IEnumerable<BaseElement> {
        private GridLayoutGroup grid;

        protected readonly HashSet<BaseElement> Elements = new HashSet<BaseElement>();

        private void Awake() {
            grid = GetComponent<GridLayoutGroup>();

            ResizeCells(transform.parent.GetComponent<GridLayoutGroup>().cellSize);

            IEnumerator SetSizeNextFrame() {
                yield return null;
            
                var draggable = GetComponent<Draggable>();
                if (draggable) {
                    draggable.SizeChanged += ResizeCells;
                }
            }

            StartCoroutine(SetSizeNextFrame());
        }

        private void ResizeCells(Vector2 parentCellSize) {
            var size = parentCellSize.MinComponent() / 3;
            grid.cellSize = new Vector2(size, size);
        }
        
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

        public override string ToString() {
            return $"{{{string.Join(", ", Elements.Select(e => e.ToString()))}}}";
        }

        public IEnumerator<BaseElement> GetEnumerator() {
            return Elements.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return Elements.GetEnumerator();
        }
    }
}
