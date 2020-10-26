using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class OperationManager : MonoBehaviour {
    [SerializeField] private Text text;

    private Set[] _sets;

    private void Awake() {
        _sets = FindObjectsOfType<Set>();
        foreach (var set in _sets) {
            set.ContentsChanged += RegenerateText;
        }
    }

    private void RegenerateText() {
        text.text = string.Join("\n", _sets.Select(s => s.ToString()).Where(s => s != ""));
    }
}
