using System;
using System.ComponentModel;
using System.Configuration;
using System.Windows;

namespace AlbumArtDownloader.Properties
{
	/// <summary>
	/// Persists a Window's Size, Location and WindowState to UserScopeSettings 
	/// </summary>
	public class WindowSettings
	{
		#region WindowApplicationSettings Helper Class
		public class WindowApplicationSettings : ApplicationSettingsBase
		{
			private WindowSettings windowSettings;

			public WindowApplicationSettings(WindowSettings windowSettings)
				: base(windowSettings.window.GetType().Name)
			{
				this.windowSettings = windowSettings;
			}

			[UserScopedSetting]
			public Rect Location
			{
				get
				{
					if (this["Location"] != null)
					{
						return ((Rect)this["Location"]);
					}
					return Rect.Empty;
				}
				set
				{
					this["Location"] = value;
				}
			}

			[UserScopedSetting]
			public WindowState WindowState
			{
				get
				{
					if (this["WindowState"] != null)
					{
						return (WindowState)this["WindowState"];
					}
					return WindowState.Normal;
				}
				set
				{
					this["WindowState"] = value;
				}
			}

			//Version stored for upgrade purposes
			[UserScopedSetting]
			public string ApplicationVersion
			{
				get
				{
					if (this["ApplicationVersion"] != null)
					{
						return (string)this["ApplicationVersion"];
					}
					return String.Empty;
				}
				set
				{
					this["ApplicationVersion"] = value;
				}
			}

			protected override void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
			{
				base.OnPropertyChanged(sender, e);
			}

			protected override void OnSettingChanging(object sender, SettingChangingEventArgs e)
			{
				base.OnSettingChanging(sender, e);
			}

		}
		#endregion

		#region Constructor
		private Window window = null;

		public WindowSettings(Window window)
		{
			this.window = window;
		}

		#endregion

		#region Attached "Save" Property Implementation
		/// <summary>
		/// Register the "Save" attached property and the "OnSaveInvalidated" callback 
		/// </summary>
		public static readonly DependencyProperty SaveProperty
		   = DependencyProperty.RegisterAttached("Save", typeof(bool), typeof(WindowSettings),
				new FrameworkPropertyMetadata(new PropertyChangedCallback(OnSaveInvalidated)));

		public static void SetSave(DependencyObject dependencyObject, bool enabled)
		{
			dependencyObject.SetValue(SaveProperty, enabled);
		}

		/// <summary>
		/// Called when Save is changed on an object.
		/// </summary>
		private static void OnSaveInvalidated(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
		{
			Window window = dependencyObject as Window;
			if (window != null)
			{
				if ((bool)e.NewValue)
				{
					WindowSettings settings = new WindowSettings(window);
					settings.Attach();
				}
			}
		}

		#endregion

		#region Attached readonly "WindowSettings" property
		private static readonly DependencyPropertyKey WindowSettingsPropertyKey = DependencyProperty.RegisterAttachedReadOnly("WindowSettings", typeof(WindowSettings), typeof(WindowSettings), new FrameworkPropertyMetadata());
		public static readonly DependencyProperty WindowSettingsProperty = WindowSettingsPropertyKey.DependencyProperty;

		public static WindowSettings GetWindowSettings(DependencyObject dependencyObject)
		{
			return (WindowSettings)dependencyObject.GetValue(WindowSettingsProperty);
		}
		private static void SetWindowSettings(DependencyObject dependencyObject, WindowSettings value)
		{
			dependencyObject.SetValue(WindowSettingsPropertyKey, value);
		}

		#endregion

		#region Protected Methods
		/// <summary>
		/// Load the Window Size Location and State from the settings object
		/// </summary>
		public virtual void LoadWindowState()
		{
			this.Settings.Reload();
			if (this.Settings.Location != Rect.Empty)
			{
				this.window.Left = this.Settings.Location.Left;
				this.window.Top = this.Settings.Location.Top;
				this.window.Width = this.Settings.Location.Width;
				this.window.Height = this.Settings.Location.Height;
			}

			if (this.Settings.WindowState != WindowState.Maximized)
			{
				this.window.WindowState = this.Settings.WindowState;
			}
		}


		/// <summary>
		/// Save the Window Size, Location and State to the settings object
		/// </summary>
		public virtual void SaveWindowState()
		{
			this.Settings.WindowState = this.window.WindowState;
			this.Settings.Location = this.window.RestoreBounds;
			this.Settings.ApplicationVersion = GetVersionString();

			try
			{
				this.Settings.Save();
			}
			catch (Exception e)
			{
				System.Diagnostics.Trace.TraceError("Could not save window state settings: " + e.Message);
			}
		}
		#endregion

		#region Private Methods

		private void Attach()
		{
			if (this.window != null)
			{
				this.window.Closing += new CancelEventHandler(window_Closing);
				this.window.Initialized += new EventHandler(window_Initialized);
				this.window.Activated += window_Activated;
				SetWindowSettings(window, this);
			}
		}

		private void window_Activated(object sender, EventArgs e)
		{
			this.window.Activated -= window_Activated; //Only do this once
			if (this.Settings.WindowState == WindowState.Maximized)
			{
				this.window.WindowState = this.Settings.WindowState;
			}
		}

		private void window_Initialized(object sender, EventArgs e)
		{
			LoadWindowState();
		}

		private void window_Closing(object sender, CancelEventArgs e)
		{
			SaveWindowState();
		}
		#endregion

		#region Settings Property Implementation
		private WindowApplicationSettings windowApplicationSettings = null;

		protected virtual WindowApplicationSettings CreateWindowApplicationSettingsInstance()
		{
			return new WindowApplicationSettings(this);
		}

		[Browsable(false)]
		public WindowApplicationSettings Settings
		{
			get
			{
				if (windowApplicationSettings == null)
				{
					windowApplicationSettings = CreateWindowApplicationSettingsInstance();

					//Settings may need upgrading from an earlier version
					string currentVersion = GetVersionString();
					if (windowApplicationSettings.ApplicationVersion != currentVersion)
					{
						System.Diagnostics.Debug.WriteLine("Upgrading window settings");
						windowApplicationSettings.Upgrade();
					}

#if EPHEMERAL_SETTINGS
					windowApplicationSettings.Reset();
#endif
				}
				return windowApplicationSettings;
			}
		}
		#endregion

		/// <summary>
		/// String used to determine whether an upgrade of the settings is reqired
		/// </summary>
		private static string GetVersionString()
		{
			return System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
		}
	}
}

