using System;
using System.Collections;
using Puzzle.Actions;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Puzzle {
    [RequireComponent(typeof(ActionStack))]
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
        public void BeginPuzzle() {
            StartCoroutine(Fade(worldUi, fadeLength, false, EnablePuzzleUI));
        }

        private void EnablePuzzleUI() {
            puzzleUi.gameObject.SetActive(true);
            worldUi.gameObject.SetActive(false);
            CreatePuzzle();
        }

        private int tempCount = -1;
        public void RemoveFixedSet() {
            if (tempCount == -1) {
                tempCount = puzzle.FixedSetCount;
            }
            input.cellSize = inputGrids[tempCount - 2].cellSize;
            Destroy(input.transform.GetChild(tempCount - 1).gameObject);
            tempCount--;
        }

        private void CreatePuzzle() {
            ActionStack.Clear();
            
            undoButton.interactable = false;
            redoButton.interactable = false;
            
            puzzle.CreateElements(elementsTray, dragHolder, ActionStack);

            input.cellSize = inputGrids[puzzle.FixedSetCount - 1].cellSize;

            var sets = puzzle.CreateFixedSets(input.transform, dragHolder, elementsTray, ActionStack);
            foreach (var go in sets) {
                var set = go.GetComponent<MutableSet>();
                set.ContentsChanged += () => CheckComplete(set);
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