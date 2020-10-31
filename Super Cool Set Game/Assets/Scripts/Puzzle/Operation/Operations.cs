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
        public int[] operationUses = new int[4];
        
        private ActionStack actionStack;
        private PuzzleLoader puzzleLoader;

        public Text[] operationUsesTexts;
        [SerializeField] private Image[] buttonImages;
        
        private Operation operation;
        private Coroutine coro;

        private void StartOperation(Operation newOperation) {
            operation = newOperation;
            buttonImages[operation.ToIndex()].color = Color.green;
        }

        private void FinishOperation(bool consume) {
            buttonImages[operation.ToIndex()].color = Color.white;
            if (consume) {
                operationUses[operation.ToIndex()]--;
                operationUsesTexts[operation.ToIndex()].text = operationUses[operation.ToIndex()].ToString();
            }
            operation = Operation.None;
            coro = null;
        }
        
        private void CancelOperation() {
            var oldCoro = coro;
            FinishOperation(false);
            StopCoroutine(oldCoro);
        }

        public void StartUnion() {
            var oldOperation = operation;
            if (operationUses[Operation.Union.ToIndex()] == 0) {
                return;
            }
            if (coro != null) {
                CancelOperation();
            }
            if (oldOperation != Operation.Union) {
                StartOperation(Operation.Union);
                coro = StartCoroutine(SelectSets(2, sets => {
                    actionStack.PerformAction(new UnionSets(puzzleLoader, sets[0], sets[1], actionStack));
                    FinishOperation(true);
                }));
            }
        }

        public void StartIntersection() {
            var oldOperation = operation;
            if (operationUses[Operation.Intersection.ToIndex()] == 0) {
                return;
            }
            if (coro != null) {
                CancelOperation();
            }
            if (oldOperation != Operation.Intersection) {
                StartOperation(Operation.Intersection);
                coro = StartCoroutine(SelectSets(2, sets => {
                    actionStack.PerformAction(new IntersectSets(puzzleLoader, sets[0], sets[1], actionStack));
                    FinishOperation(true);
                }));
            }
        }

        public void StartDifference() {
            var oldOperation = operation;
            if (operationUses[Operation.Difference.ToIndex()] == 0) {
                return;
            }
            if (coro != null) {
                CancelOperation();
            }
            if (oldOperation != Operation.Difference) {
                StartOperation(Operation.Difference);
                coro = StartCoroutine(SelectSets(2, sets => {
                    actionStack.PerformAction(new SubtractSets(puzzleLoader, sets[0], sets[1], actionStack));
                    FinishOperation(true);
                }));
            }
        }

        public void StartPowerset() {
            var oldOperation = operation;
            if (operationUses[Operation.Powerset.ToIndex()] == 0) {
                return;
            }
            if (coro != null) {
                CancelOperation();
            }
            if (oldOperation != Operation.Powerset) {
                StartOperation(Operation.Powerset);
                coro = StartCoroutine(SelectSets(1, sets => {
                    if (sets[0].Count <= 3) {
                        actionStack.PerformAction(new MakePowerset(puzzleLoader, sets[0], actionStack));                        
                    }
                    FinishOperation(true);
                }));
            }
        }
        
        private IEnumerator SelectSets(int count, Action<Set[]> withSets) {
            var sets = new Set[count];
            for (var i = 0; i < count; i++) {
                while (sets[i].IsNull()) {
                    if (Input.GetMouseButtonDown(0)) {
                        var data = new PointerEventData(null) { position = Input.mousePosition };
                        var list = new List<RaycastResult>();
                        EventSystem.current.RaycastAll(data, list);

                        var hoveredSet = list
                            .Select(res => res.gameObject.GetComponent<Set>())
                            .FirstOrDefault(s => s);

                        // ReSharper disable once PossibleNullReferenceException
                        if (!hoveredSet.IsNull() && !ReferenceEquals(hoveredSet, puzzleLoader.target)) {
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