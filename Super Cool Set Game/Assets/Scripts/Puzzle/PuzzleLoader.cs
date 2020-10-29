using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Puzzle {
    public class PuzzleLoader : MonoBehaviour {
#pragma warning disable 0649
        [SerializeField] private Object.Puzzle puzzle;
        
        [SerializeField] private float fadeLength;

        [SerializeField] private CanvasGroup worldUi;
        [SerializeField] private CanvasGroup puzzleUi;
        
        [SerializeField] private RectTransform dragHolder;
        [SerializeField] private RectTransform elementsTray;
        [SerializeField] private GridLayoutGroup input;
        [SerializeField] private GridLayoutGroup[] inputGridByCount;
        [SerializeField] private RectTransform output;
#pragma warning restore 0649

        private Set target;

        private void Awake() {
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
            input.cellSize = inputGridByCount[tempCount - 2].cellSize;
            Destroy(input.transform.GetChild(tempCount - 1).gameObject);
            tempCount--;
        }

        private void CreatePuzzle() {
            puzzle.CreateElements(elementsTray, dragHolder);

            input.cellSize = inputGridByCount[puzzle.FixedSetCount - 1].cellSize;

            var sets = puzzle.CreateFixedSets(input.transform, dragHolder, elementsTray);
            foreach (var go in sets) {
                var set = go.GetComponent<MutableSet>();
                set.ContentsChanged += () => CheckComplete(set);
            }

            var newTarget = puzzle.CreateTargetSet(output);
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