namespace amethyst_installer_gui {
    public struct Vec2 {
        public double x;
        public double y;

        public Vec2(double x, double y) {
            this.x = x;
            this.y = y;
        }

        public Vec2(double mag) : this(mag, mag) { }

        public override string ToString() {
            return $"{{ {x}, {y} }}";
        }

        public static readonly Vec2 Zero    = new Vec2(0, 0);
        public static readonly Vec2 One     = new Vec2(1, 1);
        public static readonly Vec2 Left    = new Vec2(-1, 0);
        public static readonly Vec2 Right   = new Vec2(1, 0);
        public static readonly Vec2 Up      = new Vec2(0, 1);
        public static readonly Vec2 Down    = new Vec2(0, -1);
    }
}
