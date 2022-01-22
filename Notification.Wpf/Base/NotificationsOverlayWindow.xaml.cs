using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using Notification.Wpf.Constants;
using Notification.Wpf.Controls;

namespace Notification.Wpf
{
    /// <summary>
    /// Interaction logic for ToastWindow
    /// </summary>
    public partial class NotificationsOverlayWindow : Window
    {
        #region MaxWindowItems : int - Maximum Window Items

        /// <summary>Maximum Window Items</summary>
        public static readonly DependencyProperty MaxWindowItemsProperty =
            DependencyProperty.Register(
                nameof(MaxWindowItems),
                typeof(uint),
                typeof(NotificationsOverlayWindow),
                new PropertyMetadata(NotificationConstants.NotificationsOverlayWindowMaxCount));

        /// <summary>Maximum Window Items</summary>
        public uint MaxWindowItems { get => (uint)GetValue(MaxWindowItemsProperty); set => SetValue(MaxWindowItemsProperty, value); }

        #endregion

        #region MessagePosition : NotificationPosition - Позиция сообщений в окне

        /// <summary>Позиция сообщений в окне</summary>
        public static readonly DependencyProperty MessagePositionProperty =
            DependencyProperty.Register(
                nameof(MessagePosition),
                typeof(NotificationPosition),
                typeof(NotificationsOverlayWindow),
                new PropertyMetadata(NotificationConstants.MessagePosition));

        /// <summary>Позиция сообщений в окне</summary>
        public NotificationPosition MessagePosition { get => (NotificationPosition)GetValue(MessagePositionProperty); set => SetValue(MessagePositionProperty, value); }

        #endregion

        #region CollapseProgressAutoIfMoreMessages : bool - Need collapse notification if count more that maximum

        /// <summary>Need collapse notification if count more that maximum</summary>
        public static readonly DependencyProperty CollapseProgressAutoIfMoreMessagesProperty =
            DependencyProperty.Register(
                nameof(CollapseProgressAutoIfMoreMessages),
                typeof(bool),
                typeof(NotificationsOverlayWindow),
                new PropertyMetadata(NotificationConstants.CollapseProgressIfMoreRows));

        /// <summary>Need collapse notification if count more that maximum</summary>
        public bool CollapseProgressAutoIfMoreMessages { get => (bool)GetValue(CollapseProgressAutoIfMoreMessagesProperty); set => SetValue(CollapseProgressAutoIfMoreMessagesProperty, value); }

        internal NotificationArea NotificationArea => this._notificationArea;
        
        #endregion
        public NotificationsOverlayWindow()
        {
            InitializeComponent();
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            try
            {
                //prevents overlay window from displaying in alt + tab 
                WindowInteropHelper wndHelper = new WindowInteropHelper(this);
                int exStyle = (int)GetWindowLong(wndHelper.Handle, GWL_EXSTYLE);
                exStyle |= WS_EX_TOOLWINDOW;
                SetWindowLong(wndHelper.Handle, GWL_EXSTYLE, (IntPtr)exStyle);
            }
            catch { }
        }

        private const int WS_EX_TOOLWINDOW = 0x00000080;
        private const int GWL_EXSTYLE = (-20);

        private static IntPtr SetWindowLong(IntPtr hWnd, int nIndex, IntPtr dwNewLong)
        {
            int error = 0;
            IntPtr result = IntPtr.Zero;
            SetLastError(0);

            if (IntPtr.Size == 4)
            {
                Int32 tempResult = IntSetWindowLong(hWnd, nIndex, IntPtrToInt32(dwNewLong));
                error = Marshal.GetLastWin32Error();
                result = new IntPtr(tempResult);
            }
            else
            {
                result = IntSetWindowLongPtr(hWnd, nIndex, dwNewLong);
                error = Marshal.GetLastWin32Error();
            }

            if ((result == IntPtr.Zero) && (error != 0))
            {
                throw new System.ComponentModel.Win32Exception(error);
            }

            return result;
        }

        private static int IntPtrToInt32(IntPtr intPtr)
        {
            return unchecked((int)intPtr.ToInt64());
        }

        [DllImport("user32.dll")]
        private static extern IntPtr GetWindowLong(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll", EntryPoint = "SetWindowLongPtr", SetLastError = true)]
        private static extern IntPtr IntSetWindowLongPtr(IntPtr hWnd, int nIndex, IntPtr dwNewLong);

        [DllImport("user32.dll", EntryPoint = "SetWindowLong", SetLastError = true)]
        private static extern int IntSetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

        [DllImport("kernel32.dll", EntryPoint = "SetLastError")]
        private static extern void SetLastError(int dwErrorCode);
    }
}
