using System;
using UnityEngine;

namespace Puzzle {
    /**
     * Component base for elements.
     */
    public abstract class BaseElement : MonoBehaviour, IEquatable<BaseElement> {
        public abstract bool Equals(BaseElement other);

        public sealed override int GetHashCode() {
            // effectively final
            // ReSharper disable once NonReadonlyMemberInGetHashCode
            return Scriptable.GetHashCode();
        }

        public Scriptable.BaseElement Scriptable { get; set; }
    }
}