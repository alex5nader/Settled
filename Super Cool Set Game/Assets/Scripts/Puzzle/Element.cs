using System.Diagnostics.CodeAnalysis;
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

        // fields are effectively final and will not be mutated while in HashSet
        [SuppressMessage("ReSharper", "NonReadonlyMemberInGetHashCode")]
        public override int GetHashCode() {
            return value.GetHashCode();
        }

        public override string ToString() {
            return value.ToString();
        }
    }
}
