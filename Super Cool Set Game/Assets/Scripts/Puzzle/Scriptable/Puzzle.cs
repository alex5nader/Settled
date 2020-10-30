using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Puzzle.Actions;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace Puzzle.Scriptable {
    [CreateAssetMenu(fileName = "Puzzle", menuName = "Puzzle", order = 0)]
    public class Puzzle : ScriptableObject {
#pragma warning disable 0649
        [SerializeField] private BaseElement[] elements;
        [SerializeField] private SetElement[] fixedSets;
        [SerializeField] private SetElement target;
#pragma warning restore 0649

        private static GameObject MakeFixedElement(SpriteElement data, Transform parent) {
            var go = new GameObject($"element {data.name}", typeof(RectTransform));

            go.transform.SetParent(parent, false);
            var pos = go.transform.position;
            go.transform.position = new Vector3(pos.x, pos.y, pos.z + 1);

            var rend = go.AddComponent<Image>();
            rend.sprite = data.sprite;
            rend.color = new Color(Random.value, Random.value, Random.value);

            go.AddComponent<Element>().Scriptable = data;

            return go;
        }

        private static GameObject MakeFloatingElement(SpriteElement data,
            Transform parent,
            Transform dragHolder,
            Transform elementsTray, ActionStack actionStack) {
            var go = MakeFixedElement(data, parent);
            var drag = go.AddComponent<Draggable>();
            drag.holder = dragHolder;
            drag.elementsTray = elementsTray;
            drag.actionStack = actionStack;
            return go;
        }

        public static GameObject MakeFixedSet(SetElement data, Transform parent, bool mutable,
            [CanBeNull] Transform dragHolder, [CanBeNull] Transform elementsTray, ActionStack actionStack) {
            var go = new GameObject("fixed set", typeof(RectTransform));

            go.transform.SetParent(parent, false);
            var tr = (RectTransform) go.transform;
            var pos = tr.position;
            tr.position = new Vector3(pos.x, pos.y, pos.z + 1);

            go.AddComponent<Image>().sprite = data.backgroundSprite;

            var grid = go.AddComponent<GridLayoutGroup>();
            grid.childAlignment = TextAnchor.MiddleCenter;

            var set = mutable ? go.AddComponent<MutableSet>() : go.AddComponent<Set>();
            set.Scriptable = data;

            if (data.elements.Count != 0) {
                foreach (var child in data.elements) {
                    switch (child) {
                    case SpriteElement element:
                        var elementGo = mutable
                            ? MakeFloatingElement(element, tr, dragHolder, elementsTray, actionStack)
                            : MakeFixedElement(element, tr);
                        elementGo.transform.SetParent(tr, false);
                        break;
                    case SetElement childSet:
                        var childGo = MakeFixedSet(childSet, tr, false, null, null, actionStack);
                        childGo.transform.SetParent(tr, false);
                        break;
                    }
                }
            }

            set.RecalculateElements();

            return go;
        }

        private static void MakeFloatingSet(SetElement data,
            Transform parent,
            Transform dragHolder,
            Transform elementsTray, ActionStack actionStack) {
            var go = MakeFixedSet(data, parent, false, dragHolder, elementsTray, actionStack);

            go.name = "floating set";

            go.transform.position = Vector3.zero;

            var drag = go.AddComponent<Draggable>();
            drag.holder = dragHolder;
            drag.elementsTray = elementsTray;
            drag.actionStack = actionStack;

            go.GetComponent<Image>().color = new Color(Random.value, Random.value, Random.value);
        }

        public void CreateElements(Transform elementsTray, Transform dragHolder, ActionStack actionStack) {
            foreach (var e in elements) {
                switch (e) {
                case SpriteElement el:
                    MakeFloatingElement(el, elementsTray, dragHolder, elementsTray, actionStack);
                    break;
                case SetElement set:
                    MakeFloatingSet(set, elementsTray, dragHolder, elementsTray, actionStack);
                    break;
                }
            }
        }

        public int FixedSetCount => fixedSets.Length;

        public IEnumerable<GameObject> CreateFixedSets(Transform parent, Transform dragHolder, Transform elementsTray,
            ActionStack actionStack) =>
            fixedSets.Select(s => MakeFixedSet(s, parent, true, dragHolder, elementsTray, actionStack));

        public GameObject CreateTargetSet(Transform parent, ActionStack actionStack) {
            var set = MakeFixedSet(target, parent, false, null, null, actionStack);
            set.name = "target set";
            return set;
        }
    }
}