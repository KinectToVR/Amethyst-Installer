using System;

namespace amethyst_installer_gui {
    public static class MathExtensions {
        public static float Lerp(float a, float b, float t) {
            return a * ( 1.0f - t ) + t * b;
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