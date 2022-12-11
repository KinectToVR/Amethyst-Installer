using System;
using System.Runtime.InteropServices;

namespace amethyst_installer_gui.DirectX.Renderdoc {
    
    /// <summary>
	/// Sets an option that controls how RenderDoc behaves on capture.
	/// Returns 1 if the option and value are valid
	/// Returns 0 if either is invalid and the option is unchanged
	/// </summary>
	public delegate int pRENDERDOC_SetCaptureOptionU32(RENDERDOC_CaptureOption opt, uint val);

	public delegate int pRENDERDOC_SetCaptureOptionF32(RENDERDOC_CaptureOption opt, float val);

	/// <summary>
	/// Gets the current value of an option as a uint32_t
	/// If the option is invalid, 0xffffffff is returned
	/// </summary>
	public delegate uint pRENDERDOC_GetCaptureOptionU32(RENDERDOC_CaptureOption opt);

	/// <summary>
	/// Gets the current value of an option as a float
	/// If the option is invalid, -FLT_MAX is returned
	/// </summary>
	public delegate float pRENDERDOC_GetCaptureOptionF32(RENDERDOC_CaptureOption opt);

	/// <summary>
	/// Sets which key or keys can be used to toggle focus between multiple windows
	/// If keys is NULL or num is 0, toggle keys will be disabled
	/// </summary>
	public delegate void pRENDERDOC_SetFocusToggleKeys(ref RENDERDOC_InputButton keys, int num);

	/// <summary>
	/// Sets which key or keys can be used to capture the next frame
	/// If keys is NULL or num is 0, captures keys will be disabled
	/// </summary>
	public delegate void pRENDERDOC_SetCaptureKeys(ref RENDERDOC_InputButton keys, int num);

	/// <summary>
	/// returns the overlay bits that have been set
	/// </summary>
	public delegate uint pRENDERDOC_GetOverlayBits();

	/// <summary>
	/// sets the overlay bits with an and 
	/// &
	/// or mask
	/// </summary>
	public delegate void pRENDERDOC_MaskOverlayBits(uint And, uint Or);

	/// <summary>
	/// this function will attempt to remove RenderDoc's hooks in the application.
	/// Note: that this can only work correctly if done immediately after
	/// the module is loaded, before any API work happens. RenderDoc will remove its
	/// injected hooks and shut down. Behaviour is undefined if this is called
	/// after any API functions have been called, and there is still no guarantee of
	/// success.
	/// </summary>
	public delegate void pRENDERDOC_RemoveHooks();

	/// <summary>
	/// This function will unload RenderDoc's crash handler.
	/// If you use your own crash handler and don't want RenderDoc's handler to
	/// intercede, you can call this function to unload it and any unhandled
	/// exceptions will pass to the next handler.
	/// </summary>
	public delegate void pRENDERDOC_UnloadCrashHandler();

	/// <summary>
	/// Sets the capture file path template
	/// pathtemplate is a UTF-8 string that gives a template for how captures will be named
	/// and where they will be saved.
	/// Any extension is stripped off the path, and captures are saved in the directory
	/// specified, and named with the filename and the frame number appended. If the
	/// directory does not exist it will be created, including any parent directories.
	/// If pathtemplate is NULL, the template will remain unchanged
	/// Example:
	/// SetCaptureFilePathTemplate("my_captures/example");
	/// Capture #1 -> my_captures/example_frame123.rdc
	/// Capture #2 -> my_captures/example_frame456.rdc
	/// </summary>
	public delegate void pRENDERDOC_SetCaptureFilePathTemplate([MarshalAs(UnmanagedType.LPStr)] string pathtemplate);

	/// <summary>
	/// returns the current capture path template, see SetCaptureFileTemplate above, as a UTF-8 string
	/// </summary>
	public delegate string pRENDERDOC_GetCaptureFilePathTemplate();

	/// <summary>
	/// returns the number of captures that have been made
	/// </summary>
	public delegate uint pRENDERDOC_GetNumCaptures();

	/// <summary>
	/// This function returns the details of a capture, by index. New captures are added
	/// to the end of the list.
	/// filename will be filled with the absolute path to the capture file, as a UTF-8 string
	/// pathlength will be written with the length in bytes of the filename string
	/// timestamp will be written with the time of the capture, in seconds since the Unix epoch
	/// Any of the parameters can be NULL and they'll be skipped.
	/// The function will return 1 if the capture index is valid, or 0 if the index is invalid
	/// If the index is invalid, the values will be unchanged
	/// Note: when captures are deleted in the UI they will remain in this list, so the
	/// capture path may not exist anymore.
	/// </summary>
	public delegate uint pRENDERDOC_GetCapture(uint idx, [MarshalAs(UnmanagedType.LPStr)] string filename, out uint pathlength, out ulong timestamp);

	/// <summary>
	/// Sets the comments associated with a capture file. These comments are displayed in the
	/// UI program when opening.
	/// filePath should be a path to the capture file to add comments to. If set to NULL or ""
	/// the most recent capture file created made will be used instead.
	/// comments should be a NULL-terminated UTF-8 string to add as comments.
	/// Any existing comments will be overwritten.
	/// </summary>
	public delegate void pRENDERDOC_SetCaptureFileComments([MarshalAs(UnmanagedType.LPStr)] string filePath, [MarshalAs(UnmanagedType.LPStr)] string comments);

	/// <summary>
	/// returns 1 if the RenderDoc UI is connected to this application, 0 otherwise
	/// </summary>
	public delegate uint pRENDERDOC_IsTargetControlConnected();

	/// <summary>
	/// This function will launch the Replay UI associated with the RenderDoc library injected
	/// into the running application.
	/// if connectTargetControl is 1, the Replay UI will be launched with a command line parameter
	/// to connect to this application
	/// cmdline is the rest of the command line, as a UTF-8 string. E.g. a captures to open
	/// if cmdline is NULL, the command line will be empty.
	/// returns the PID of the replay UI if successful, 0 if not successful.
	/// </summary>
	public delegate uint pRENDERDOC_LaunchReplayUI(uint connectTargetControl, [MarshalAs(UnmanagedType.LPStr)] string cmdline);

	/// <summary>
	/// RenderDoc can return a higher version than requested if it's backwards compatible,
	/// this function returns the actual version returned. If a parameter is NULL, it will be
	/// ignored and the others will be filled out.
	/// </summary>
	public delegate void pRENDERDOC_GetAPIVersion(out int major, out int minor, out int patch);

	/// <summary>
	/// Requests that the replay UI show itself (if hidden or not the current top window). This can be
	/// used in conjunction with IsTargetControlConnected and LaunchReplayUI to intelligently handle
	/// showing the UI after making a capture.
	/// This will return 1 if the request was successfully passed on, though it's not guaranteed that
	/// the UI will be on top in all cases depending on OS rules. It will return 0 if there is no current
	/// target control connection to make such a request, or if there was another error
	/// </summary>
	public delegate uint pRENDERDOC_ShowReplayUI();

	/// <summary>
	/// This sets the RenderDoc in-app overlay in the API/window pair as 'active' and it will
	/// respond to keypresses. Neither parameter can be NULL
	/// </summary>
	public delegate void pRENDERDOC_SetActiveWindow(IntPtr device, IntPtr wndHandle);

	/// <summary>
	/// capture the next frame on whichever window and API is currently considered active
	/// </summary>
	public delegate void pRENDERDOC_TriggerCapture();

	/// <summary>
	/// capture the next N frames on whichever window and API is currently considered active
	/// </summary>
	public delegate void pRENDERDOC_TriggerMultiFrameCapture(uint numFrames);

	/// <summary>
	/// Immediately starts capturing API calls on the specified device pointer and window handle.
	/// If there is no matching thing to capture (e.g. no supported API has been initialised),
	/// this will do nothing.
	/// The results are undefined (including crashes) if two captures are started overlapping,
	/// even on separate devices and/oror windows.
	/// </summary>
	public delegate void pRENDERDOC_StartFrameCapture(IntPtr device, IntPtr wndHandle);

	/// <summary>
	/// Returns whether or not a frame capture is currently ongoing anywhere.
	/// This will return 1 if a capture is ongoing, and 0 if there is no capture running
	/// </summary>
	public delegate uint pRENDERDOC_IsFrameCapturing();

	/// <summary>
	/// Ends capturing immediately.
	/// This will return 1 if the capture succeeded, and 0 if there was an error capturing.
	/// </summary>
	public delegate uint pRENDERDOC_EndFrameCapture(IntPtr device, IntPtr wndHandle);

	/// <summary>
	/// Ends capturing immediately and discard any data stored without saving to disk.
	/// This will return 1 if the capture was discarded, and 0 if there was an error or no capture
	/// was in progress
	/// </summary>
	public delegate uint pRENDERDOC_DiscardFrameCapture(IntPtr device, IntPtr wndHandle);

	/// <summary>
	/// Only valid to be called between a call to StartFrameCapture and EndFrameCapture. Gives a custom
	/// title to the capture produced which will be displayed in the UI.
	/// If multiple captures are ongoing, this title will be applied to the first capture to end after
	/// this call. The second capture to end will have no title, unless this function is called again.
	/// Calling this function has no effect if no capture is currently running, and if it is called
	/// multiple times only the last title will be used.
	/// </summary>
	public delegate void pRENDERDOC_SetCaptureTitle([MarshalAs(UnmanagedType.LPStr)] string title);

	/// <summary>
	/// RenderDoc API entry point
	/// This entry point can be obtained via GetProcAddress/dlsym if RenderDoc is available.
	/// The name is the same as the typedef - "RENDERDOC_GetAPI"
	/// This function is not thread safe, and should not be called on multiple threads at once.
	/// Ideally, call this once as early as possible in your application's startup, before doing
	/// any API work, since some configuration functionality etc has to be done also before
	/// initialising any APIs.
	/// Parameters:
	/// version is a single value from the RENDERDOC_Version above.
	/// outAPIPointers will be filled out with a pointer to the corresponding struct of function
	/// pointers.
	/// Returns:
	/// 1 - if the outAPIPointers has been filled with a pointer to the API struct requested
	/// 0 - if the requested version is not supported or the arguments are invalid.
	/// </summary>
	public delegate int pRENDERDOC_GetAPI(RENDERDOC_Version version, ref IntPtr outAPIPointers);
    
    /// <summary>
	/// RenderDoc capture options
	/// </summary>
	public enum RENDERDOC_CaptureOption
	{

		/// <summary>
		/// Allow the application to enable vsync
		/// Default - enabled
		/// 1 - The application can enable or disable vsync at will
		/// 0 - vsync is force disabled
		/// </summary>
		eRENDERDOC_Option_AllowVSync = 0,

		/// <summary>
		/// Allow the application to enable fullscreen
		/// Default - enabled
		/// 1 - The application can enable or disable fullscreen at will
		/// 0 - fullscreen is force disabled
		/// </summary>
		eRENDERDOC_Option_AllowFullscreen = 1,

		/// <summary>
		/// Record API debugging events and messages
		/// Default - disabled
		/// 1 - Enable built-in API debugging features and records the results into
		/// the capture, which is matched up with events on replay
		/// 0 - no API debugging is forcibly enabled
		/// </summary>
		eRENDERDOC_Option_APIValidation = 2,

		/// <summary>
		/// deprecated name of this enum
		/// </summary>
		eRENDERDOC_Option_DebugDeviceMode = 2,

		/// <summary>
		/// Capture CPU callstacks for API events
		/// Default - disabled
		/// 1 - Enables capturing of callstacks
		/// 0 - no callstacks are captured
		/// </summary>
		eRENDERDOC_Option_CaptureCallstacks = 3,

		/// <summary>
		/// When capturing CPU callstacks, only capture them from actions.
		/// This option does nothing without the above option being enabled
		/// Default - disabled
		/// 1 - Only captures callstacks for actions.
		/// Ignored if CaptureCallstacks is disabled
		/// 0 - Callstacks, if enabled, are captured for every event.
		/// </summary>
		eRENDERDOC_Option_CaptureCallstacksOnlyDraws = 4,

		/// <summary>
		/// When capturing CPU callstacks, only capture them from actions.
		/// This option does nothing without the above option being enabled
		/// Default - disabled
		/// 1 - Only captures callstacks for actions.
		/// Ignored if CaptureCallstacks is disabled
		/// 0 - Callstacks, if enabled, are captured for every event.
		/// </summary>
		eRENDERDOC_Option_CaptureCallstacksOnlyActions = 4,

		/// <summary>
		/// Specify a delay in seconds to wait for a debugger to attach, after
		/// creating or injecting into a process, before continuing to allow it to run.
		/// 0 indicates no delay, and the process will run immediately after injection
		/// Default - 0 seconds
		/// </summary>
		eRENDERDOC_Option_DelayForDebugger = 5,

		/// <summary>
		/// Verify buffer access. This includes checking the memory returned by a Map() call to
		/// detect any out-of-bounds modification, as well as initialising buffers with undefined contents
		/// to a marker value to catch use of uninitialised memory.
		/// NOTE: This option is only valid for OpenGL and D3D11. Explicit APIs such as D3D12 and Vulkan do
		/// not do the same kind of interception 
		/// &
		/// checking and undefined contents are really undefined.
		/// Default - disabled
		/// 1 - Verify buffer access
		/// 0 - No verification is performed, and overwriting bounds may cause crashes or corruption in
		/// RenderDoc.
		/// </summary>
		eRENDERDOC_Option_VerifyBufferAccess = 6,

		/// <summary>
		/// The old name for eRENDERDOC_Option_VerifyBufferAccess was eRENDERDOC_Option_VerifyMapWrites.
		/// This option now controls the filling of uninitialised buffers with 0xdddddddd which was
		/// previously always enabled
		/// </summary>
		eRENDERDOC_Option_VerifyMapWrites = 6,

		/// <summary>
		/// Hooks any system API calls that create child processes, and injects
		/// RenderDoc into them recursively with the same options.
		/// Default - disabled
		/// 1 - Hooks into spawned child processes
		/// 0 - Child processes are not hooked by RenderDoc
		/// </summary>
		eRENDERDOC_Option_HookIntoChildren = 7,

		/// <summary>
		/// By default RenderDoc only includes resources in the final capture necessary
		/// for that frame, this allows you to override that behaviour.
		/// Default - disabled
		/// 1 - all live resources at the time of capture are included in the capture
		/// and available for inspection
		/// 0 - only the resources referenced by the captured frame are included
		/// </summary>
		eRENDERDOC_Option_RefAllResources = 8,

		/// <summary>
		/// **NOTE**: As of RenderDoc v1.1 this option has been deprecated. Setting or
		/// getting it will be ignored, to allow compatibility with older versions.
		/// In v1.1 the option acts as if it's always enabled.
		/// By default RenderDoc skips saving initial states for resources where the
		/// previous contents don't appear to be used, assuming that writes before
		/// reads indicate previous contents aren't used.
		/// Default - disabled
		/// 1 - initial contents at the start of each captured frame are saved, even if
		/// they are later overwritten or cleared before being used.
		/// 0 - unless a read is detected, initial contents will not be saved and will
		/// appear as black or empty data.
		/// </summary>
		eRENDERDOC_Option_SaveAllInitials = 9,

		/// <summary>
		/// In APIs that allow for the recording of command lists to be replayed later,
		/// RenderDoc may choose to not capture command lists before a frame capture is
		/// triggered, to reduce overheads. This means any command lists recorded once
		/// and replayed many times will not be available and may cause a failure to
		/// capture.
		/// NOTE: This is only true for APIs where multithreading is difficult or
		/// discouraged. Newer APIs like Vulkan and D3D12 will ignore this option
		/// and always capture all command lists since the API is heavily oriented
		/// around it and the overheads have been reduced by API design.
		/// 1 - All command lists are captured from the start of the application
		/// 0 - Command lists are only captured if their recording begins during
		/// the period when a frame capture is in progress.
		/// </summary>
		eRENDERDOC_Option_CaptureAllCmdLists = 10,

		/// <summary>
		/// Mute API debugging output when the API validation mode option is enabled
		/// Default - enabled
		/// 1 - Mute any API debug messages from being displayed or passed through
		/// 0 - API debugging is displayed as normal
		/// </summary>
		eRENDERDOC_Option_DebugOutputMute = 11,

		/// <summary>
		/// Option to allow vendor extensions to be used even when they may be
		/// incompatible with RenderDoc and cause corrupted replays or crashes.
		/// Default - inactive
		/// No values are documented, this option should only be used when absolutely
		/// necessary as directed by a RenderDoc developer.
		/// </summary>
		eRENDERDOC_Option_AllowUnsupportedVendorExtensions = 12,
	}

	public enum RENDERDOC_InputButton
	{

		/// <summary>
		/// '0' - '9' matches ASCII values
		/// </summary>
		eRENDERDOC_Key_0 = 48,

		/// <summary>
		/// '0' - '9' matches ASCII values
		/// </summary>
		eRENDERDOC_Key_1 = 49,

		/// <summary>
		/// '0' - '9' matches ASCII values
		/// </summary>
		eRENDERDOC_Key_2 = 50,

		/// <summary>
		/// '0' - '9' matches ASCII values
		/// </summary>
		eRENDERDOC_Key_3 = 51,

		/// <summary>
		/// '0' - '9' matches ASCII values
		/// </summary>
		eRENDERDOC_Key_4 = 52,

		/// <summary>
		/// '0' - '9' matches ASCII values
		/// </summary>
		eRENDERDOC_Key_5 = 53,

		/// <summary>
		/// '0' - '9' matches ASCII values
		/// </summary>
		eRENDERDOC_Key_6 = 54,

		/// <summary>
		/// '0' - '9' matches ASCII values
		/// </summary>
		eRENDERDOC_Key_7 = 55,

		/// <summary>
		/// '0' - '9' matches ASCII values
		/// </summary>
		eRENDERDOC_Key_8 = 56,

		/// <summary>
		/// '0' - '9' matches ASCII values
		/// </summary>
		eRENDERDOC_Key_9 = 57,

		/// <summary>
		/// 'A' - 'Z' matches ASCII values
		/// </summary>
		eRENDERDOC_Key_A = 65,

		/// <summary>
		/// 'A' - 'Z' matches ASCII values
		/// </summary>
		eRENDERDOC_Key_B = 66,

		/// <summary>
		/// 'A' - 'Z' matches ASCII values
		/// </summary>
		eRENDERDOC_Key_C = 67,

		/// <summary>
		/// 'A' - 'Z' matches ASCII values
		/// </summary>
		eRENDERDOC_Key_D = 68,

		/// <summary>
		/// 'A' - 'Z' matches ASCII values
		/// </summary>
		eRENDERDOC_Key_E = 69,

		/// <summary>
		/// 'A' - 'Z' matches ASCII values
		/// </summary>
		eRENDERDOC_Key_F = 70,

		/// <summary>
		/// 'A' - 'Z' matches ASCII values
		/// </summary>
		eRENDERDOC_Key_G = 71,

		/// <summary>
		/// 'A' - 'Z' matches ASCII values
		/// </summary>
		eRENDERDOC_Key_H = 72,

		/// <summary>
		/// 'A' - 'Z' matches ASCII values
		/// </summary>
		eRENDERDOC_Key_I = 73,

		/// <summary>
		/// 'A' - 'Z' matches ASCII values
		/// </summary>
		eRENDERDOC_Key_J = 74,

		/// <summary>
		/// 'A' - 'Z' matches ASCII values
		/// </summary>
		eRENDERDOC_Key_K = 75,

		/// <summary>
		/// 'A' - 'Z' matches ASCII values
		/// </summary>
		eRENDERDOC_Key_L = 76,

		/// <summary>
		/// 'A' - 'Z' matches ASCII values
		/// </summary>
		eRENDERDOC_Key_M = 77,

		/// <summary>
		/// 'A' - 'Z' matches ASCII values
		/// </summary>
		eRENDERDOC_Key_N = 78,

		/// <summary>
		/// 'A' - 'Z' matches ASCII values
		/// </summary>
		eRENDERDOC_Key_O = 79,

		/// <summary>
		/// 'A' - 'Z' matches ASCII values
		/// </summary>
		eRENDERDOC_Key_P = 80,

		/// <summary>
		/// 'A' - 'Z' matches ASCII values
		/// </summary>
		eRENDERDOC_Key_Q = 81,

		/// <summary>
		/// 'A' - 'Z' matches ASCII values
		/// </summary>
		eRENDERDOC_Key_R = 82,

		/// <summary>
		/// 'A' - 'Z' matches ASCII values
		/// </summary>
		eRENDERDOC_Key_S = 83,

		/// <summary>
		/// 'A' - 'Z' matches ASCII values
		/// </summary>
		eRENDERDOC_Key_T = 84,

		/// <summary>
		/// 'A' - 'Z' matches ASCII values
		/// </summary>
		eRENDERDOC_Key_U = 85,

		/// <summary>
		/// 'A' - 'Z' matches ASCII values
		/// </summary>
		eRENDERDOC_Key_V = 86,

		/// <summary>
		/// 'A' - 'Z' matches ASCII values
		/// </summary>
		eRENDERDOC_Key_W = 87,

		/// <summary>
		/// 'A' - 'Z' matches ASCII values
		/// </summary>
		eRENDERDOC_Key_X = 88,

		/// <summary>
		/// 'A' - 'Z' matches ASCII values
		/// </summary>
		eRENDERDOC_Key_Y = 89,

		/// <summary>
		/// 'A' - 'Z' matches ASCII values
		/// </summary>
		eRENDERDOC_Key_Z = 90,

		/// <summary>
		/// leave the rest of the ASCII range free
		/// in case we want to use it later
		/// </summary>
		eRENDERDOC_Key_NonPrintable = 256,

		/// <summary>
		/// leave the rest of the ASCII range free
		/// in case we want to use it later
		/// </summary>
		eRENDERDOC_Key_Divide = 257,

		/// <summary>
		/// leave the rest of the ASCII range free
		/// in case we want to use it later
		/// </summary>
		eRENDERDOC_Key_Multiply = 258,

		/// <summary>
		/// leave the rest of the ASCII range free
		/// in case we want to use it later
		/// </summary>
		eRENDERDOC_Key_Subtract = 259,

		/// <summary>
		/// leave the rest of the ASCII range free
		/// in case we want to use it later
		/// </summary>
		eRENDERDOC_Key_Plus = 260,

		/// <summary>
		/// leave the rest of the ASCII range free
		/// in case we want to use it later
		/// </summary>
		eRENDERDOC_Key_F1 = 261,

		/// <summary>
		/// leave the rest of the ASCII range free
		/// in case we want to use it later
		/// </summary>
		eRENDERDOC_Key_F2 = 262,

		/// <summary>
		/// leave the rest of the ASCII range free
		/// in case we want to use it later
		/// </summary>
		eRENDERDOC_Key_F3 = 263,

		/// <summary>
		/// leave the rest of the ASCII range free
		/// in case we want to use it later
		/// </summary>
		eRENDERDOC_Key_F4 = 264,

		/// <summary>
		/// leave the rest of the ASCII range free
		/// in case we want to use it later
		/// </summary>
		eRENDERDOC_Key_F5 = 265,

		/// <summary>
		/// leave the rest of the ASCII range free
		/// in case we want to use it later
		/// </summary>
		eRENDERDOC_Key_F6 = 266,

		/// <summary>
		/// leave the rest of the ASCII range free
		/// in case we want to use it later
		/// </summary>
		eRENDERDOC_Key_F7 = 267,

		/// <summary>
		/// leave the rest of the ASCII range free
		/// in case we want to use it later
		/// </summary>
		eRENDERDOC_Key_F8 = 268,

		/// <summary>
		/// leave the rest of the ASCII range free
		/// in case we want to use it later
		/// </summary>
		eRENDERDOC_Key_F9 = 269,

		/// <summary>
		/// leave the rest of the ASCII range free
		/// in case we want to use it later
		/// </summary>
		eRENDERDOC_Key_F10 = 270,

		/// <summary>
		/// leave the rest of the ASCII range free
		/// in case we want to use it later
		/// </summary>
		eRENDERDOC_Key_F11 = 271,

		/// <summary>
		/// leave the rest of the ASCII range free
		/// in case we want to use it later
		/// </summary>
		eRENDERDOC_Key_F12 = 272,

		/// <summary>
		/// leave the rest of the ASCII range free
		/// in case we want to use it later
		/// </summary>
		eRENDERDOC_Key_Home = 273,

		/// <summary>
		/// leave the rest of the ASCII range free
		/// in case we want to use it later
		/// </summary>
		eRENDERDOC_Key_End = 274,

		/// <summary>
		/// leave the rest of the ASCII range free
		/// in case we want to use it later
		/// </summary>
		eRENDERDOC_Key_Insert = 275,

		/// <summary>
		/// leave the rest of the ASCII range free
		/// in case we want to use it later
		/// </summary>
		eRENDERDOC_Key_Delete = 276,

		/// <summary>
		/// leave the rest of the ASCII range free
		/// in case we want to use it later
		/// </summary>
		eRENDERDOC_Key_PageUp = 277,

		/// <summary>
		/// leave the rest of the ASCII range free
		/// in case we want to use it later
		/// </summary>
		eRENDERDOC_Key_PageDn = 278,

		/// <summary>
		/// leave the rest of the ASCII range free
		/// in case we want to use it later
		/// </summary>
		eRENDERDOC_Key_Backspace = 279,

		/// <summary>
		/// leave the rest of the ASCII range free
		/// in case we want to use it later
		/// </summary>
		eRENDERDOC_Key_Tab = 280,

		/// <summary>
		/// leave the rest of the ASCII range free
		/// in case we want to use it later
		/// </summary>
		eRENDERDOC_Key_PrtScrn = 281,

		/// <summary>
		/// leave the rest of the ASCII range free
		/// in case we want to use it later
		/// </summary>
		eRENDERDOC_Key_Pause = 282,

		/// <summary>
		/// leave the rest of the ASCII range free
		/// in case we want to use it later
		/// </summary>
		eRENDERDOC_Key_Max = 283,
	}

	public enum RENDERDOC_OverlayBits
	{

		/// <summary>
		/// This single bit controls whether the overlay is enabled or disabled globally
		/// </summary>
		eRENDERDOC_Overlay_Enabled = 1,

		/// <summary>
		/// Show the average framerate over several seconds as well as min/max
		/// </summary>
		eRENDERDOC_Overlay_FrameRate = 2,

		/// <summary>
		/// Show the current frame number
		/// </summary>
		eRENDERDOC_Overlay_FrameNumber = 4,

		/// <summary>
		/// Show a list of recent captures, and how many captures have been made
		/// </summary>
		eRENDERDOC_Overlay_CaptureList = 8,

		/// <summary>
		/// Default values for the overlay mask
		/// </summary>
		eRENDERDOC_Overlay_Default = 15,

		/// <summary>
		/// Enable all bits
		/// </summary>
		eRENDERDOC_Overlay_All = -1,

		/// <summary>
		/// Disable all bits
		/// </summary>
		eRENDERDOC_Overlay_None = 0,
	}

	/// <summary>
	/// RenderDoc uses semantic versioning (http://semver.org/).
	/// MAJOR version is incremented when incompatible API changes happen.
	/// MINOR version is incremented when functionality is added in a backwards-compatible manner.
	/// PATCH version is incremented when backwards-compatible bug fixes happen.
	/// Note that this means the API returned can be higher than the one you might have requested.
	/// e.g. if you are running against a newer RenderDoc that supports 1.0.1, it will be returned
	/// instead of 1.0.0. You can check this with the GetAPIVersion entry point
	/// </summary>
	public enum RENDERDOC_Version
	{

		/// <summary>
		/// RENDERDOC_API_1_0_0 = 1 00 00
		/// </summary>
		eRENDERDOC_API_Version_1_0_0 = 10000,

		/// <summary>
		/// RENDERDOC_API_1_0_1 = 1 00 01
		/// </summary>
		eRENDERDOC_API_Version_1_0_1 = 10001,

		/// <summary>
		/// RENDERDOC_API_1_0_2 = 1 00 02
		/// </summary>
		eRENDERDOC_API_Version_1_0_2 = 10002,

		/// <summary>
		/// RENDERDOC_API_1_1_0 = 1 01 00
		/// </summary>
		eRENDERDOC_API_Version_1_1_0 = 10100,

		/// <summary>
		/// RENDERDOC_API_1_1_1 = 1 01 01
		/// </summary>
		eRENDERDOC_API_Version_1_1_1 = 10101,

		/// <summary>
		/// RENDERDOC_API_1_1_2 = 1 01 02
		/// </summary>
		eRENDERDOC_API_Version_1_1_2 = 10102,

		/// <summary>
		/// RENDERDOC_API_1_2_0 = 1 02 00
		/// </summary>
		eRENDERDOC_API_Version_1_2_0 = 10200,

		/// <summary>
		/// RENDERDOC_API_1_3_0 = 1 03 00
		/// </summary>
		eRENDERDOC_API_Version_1_3_0 = 10300,

		/// <summary>
		/// RENDERDOC_API_1_4_0 = 1 04 00
		/// </summary>
		eRENDERDOC_API_Version_1_4_0 = 10400,

		/// <summary>
		/// RENDERDOC_API_1_4_1 = 1 04 01
		/// </summary>
		eRENDERDOC_API_Version_1_4_1 = 10401,

		/// <summary>
		/// RENDERDOC_API_1_4_2 = 1 04 02
		/// </summary>
		eRENDERDOC_API_Version_1_4_2 = 10402,

		/// <summary>
		/// RENDERDOC_API_1_5_0 = 1 05 00
		/// </summary>
		eRENDERDOC_API_Version_1_5_0 = 10500,

		/// <summary>
		/// RENDERDOC_API_1_6_0 = 1 06 00
		/// </summary>
		eRENDERDOC_API_Version_1_6_0 = 10600,
	}
    
    /// <summary>
	/// API version changelog:
	/// 1.0.0 - initial release
	/// 1.0.1 - Bugfix: IsFrameCapturing() was returning false for captures that were triggered
	/// by keypress or TriggerCapture, instead of Start/EndFrameCapture.
	/// 1.0.2 - Refactor: Renamed eRENDERDOC_Option_DebugDeviceMode to eRENDERDOC_Option_APIValidation
	/// 1.1.0 - Add feature: TriggerMultiFrameCapture(). Backwards compatible with 1.0.x since the new
	/// function pointer is added to the end of the struct, the original layout is identical
	/// 1.1.1 - Refactor: Renamed remote access to target control (to better disambiguate from remote
	/// replay/remote server concept in replay UI)
	/// 1.1.2 - Refactor: Renamed "log file" in function names to just capture, to clarify that these
	/// are captures and not debug logging files. This is the first API version in the v1.0
	/// branch.
	/// 1.2.0 - Added feature: SetCaptureFileComments() to add comments to a capture file that will be
	/// displayed in the UI program on load.
	/// 1.3.0 - Added feature: New capture option eRENDERDOC_Option_AllowUnsupportedVendorExtensions
	/// which allows users to opt-in to allowing unsupported vendor extensions to function.
	/// Should be used at the user's own risk.
	/// Refactor: Renamed eRENDERDOC_Option_VerifyMapWrites to
	/// eRENDERDOC_Option_VerifyBufferAccess, which now also controls initialisation to
	/// 0xdddddddd of uninitialised buffer contents.
	/// 1.4.0 - Added feature: DiscardFrameCapture() to discard a frame capture in progress and stop
	/// capturing without saving anything to disk.
	/// 1.4.1 - Refactor: Renamed Shutdown to RemoveHooks to better clarify what is happening
	/// 1.4.2 - Refactor: Renamed 'draws' to 'actions' in callstack capture option.
	/// 1.5.0 - Added feature: ShowReplayUI() to request that the replay UI show itself if connected
	/// 1.6.0 - Added feature: SetCaptureTitle() which can be used to set a title for a
	/// capture made with StartFrameCapture() or EndFrameCapture()
	/// </summary>
	[StructLayout(LayoutKind.Sequential)]
	public struct RENDERDOC_API_1_6_0
	{
		public pRENDERDOC_GetAPIVersion GetAPIVersion;
		public pRENDERDOC_SetCaptureOptionU32 SetCaptureOptionU32;
		public pRENDERDOC_SetCaptureOptionF32 SetCaptureOptionF32;
		public pRENDERDOC_GetCaptureOptionU32 GetCaptureOptionU32;
		public pRENDERDOC_GetCaptureOptionF32 GetCaptureOptionF32;
		public pRENDERDOC_SetFocusToggleKeys SetFocusToggleKeys;
		public pRENDERDOC_SetCaptureKeys SetCaptureKeys;
		public pRENDERDOC_GetOverlayBits GetOverlayBits;
		public pRENDERDOC_MaskOverlayBits MaskOverlayBits;

		/// <summary>
		/// Shutdown was renamed to RemoveHooks in 1.4.1.
		/// These unions allow old code to continue compiling without changes
		/// </summary>
		public pRENDERDOC_RemoveHooks RemoveHooks;
		public pRENDERDOC_UnloadCrashHandler UnloadCrashHandler;

		/// <summary>
		/// Get/SetLogFilePathTemplate was renamed to Get/SetCaptureFilePathTemplate in 1.1.2.
		/// These unions allow old code to continue compiling without changes
		/// </summary>
		public pRENDERDOC_SetCaptureFilePathTemplate SetCaptureFilePathTemplate;
		public pRENDERDOC_GetCaptureFilePathTemplate GetCaptureFilePathTemplate;
		public pRENDERDOC_GetNumCaptures GetNumCaptures;
		public pRENDERDOC_GetCapture GetCapture;
		public pRENDERDOC_TriggerCapture TriggerCapture;

		/// <summary>
		/// IsRemoteAccessConnected was renamed to IsTargetControlConnected in 1.1.1.
		/// This union allows old code to continue compiling without changes
		/// </summary>
		public pRENDERDOC_IsTargetControlConnected IsTargetControlConnected;
		public pRENDERDOC_LaunchReplayUI LaunchReplayUI;
		public pRENDERDOC_SetActiveWindow SetActiveWindow;
		public pRENDERDOC_StartFrameCapture StartFrameCapture;
		public pRENDERDOC_IsFrameCapturing IsFrameCapturing;
		public pRENDERDOC_EndFrameCapture EndFrameCapture;

		/// <summary>
		/// new function in 1.1.0
		/// </summary>
		public pRENDERDOC_TriggerMultiFrameCapture TriggerMultiFrameCapture;

		/// <summary>
		/// new function in 1.2.0
		/// </summary>
		public pRENDERDOC_SetCaptureFileComments SetCaptureFileComments;

		/// <summary>
		/// new function in 1.4.0
		/// </summary>
		public pRENDERDOC_DiscardFrameCapture DiscardFrameCapture;

		/// <summary>
		/// new function in 1.5.0
		/// </summary>
		public pRENDERDOC_ShowReplayUI ShowReplayUI;

		/// <summary>
		/// new function in 1.6.0
		/// </summary>
		public pRENDERDOC_SetCaptureTitle SetCaptureTitle;
	}
}