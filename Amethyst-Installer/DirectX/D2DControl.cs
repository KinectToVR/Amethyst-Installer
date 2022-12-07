using Microsoft.Win32;
using SharpDX;
using SharpDX.Direct2D1;
using SharpDX.Direct3D;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Threading;
using DeviceContext = SharpDX.Direct2D1.DeviceContext;

namespace amethyst_installer_gui.DirectX {
    public abstract class D2DControl : System.Windows.Controls.Image, IDisposable {

        /// <summary>
        /// Called whenever we're supposed to render a frame
        /// </summary>
        public abstract void Render(SharpDX.Direct2D1.DeviceContext target);

        protected SharpDX.Direct3D11.Device device;
        protected Texture2D sharedTarget;
        protected Texture2D dx11Target;
        protected DX11ImageSource d3DSurface;
        protected DeviceContext d2DRenderTarget;

        protected ResourceCache resCache = new ResourceCache();

        public TimeSpan Timeout {
            get { return m_timeout; }
            set { m_timeout = value; }
        }

        public static bool IsInDesignMode {
            get {
                var prop = DesignerProperties.IsInDesignModeProperty;
                var isDesignMode = (bool)DependencyPropertyDescriptor.FromProperty(prop, typeof(FrameworkElement)).Metadata.DefaultValue;
                return isDesignMode;
            }
        }

        private static readonly DependencyPropertyKey FpsPropertyKey = DependencyProperty.RegisterReadOnly("Fps", typeof(int), typeof(D2DControl), new FrameworkPropertyMetadata(0, FrameworkPropertyMetadataOptions.None));
        public static readonly DependencyProperty FpsProperty = FpsPropertyKey.DependencyProperty;

        public int Fps {
            get { return ( int ) GetValue(FpsProperty); }
            protected set { SetValue(FpsPropertyKey, value); }
        }

        public static readonly DependencyPropertyKey FrameTimePropertyKey = DependencyProperty.RegisterReadOnly("FrameTime", typeof(double), typeof(D2DControl), new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.None));
        public double FrameTime {
            get { return ( double ) GetValue(FrameTimePropertyKey.DependencyProperty); }
            protected set { SetValue(FrameTimePropertyKey, value); }
        }

        public SharpDX.Direct3D11.Device DX11Device => device;
        public SharpDX.Direct3D11.DeviceContext DX11Context => device.ImmediateContext;
        public SharpDX.Direct2D1.DeviceContext D2DRenderTarget => d2DRenderTarget;
        public D2DControlError Error => m_error;

        private bool m_initialized = false;
        private bool m_isError = false;

        private bool m_hardwareAcceleration = true;
        private readonly Stopwatch m_renderTimer = new Stopwatch();

        private long m_lastFrameTime = 0;
        private int m_frameCount = 0;
        private int m_frameCountHistTotal = 0;
        private Queue<int> m_frameCountHist = new Queue<int>();

        private D2DControlError m_error = D2DControlError.OK;
        private TimeSpan m_timeout = new TimeSpan(0, 0, 0, 0, 33);

        private const int MIN_WIDTH = 128;
        private const int MIN_HEIGHT = 128;

        public D2DControl() {
            base.Loaded += Control_Loaded;
            base.Unloaded += Control_Closing;

            base.Stretch = System.Windows.Media.Stretch.Fill;
        }

        protected void Shutdown() {
            StopRendering();
            EndD3D();
        }

        public void Dispose() {
            Shutdown();
            Destroy();
            resCache.Clear();
        }

        /// <summary>
        /// Called whenever we must re-create resources
        /// </summary>
        public virtual void TargetsCreated() { }

        private void Control_Loaded(object sender, RoutedEventArgs e) {
            InitializeInternal();
        }

        private void InitializeInternal() {
            if ( D2DControl.IsInDesignMode ) {
                m_error = D2DControlError.ERR_DESIGN_MODE;
                return;
            }

            if ( m_initialized ) {
                return;
            }

            m_initialized = true;
            m_error = D2DControlError.OK;
            StartD3D();
            StartRendering();
        }

        private void Control_Closing(object sender, RoutedEventArgs e) {
            if ( D2DControl.IsInDesignMode ) {
                m_error = D2DControlError.ERR_DESIGN_MODE;
                return;
            }

            m_error = D2DControlError.OK;
            Shutdown();
        }

        private void OnRendering(object sender, EventArgs e) {
            if ( !m_renderTimer.IsRunning ) {
                m_error = D2DControlError.ERR_RENDER_TIMER_NOT_RUNNING;
                return;
            }

            if ( m_isError ) {
                Reinitialize();
                m_isError = false;
            }

            OnRenderFrameInternal();
        }

        private void OnRenderFrameInternal() {
            m_error = D2DControlError.OK;
            frameTimer.Restart();
            PrepareAndCallRender();
            if ( d3DSurface.TryLock(new Duration(m_timeout)) ) {
                device.ImmediateContext.ResolveSubresource(dx11Target, 0, sharedTarget, 0, Format.B8G8R8A8_UNorm);
                d3DSurface.InvalidateD3DImage();
                d3DSurface.Unlock();
            }
            FrameTime = _timeHelper.Push(frameTimer.Elapsed.TotalMilliseconds);

        }

        private void Reinitialize() {

            DetectHardwareAcceleration();

            Shutdown();
            Destroy();
            InitializeInternal();
            CreateAndBindTargets();
        }

        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo) {
            CreateAndBindTargets();
            base.OnRenderSizeChanged(sizeInfo);
        }

        private void OnIsFrontBufferAvailableChanged(object sender, DependencyPropertyChangedEventArgs e) {

            if ( !m_hardwareAcceleration ) {
                return;
            }

            if ( d3DSurface == null ) {
                m_error = D2DControlError.ERR_DEVICE_NULL;
                return;
            }

            if ( d3DSurface.IsFrontBufferAvailable ) {
                m_error = D2DControlError.OK;
                StartRendering();
            } else {
                m_error = D2DControlError.ERR_FRONT_BUFFER_UNAVAILABLE;
                StopRendering();
            }
        }

        // - private methods -------------------------------------------------------------

        private void StartD3D() {
            device = new SharpDX.Direct3D11.Device(DriverType.Hardware, DeviceCreationFlags.BgraSupport |
#if DEBUG
                                                                         DeviceCreationFlags.Debug |
#endif
                                                                         DeviceCreationFlags.None);

            DetectHardwareAcceleration();
            d3DSurface = new DX11ImageSource();
            d3DSurface.IsFrontBufferAvailableChanged += OnIsFrontBufferAvailableChanged;

            CreateAndBindTargets();

            base.Source = d3DSurface;
        }

        private void EndD3D() {
            d3DSurface.IsFrontBufferAvailableChanged -= OnIsFrontBufferAvailableChanged;
            base.Source = null;

            // Hint to the graphics driver that the VRAM which is currently in use may be used by other apps
            using ( var dxgiDevice = device.QueryInterface<SharpDX.DXGI.Device3>() )
                dxgiDevice.Trim();

            Disposer.SafeDispose(ref d2DRenderTarget);
            Disposer.SafeDispose(ref d3DSurface);
            Disposer.SafeDispose(ref sharedTarget);
            Disposer.SafeDispose(ref dx11Target);
            Disposer.SafeDispose(ref device);
        }

        private void CreateAndBindTargets() {

            if ( d3DSurface == null )
                return;

            try {

                d3DSurface.SetRenderTarget(null);

                Disposer.SafeDispose(ref d2DRenderTarget);
                Disposer.SafeDispose(ref sharedTarget);
                Disposer.SafeDispose(ref dx11Target);

                int width  = Math.Max((int)ActualWidth , MIN_WIDTH);
                int height = Math.Max((int)ActualHeight, MIN_HEIGHT);

                var frontDesc = new Texture2DDescription {
                    BindFlags = BindFlags.RenderTarget | BindFlags.ShaderResource,
                    Format = Format.B8G8R8A8_UNorm,
                    Width = width,
                    Height = height,
                    MipLevels = 1,
                    SampleDescription = new SampleDescription(1, 0),
                    Usage = ResourceUsage.Default,
                    OptionFlags = ResourceOptionFlags.Shared,
                    CpuAccessFlags = CpuAccessFlags.None,
                    ArraySize = 1
                };

                var backDesc = new Texture2DDescription {
                    BindFlags = BindFlags.RenderTarget | BindFlags.ShaderResource,
                    Format = Format.B8G8R8A8_UNorm,
                    Width = width,
                    Height = height,
                    MipLevels = 1,
                    SampleDescription = new SampleDescription(1, 0),
                    Usage = ResourceUsage.Default,
                    OptionFlags = ResourceOptionFlags.None,
                    CpuAccessFlags = CpuAccessFlags.None,
                    ArraySize = 1
                };

                sharedTarget = new Texture2D(device, frontDesc);
                dx11Target = new Texture2D(device, backDesc);

                using ( var surface = dx11Target.QueryInterface<Surface>() ) {

                    d2DRenderTarget = new SharpDX.Direct2D1.DeviceContext(surface, new CreationProperties() {
                        Options = DeviceContextOptions.EnableMultithreadedOptimizations,
#if DEBUG
                        DebugLevel = DebugLevel.Information,
#endif
                        ThreadingMode = ThreadingMode.SingleThreaded,
                    });
                }

                resCache.RenderTarget = d2DRenderTarget;

                d3DSurface.SetRenderTarget(sharedTarget);

                device.ImmediateContext.Rasterizer.SetViewport(0, 0, width, height, 0.0f, 1.0f);
                TargetsCreated();
            } catch {
                m_isError = true;
            }
        }

        private void StartRendering() {
            if ( m_renderTimer.IsRunning ) {
                return;
            }

            System.Windows.Media.CompositionTarget.Rendering += OnRendering;
            m_renderTimer.Start();
        }

        private void StopRendering() {
            if ( !m_renderTimer.IsRunning ) {
                return;
            }

            System.Windows.Media.CompositionTarget.Rendering -= OnRendering;
            m_renderTimer.Stop();
        }
        private void Destroy() {

            if (IsInDesignMode) {
                return;
            }

            if ( !m_initialized ) {
                return;
            }

            m_initialized = false;
            Disposer.SafeDispose(ref device);
        }

        private Stopwatch frameTimer = new Stopwatch();
        private FrameTimeHelper _timeHelper = new FrameTimeHelper(60);
        private void PrepareAndCallRender() {
            if ( device == null ) {
                return;
            }

            d2DRenderTarget.BeginDraw();
            Render(d2DRenderTarget);
            d2DRenderTarget.EndDraw();

            ComputeFramerate();

            device.ImmediateContext.Flush();
        }

        private void ComputeFramerate() {
            m_frameCount++;
            if ( m_renderTimer.ElapsedMilliseconds - m_lastFrameTime > 1000 ) {
                m_frameCountHist.Enqueue(m_frameCount);
                m_frameCountHistTotal += m_frameCount;
                if ( m_frameCountHist.Count > 5 ) {
                    m_frameCountHistTotal -= m_frameCountHist.Dequeue();
                }

                Fps = m_frameCountHistTotal / m_frameCountHist.Count;

                m_frameCount = 0;
                m_lastFrameTime = m_renderTimer.ElapsedMilliseconds;
            }
        }

        private void DetectHardwareAcceleration() {

            // Check if the GPU can even do hardware acceleration (this should never trigger in 2022)
            if ( ( RenderCapability.Tier >> 16 ) == 0 ) {
                m_hardwareAcceleration = false;
                return;
            }

            // Remote sessions can break hardware accelerate WPF rendering
            if (GetSystemMetrics(SM_REMOTESESSION) != 0) {
                m_hardwareAcceleration = false;
                return;
            }

            // WPF has a hard coded machine-wide override here
            var subKey = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Avalon.Graphics");
            if ( subKey != null ) {
                if ( subKey.GetValue("DisableHWAcceleration") is int d ) {
                    if ( d == 1 ) {
                        m_hardwareAcceleration = false;
                        return;
                    }
                }
            }

            // Additionally this is also checked after the registry key override
            if (RenderOptions.ProcessRenderMode == RenderMode.SoftwareOnly) {
                m_hardwareAcceleration = false;
                return;
            }

            // Lastly the process level flag is checked
            var hwndSource = PresentationSource.FromVisual(this) as HwndSource;

            if ( hwndSource != null && hwndSource.CompositionTarget.RenderMode == RenderMode.SoftwareOnly ) {
                m_hardwareAcceleration = false;
                return;
            }

            m_hardwareAcceleration = true;
        }

        // Remote desktop stuff
        private const int SM_REMOTESESSION = 0x1000;

        [DllImport("user32")]
        private static extern int GetSystemMetrics(int index);
    }
}
