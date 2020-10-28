using System.Collections.Generic;
using UnityEngine;

public static class Extensions {
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

    private static Vector2 XY(this Vector3 v) {
        return new Vector2(v.x, v.y);
    }
}