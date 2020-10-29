using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace Puzzle.Object {
    [CreateAssetMenu(fileName = "Puzzle", menuName = "Puzzle", order = 0)]
    public class Puzzle : ScriptableObject {

#pragma warning disable 0649
        [SerializeField] private BaseElement[] elements;
        [SerializeField] private SetElement[] fixedSets;
        [SerializeField] private SetElement target;
#pragma warning restore 0649

        private static GameObject MakeFixedElement(Sprite sprite, Transform parent) {
            var go = new GameObject($"element {sprite.name}", typeof(RectTransform));

            go.transform.SetParent(parent, false);
            var pos = go.transform.position;
            go.transform.position = new Vector3(pos.x, pos.y, pos.z + 1);

            var rend = go.AddComponent<Image>();
            rend.sprite = sprite;
            rend.color = new Color(Random.value, Random.value, Random.value);

            go.AddComponent<Element>();

            return go;
        }

        private static GameObject MakeFloatingElement(
            Sprite sprite,
            Transform parent,
            Transform dragHolder,
            Transform elementsTray
        ) {
            var go = MakeFixedElement(sprite, parent);
            var drag = go.AddComponent<Draggable>();
            drag.holder = dragHolder;
            drag.elementsTray = elementsTray;
            return go;
        }

        private static GameObject MakeFixedSet(SetElement data, Transform parent, bool mutable, [CanBeNull] Transform dragHolder, [CanBeNull] Transform elementsTray) {
            var go = new GameObject("fixed set", typeof(RectTransform));

            go.transform.SetParent(parent, false);
            var tr = (RectTransform) go.transform;
            var pos = tr.position;
            tr.position = new Vector3(pos.x, pos.y, pos.z + 1);

            go.AddComponent<Image>().sprite = data.backgroundSprite;

            var grid = go.AddComponent<GridLayoutGroup>();
            grid.childAlignment = TextAnchor.MiddleCenter;

            var set = mutable ? go.AddComponent<MutableSet>() : go.AddComponent<Set>();

            if (data.elements.Count != 0) {
                foreach (var child in data.elements) {
                    switch (child) {
                    case SpriteElement element: {
                        var elementGo = mutable ? MakeFloatingElement(element.sprite, tr, dragHolder, elementsTray) : MakeFixedElement(element.sprite, tr);
                        elementGo.transform.SetParent(tr, false);
                        break;
                    }
                    case SetElement childSet:
                        var childGo = MakeFixedSet(childSet, tr, false, null, null);
                        childGo.transform.SetParent(tr, false);
                        break;
                    }
                }
            }

            set.RecalculateElements();

            return go;
        }

        private static void MakeFloatingSet(
            SetElement data, 
            Transform parent, 
            Transform dragHolder,
            Transform elementsTray
        ) {
            var go = MakeFixedSet(data, parent, false, dragHolder, elementsTray);

            go.name = "floating set";

            go.transform.position = Vector3.zero;

            var drag = go.AddComponent<Draggable>();
            drag.holder = dragHolder;
            drag.elementsTray = elementsTray;

            go.GetComponent<Image>().color = new Color(Random.value, Random.value, Random.value);
        }

        public void CreateElements(Transform elementsTray, Transform dragHolder) {
            foreach (var e in elements) {
                switch (e) {
                case SpriteElement el: {
                    MakeFloatingElement(el.sprite, elementsTray, dragHolder, elementsTray);
                    break;
                }
                case SetElement set: {
                    MakeFloatingSet(set, elementsTray, dragHolder, elementsTray);
                    break;
                }
                }
            }
        }

        public int FixedSetCount => fixedSets.Length;

        public IEnumerable<GameObject> CreateFixedSets(Transform parent, Transform dragHolder, Transform elementsTray) =>
            fixedSets.Select(s => MakeFixedSet(s, parent, true, dragHolder, elementsTray)).ToList();

        public GameObject CreateTargetSet(Transform parent) {
            var set = MakeFixedSet(target, parent, false, null, null);
            set.name = "target set";
            return set;
        }
    }
}