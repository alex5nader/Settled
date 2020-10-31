using UnityEngine;
using UnityEngine.UI;

namespace Puzzle {
    /**
     * Component for sprite elements.
     */
    [RequireComponent(typeof(Image))]
    public class SpriteElement : BaseElement {
        private void Awake() {
            var sprite = GetComponent<Image>().sprite;
            value = sprite.GetInstanceID();
            repr = sprite.name;
        }
        
        public int value;
        private string repr;

        public override bool Equals(BaseElement other) {
            if (!(other is SpriteElement otherElement)) {
                return false;
            }

            return value == otherElement.value;
        }

        public override string ToString() {
            return repr;
        }
    }
}
