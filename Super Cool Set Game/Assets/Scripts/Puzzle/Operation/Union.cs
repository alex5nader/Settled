using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Puzzle.Actions;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Puzzle.Operation {
    /**
     * The union set operation. Combines two sets, only leaving one copy of any duplicated elements.
     */
    public class Union : Operation {
        private MutableSet a, b;

        private Coroutine coro;

        public void StartSelection() {
            if (coro != null) {
                StopCoroutine(coro);
            } else {
                coro = StartCoroutine(Selection());
            }
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
            
            a = null;
            while (a.IsNull()) {
                TryGetClicked(ref a);
                yield return null;
            }
            b = null;
            while (b.IsNull()) {
                TryGetClicked(ref b);
                yield return null;
            }

            // ReSharper disable twice PossibleNullReferenceException
            if (a.transform.GetSiblingIndex() > b.transform.GetSiblingIndex()) {
                var tmp = a;
                a = b;
                b = tmp;
            }

            ActionStack.PerformAction(new UnionSets(PuzzleLoader, a, b, ActionStack));
            coro = null;
        }
    }
}