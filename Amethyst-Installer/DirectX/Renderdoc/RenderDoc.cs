using amethyst_installer_gui.PInvoke;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;

namespace amethyst_installer_gui.DirectX.Renderdoc {
    public class RenderDoc {
        private readonly RENDERDOC_API_1_6_0 s_api;
        private static IntPtr handle;

        private static IntPtr LoadRenderDocLib(string[] paths) {
            IntPtr ret = IntPtr.Zero;
            foreach (string name in paths)
            {
                ret = LoadWithResolver(name);
                if (ret != IntPtr.Zero)
                {
                    break;
                }
            }

            return ret;
        }

        private static IntPtr LoadWithResolver(string name) {
            if (Path.IsPathRooted(name))
            {
                return Kernel.LoadLibrary(name);
            }
            else
            {
                // foreach (string loadTarget in pathResolver.EnumeratePossibleLibraryLoadTargets(name))
                // {
                //     if (!Path.IsPathRooted(loadTarget) || File.Exists(loadTarget))
                //     {
                //         IntPtr ret = Kernel.LoadLibrary(loadTarget);
                //         if (ret != IntPtr.Zero)
                //         {
                //             return ret;
                //         }
                //     }
                // }
	
                return IntPtr.Zero;
            }
        }

        private static T LoadFunction<T>(string name) {
            IntPtr functionPtr = Kernel.GetProcAddress(handle, name);
            
            if (functionPtr == IntPtr.Zero)
            {
                throw new InvalidOperationException($"No function was found with the name {name}.");
            }

            return Marshal.GetDelegateForFunctionPointer<T>(functionPtr);
        }
        public void LoadFunction<T>(string name, out T field)
        {
            IntPtr funcPtr = Kernel.GetProcAddress(handle, name);
	
            if (funcPtr != IntPtr.Zero)
            {
                field = Marshal.GetDelegateForFunctionPointer<T>(funcPtr);
            }
            else
            {
                field = default(T);
            }
        }
        
        private RenderDoc()
        {
            LoadFunction("RENDERDOC_GetAPI", out pRENDERDOC_GetAPI getApiFunc);
            IntPtr apiPointers = IntPtr.Zero;
            int result = getApiFunc(RENDERDOC_Version.eRENDERDOC_API_Version_1_4_1, ref apiPointers);
            if (result != 1)
            {
                throw new InvalidOperationException("Failed to load RenderDoc API.");
            }

            s_api = Marshal.PtrToStructure<RENDERDOC_API_1_6_0>(apiPointers);
        }
        
        /// <summary>
        /// Allow the application to enable vsync.
        /// Default value: true.
        /// true: The application can enable or disable vsync at will.
        /// false: vsync is force disabled.
        /// </summary>
        public bool AllowVSync
        {
            get => s_api.GetCaptureOptionU32(RENDERDOC_CaptureOption.eRENDERDOC_Option_AllowVSync) != 0;
            set => s_api.SetCaptureOptionU32(RENDERDOC_CaptureOption.eRENDERDOC_Option_AllowVSync, value ? 1u : 0u);
        }

        /// <summary>
        /// Allow the application to enable fullscreen.
        /// Default value: true.
        /// true: The application can enable or disable fullscreen at will.
        /// false: fullscreen is force disabled.
        /// </summary>
        public bool AllowFullscreen
        {
            get => s_api.GetCaptureOptionU32(RENDERDOC_CaptureOption.eRENDERDOC_Option_AllowFullscreen) != 0;
            set => s_api.SetCaptureOptionU32(RENDERDOC_CaptureOption.eRENDERDOC_Option_AllowFullscreen, value ? 1u : 0u);
        }

        /// <summary>
        /// Record API debugging events and messages.
        /// Default value: false.
        /// true: Enable built-in API debugging features and records the results into the capture, which is matched up with
        /// events on replay.
        /// false: no API debugging is forcibly enabled.
        /// </summary>
        public bool APIValidation
        {
            get => s_api.GetCaptureOptionU32(RENDERDOC_CaptureOption.eRENDERDOC_Option_APIValidation) != 0;
            set => s_api.SetCaptureOptionU32(RENDERDOC_CaptureOption.eRENDERDOC_Option_APIValidation, value ? 1u : 0u);
        }

        /// <summary>
        /// Capture CPU callstacks for API events.
        /// Default value: false.
        /// true: Enables capturing of callstacks.
        /// false: no callstacks are captured.
        /// </summary>
        public bool CaptureCallstacks
        {
            get => s_api.GetCaptureOptionU32(RENDERDOC_CaptureOption.eRENDERDOC_Option_CaptureCallstacks) != 0;
            set => s_api.SetCaptureOptionU32(RENDERDOC_CaptureOption.eRENDERDOC_Option_CaptureCallstacks, value ? 1u : 0u);
        }

        /// <summary>
        /// When capturing CPU callstacks, only capture them from drawcalls.
        /// This option does nothing without CaptureCallstacks being enabled.
        /// Default value: false.
        /// true: Only captures callstacks for drawcall type API events. Ignored if CaptureCallstacks is disabled.
        /// false: Callstacks, if enabled, are captured for every event.
        /// </summary>
        public bool CaptureCallstacksOnlyDraws
        {
            get => s_api.GetCaptureOptionU32(RENDERDOC_CaptureOption.eRENDERDOC_Option_CaptureCallstacksOnlyDraws) != 0;
            set => s_api.SetCaptureOptionU32(RENDERDOC_CaptureOption.eRENDERDOC_Option_CaptureCallstacksOnlyDraws, value ? 1u : 0u);
        }

        /// <summary>
        /// Specify a delay in seconds to wait for a debugger to attach, after creating or injecting into a process, before
        /// continuing to allow it to run.
        /// A value of 0 indicates no delay, and the process will run immediately after injection.
        /// Default value: 0 seconds.
        /// </summary>
        public uint DelayForDebugger
        {
            get => s_api.GetCaptureOptionU32(RENDERDOC_CaptureOption.eRENDERDOC_Option_DelayForDebugger);
            set => s_api.SetCaptureOptionU32(RENDERDOC_CaptureOption.eRENDERDOC_Option_DelayForDebugger, value);
        }

        /// <summary>
        /// Verify buffer access. This includes checking the memory returned by a Map() call to detect any out-of-bounds
        /// modification, as well as initialising buffers with undefined contents to a marker value to catch use of uninitialised
        /// memory.
        /// NOTE: This option is only valid for OpenGL and D3D11. Explicit APIs such as D3D12 and Vulkan do
        /// not do the same kind of interception & checking and undefined contents are really undefined.
        /// Default value: false.
        /// true: Verify buffer access.
        /// false: No verification is performed, and overwriting bounds may cause crashes or corruption in RenderDoc.
        /// </summary>
        public bool VerifyBufferAccess
        {
            get => s_api.GetCaptureOptionU32(RENDERDOC_CaptureOption.eRENDERDOC_Option_VerifyBufferAccess) != 0;
            set => s_api.SetCaptureOptionU32(RENDERDOC_CaptureOption.eRENDERDOC_Option_VerifyBufferAccess, value ? 1u : 0u);
        }

        /// <summary>
        /// Hooks any system API calls that create child processes, and injects RenderDoc into them recursively with the same
        /// options.
        /// Default value: false.
        /// true: Hooks into spawned child processes.
        /// false: Child processes are not hooked by RenderDoc.
        /// </summary>
        public bool HookIntoChildren
        {
            get => s_api.GetCaptureOptionU32(RENDERDOC_CaptureOption.eRENDERDOC_Option_HookIntoChildren) != 0;
            set => s_api.SetCaptureOptionU32(RENDERDOC_CaptureOption.eRENDERDOC_Option_HookIntoChildren, value ? 1u : 0u);
        }

        /// <summary>
        /// By default RenderDoc only includes resources in the final capture necessary for that frame, this allows you to
        /// override that behaviour.
        /// Default value: false.
        /// true: all live resources at the time of capture are included in the capture and available for inspection.
        /// false: only the resources referenced by the captured frame are included.
        /// </summary>
        public bool RefAllResources
        {
            get => s_api.GetCaptureOptionU32(RENDERDOC_CaptureOption.eRENDERDOC_Option_RefAllResources) != 0;
            set => s_api.SetCaptureOptionU32(RENDERDOC_CaptureOption.eRENDERDOC_Option_RefAllResources, value ? 1u : 0u);
        }

        /// <summary>
        /// In APIs that allow for the recording of command lists to be replayed later, RenderDoc may choose to not capture
        /// command lists before a frame capture is triggered, to reduce overheads. This means any command lists recorded once
        /// and replayed many times will not be available and may cause a failure to capture.
        /// NOTE: This is only true for APIs where multithreading is difficult or discouraged. Newer APIs like Vulkan and D3D12
        /// will ignore this option and always capture all command lists since the API is heavily oriented around it and the
        /// overheads have been reduced by API design.
        /// true: All command lists are captured from the start of the application.
        /// false: Command lists are only captured if their recording begins during the period when a frame capture is in
        /// progress.
        /// </summary>
        public bool CaptureAllCmdLists
        {
            get => s_api.GetCaptureOptionU32(RENDERDOC_CaptureOption.eRENDERDOC_Option_CaptureAllCmdLists) != 0;
            set => s_api.SetCaptureOptionU32(RENDERDOC_CaptureOption.eRENDERDOC_Option_CaptureAllCmdLists, value ? 1u : 0u);
        }

        /// <summary>
        /// Mute API debugging output when the API validation mode option is enabled
        /// Default value: true.
        /// true: Mute any API debug messages from being displayed or passed through.
        /// false: API debugging is displayed as normal.
        /// </summary>
        public bool DebugOutputMute
        {
            get => s_api.GetCaptureOptionU32(RENDERDOC_CaptureOption.eRENDERDOC_Option_DebugOutputMute) != 0;
            set => s_api.SetCaptureOptionU32(RENDERDOC_CaptureOption.eRENDERDOC_Option_DebugOutputMute, value ? 1u : 0u);
        }

        /// <summary>
        /// Capture the next frame on whichever window and API is currently considered active.
        /// </summary>
        public void TriggerCapture() => s_api.TriggerCapture();

        /// <summary>
        /// Capture the next N frames on whichever window and API is currently considered active.
        /// </summary>
        /// <param name="numFrames">The number of frames to capture.</param>
        public void TriggerCapture(uint numFrames) => s_api.TriggerMultiFrameCapture(numFrames);

        /// <summary>
        /// Immediately starts capturing API calls on the active device and window.
        /// If there is no matching thing to capture (e.g. no supported API has been initialised), this will do nothing.
        /// The results are undefined (including crashes) if two captures are started overlapping, even on separate devices
        /// and/or windows.
        /// </summary>
        public void StartFrameCapture() => s_api.StartFrameCapture(IntPtr.Zero, IntPtr.Zero);

        /// <summary>
        /// Immediately starts capturing API calls on the active device and window.
        /// If there is no matching thing to capture (e.g. no supported API has been initialised), this will do nothing.
        /// The results are undefined (including crashes) if two captures are started overlapping, even on separate devices
        /// and/or windows.
        /// </summary>
        /// <param name="device"> is a handle to the API ‘device’ object that will be set active. May be null for wildcard match.</param>
        /// <param name="wndHandle"> is a handle to the platform window handle that will be set active. May be null for wildcard match.</param>
        public void StartFrameCapture(IntPtr device, IntPtr wndHandle) => s_api.StartFrameCapture(device, wndHandle);


        /// <summary>
        /// Returns whether or not a frame capture is currently ongoing anywhere.
        /// </summary>
        /// <returns>True if a capture is ongoing, false if there is no capture running.</returns>
        public bool IsFrameCapturing() => s_api.IsFrameCapturing() != 0;

        /// <summary>
        /// Ends capturing immediately.
        /// </summary>
        /// <returns>True if the capture succeeded, false if there was an error capturing.</returns>
        public bool EndFrameCapture() => s_api.EndFrameCapture(IntPtr.Zero, IntPtr.Zero) != 0;

        /// <summary>
        /// Ends capturing immediately.
        /// </summary>
        /// <param name="device"> is a handle to the API ‘device’ object that will be set active. May be null for wildcard match.</param>
        /// <param name="wndHandle"> is a handle to the platform window handle that will be set active. May be null for wildcard match.</param>
        /// <returns>True if the capture succeeded, false if there was an error capturing.</returns>
        public bool EndFrameCapture(IntPtr device, IntPtr wndHandle) => s_api.EndFrameCapture(device, wndHandle) != 0;

        /// <summary>
        /// This function will launch the Replay UI associated with the RenderDoc library injected into the running application.
        /// </summary>
        /// <returns>The PID of the replay UI if successful, 0 if not successful.</returns>
        public uint LaunchReplayUI() => s_api.LaunchReplayUI(1, null);

        /// <summary>
        /// This function will explicitly set which window is considered active.
        ///  The active window is the one that will be captured when the keybind to trigger a capture is pressed.
        /// </summary>
        /// <param name="device"> is a handle to the API ‘device’ object that will be set active. Must be valid.</param>
        /// <param name="wndHandle"> is a handle to the platform window handle that will be set active. Must be valid.</param>
        public void SetActiveWindow(IntPtr device, IntPtr wndHandle) => s_api.SetActiveWindow(device, wndHandle);

        /// <summary>
        /// This function will launch the Replay UI associated with the RenderDoc library injected into the running application.
        /// </summary>
        /// <param name="args">The rest of the command line, e.g. a capture file to open</param>
        /// <returns>The PID of the replay UI if successful, 0 if not successful.</returns>
        public uint LaunchReplayUI(string args)
        {
            return s_api.LaunchReplayUI(1, args);
        }

        /// <summary>
        /// Gets the number of captures that have been made.
        /// </summary>
        public uint CaptureCount => s_api.GetNumCaptures();

        /// <summary>
        /// Sets the path into which capture files will be saved.
        /// </summary>
        /// <param name="path">The path to save capture files under.</param>
        public void SetCaptureSavePath(string path)
        {
            s_api.SetCaptureFilePathTemplate(path);
        }

        /// <summary>
        /// Gets a value indicating whether the RenderDoc UI is connected to this application.
        /// </summary>
        /// <returns>true if the RenderDoc UI is connected to this application, false otherwise.</returns>
        public bool IsTargetControlConnected() => s_api.IsTargetControlConnected() == 1;

        /// <summary>
        /// Controls whether the overlay is enabled or disabled globally.
        /// </summary>
        public bool OverlayEnabled
        {
            get => (s_api.GetOverlayBits() & (uint)RENDERDOC_OverlayBits.eRENDERDOC_Overlay_Enabled) != 0;
            set
            {
                uint bit = (uint)RENDERDOC_OverlayBits.eRENDERDOC_Overlay_Enabled;
                if (value) { s_api.MaskOverlayBits(~0u, bit); }
                else { s_api.MaskOverlayBits(~bit, 0); }
            }
        }

        /// <summary>
        /// Controls whether the overlay displays the average framerate over several seconds as well as min/max.
        /// </summary>
        public bool OverlayFrameRate
        {
            get => (s_api.GetOverlayBits() & (uint)RENDERDOC_OverlayBits.eRENDERDOC_Overlay_FrameRate) != 0;
            set
            {
                uint bit = (uint)RENDERDOC_OverlayBits.eRENDERDOC_Overlay_FrameRate;
                if (value) { s_api.MaskOverlayBits(~0u, bit); }
                else { s_api.MaskOverlayBits(~bit, 0); }
            }
        }

        /// <summary>
        /// Controls whether the overlay displays the current frame number.
        /// </summary>
        public bool OverlayFrameNumber
        {
            get => (s_api.GetOverlayBits() & (uint)RENDERDOC_OverlayBits.eRENDERDOC_Overlay_FrameNumber) != 0;
            set
            {
                uint bit = (uint)RENDERDOC_OverlayBits.eRENDERDOC_Overlay_FrameNumber;
                if (value) { s_api.MaskOverlayBits(~0u, bit); }
                else { s_api.MaskOverlayBits(~bit, 0); }
            }
        }

        /// <summary>
        /// Controls whether the overlay displays a list of recent captures, and how many captures have been made.
        /// </summary>
        public bool OverlayCaptureList
        {
            get => (s_api.GetOverlayBits() & (uint)RENDERDOC_OverlayBits.eRENDERDOC_Overlay_CaptureList) != 0;
            set
            {
                uint bit = (uint)RENDERDOC_OverlayBits.eRENDERDOC_Overlay_CaptureList;
                if (value) { s_api.MaskOverlayBits(~0u, bit); }
                else { s_api.MaskOverlayBits(~bit, 0); }
            }
        }

        /// <summary>
        /// Attempts to load RenderDoc using system-default names and paths.
        /// </summary>
        /// <param name="renderDoc">If successful, this parameter contains a loaded <see cref="RenderDoc"/> instance.</param>
        /// <returns>Whether or not RenderDoc was successfully loaded.</returns>
        public static bool Load(out RenderDoc renderDoc) => Load(GetLibNames(), out renderDoc);

        /// Attempts to load RenderDoc from the given path.
        /// </summary>
        /// <param name="renderDocLibPath">The path to the RenderDoc shared library.</param>
        /// <param name="renderDoc">If successful, this parameter contains a loaded <see cref="RenderDoc"/> instance.</param>
        /// <returns>Whether or not RenderDoc was successfully loaded.</returns>
        public static bool Load(string renderDocLibPath, out RenderDoc renderDoc) => Load(new[] { renderDocLibPath }, out renderDoc);

        private static bool Load(string[] renderDocLibPaths, out RenderDoc renderDoc)
        {
            try
            {
                handle = LoadRenderDocLib(renderDocLibPaths);
                renderDoc = new RenderDoc();
                return true;
            }
            catch
            {
                renderDoc = null;
                return false;
            }
        }

        private static string[] GetLibNames()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                List<string> paths = new List<string>();
                string programFiles = Environment.GetEnvironmentVariable("ProgramFiles");
                if (programFiles != null)
                {
                    string systemInstallPath = Path.Combine(programFiles, "RenderDoc", "renderdoc.dll");
                    if (File.Exists(systemInstallPath))
                    {
                        paths.Add(systemInstallPath);
                    }
                }
                paths.Add("renderdoc.dll");

                return paths.ToArray();
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                return new[] { "librenderdoc.dylib" };
            }
            else
            {
                return new[] { "librenderdoc.so" };
            }
        }
    }
}