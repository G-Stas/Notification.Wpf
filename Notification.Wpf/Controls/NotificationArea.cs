using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Notification.Wpf.Constants;
using Notification.Wpf.View;

namespace Notification.Wpf.Controls
{
    public class NotificationArea : Control, IDisposable
    {
        #region CollapseProgressAuto : bool - Progress bar will automatically collapsed if items count more that max items

        /// <summary>Progress bar will automatically collapsed if items count more that max items</summary>
        public static readonly DependencyProperty CollapseProgressAutoProperty =
            DependencyProperty.Register(
                nameof(CollapseProgressAuto),
                typeof(bool),
                typeof(NotificationArea),
                new PropertyMetadata(default(bool)));

        /// <summary>Progress bar will automatically collapsed if items count more that max items</summary>
        public bool CollapseProgressAuto { get => (bool)GetValue(CollapseProgressAutoProperty); set => SetValue(CollapseProgressAutoProperty, value); }

        #endregion

        //public NotificationPosition Position
        //{
        //    get => (NotificationPosition)GetValue(PositionProperty);
        //    set => SetValue(PositionProperty, value);
        //}

        //// Using a DependencyProperty as the backing store for Position.  This enables animation, styling, binding, etc...
        //public static readonly DependencyProperty PositionProperty =
        //    DependencyProperty.Register("Position", typeof(NotificationPosition), typeof(NotificationArea), new PropertyMetadata(NotificationPosition.BottomRight));

        #region Position : NotificationPosition - Area position on overlay window

        /// <summary>Area position on overlay window</summary>
        public static readonly DependencyProperty PositionProperty =
            DependencyProperty.Register(
                nameof(Position),
                typeof(NotificationPosition),
                typeof(NotificationArea),
                new PropertyMetadata(NotificationPosition.TopLeft));

        /// <summary>Area position on overlay window</summary>
        public NotificationPosition Position
        {
            get => (NotificationPosition)GetValue(PositionProperty);
            set => SetValue(PositionProperty, value);
        }

        #endregion

        public int MaxItems
        {
            get => (int)GetValue(MaxItemsProperty);
            set => SetValue(MaxItemsProperty, value);
        }

        public static readonly DependencyProperty MaxItemsProperty =
            DependencyProperty.Register("MaxItems", typeof(int), typeof(NotificationArea), new PropertyMetadata(int.MaxValue));

        #region IsReversed : bool - Are is reversed

        /// <summary>Are is reversed</summary>
        public static readonly DependencyProperty IsReversedProperty =
            DependencyProperty.Register(
                nameof(IsReversed),
                typeof(bool),
                typeof(NotificationArea),
                new PropertyMetadata(default(bool)));

        /// <summary>Are is reversed</summary>
        public bool IsReversed { get => (bool)GetValue(IsReversedProperty); set => SetValue(IsReversedProperty, value); }

        #endregion

        private IList _items;

        public NotificationArea()
        {
            NotificationManager.AddArea(this);
            this.Unloaded += (sender, args) =>
            {
                NotificationManager.RemoveArea(this);
            };
        }

        static NotificationArea()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(NotificationArea),
                new FrameworkPropertyMetadata(typeof(NotificationArea)));
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            var itemsControl = GetTemplateChild("PART_Items") as Panel;
            _items = itemsControl?.Children;

            IsReversed = NotificationConstants.IsReversedPanel is { } reverse ? reverse
            : Position is NotificationPosition.BottomCenter or NotificationPosition.BottomLeft or NotificationPosition.BottomRight;
        }

        private readonly Dictionary<string, VolumeNotification> _displayedNotifications = new ();
        public int NotificationsCount => this._displayedNotifications.Keys.Count;
        
        public void ShowNotification(object content, string key, bool displayCloseButton, TimeSpan expirationTime)
        {
            if(this._displayedNotifications.TryGetValue(key, out VolumeNotification ntf))
            {
                ntf.ResetDisplayTimer();
                return;
            }
            
            var notification = new VolumeNotification(content, key, displayCloseButton, expirationTime);
            this._displayedNotifications.Add(key, notification);
            
            notification.NotificationClosed += OnNotificationClosed;

            ShowNotificationContent(notification, expirationTime);
        }

        public void CloseNotification(string key)
        {
            if(this._displayedNotifications.TryGetValue(key, out var ntf))
            {
                ntf.Close();
            }
        }

        private void OnNotificationClosed(object sender, RoutedEventArgs e)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                this._items.Remove(sender);
            });
            if(sender is VolumeNotification vn)
            {
                this._displayedNotifications.Remove(vn.Key);
                vn.NotificationClosed -= OnNotificationClosed;
            }
            
        }

        private void ShowNotificationContent(VolumeNotification notification, TimeSpan? expirationTime = null)
        {
            if (!IsLoaded)
                return;

            var w = Window.GetWindow(this);
            var x = PresentationSource.FromVisual(w);
            if (x == null)
                return;
            lock (_items)
            {
                _items.Add(notification);
                if(expirationTime != TimeSpan.MaxValue)
                    notification.InvokeDisplayTimer();

                if (_items.OfType<Notification>().Count(i => !i.IsClosing) > MaxItems)
                {
                    if (_items.OfType<Notification>().Where(i => i.Content is not NotificationProgress).Count(i => !i.IsClosing) > MaxItems)
                        _items.OfType<Notification>().Where(i => i.Content is not NotificationProgress).FirstOrDefault(i => !i.IsClosing)?.Close();
                }
            }
        }

        public void Dispose()
        {
            foreach(var notification in this._displayedNotifications.Values)
                notification.Close();
        }
    }
}