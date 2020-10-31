using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Puzzle.Actions;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace Puzzle.Scriptable {
    /**
     * A single puzzle.
     */
    [CreateAssetMenu(fileName = "Puzzle", menuName = "Puzzle", order = 0)]
    public class Puzzle : ScriptableObject {
#pragma warning disable 0649
        [SerializeField] private BaseElement[] elements;
        [SerializeField] private SetElement[] fixedSets;
        [SerializeField] private SetElement target;
#pragma warning restore 0649

        public int unionCount;
        public int intersectionCount;
        public int differenceCount;
        public int powersetCount;

        /**
         * Makes a sprite element which cannot be dragged.
         */
        private static GameObject MakeFixedElement(SpriteElement data, Transform parent) {
            var go = new GameObject($"element {data.name}", typeof(RectTransform));

            go.transform.SetParent(parent, false);
            var pos = go.transform.position;
            go.transform.position = new Vector3(pos.x, pos.y, pos.z + 1);

            var rend = go.AddComponent<Image>();
            rend.sprite = data.sprite;

            go.AddComponent<global::Puzzle.SpriteElement>().Scriptable = data;

            return go;
        }

        /**
         * Makes a sprite element which can be dragged.
         */
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

        /**
         * Makes a set element which cannot be dragged.
         */
        public static GameObject MakeFixedSet(Sprite setBackground, SetElement data, Transform parent, bool mutable,
            [CanBeNull] Transform dragHolder, [CanBeNull] Transform elementsTray, ActionStack actionStack) {
            var go = new GameObject("fixed set", typeof(RectTransform));

            go.transform.SetParent(parent, false);
            var tr = (RectTransform) go.transform;
            var pos = tr.position;
            tr.position = new Vector3(pos.x, pos.y, pos.z + 1);

            var image = go.AddComponent<Image>();
            image.sprite = setBackground;
            image.type = Image.Type.Sliced;

            var grid = go.AddComponent<GridLayoutGroup>();
            grid.childAlignment = TextAnchor.MiddleCenter;

            var set = mutable ? go.AddComponent<MutableSet>() : go.AddComponent<Set>();

            if (data.IsNull()) {
                data = CreateInstance<SetElement>();
                data.elements = new List<BaseElement>();
            }
            
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
                        var childGo = mutable
                            ? MakeFloatingSet(setBackground, childSet, tr, dragHolder, elementsTray, actionStack)
                            : MakeFixedSet(setBackground, childSet, tr, false, dragHolder, elementsTray, actionStack);
                        childGo.transform.SetParent(tr, false);
                        break;
                    }
                }
            }

            set.RecalculateElements();

            return go;
        }

        /**
         * Makes a set element which can be dragged.
         */
        private static GameObject MakeFloatingSet(Sprite setBackground, SetElement data,
            Transform parent,
            Transform dragHolder,
            Transform elementsTray, ActionStack actionStack) {
            var go = MakeFixedSet(setBackground, data, parent, false, dragHolder, elementsTray, actionStack);

            go.name = "floating set";

            go.transform.position = Vector3.zero;

            var drag = go.AddComponent<Draggable>();
            drag.holder = dragHolder;
            drag.elementsTray = elementsTray;
            drag.actionStack = actionStack;

            return go;
        }

        /**
         * Creates all of this puzzle's tray elements.
         */
        public void CreateElements(Sprite setBackground, Transform elementsTray, Transform dragHolder, ActionStack actionStack) {
            foreach (var e in elements) {
                switch (e) {
                case SpriteElement el:
                    MakeFloatingElement(el, elementsTray, dragHolder, elementsTray, actionStack);
                    break;
                case SetElement set:
                    MakeFloatingSet(setBackground, set, elementsTray, dragHolder, elementsTray, actionStack);
                    break;
                }
            }
        }

        /**
         * The number of non-draggable sets in this puzzle.
         */
        public int FixedSetCount => fixedSets.Length;

        /**
         * Creates and returns all of this puzzle's non draggable sets.
         */
        public IEnumerable<GameObject> CreateFixedSets(Sprite setBackground, Transform parent, Transform dragHolder, Transform elementsTray,
            ActionStack actionStack) =>
            fixedSets.Select(s => MakeFixedSet(setBackground, s, parent, true, dragHolder, elementsTray, actionStack));

        /**
         * Creates and returns this puzzle's target set.
         */
        public GameObject CreateTargetSet(Sprite setBackground, Transform parent, ActionStack actionStack) {
            var set = MakeFixedSet(setBackground, target, parent, false, null, null, actionStack);
            set.name = "target set";
            return set;
        }
    }
}