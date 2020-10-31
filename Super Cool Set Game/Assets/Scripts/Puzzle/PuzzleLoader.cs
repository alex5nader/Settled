using System;
using System.Collections;
using Puzzle.Actions;
using Puzzle.Operation;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Puzzle {
    /**
     * Component for loading puzzles.
     */
    [RequireComponent(typeof(ActionStack), typeof(Operations))]
    public class PuzzleLoader : MonoBehaviour {
        public Scriptable.Puzzle puzzle;
        
#pragma warning disable 0649
        [SerializeField] private float fadeLength;

        [SerializeField] private CanvasGroup worldUi;
        [SerializeField] private CanvasGroup puzzleUi;

        [SerializeField] private Button undoButton;
        [SerializeField] private Button redoButton;
        
        [SerializeField] private RectTransform output;
#pragma warning restore 0649

        public GridLayoutGroup input;
        public RectTransform dragHolder;
        public RectTransform elementsTray;

        public Sprite setBackground;
        
        [FormerlySerializedAs("inputGridByCount")] [SerializeField] public GridLayoutGroup[] inputGrids;
        
        public ActionStack ActionStack { get; private set; }
        private Operations operations;

        private Set target;

        private void Awake() {
            ActionStack = GetComponent<ActionStack>();
            operations = GetComponent<Operations>();
            
            worldUi.gameObject.SetActive(true);
            worldUi.alpha = 1;
            puzzleUi.gameObject.SetActive(false);
            puzzleUi.alpha = 0;
        }

        private static IEnumerator Fade(CanvasGroup group, float length, bool fadeIn, Action after = null) {
            if (fadeIn) {
                for (var time = 0f; time < length; time += Time.unscaledDeltaTime) {
                    group.alpha = time / length;
                    yield return null;
                }
                group.alpha = 1;
            } else {
                for (var time = 0f; time < length; time += Time.unscaledDeltaTime) {
                    group.alpha = 1 - time/length;
                    yield return null;
                }
                group.alpha = 0;
            }

            after?.Invoke();
        }

        #region Puzzle Entrance
        public void BeginPuzzle() {
            StartCoroutine(Fade(worldUi, fadeLength, false, EnablePuzzleUI));
        }

        /// <summary>
        /// Begins a puzzle with the specified puzzle object.
        /// </summary>
        /// <param name="puzzleToLoad">The puzzle to load.</param>
        public void BeginPuzzle(Puzzle.Scriptable.Puzzle puzzleToLoad)
        {
            puzzle = puzzleToLoad;
            BeginPuzzle();
        }

        public void ResetPuzzle() {
            RemovePuzzle();
            CreatePuzzle(false);
        }

        private void EnablePuzzleUI() {
            puzzleUi.gameObject.SetActive(true);
            worldUi.gameObject.SetActive(false);
            CreatePuzzle();
        }

        public void SubscribeToChanges(MutableSet set) {
            set.ContentsChanged += () => CheckComplete(set);
            CheckComplete(set);
        }

        private void CreatePuzzle(bool fadeIn = true) {
            operations.operationUses[Operation.Operation.Union.ToIndex()] = puzzle.unionCount;
            operations.operationUsesTexts[Operation.Operation.Union.ToIndex()].text = puzzle.unionCount.ToString();
            operations.operationUses[Operation.Operation.Intersection.ToIndex()] = puzzle.intersectionCount;
            operations.operationUsesTexts[Operation.Operation.Intersection.ToIndex()].text = puzzle.intersectionCount.ToString();
            operations.operationUses[Operation.Operation.Difference.ToIndex()] = puzzle.differenceCount;
            operations.operationUsesTexts[Operation.Operation.Difference.ToIndex()].text = puzzle.differenceCount.ToString();
            operations.operationUses[Operation.Operation.Powerset.ToIndex()] = puzzle.powersetCount;
            operations.operationUsesTexts[Operation.Operation.Powerset.ToIndex()].text = puzzle.powersetCount.ToString();

            ActionStack.Clear();
            
            undoButton.interactable = false;
            redoButton.interactable = false;
            
            puzzle.CreateElements(setBackground, elementsTray, dragHolder, ActionStack);

            input.cellSize = inputGrids[puzzle.FixedSetCount - 1].cellSize;

            var sets = puzzle.CreateFixedSets(setBackground, input.transform, dragHolder, elementsTray, ActionStack);
            foreach (var go in sets) {
                SubscribeToChanges(go.GetComponent<MutableSet>());
            }

            var newTarget = puzzle.CreateTargetSet(setBackground, output, ActionStack);
            target = newTarget.GetComponent<Set>();

            if (fadeIn) {
                StartCoroutine(Fade(puzzleUi, fadeLength, true, StopTime));
            }
        }

        private void StopTime() {
            Time.timeScale = 0;
        }
        #endregion Puzzle Entrance

        #region Puzzle Exit
        private void CheckComplete(MutableSet changed) {
            if (changed.Equals(target)) {
                Time.timeScale = 1;
                StartCoroutine(Fade(puzzleUi, fadeLength, false, DisablePuzzleUI));
            }
        }

        private void DisablePuzzleUI() {
            puzzleUi.gameObject.SetActive(false);
            
            worldUi.gameObject.SetActive(true);
            StartCoroutine(Fade(worldUi, fadeLength, true));
            RemovePuzzle();
        }
        
        private void RemovePuzzle() {
            input.transform.DestroyChildren();
            
            elementsTray.DestroyChildren();
            output.DestroyChildren();
            dragHolder.DestroyChildren();
        }
        #endregion Puzzle Exit
    }
}