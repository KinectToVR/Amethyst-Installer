namespace amethyst_installer_gui {
    public struct Vec2 {
        public double x;
        public double y;

        public Vec2(double x, double y) {
            this.x = x;
            this.y = y;
        }

        public Vec2(double mag) : this(mag, mag) { }
    }
}
