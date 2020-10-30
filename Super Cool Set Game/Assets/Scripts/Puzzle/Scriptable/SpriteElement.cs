using UnityEngine;

namespace Puzzle.Scriptable {
    /**
     * Scriptable object for a sprite element.
     */
    [CreateAssetMenu(fileName = "Sprite Element", menuName = "Sprite Element", order = 0)]
    public class SpriteElement : BaseElement {
        public Sprite sprite;

        public override int GetHashCode() {
            // effectively final
            // ReSharper disable once NonReadonlyMemberInGetHashCode
            return sprite.GetInstanceID();
        }
    }
}