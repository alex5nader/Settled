using System.Collections.Generic;
using UnityEngine;

public static class Extensions {
    public static bool Contains(this Collider2D self, Collider2D other) {
        var selfBounds = self.bounds;

        var selfMin = selfBounds.min.XY();
        var selfMax = selfBounds.max.XY();
        var otherCenter = other.bounds.center.XY();

        bool Between(float t, float a, float b) {
            return a <= t && t < b;
        }

        return Between(otherCenter.x, selfMin.x, selfMax.x)
            && Between(otherCenter.y, selfMin.y, selfMax.y);
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