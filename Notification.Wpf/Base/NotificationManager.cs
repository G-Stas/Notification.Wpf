using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;
using Notification.Wpf.Constants;
using Notification.Wpf.Controls;

namespace Notification.Wpf
{
    /// <inheritdoc />
    public class NotificationManager : INotificationManager
    {
        private readonly Dispatcher _dispatcher;
        private static readonly List<NotificationArea> Areas = new();
        private static NotificationsOverlayWindow _window;
        private static int _verticalIndent;
        private static int _horizontalIndent;
        
        private static double _workAreaLeft = SystemParameters.WorkArea.Left;
        private static double _workAreaTop = SystemParameters.WorkArea.Top;
        private static double _workAreaHeight = SystemParameters.PrimaryScreenHeight;
        private static double _workAreaWidth = SystemParameters.PrimaryScreenHeight;

        /// <summary>
        /// Initialize new notification manager
        /// </summary>
        /// <param name="dispatcher">dispatcher for manager (can be null)</param>
        public NotificationManager(Dispatcher dispatcher = null)
        {
            dispatcher ??= Application.Current?.Dispatcher ?? Dispatcher.CurrentDispatcher;
            _dispatcher = dispatcher;
        }

        /// <summary>
        /// Show any content
        /// </summary>
        /// <param name="content"></param>
        /// <param name="key">Used to access existing notifications to reset its display time</param>
        /// <param name="expirationTime"></param>
        /// <param name="displayCloseButton"></param>
        public void ShowNotification(object content, string key, bool displayCloseButton, TimeSpan? expirationTime = null)
        {
            if (!_dispatcher.CheckAccess())
            {
                _dispatcher.BeginInvoke(new Action(() => ShowNotification(content, key, displayCloseButton, expirationTime)));
                return;
            }
            try
            {
                ShowMessageContent(content, key, displayCloseButton, expirationTime);
                UpdateHorizontalIndent();
                UpdateVerticalIndent();
            }
            catch
            {
                _window?.NotificationArea.Dispose();
                _window?.Close();
                throw;
            }
        }

        public void CloseNotification(string key)
        {
            if(Areas == null)
                return;
            try { _window?.NotificationArea.CloseNotification(key); }
            catch
            {
                _window?.NotificationArea.Dispose();
                _window?.Close();
                throw;
            }
        }

        public bool IsDisplayed(string key, bool resetDisplayTimer)
        {
            return Areas != null && Areas.Any(a => a.IsDisplayed(key, resetDisplayTimer));
        }

        public void ChangeNotificationAreaPosition(NotificationPosition newPosition)
        {
            NotificationConstants.MessagePosition = newPosition;
            
            if(_window == null)
                return;
            _window.MessagePosition = newPosition;
            UpdateHorizontalIndent();
            UpdateVerticalIndent();
        }

        public void SetNotificationAreaHorizontalIndent(int indent)
        {
            if(_horizontalIndent == indent)
                return;
            _horizontalIndent = indent;
            UpdateHorizontalIndent();
        }

        public void SetNotificationAreaVerticalIndent(int indent)
        {
            if(_verticalIndent == indent)
                return;
            _verticalIndent = indent;
            UpdateVerticalIndent();
        }

        public void SetNotificationsMaxDisplayCount(uint count)
        {
            NotificationConstants.NotificationsOverlayWindowMaxCount = count;
        }

        public void SetWorkArea(double left, double top, double width, double height)
        {
            _workAreaLeft = left;
            _workAreaTop = top;
            _workAreaWidth = width;
            _workAreaHeight = height;

            if(_window != null)
            {
                _window.Width = _workAreaWidth;
                _window.Height = _workAreaHeight;
            }
            
            UpdateHorizontalIndent();
            UpdateVerticalIndent();
        }

        private void UpdateHorizontalIndent()
        {
            if(_window != null)
            {
                switch(NotificationConstants.MessagePosition)
                {
                    case NotificationPosition.TopLeft:
                    case NotificationPosition.BottomLeft:
                    case NotificationPosition.CenterLeft:
                    {
                        _window.Left = _workAreaLeft + _horizontalIndent;
                        break;
                    }
                    case NotificationPosition.TopRight:
                    case NotificationPosition.BottomRight:
                    case NotificationPosition.CenterRight:
                    {
                        _window.Left = _workAreaLeft - _horizontalIndent;
                        break;
                    }
                    case NotificationPosition.TopCenter:
                    case NotificationPosition.BottomCenter:
                    {
                        _window.Left = _workAreaLeft + (_workAreaLeft / 2);
                        break;
                    }
                }
            }
        }

        private void UpdateVerticalIndent()
        {
            if(_window != null)
            {
                switch(NotificationConstants.MessagePosition)
                {
                    case NotificationPosition.TopLeft:
                    case NotificationPosition.TopRight:
                    case NotificationPosition.TopCenter:
                    {
                        _window.Top = _workAreaTop + _verticalIndent;
                        break;
                    }
                    case NotificationPosition.BottomLeft:
                    case NotificationPosition.BottomRight:
                    case NotificationPosition.BottomCenter:
                    {
                        _window.Top = _workAreaTop - _verticalIndent;
                        break;
                    }
                }
            }
        }
        
        private ThemeColors Colors = ThemeColors.Instance;

        public void UpdateColors(Brush background, Brush foreground, Brush hover)
        {
            Colors.Background = background;
            Colors.Foreground = foreground;
            Colors.Hover = hover;
        }

        private static void ShowMessageContent(object content, string key, bool displayCloseButton, TimeSpan? expirationTime = null)
        {
            expirationTime ??= TimeSpan.FromSeconds(3);

            if (_window == null)
            {
                _window = new NotificationsOverlayWindow
                {
                    Width = _workAreaWidth,
                    Height = _workAreaHeight,
                    CollapseProgressAutoIfMoreMessages = NotificationConstants.CollapseProgressIfMoreRows,
                    MaxWindowItems = NotificationConstants.NotificationsOverlayWindowMaxCount,
                    MessagePosition = NotificationConstants.MessagePosition,
                    WindowStartupLocation = WindowStartupLocation.Manual
                };
                _window.Closed += (_, _) =>
                {
                    _window.NotificationArea.Dispose();
                    _window = null;
                };
            }

            if(Areas != null)
            {
                if(_window is { IsVisible: false })
                    _window.Show();
            }
            else return;
            
            _window.NotificationArea.ShowNotification(content, key, displayCloseButton, (TimeSpan)expirationTime);
        }

        internal static void AddArea(NotificationArea area)
        {
            Areas.Add(area);
        }

        internal static void RemoveArea(NotificationArea area)
        {
            Areas.Remove(area);
        }
    }
}