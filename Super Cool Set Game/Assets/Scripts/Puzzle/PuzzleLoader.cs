using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Puzzle {
    public class PuzzleLoader : MonoBehaviour {
        [SerializeField]
        private Puzzle puzzle;

        [SerializeField] private RectTransform dragHolder;
        [SerializeField] private RectTransform elements;
        [SerializeField] private RectTransform[] inputBySize;
        [SerializeField] private RectTransform output;

        private void StartAll(IEnumerable<IEnumerator> coros) {
            foreach (var coro in coros) {
                StartCoroutine(coro);
            }
        }

        public void Load() {
            StartAll(puzzle.CreateElements(elements, dragHolder, 50));
            
            for (var i = 0; i < puzzle.FixedSetCount; i++) {
                inputBySize[i].gameObject.SetActive(false);
            }
            var fixedSetParent = inputBySize[puzzle.FixedSetCount - 1];
            fixedSetParent.gameObject.SetActive(true);
            StartAll(puzzle.CreateFixedSets(fixedSetParent, 250));

            StartAll(puzzle.CreateTargetSet(output, 250));
        }
    }
}