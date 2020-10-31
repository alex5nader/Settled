using System;
using System.Collections;
using Puzzle.Actions;
using Puzzle.Scriptable;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Puzzle {

    public delegate void OnPuzzleComplete();

    /**
     * Component for loading puzzles.
     */
    [RequireComponent(typeof(ActionStack))]
    public class PuzzleLoader : MonoBehaviour {
        private Scriptable.Puzzle puzzle = null;
        public OnPuzzleComplete currentPuzzleComplete;
        
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
        
        [FormerlySerializedAs("inputGridByCount")] [SerializeField] public GridLayoutGroup[] inputGrids;
        
        public ActionStack ActionStack { get; private set; }

        private Set target;

        private void Awake() {
            ActionStack = GetComponent<ActionStack>();
            
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

        /// <summary>
        /// Begin puzzle with no delegate to call (necessary for Unity Serialization to be used with Buttons)
        /// </summary>
        /// <param name="puzzleToLoad">The puzzle to load.</param>
        public void BeginPuzzle(Puzzle.Scriptable.Puzzle puzzleToLoad)
        {
            BeginPuzzle(puzzleToLoad, null);
        }

        /// <summary>
        /// Begins a puzzle with the specified puzzle object.
        /// </summary>
        /// <param name="puzzleToLoad">The puzzle to load.</param>
        /// <param name="onPuzzleComplete">The delegate to call when the puzzle is complete.</param>
        public void BeginPuzzle(Puzzle.Scriptable.Puzzle puzzleToLoad, OnPuzzleComplete onPuzzleComplete = null)
        {
            if (puzzle) // check that we are not loading multiple puzzles at once
            {
                Debug.Log("Cannot start new puzzle because a puzzle is already loaded.");
                return;
            }

            // if the on complete delegate is not null, assign it
            currentPuzzleComplete += onPuzzleComplete ?? (() => { });

            puzzle = puzzleToLoad;

            StartCoroutine(Fade(worldUi, fadeLength, false, EnablePuzzleUI));
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

        private void CreatePuzzle() {
            ActionStack.Clear();
            
            undoButton.interactable = false;
            redoButton.interactable = false;
            
            puzzle.CreateElements(elementsTray, dragHolder, ActionStack);

            input.cellSize = inputGrids[puzzle.FixedSetCount - 1].cellSize;

            var sets = puzzle.CreateFixedSets(input.transform, dragHolder, elementsTray, ActionStack);
            foreach (var go in sets) {
                SubscribeToChanges(go.GetComponent<MutableSet>());
            }

            var newTarget = puzzle.CreateTargetSet(output, ActionStack);
            target = newTarget.GetComponent<Set>();

            StartCoroutine(Fade(puzzleUi, fadeLength, true, StopTime));
        }

        private void StopTime() {
            Time.timeScale = 0;
        }
        #endregion Puzzle Entrance

        #region Puzzle Exit
        private void CheckComplete(MutableSet changed) {
            if (changed.Equals(target))
                CompletePuzzle();
        }

        private void CompletePuzzle()
        {
            GameManager.MarkPuzzleComplete(puzzle);
            currentPuzzleComplete?.Invoke();            // call the completed puzzle delegate
            currentPuzzleComplete = null;

            ClosePuzzle();
        }

        private void ClosePuzzle()
        {
            Time.timeScale = 1;
            StartCoroutine(Fade(puzzleUi, fadeLength, false, DisablePuzzleUI));

            puzzle = null;
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