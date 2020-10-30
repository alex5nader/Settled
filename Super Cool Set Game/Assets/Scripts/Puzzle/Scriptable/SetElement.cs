using System.Collections.Generic;
using UnityEngine;

namespace Puzzle.Scriptable {
    [CreateAssetMenu(fileName = "Set Element", menuName = "Set Element", order = 0)]
    public class SetElement : BaseElement {
        public Sprite backgroundSprite;
        public List<BaseElement> elements;
    }
}