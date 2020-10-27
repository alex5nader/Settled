using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Puzzle {
    public abstract class BaseElement : MonoBehaviour, IEquatable<BaseElement> {
        protected abstract IEnumerable<BaseElement> Children();
        
        public abstract bool Equals(BaseElement other);
    }
}