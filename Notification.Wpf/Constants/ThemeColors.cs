using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Media;

namespace Notification.Wpf.Constants;

public class ThemeColors : INotifyPropertyChanged
{
	private Brush _background;
	public Brush Background
	{
		get => this._background;
		set
		{
			this._background = value;
			OnPropertyChanged();
		}
	}

	private Brush _foreground;
	public Brush Foreground
	{
		get => this._foreground;
		set
		{
			this._foreground = value;
			OnPropertyChanged();
		}
	}
	
	private Brush _hover;
	public Brush Hover
	{
		get => this._hover;
		set
		{
			this._hover = value;
			OnPropertyChanged();
		}
	}

	private ThemeColors() { }

	private static ThemeColors _instance;
	internal static ThemeColors Instance
	{
		get
		{
			if(_instance == null)
				_instance = new ThemeColors();
			return _instance;
		}
	}
	
	public event PropertyChangedEventHandler PropertyChanged;

	protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
		=> PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}