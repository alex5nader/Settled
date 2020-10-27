using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Puzzle {
    public class OperationManager : MonoBehaviour {
        [SerializeField] private Text text;

        private MutableSet[] _sets;

        private void Awake() {
            _sets = FindObjectsOfType<MutableSet>();
            foreach (var set in _sets) {
                set.ContentsChanged += RegenerateText;
            }
        }

        private void RegenerateText() {
            text.text = string.Join("\n", _sets.Select(s => s.ToString()).Where(s => s != ""));
        }
    }
}
