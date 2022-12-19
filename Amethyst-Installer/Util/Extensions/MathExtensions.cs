using System;
using System.Numerics;

namespace amethyst_installer_gui {
    public static class MathExtensions {
        public static float Lerp(float a, float b, float t) {
            return a * ( 1.0f - t ) + t * b;
        }
        public static Vector2 Lerp(Vector2 a, Vector2 b, float t) {
            return a * ( 1.0f - t ) + t * b;
        }
        public static Vector4 Lerp(Vector4 a, Vector4 b, float t) {
            return a * ( 1.0f - t ) + t * b;
        }
        public static Vector3 Lerp(Vector3 a, Vector3 b, float t) {
            return a * ( 1.0f - t ) + t * b;
        }

        public static float Saturate(float x) {
            return x < 0.0f ? 0.0f : (x > 1.0f ? 1.0f : x);
        }
        public static float InverseLerp(float from, float to, float value) {
            return ( value - from ) / ( to - from );
        }
        public static float Remap(float srcFrom, float srcTo, float destFrom, float destTo, float t) {
            float rel = InverseLerp(srcFrom, srcTo, t);
            return Lerp(destFrom, destTo, rel);
        }
    }

    public static class RandomExtensions {
        public static float NextFloat(this Random rng) {
            return (float)rng.NextDouble();
        }
        public static float NextFloat(this Random rng, float min, float max) {
            return MathExtensions.Lerp(min, max, (float)rng.NextDouble());
        }
    }
}