using System;
using System.Collections.Generic;
using UnityEngine;

namespace Puzzle {
    public abstract class BaseElement : MonoBehaviour, IEquatable<BaseElement> {
        public abstract bool Equals(BaseElement other);

        public abstract override int GetHashCode();
    }
}