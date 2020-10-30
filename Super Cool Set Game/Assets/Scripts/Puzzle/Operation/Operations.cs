using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Puzzle.Actions;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Puzzle.Operation {
    /**
     * Component which sends set operation actions.
     */
    [RequireComponent(typeof(ActionStack), typeof(PuzzleLoader))]
    public class Operations : MonoBehaviour {
        private enum Operation {
            None, Union, Intersection, Difference, Powerset
        }
        
        private ActionStack actionStack;
        private PuzzleLoader puzzleLoader;
        
        [SerializeField] private Image[] buttonImages;
        
        private Operation operation;
        private Coroutine coro;

        private void StartOperation(Operation newOperation) {
            operation = newOperation;
            buttonImages[(int) operation - 1].color = Color.green;
        }

        private void FinishOperation() {
            buttonImages[(int) operation - 1].color = Color.white;
            operation = Operation.None;
            coro = null;
        }
        
        private void CancelOperation() {
            var oldCoro = coro;
            FinishOperation();
            StopCoroutine(oldCoro);
        }

        public void StartUnion() {
            var oldOperation = operation;
            if (coro != null) {
                CancelOperation();
            }
            if (oldOperation != Operation.Union) {
                StartOperation(Operation.Union);
                coro = StartCoroutine(SelectSets(2, sets => {
                    actionStack.PerformAction(new UnionSets(puzzleLoader, sets[0], sets[1], actionStack));
                    FinishOperation();
                }));
            }
        }

        public void StartIntersection() {
            var oldOperation = operation;
            if (coro != null) {
                CancelOperation();
            }
            if (oldOperation != Operation.Intersection) {
                StartOperation(Operation.Intersection);
                coro = StartCoroutine(SelectSets(2, sets => {
                    actionStack.PerformAction(new IntersectSets(puzzleLoader, sets[0], sets[1], actionStack));
                    FinishOperation();
                }));
            }
        }

        public void StartDifference() {
            var oldOperation = operation;
            if (coro != null) {
                CancelOperation();
            }
            if (oldOperation != Operation.Difference) {
                StartOperation(Operation.Difference);
                coro = StartCoroutine(SelectSets(2, sets => {
                    actionStack.PerformAction(new SubtractSets(puzzleLoader, sets[0], sets[1], actionStack));
                    FinishOperation();
                }));
            }
        }
        
        private static IEnumerator SelectSets(int count, Action<MutableSet[]> withSets) {
            var sets = new MutableSet[count];
            for (var i = 0; i < count; i++) {
                while (sets[i].IsNull()) {
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
                            sets[i] = hoveredSet;
                        }
                    }
                    yield return null;
                }
            }

            Array.Sort(sets, (a, b) => a.transform.GetSiblingIndex().CompareTo(b.transform.GetSiblingIndex()));

            withSets(sets);
        }

        private void Awake() {
            actionStack = GetComponent<ActionStack>();
            puzzleLoader = GetComponent<PuzzleLoader>();
        }
    }
}