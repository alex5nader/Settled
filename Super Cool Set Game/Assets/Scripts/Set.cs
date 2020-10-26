using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Collider2D))]
public class Set : MonoBehaviour {
    public delegate void ContentChangeEvent();
    
    [SerializeField] private Text contentsText;
    
    private Collider2D _collider;

    private readonly List<GameObject> _elements = new List<GameObject>();

    public event ContentChangeEvent ContentsChanged;

    private void UpdateContentsText() {
        contentsText.text = ToString();
    }

#if UNITY_EDITOR
    private void Reset() {
        if (gameObject.layer != LayerMask.NameToLayer("Editable Set")) {
            Debug.LogError($"{GetType().FullName} must be on the Editable Set layer.");
        }
    }
#endif // UNITY_EDITOR

    private void Awake() {
        _collider = GetComponent<Collider2D>();
#if UNITY_EDITOR
        if (!contentsText) {
            Debug.LogWarning($"No contents text defined; set will not display its contents.", gameObject);
        }
#endif
    }

    private void OnTriggerStay2D(Collider2D other) {
#if UNITY_EDITOR
        if (other.gameObject.layer != LayerMask.NameToLayer("Element")) {
            Debug.LogError($"{other.gameObject.name} must be on the Element layer.");
        }
#endif // UNITY_EDITOR
        var alreadyTracked = _elements.Contains(other.gameObject); 
        var inside = _collider.Contains(other);
        if (!alreadyTracked && inside) {
            _elements.Add(other.gameObject);
            OnContentsChanged();
        } else if (alreadyTracked && !inside) {
            _elements.Remove(other.gameObject);
            OnContentsChanged();
        }
    }

    protected virtual void OnContentsChanged() {
        UpdateContentsText();
        ContentsChanged?.Invoke();
    }

    public override string ToString() {
        return string.Join("\n", _elements.Select(x => x.name));
    }
}