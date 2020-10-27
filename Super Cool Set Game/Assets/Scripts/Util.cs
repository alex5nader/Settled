using UnityEngine;

public static class Util {
    public static float RandBetween(float min, float max) {
        return Random.value * (max - min) + min;
    }
}