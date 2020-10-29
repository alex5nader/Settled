using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

public static class Extensions {
    public static float Min(this Vector2 vec) {
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