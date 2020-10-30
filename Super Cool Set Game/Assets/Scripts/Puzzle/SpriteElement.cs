using UnityEngine;
using UnityEngine.UI;

namespace Puzzle {
    /**
     * Component for sprite elements.
     */
    [RequireComponent(typeof(Image))]
    public class SpriteElement : BaseElement {
        private void Awake() {
            value = GetComponent<Image>().sprite.GetInstanceID();
        }
        
        public int value;

        public override bool Equals(BaseElement other) {
            if (!(other is SpriteElement otherElement)) {
                return false;
            }

            return value == otherElement.value;
        }

        public override string ToString() {
            return value.ToString();
        }
    }
}
