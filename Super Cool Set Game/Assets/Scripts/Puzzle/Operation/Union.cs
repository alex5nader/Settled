using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Puzzle.Actions;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Puzzle.Operation {
    public class Union : Operation {
        private MutableSet a, b;

        public void StartSelection() {
            StartCoroutine(Selection());
        }

        private IEnumerator Selection() {
            void TryGetClicked(ref MutableSet target) {
                if (Input.GetMouseButtonDown(0)) {
                    var data = new PointerEventData(null) { position = Input.mousePosition };
                    var list = new List<RaycastResult>();
                    EventSystem.current.RaycastAll(data, list);

                    var hoveredSet = list
                        .Select(res => res.gameObject.GetComponent<MutableSet>())
                        .FirstOrDefault(s => s);

                    // ReSharper disable once PossibleNullReferenceException
                    if (!hoveredSet.IsNull()) {
                        Debug.Log($"clicked on {hoveredSet.name}", hoveredSet);
                        target = hoveredSet;
                    }
                }
            }
            
            while (a.IsNull()) {
                TryGetClicked(ref a);
                yield return null;
            }
            while (b.IsNull()) {
                TryGetClicked(ref b);
                yield return null;
            }

            ActionStack.PerformAction(new UnionSets(input, a, b));
        }
    }
}