using System;
using System.Collections;
using UnityEngine;

namespace Puzzle {
    public class PuzzleLoader : MonoBehaviour {
#pragma warning disable 0649
        [SerializeField] private Object.Puzzle puzzle;
        
        [SerializeField] private float fadeLength;

        [SerializeField] [InspectorName("World UI")] private CanvasGroup worldUi;
        
        [SerializeField] [InspectorName("Puzzle UI")] private CanvasGroup puzzleUi;

        private int x;
        
        [SerializeField] private RectTransform dragHolder;
        [SerializeField] private RectTransform elements;
        [SerializeField] private RectTransform[] inputBySize;
        [SerializeField] private RectTransform output;
#pragma warning restore 0649

        private Set[] inputs;
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
            } else {
                for (var time = 0f; time < length; time += Time.unscaledDeltaTime) {
                    group.alpha = (1 - time) / length;
                    yield return null;
                }
            }

            after?.Invoke();
        }

        #region Puzzle Entrance
        public void BeginPuzzle() {
            StartCoroutine(Fade(worldUi, fadeLength, false, EnablePuzzleUI));
        }

        private void EnablePuzzleUI() {
            puzzleUi.gameObject.SetActive(true);
            CreatePuzzle();
        }

        private void CreatePuzzle() {
            puzzle.CreateElements(elements, dragHolder, 50);

            inputs = new Set[puzzle.FixedSetCount];
            for (var i = 0; i < puzzle.FixedSetCount; i++) {
                inputBySize[i].gameObject.SetActive(false);
            }
            var fixedSetParent = inputBySize[puzzle.FixedSetCount - 1];
            fixedSetParent.gameObject.SetActive(true);

            {
                var sets = puzzle.CreateFixedSets(fixedSetParent, 250);
                var i = 0;
                foreach (var go in sets) {
                    var set = go.GetComponent<MutableSet>();
                    inputs[i] = set;
                    set.ContentsChanged += () => CheckComplete(set);
                    i++;
                }
            }

            var newTarget = puzzle.CreateTargetSet(output, 250);
            target = newTarget.GetComponent<Set>();

            StartCoroutine(Fade(puzzleUi, fadeLength, true, StopTime));
        }

        private void StopTime() {
            Time.timeScale = 0;
        }
        #endregion Puzzle Entrance

        #region Puzzle Exit
        private void CheckComplete(MutableSet changed) {
            Debug.Log($"comparing {changed} to {target}");
            if (changed.Equals(target)) {
                Debug.Log("correct!");
                Time.timeScale = 1;
                StartCoroutine(Fade(puzzleUi, fadeLength, false, DisablePuzzleUI));
            } else {
                Debug.Log("incorrect smh");
            }
        }

        private void DisablePuzzleUI() {
            puzzleUi.gameObject.SetActive(false);
            
            StartCoroutine(Fade(worldUi, fadeLength, true));
            RemovePuzzle();
        }
        
        private void RemovePuzzle() {
            inputs = null;
            foreach (var input in inputBySize) {
                input.DestroyChildren();
            }
            
            elements.DestroyChildren();
            output.DestroyChildren();
            dragHolder.DestroyChildren();
        }
        #endregion Puzzle Exit
    }
}