using System;
using System.Timers;
using System.Windows;
using System.Windows.Input;

namespace Notification.Wpf.Controls;

public class VolumeNotification : Notification
{
	private Timer _displayTimer;
	private string _key;

	internal string Key => this._key;

	public VolumeNotification(object content, string key, bool displayCloseButton, TimeSpan expirationTime) : base(content, ShowXbtn: false)
	{
		this._key = key;
		this.XbtnVisibility = displayCloseButton ? Visibility.Visible : Visibility.Hidden;
		if(expirationTime != TimeSpan.MaxValue)
		{
			this._displayTimer = new Timer(expirationTime.TotalMilliseconds);
			this._displayTimer.AutoReset = false;
			this._displayTimer.Elapsed += OnDisplayTimerElapsed;
		}
		this.MouseEnter += OnMouseEnter;
		this.MouseLeave += OnMouseLeave;
	}

	private void OnMouseEnter(object sender, MouseEventArgs e)
		=> this._displayTimer?.Stop();
	
	private void OnMouseLeave(object sender, MouseEventArgs e)
		=> this._displayTimer?.Start();

	public override void Close()
	{
		if(this._displayTimer != null)
		{
			this._displayTimer.Stop();
			this._displayTimer.Dispose();
			this._displayTimer.Elapsed -= OnDisplayTimerElapsed;
		}

		this.MouseEnter -= OnMouseEnter;
		this.MouseLeave -= OnMouseLeave;
		
		Application.Current.Dispatcher.Invoke(() =>
		{
			base.Close();
		}); 
	}

	private void OnDisplayTimerElapsed(object sender, ElapsedEventArgs e)
	{
		this.Close();
	}

	public void InvokeDisplayTimer()
	{
		if(this._displayTimer.Enabled)
			this._displayTimer.Stop();
		this._displayTimer.Start();
	}

	public void ResetDisplayTimer()
	{
		if(this._displayTimer == null)
			return;
		this._displayTimer.Stop();
		this._displayTimer.Start();
	}
}