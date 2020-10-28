using System;
using System.Collections.Generic;
using System.Linq;
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

        private static void MakeFloatingElement(Sprite sprite, Transform parent, Transform dragHolder,
            Transform elementsTray) {
            var go = MakeFixedElement(sprite, parent);
            var drag = go.AddComponent<Draggable>();
            drag.holder = dragHolder;
            drag.elementsTray = elementsTray;
        }

        private static GameObject MakeFixedSet(SetElement data, Transform parent, float itemSize, bool mutable) {
            var go = new GameObject("fixed set", typeof(RectTransform));

            go.transform.SetParent(parent, false);
            var tr = (RectTransform) go.transform;
            var rect = tr.rect;
            tr.ForceUpdateRectTransforms();
            var pos = tr.position;
            tr.position = new Vector3(pos.x, pos.y, pos.z + 1);

            go.AddComponent<Image>().sprite = data.backgroundSprite;

            var grid = go.AddComponent<GridLayoutGroup>();
            grid.cellSize = new Vector2(itemSize, itemSize);
            grid.childAlignment = TextAnchor.MiddleCenter;

            var set = mutable ? go.AddComponent<MutableSet>() : go.AddComponent<Set>();

            if (data.elements.Count != 0) {
                var size = Math.Min(rect.width, rect.height);

                foreach (var child in data.elements) {
                    switch (child) {
                    case SpriteElement element: {
                        var elementGo = MakeFixedElement(element.sprite, tr);
                        elementGo.transform.SetParent(tr, false);
                        break;
                    }
                    case SetElement childSet:
                        var childGo = MakeFixedSet(childSet, tr, itemSize / size, false);
                        childGo.transform.SetParent(tr, false);
                        break;
                    }
                }
            }

            set.RecalculateElements();

            return go;
        }

        private static void MakeFloatingSet(SetElement data, Transform parent, Transform dragHolder,
            Transform elementsTray, float itemSize) {
            var go = MakeFixedSet(data, parent, itemSize, false);

            go.name = "floating set";

            go.transform.position = Vector3.zero;

            var drag = go.AddComponent<Draggable>();
            drag.holder = dragHolder;
            drag.elementsTray = elementsTray;

            go.GetComponent<Image>().color = new Color(Random.value, Random.value, Random.value);
        }

        public void CreateElements(Transform parent, Transform dragHolder, float itemSize) {
            foreach (var e in elements) {
                switch (e) {
                case SpriteElement el: {
                    MakeFloatingElement(el.sprite, parent, dragHolder, parent);
                    break;
                }
                case SetElement set: {
                    MakeFloatingSet(set, parent, dragHolder, parent, itemSize);
                    break;
                }
                }
            }
        }

        public int FixedSetCount => fixedSets.Length;

        public IEnumerable<GameObject> CreateFixedSets(Transform parent, float itemSize) =>
            fixedSets.Select(s => MakeFixedSet(s, parent, itemSize, true)).ToList();

        public GameObject CreateTargetSet(Transform parent, float itemSize) {
            var set = MakeFixedSet(target, parent, itemSize, false);
            set.name = "target set";
            return set;
        }
    }
}