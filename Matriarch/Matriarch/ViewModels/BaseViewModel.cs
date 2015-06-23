using Matriarch.Helpers;
using KinderChat.ServerClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Matriarch.ViewModels
{

	public class BaseViewModel : INotifyPropertyChanged
	{
		public BaseViewModel()
		{
		}

		private string title = string.Empty;
		public const string TitlePropertyName = "Title";

		/// <summary>
		/// Gets or sets the "Title" property
		/// </summary>
		/// <value>The title.</value>
		public string Title
		{
			get { return title; }
			set { SetProperty(ref title, value); }
		}

		private string subTitle = string.Empty;
		/// <summary>
		/// Gets or sets the "Subtitle" property
		/// </summary>
		public const string SubtitlePropertyName = "Subtitle";
		public string Subtitle
		{
			get { return subTitle; }
			set { SetProperty(ref subTitle, value); }
		}

		private string icon = null;
		/// <summary>
		/// Gets or sets the "Icon" of the viewmodel
		/// </summary>
		public const string IconPropertyName = "Icon";
		public string Icon
		{
			get { return icon; }
			set { SetProperty(ref icon, value); }
		}

		private bool isBusy;
		/// <summary>
		/// Gets or sets if the view is busy.
		/// </summary>
		public const string IsBusyPropertyName = "IsBusy";
		public bool IsBusy
		{
			get { return isBusy; }
			set { SetProperty(ref isBusy, value); }
		}

		private bool canLoadMore = true;
		/// <summary>
		/// Gets or sets if we can load more.
		/// </summary>
		public const string CanLoadMorePropertyName = "CanLoadMore";
		public bool CanLoadMore
		{
			get { return canLoadMore; }
			set { SetProperty(ref canLoadMore, value); }
		}

		protected void SetProperty<T>(
			ref T backingStore, T value,
			[CallerMemberName]string propertyName = "",
			Action onChanged = null)
		{


			if (EqualityComparer<T>.Default.Equals(backingStore, value))
				return;

			backingStore = value;

			if (onChanged != null)
				onChanged();

			OnPropertyChanged(propertyName);
		}

		#region INotifyPropertyChanged implementation
		public event PropertyChangedEventHandler PropertyChanged;
		#endregion

		public void OnPropertyChanged(string propertyName)
		{
			if (PropertyChanged == null)
				return;

			PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
		}

		protected async Task<bool> RefreshToken()
		{


			if (DateTime.UtcNow <= new DateTime(Settings.KeyValidUntil))
				return true;

			var authManager = new AuthenticationManager();

			var finalToken = await authManager.Authenticate(Settings.UserDeviceLoginId, Settings.AccessToken);
			if (finalToken == null || finalToken.AccessToken == null)
			{
				
				return false;
			}

			Settings.AccessToken = finalToken.AccessToken;
			var nextTime = DateTime.UtcNow.AddSeconds(finalToken.ExpiresIn).Ticks;
			Settings.KeyValidUntil = nextTime;
			return true;
		}
	}
}
