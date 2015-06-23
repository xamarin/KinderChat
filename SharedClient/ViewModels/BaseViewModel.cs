using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace KinderChat
{
	public class BaseViewModel : INotifyPropertyChanged
	{
		public event EventHandler<NotificationEventArgs> OnNotification;

		public bool Initialized { get; set; }

		string title = string.Empty;
		public const string TitlePropertyName = "Title";

		/// <summary>
		/// Gets or sets the "Title" property
		/// </summary>
		/// <value>The title.</value>
		public string Title {
			get { return title; }
			set { SetProperty (ref title, value); }
		}

		bool isBusy;
		/// <summary>
		/// Gets or sets if the view is busy.
		/// </summary>
		public const string IsBusyPropertyName = "IsBusy";

		public bool IsBusy {
			get { return isBusy; }
			set { SetProperty (ref isBusy, value); }
		}

		bool canLoadMore = true;
		/// <summary>
		/// Gets or sets if we can load more.
		/// </summary>
		public const string CanLoadMorePropertyName = "CanLoadMore";

		public bool CanLoadMore {
			get { return canLoadMore; }
			set { SetProperty (ref canLoadMore, value); }
		}

	    public virtual void OnClose()
	    {
	    }

		protected void SetProperty<T> (
			ref T backingStore, T value,
			[CallerMemberName]string propertyName = "",
			Action onChanged = null)
		{


			if (EqualityComparer<T>.Default.Equals (backingStore, value))
				return;

			backingStore = value;

			if (onChanged != null)
				onChanged ();

			OnPropertyChanged (propertyName);
		}

		protected async Task<bool> RefreshToken()
		{
			if (App.FakeSignup) {
				return true;
			}

			if (DateTime.UtcNow <= new DateTime (Settings.KeyValidUntil))
				return true;

			var finalToken = await App.AuthenticationManager.Authenticate(Settings.UserDeviceLoginId, Settings.DevicePassword).ConfigureAwait (false);
			if(finalToken == null || finalToken.AccessToken == null){
				App.MessageDialog.SendToast ("Invalid PIN, please try again");
				return false;
			}

			Settings.AccessToken = finalToken.AccessToken;
			var nextTime = DateTime.UtcNow.AddSeconds(finalToken.ExpiresIn).Ticks;
			Settings.KeyValidUntil = nextTime;
			return true;
		}


		#region INotifyPropertyChanged implementation

		public event PropertyChangedEventHandler PropertyChanged;

		#endregion

		public void OnPropertyChanged ([CallerMemberName]string propertyName = "")
		{
			if (PropertyChanged == null)
				return;

			PropertyChanged (this, new PropertyChangedEventArgs (propertyName));
		}

		protected void RaiseNotification(string msg, string title = "")
		{
			var handler = OnNotification;
			if (handler != null)
				handler (this, new NotificationEventArgs { Message = msg, Title = title });
		}

		protected void RaiseError(string msg)
		{
			var handler = OnNotification;
			if (handler != null)
				handler (this, new NotificationEventArgs { Message = msg, Title = "Error" });
		}

		protected IDisposable BusyContext ()
		{
            return new DisposableContext(() => IsBusy = true, () => IsBusy = false);
		}
	}
}
