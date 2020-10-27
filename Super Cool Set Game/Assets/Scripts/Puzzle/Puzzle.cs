using System;
using System.Collections.Generic;
using System.Linq;
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
            public string textName;
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

        private static GameObject MakeFixedSet(SetData data, Transform parent, float itemSize, bool mutable) {
            var go = new GameObject("fixed set", typeof(RectTransform));
            
            go.transform.SetParent(parent, false);
            var tr = go.transform;
            var rect = ((RectTransform) tr).rect;
            var pos = tr.position;
            tr.position = new Vector3(pos.x, pos.y, pos.z + 1);
            
            go.AddComponent<Image>().sprite = data.sprite;
            
            var grid = go.AddComponent<GridLayoutGroup>();
            grid.cellSize = new Vector2(itemSize, itemSize);
            grid.childAlignment = TextAnchor.MiddleCenter;

            var coll = go.AddComponent<BoxCollider2D>();
            coll.isTrigger = true;
            var bounds = coll.bounds;

            go.AddComponent<Rigidbody2D>().isKinematic = true;

            if (mutable) {
                var set = go.AddComponent<MutableSet>();
                if (!string.IsNullOrWhiteSpace(data.textName)) {
                    var textGo = GameObject.Find(data.textName);
                    Debug.Log("found text", textGo);
                    set.contentsText = textGo.GetComponent<Text>();
                }
            }

            if (data.setData.Length != 0) {
                var size = Math.Min(rect.width, rect.height);
                
                foreach (var child in data.setData) {
                    MakeFixedSet(child, tr, itemSize / size, false).transform.parent = tr;
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

            return go;
        }

        private static void MakeFloatingSet(SetData data, Transform parent, Transform dragHolder, Transform elementsTray, float itemSize) {
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
                MakeFloatingElement(e, parent, dragHolder, parent);
            }
            foreach (var s in setElements) {
                MakeFloatingSet(s, parent, dragHolder, parent, itemSize);
            }
        }

        [SerializeField] private SetData[] fixedSets;
        
        public int FixedSetCount => fixedSets.Length;

        public void CreateFixedSets(Transform parent, float itemSize) {
            foreach (var s in fixedSets) {
                MakeFixedSet(s, parent, itemSize, true);
            }
        }

        public void CreateTargetSet(Transform parent, float itemSize) {
            var go = MakeFixedSet(target, parent, itemSize, false);
            go.name = "target set";
        }
    }
}