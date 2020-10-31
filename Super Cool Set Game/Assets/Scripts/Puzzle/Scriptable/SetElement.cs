using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Puzzle.Scriptable {
    /**
     * Scriptable object for a set element.
     */
    [CreateAssetMenu(fileName = "Set Element", menuName = "Set Element", order = 0)]
    public class SetElement : BaseElement {
        public List<BaseElement> elements;

        public override int GetHashCode() {
            // effectively final
            // ReSharper disable once NonReadonlyMemberInGetHashCode
            return elements
                .Select(b => b.GetHashCode())
                .OrderBy(h => h)
                .Aggregate(19, (hs, h) => hs * 31 + h);
        }
    }
}