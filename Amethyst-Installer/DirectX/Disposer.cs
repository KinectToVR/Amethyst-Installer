﻿using System;

namespace amethyst_installer_gui.DirectX {
    public static class Disposer {
        public static void SafeDispose<T>(ref T resource) where T : class {
            if ( resource == null ) {
                return;
            }

            var disposer = resource as IDisposable;
            if ( disposer != null ) {
                try {
                    disposer.Dispose();
                } catch {
                }
            }

            resource = null;
        }
    }
}
