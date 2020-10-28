using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace Puzzle {
    [CreateAssetMenu(fileName = "Puzzle", menuName = "Puzzle", order = 0)]
    public class Puzzle : ScriptableObject {
        [System.Serializable]
        public struct SetData {
            public SetData[] setData;
            public Sprite[] elementData;
            public Sprite sprite;
        }
        
        [SerializeField] private Sprite[] elements;
        [SerializeField] private SetData[] setElements;
        [SerializeField] private SetData target;

        private static GameObject MakeFixedElement(Sprite sprite, Transform parent, bool collide) {
            var go = new GameObject($"element {sprite.name}", typeof(RectTransform));
            
            go.transform.SetParent(parent, false);
            var pos = go.transform.position;
            go.transform.position = new Vector3(pos.x, pos.y, pos.z + 1);
            
            var rend = go.AddComponent<Image>();
            rend.sprite = sprite;
            rend.color = new Color(Random.value, Random.value, Random.value);

            go.AddComponent<Element>();
            
            if (collide) {
                go.AddComponent<BoxCollider2D>();
            }
            
            return go;
        }

        private static void MakeFloatingElement(Sprite sprite, Transform parent, Transform dragHolder, Transform elementsTray) {
            var go = MakeFixedElement(sprite, parent, true);
            var drag = go.AddComponent<Draggable>();
            drag.holder = dragHolder;
            drag.elementsTray = elementsTray;
        }

        private static (GameObject, IEnumerable<IEnumerator>) MakeFixedSet(SetData data, Transform parent, float itemSize, bool mutable) {
            var coros = new List<IEnumerator>();
            
            var go = new GameObject("fixed set", typeof(RectTransform));
            
            go.transform.SetParent(parent, false);
            var tr = (RectTransform) go.transform;
            var rect = tr.rect;
            tr.ForceUpdateRectTransforms();
            var pos = tr.position;
            tr.position = new Vector3(pos.x, pos.y, pos.z + 1);
            
            go.AddComponent<Image>().sprite = data.sprite;
            
            var grid = go.AddComponent<GridLayoutGroup>();
            grid.cellSize = new Vector2(itemSize, itemSize);
            grid.childAlignment = TextAnchor.MiddleCenter;

            var coll = go.AddComponent<BoxCollider2D>();
            coll.isTrigger = true;

            IEnumerator SetSizeNextFrame() {
                tr.ForceUpdateRectTransforms();
                yield return null;
                coll.size = tr.rect.size;
            }
            coros.Add(SetSizeNextFrame());
            
            var bounds = coll.bounds;

            go.AddComponent<Rigidbody2D>().isKinematic = true;

            var set = mutable ? go.AddComponent<MutableSet>() : go.AddComponent<Set>();

            if (data.setData.Length != 0) {
                var size = Math.Min(rect.width, rect.height);
                
                foreach (var child in data.setData) {
                    var (childGo, childCoros) = MakeFixedSet(child, tr, itemSize / size, false);
                    childGo.transform.parent = tr;
                    coros.AddRange(childCoros);
                }
            }

            if (data.elementData.Length != 0) {
                foreach (var sprite in data.elementData) {
                    var element = MakeFixedElement(sprite, tr, false);
                    element.transform.SetParent(tr);
                    element.transform.position = new Vector3(
                        Util.RandBetween(bounds.min.x, bounds.max.x),
                        Util.RandBetween(bounds.min.y, bounds.max.y),
                        0
                    );
                }
            }
            
            set.RecalculateElements();

            return (go, coros);
        }

        private static IEnumerable<IEnumerator> MakeFloatingSet(SetData data, Transform parent, Transform dragHolder, Transform elementsTray, float itemSize) {
            var (go, coros) = MakeFixedSet(data, parent, itemSize, false);
            
            go.name = "floating set";
            
            go.transform.position = Vector3.zero;
            
            var drag = go.AddComponent<Draggable>();
            drag.holder = dragHolder;
            drag.elementsTray = elementsTray;
            
            go.GetComponent<Image>().color = new Color(Random.value, Random.value, Random.value);

            return coros;
        }

        public IEnumerable<IEnumerator> CreateElements(Transform parent, Transform dragHolder, float itemSize) {
            foreach (var e in elements) {
                MakeFloatingElement(e, parent, dragHolder, parent);
            }

            var coros = new List<IEnumerator>();
            foreach (var s in setElements) {
                coros.AddRange(MakeFloatingSet(s, parent, dragHolder, parent, itemSize));
            }

            return coros;
        }

        [SerializeField] private SetData[] fixedSets;
        
        public int FixedSetCount => fixedSets.Length;

        public (IEnumerable<GameObject> sets, IEnumerable<IEnumerator> coros) CreateFixedSets(Transform parent, float itemSize) {
            var coros = new List<IEnumerator>();
            var sets = new List<GameObject>();
            foreach (var s in fixedSets) {
                var (set, setCoros) = MakeFixedSet(s, parent, itemSize, true);
                sets.Add(set);
                coros.AddRange(setCoros);
            }
            return (sets, coros);
        }

        public (GameObject set, IEnumerable<IEnumerator> coros) CreateTargetSet(Transform parent, float itemSize) {
            var (set, coros) = MakeFixedSet(target, parent, itemSize, false);
            set.name = "target set";
            return (set, coros);
        }
    }
}