using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Object = UnityEngine.Object;

public static class Extensions {
    public static int ActiveChildCount(this Transform tr) {
        return tr.Children().Count(child => child.gameObject.activeSelf);
    }
    
    public static bool IsNull(this Object o) {
        return ReferenceEquals(o, null);
    }
    
    public static float MinComponent(this Vector2 vec) {
        return Math.Min(vec.x, vec.y);
    }
    
    public static void DestroyChildren(this Transform tr) {
        foreach (var child in tr.Children()) {
            Object.Destroy(child.gameObject);
        }
    }

    public static IEnumerable<Transform> Children(this Transform self) {
        var count = self.childCount;
        for (var i = 0; i < count; i++) {
            yield return self.GetChild(i);
        }
    }
}