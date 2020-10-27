using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

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

        private static GameObject MakeFixedElement(Sprite sprite, bool collide) {
            var go = new GameObject($"element {sprite.name}");
            go.layer = LayerMask.NameToLayer("Element");
            go.transform.position = new Vector3(0, 0, -1);
            var rend = go.AddComponent<SpriteRenderer>();
            rend.sprite = sprite;
            rend.color = new Color(Random.value, Random.value, Random.value);
            if (collide) {
                go.AddComponent<PolygonCollider2D>();
            }
            return go;
        }

        private static GameObject MakeFloatingElement(Sprite sprite) {
            var go = MakeFixedElement(sprite, true);
            go.AddComponent<Draggable>();
            return go;
        }

        private static float RandBetween(float min, float max) {
            return Random.value * (max - min) + min;
        }

        private static GameObject MakeFixedSet(SetData data, bool mutable) {
            var go = new GameObject("fixed set");
            go.layer = LayerMask.NameToLayer("Editable Set");
            go.transform.position = new Vector3(0, 0, 1);
            go.transform.localScale *= 1f / 3;
            go.transform.rotation = Quaternion.AngleAxis(90, Vector3.forward);
            go.AddComponent<SpriteRenderer>().sprite = data.sprite;

            var coll = go.AddComponent<PolygonCollider2D>();
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
                var sets = new GameObject("sets");
                sets.transform.parent = go.transform;   

                foreach (var child in data.setData) {
                    MakeFixedSet(child, false).transform.parent = sets.transform;
                }             
            }

            if (data.elementData.Length != 0) {
                var elements = new GameObject("elements");
                elements.transform.parent = go.transform;
                
                foreach (var sprite in data.elementData) {
                    var element = MakeFixedElement(sprite, false);
                    element.transform.parent = elements.transform;
                    element.transform.position = new Vector3(
                        RandBetween(bounds.min.x, bounds.max.x),
                        RandBetween(bounds.min.y, bounds.max.y),
                        0
                    );
                }
            }

            return go;
        }

        private static GameObject MakeFloatingSet(SetData data) {
            var go = MakeFixedSet(data, false);
            go.layer = LayerMask.NameToLayer("Element");
            go.name = "floating set";
            go.transform.position = Vector3.zero;
            go.transform.localScale *= 3f / 5;
            go.transform.rotation = Quaternion.identity;
            go.AddComponent<Draggable>();
            go.GetComponent<SpriteRenderer>().color = new Color(Random.value, Random.value, Random.value);
            return go;
        }

        public (IEnumerable<GameObject> elements, IEnumerable<GameObject> sets) CreateElements() =>
            (elements.Select(MakeFloatingElement), setElements.Select(MakeFloatingSet));

        [SerializeField] private SetData[] fixedSets;
        
        public IEnumerable<GameObject> CreateFixedSets() =>
            fixedSets.Select(s => MakeFixedSet(s, true));

        public GameObject CreateTargetSet() {
            var go = MakeFixedSet(target, false);
            go.name = "target set";
            return go;
        }
    }
}