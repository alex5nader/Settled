using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Puzzle {
    public class OperationManager : MonoBehaviour {
        [SerializeField] private Text text;

        private MutableSet[] sets;

        private void Awake() {
            sets = FindObjectsOfType<MutableSet>();
            foreach (var set in sets) {
                set.ContentsChanged += RegenerateText;
            }
        }

        private void RegenerateText() {
            text.text = string.Join("\n", sets.Select(s => s.ToString()).Where(s => s != ""));
        }
    }
}
