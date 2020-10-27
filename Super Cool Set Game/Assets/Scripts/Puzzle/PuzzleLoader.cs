using UnityEngine;

namespace Puzzle {
    public class PuzzleLoader : MonoBehaviour {
        [SerializeField]
        private Puzzle puzzle;

        [SerializeField] private RectTransform dragHolder;
        [SerializeField] private RectTransform elements;
        [SerializeField] private RectTransform[] inputBySize;
        [SerializeField] private RectTransform output;

        public void Load() {
            puzzle.CreateElements(elements, dragHolder, 50);
            
            for (var i = 0; i < puzzle.FixedSetCount; i++) {
                inputBySize[i].gameObject.SetActive(false);
            }
            var fixedSetParent = inputBySize[puzzle.FixedSetCount - 1];
            fixedSetParent.gameObject.SetActive(true);
            puzzle.CreateFixedSets(fixedSetParent, 250);

            puzzle.CreateTargetSet(output, 250);
        }
    }
}