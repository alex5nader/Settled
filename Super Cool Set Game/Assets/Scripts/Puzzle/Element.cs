using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Puzzle {
    [RequireComponent(typeof(Image))]
    public class Element : BaseElement {
        private void Awake() {
            value = GetComponent<Image>().sprite.GetInstanceID();
        }
        
        public int value;
        
        public override bool Equals(BaseElement other) {
            if (!(other is Element otherElement)) {
                return false;
            }

            return value == otherElement.value;
        }

        protected override IEnumerable<BaseElement> Children() {
            yield break;
        }

        public override string ToString() {
            return value.ToString();
        }
    }
}
