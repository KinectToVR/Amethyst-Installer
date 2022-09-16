using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;

namespace amethyst_installer_gui.PInvoke {
	public static class DeviceManaged {

		/// <summary>
		/// Called whenever a new device is added
		/// </summary>
		public static Action OnDeviceAdded;
		/// <summary>
		/// Called whenever a new device is removed
		/// </summary>
		public static Action OnDeviceRemoved;

		private static NotificationListenWindow s_window;

		public static void RegisterDeviceNotificationHandler() {
			s_window = new NotificationListenWindow();
			s_window.Show();
			if (Application.Current.MainWindow.GetType() == s_window.GetType())
				Application.Current.MainWindow = null;
		}

		public static void UnregisterDeviceNotifications() {
			if ( deviceNotificationHandle != null )
				UnregisterDeviceNotification(deviceNotificationHandle);
			if ( s_window != null ) {
				s_window.Close();
			}
		}

		private static IntPtr deviceNotificationHandle;

		private const int DbtDevicearrival = 0x8000;		// New device        
		private const int DbtDeviceremovecomplete = 0x8004; // Removed
		private const int WmDevicechange = 0x0219;			// Change event
		private const int DbtDevtypDeviceinterface = 5;
		private const int DEVICE_NOTIFY_ALL_INTERFACE_CLASSES = 4;
		private static readonly Guid GuidDevinterfaceUSBDevice = new Guid("A5DCBF10-6530-11D2-901F-00C04FB951ED"); // USB devices

		[DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		private static extern IntPtr RegisterDeviceNotification(IntPtr recipient, IntPtr notificationFilter, int flags);

		[DllImport("user32.dll")]
		private static extern bool UnregisterDeviceNotification(IntPtr handle);

		[StructLayout(LayoutKind.Sequential)]
		private struct DevBroadcastDeviceinterface {
			internal int Size;
			internal int DeviceType;
			internal int Reserved;
			internal Guid ClassGuid;
			internal short Name;
		}

		private class NotificationListenWindow : Window {

			[DllImport("user32.dll")]
			static extern IntPtr SetParent(IntPtr hwnd, IntPtr hwndNewParent);
			private const int HWND_MESSAGE = -3;

			private IntPtr windowHandle;

			public NotificationListenWindow() {}
			protected override void OnSourceInitialized(EventArgs e) {
				
				base.OnSourceInitialized(e);
				HwndSource hwndSource = PresentationSource.FromVisual(this) as HwndSource;

				if ( hwndSource != null ) {
					windowHandle = hwndSource.Handle;
					SetParent(windowHandle, ( IntPtr ) HWND_MESSAGE);
					Visibility = Visibility.Hidden;
					hwndSource.AddHook(WndProc);

					var dbi = new DevBroadcastDeviceinterface {
						DeviceType = DbtDevtypDeviceinterface,
						Reserved = 0,
						ClassGuid = GuidDevinterfaceUSBDevice,
						Name = 0
					};

					dbi.Size = Marshal.SizeOf(dbi);
					IntPtr buffer = Marshal.AllocHGlobal(dbi.Size);
					Marshal.StructureToPtr(dbi, buffer, true);

					deviceNotificationHandle = RegisterDeviceNotification(windowHandle, buffer, DEVICE_NOTIFY_ALL_INTERFACE_CLASSES);
				}
			}

			private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled) {

				if ( msg == WmDevicechange ) {
					switch ( ( int ) wParam ) {
						case DbtDeviceremovecomplete:
							if ( OnDeviceRemoved != null )
								OnDeviceRemoved();
							break;
						case DbtDevicearrival:
							if ( OnDeviceAdded != null )
								OnDeviceAdded();

							break;
					}
				}

				handled = false;
				return IntPtr.Zero;
			}
		}
	}
}