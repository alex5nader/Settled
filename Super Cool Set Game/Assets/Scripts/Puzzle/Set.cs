using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Puzzle {
    [RequireComponent(typeof(GridLayoutGroup))]
    public class Set : BaseElement {
        private GridLayoutGroup grid;
        
        protected readonly HashSet<BaseElement> Elements = new HashSet<BaseElement>();

        private void Awake() {
            grid = GetComponent<GridLayoutGroup>();

            ResizeCells(transform.parent.GetComponent<GridLayoutGroup>().cellSize);

            IEnumerator SetSizeNextFrame() {
                yield return null;
            
                var draggable = GetComponent<Draggable>();
                if (draggable) {
                    draggable.Resize += ResizeCells;
                }
            }

            StartCoroutine(SetSizeNextFrame());
        }

        private void ResizeCells(Vector2 parentCellSize) {
            var size = parentCellSize.Min() / 3;
            Debug.Log($"resizing cells to {size}x{size}", this);
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
