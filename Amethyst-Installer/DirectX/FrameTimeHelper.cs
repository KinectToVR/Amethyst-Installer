using System.Collections.Generic;
using System.Linq;

namespace amethyst_installer_gui.DirectX {
    internal class FrameTimeHelper {
        private readonly int _depth;
        private readonly Queue<double> _queue;

        public FrameTimeHelper(int depth) {
            _depth = depth;
            _queue = new Queue<double>(_depth + 1);
        }

        public double Push(double item) {
            _queue.Enqueue(item);
            if ( _queue.Count > _depth )
                _queue.Dequeue();

            return _queue.Average();
        }
    }
}
