using UnityEngine;

namespace Puzzle {
    public class PuzzleLoader : MonoBehaviour {
        [SerializeField]
        private Puzzle puzzle;

        private GameObject elements;
        private GameObject sets;
        private GameObject fixedSets;

        private void Awake() {
            var self = transform;
            elements = new GameObject("elements");
            elements.transform.parent = self;
            sets = new GameObject("sets");
            sets.transform.parent = self;
            fixedSets = new GameObject("fixed sets");
            fixedSets.transform.parent = self;
        }

        public void Load() {
            var (newElements, newSets) = puzzle.CreateElements();
            foreach (var element in newElements) {
                element.transform.parent = elements.transform;
            }

            foreach (var set in newSets) {
                set.transform.parent = sets.transform;
            }

            foreach (var fixedSet in puzzle.CreateFixedSets()) {
                fixedSet.transform.parent = fixedSets.transform;
            }

            puzzle.CreateTargetSet().transform.parent = transform;
        }
    }
}