using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Puzzle {
    public class PuzzleLoader : MonoBehaviour {
        [SerializeField]
        private Puzzle puzzle;

        [SerializeField] private GameObject worldRoot;
        
        [SerializeField] private float fadeLength;

        [SerializeField] private CanvasGroup worldGroup;
        [SerializeField] private CanvasGroup puzzleGroup; 
        
        [SerializeField] private RectTransform dragHolder;
        [SerializeField] private RectTransform elements;
        [SerializeField] private RectTransform[] inputBySize;
        [SerializeField] private RectTransform output;

        private Set[] inputs;
        private Set target;

        private void Awake() {
            worldGroup.gameObject.SetActive(true);
            worldGroup.alpha = 1;
            puzzleGroup.gameObject.SetActive(false);
            puzzleGroup.alpha = 0;
        }

        private void StartAll(IEnumerable<IEnumerator> coros) {
            foreach (var coro in coros) {
                StartCoroutine(coro);
            }
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
            StartCoroutine(Fade(worldGroup, fadeLength, false, EnablePuzzleUI));
        }

        private void EnablePuzzleUI() {
            puzzleGroup.gameObject.SetActive(true);
            CreatePuzzle();
        }

        private void CreatePuzzle() {
            StartAll(puzzle.CreateElements(elements, dragHolder, 50));

            inputs = new Set[puzzle.FixedSetCount];
            for (var i = 0; i < puzzle.FixedSetCount; i++) {
                inputBySize[i].gameObject.SetActive(false);
            }
            var fixedSetParent = inputBySize[puzzle.FixedSetCount - 1];
            fixedSetParent.gameObject.SetActive(true);

            {
                var (sets, fixedCoros) = puzzle.CreateFixedSets(fixedSetParent, 250);
                StartAll(fixedCoros);
                var i = 0;
                foreach (var go in sets) {
                    var set = go.GetComponent<MutableSet>();
                    inputs[i] = set;
                    set.ContentsChanged += () => CheckComplete(set);
                    i++;
                }
            }

            var (newTarget, targetCoros) = puzzle.CreateTargetSet(output, 250);
            StartAll(targetCoros);
            target = newTarget.GetComponent<Set>();
            target.RecalculateElements();

            StartCoroutine(Fade(puzzleGroup, fadeLength, true, StopTime));
        }

        private void StopTime() {
            Time.timeScale = 0;
        }
        #endregion Puzzle Entrance

        #region Puzzle Exit

        private void CheckComplete(MutableSet changed) {
            if (changed.Equals(target)) {
                Time.timeScale = 1;
                StartCoroutine(Fade(puzzleGroup, fadeLength, false, DisablePuzzleUI));
            }
        }

        private void DisablePuzzleUI() {
            puzzleGroup.gameObject.SetActive(false);
            
            StartCoroutine(Fade(worldGroup, fadeLength, true));
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