using System;
using System.Collections.Generic;

using UIKit;

namespace KinderChat.iOS
{
	public class Theme : IColorTheme, IFontTheme, IImagesTheme
	{
		public static event EventHandler ThemeChanged;

		public static List<Theme> AvailableThemes;

		static Theme current;
		public static Theme Current {
			get {
				return current;
			}
			set {
				current = value;
				RaiseThemeChanged (null);
			}
		}

		IColorTheme colorTheme;
		IFontTheme fontTheme;
		IImagesTheme imgTheme;

		public bool IsDirty { get; private set; }

		#region IColorTheme implementation

		UIStatusBarStyle statusBarStyle;
		public UIStatusBarStyle StatusBarStyle {
			get {
				return statusBarStyle;
			}
			set {
				statusBarStyle = value;
				IsDirty = true;
				RaiseThemeChanged (this);
			}
		}

		UIColor screenTitleColor;
		public UIColor ScreenTitleColor {
			get {
				return screenTitleColor;
			}
			set {
				screenTitleColor = value;
				IsDirty = true;
				RaiseThemeChanged (this);
			}
		}

		UIColor mainColor;
		public UIColor MainColor {
			get {
				return mainColor;
			}
			set {
				mainColor = value;
				IsDirty = true;
				RaiseThemeChanged (this);
			}
		}

		UIColor mainSaturatedColor;
		public UIColor MainSaturatedColor {
			get {
				return mainSaturatedColor;
			}
			set {
				mainSaturatedColor = value;
				IsDirty = true;
				RaiseThemeChanged (this);
			}
		}

		UIColor disabledButtonColor;
		public UIColor DisabledButtonColor {
			get {
				return disabledButtonColor;
			}
			set {
				disabledButtonColor = value;
				IsDirty = true;
				RaiseThemeChanged (this);
			}
		}

		UIColor titleTextColor;
		public UIColor TitleTextColor {
			get {
				return titleTextColor;
			}
			set {
				titleTextColor = value;
				IsDirty = true;
				RaiseThemeChanged (this);
			}
		}

		UIColor descriptionDimmedColor;
		public UIColor DescriptionDimmedColor {
			get {
				return descriptionDimmedColor;
			}
			set {
				descriptionDimmedColor = value;
				IsDirty = true;
				RaiseThemeChanged (this);
			}
		}

		UIColor backgroundColor;
		public UIColor BackgroundColor {
			get {
				return backgroundColor;
			}
			set {
				backgroundColor = value;
				IsDirty = true;
				RaiseThemeChanged (this);
			}
		}

		// TODO: Warning Unused
		UIColor navigationBarButtonColor;
		public UIColor NavigationBarButtonColor {
			get {
				return navigationBarButtonColor;
			}
			set {
				navigationBarButtonColor = value;
				IsDirty = true;
				RaiseThemeChanged (this);
			}
		}

		UIColor friendNameColor;
		public UIColor FriendNameColor {
			get {
				return friendNameColor;
			}
			set {
				friendNameColor = value;
				IsDirty = true;
				RaiseThemeChanged (this);
			}
		}

		UIColor lastMessageTextColor;
		public UIColor LastMessageTextColor {
			get {
				return lastMessageTextColor;
			}
			set {
				lastMessageTextColor = value;
				IsDirty = true;
				RaiseThemeChanged (this);
			}
		}

		UIColor dateMessageLabelColor;
		public UIColor DateMessageLabelColor {
			get {
				return dateMessageLabelColor;
			}
			set {
				dateMessageLabelColor = value;
				IsDirty = true;
				RaiseThemeChanged (this);
			}
		}

		UIColor conversationSelectedCellColor;
		public UIColor ConversationSelectedCellColor {
			get {
				return conversationSelectedCellColor;
			}
			set {
				conversationSelectedCellColor = value;
				IsDirty = true;
				RaiseThemeChanged (this);
			}
		}

		UIColor incomingBubbleStroke;
		public UIColor IncomingBubbleStroke {
			get {
				return incomingBubbleStroke;
			}
			set {
				incomingBubbleStroke = value;
				IsDirty = true;
				RaiseThemeChanged (this);
			}
		}

		UIColor incomingTextColor;
		public UIColor IncomingTextColor {
			get {
				return incomingTextColor;
			}
			set {
				incomingTextColor = value;
				IsDirty = true;
				RaiseThemeChanged (this);
			}
		}

		UIColor outgoingTextColor;
		public UIColor OutgoingTextColor {
			get {
				return outgoingTextColor;
			}
			set {
				outgoingTextColor = value;
				IsDirty = true;
				RaiseThemeChanged (this);
			}
		}

		UIColor badgeTitleColor;
		public UIColor BadgeTitleColor {
			get {
				return badgeTitleColor;
			}
			set {
				badgeTitleColor = value;
				IsDirty = true;
				RaiseThemeChanged (this);
			}
		}
		#endregion

		#region IFontTheme implementation

		UIFont screenTitleFont;
		public UIFont ScreenTitleFont {
			get {
				return screenTitleFont;
			}
			set {
				screenTitleFont = value;
				IsDirty = true;
				RaiseThemeChanged (this);
			}
		}

		UIFont navigationButtonTitleFont;
		public UIFont NavigationButtonTitleFont {
			get {
				return navigationButtonTitleFont;
			}
			set {
				navigationButtonTitleFont = value;
				IsDirty = true;
				RaiseThemeChanged (this);
			}
		}

		UIFont sausageContrinueButtonFont;
		public UIFont SausageContrinueButtonFont {
			get {
				return sausageContrinueButtonFont;
			}
			set {
				sausageContrinueButtonFont = value;
				IsDirty = true;
				RaiseThemeChanged (this);
			}
		}

		UIFont sausageSwitchIdentityButtonFont;
		public UIFont SausageSwitchIdentityButtonFont {
			get {
				return sausageSwitchIdentityButtonFont;
			}
			set {
				sausageSwitchIdentityButtonFont = value;
				IsDirty = true;
				RaiseThemeChanged (this);
			}
		}

		UIFont tabBarItemTitle;
		public UIFont TabBarItemTitle {
			get {
				return tabBarItemTitle;
			}
			set {
				tabBarItemTitle = value;
				IsDirty = true;
				RaiseThemeChanged (this);
			}
		}

		UIFont friendNameFont;
		public UIFont FriendNameFont {
			get {
				return friendNameFont;
			}
			set {
				friendNameFont = value;
				IsDirty = true;
				RaiseThemeChanged (this);
			}
		}

		UIFont lastMessageTextFont;
		public UIFont LastMessageTextFont {
			get {
				return lastMessageTextFont;
			}
			set {
				lastMessageTextFont = value;
				IsDirty = true;
				RaiseThemeChanged (this);
			}
		}

		UIFont dateMessageLabelFont;
		public UIFont DateMessageLabelFont {
			get {
				return dateMessageLabelFont;
			}
			set {
				dateMessageLabelFont = value;
				IsDirty = true;
				RaiseThemeChanged (this);
			}
		}

		UIFont messageFont;
		public UIFont MessageFont {
			get {
				return messageFont;
			}
			set {
				messageFont = value;
				IsDirty = true;
				RaiseThemeChanged (this);
			}
		}

		UIFont textTitleFont;
		public UIFont TextTitleFont {
			get {
				return textTitleFont;
			}
			set {
				textTitleFont = value;
				IsDirty = true;
				RaiseThemeChanged (this);
			}
		}

		UIFont badgeValueFont;
		public UIFont BadgeValueFont {
			get {
				return badgeValueFont;
			}
			set {
				badgeValueFont = value;
				IsDirty = true;
				RaiseThemeChanged (this);
			}
		}

		#endregion

		#region IImagesTheme implementation

		public UIImage ApplyEffects (UIImage image)
		{
			return imgTheme.ApplyEffects (image);
		}

		public UIImage SignUpIcon {
			get {
				return imgTheme.SignUpIcon;
			}
		}

		#endregion

		static Theme()
		{
			AvailableThemes = new List<Theme> ();
		}

		public Theme(IColorTheme colorTheme, IFontTheme fontTheme, IImagesTheme imgTheme)
		{
			this.colorTheme = colorTheme;
			this.fontTheme = fontTheme;
			this.imgTheme = imgTheme;

			ResetToDefaults ();
		}

		public void ResetToDefaults()
		{
			statusBarStyle = colorTheme.StatusBarStyle;
			screenTitleColor = colorTheme.ScreenTitleColor;

			mainColor = colorTheme.MainColor;
			mainSaturatedColor = colorTheme.MainSaturatedColor;

			disabledButtonColor = colorTheme.DisabledButtonColor;
			titleTextColor = colorTheme.TitleTextColor;
			descriptionDimmedColor = colorTheme.DescriptionDimmedColor;
			backgroundColor = colorTheme.BackgroundColor;
			navigationBarButtonColor = colorTheme.NavigationBarButtonColor;
			friendNameColor = colorTheme.FriendNameColor;
			lastMessageTextColor = colorTheme.LastMessageTextColor;
			dateMessageLabelColor = colorTheme.DateMessageLabelColor;
			conversationSelectedCellColor = colorTheme.ConversationSelectedCellColor;
			incomingBubbleStroke = colorTheme.IncomingBubbleStroke;
			incomingTextColor = colorTheme.IncomingTextColor;
			outgoingTextColor = colorTheme.OutgoingTextColor;
			badgeTitleColor = colorTheme.BadgeTitleColor;

			screenTitleFont = fontTheme.ScreenTitleFont;
			navigationButtonTitleFont = fontTheme.NavigationButtonTitleFont;
			sausageContrinueButtonFont = fontTheme.SausageContrinueButtonFont;
			sausageSwitchIdentityButtonFont = fontTheme.SausageSwitchIdentityButtonFont;
			tabBarItemTitle = fontTheme.TabBarItemTitle;
			friendNameFont = fontTheme.FriendNameFont;
			lastMessageTextFont = fontTheme.LastMessageTextFont;
			dateMessageLabelFont = fontTheme.DateMessageLabelFont;
			messageFont = fontTheme.MessageFont;
			textTitleFont = fontTheme.TextTitleFont;
			badgeValueFont = fontTheme.BadgeValueFont;

			IsDirty = false;
			RaiseThemeChanged (this);
		}

		public static void RaiseThemeChanged(object sender)
		{
			var handler = ThemeChanged;
			if (handler != null)
				handler (sender, EventArgs.Empty);
		}

		public static void SetNextTheme()
		{
			int curIndex = AvailableThemes.IndexOf (Current);
			curIndex = Math.Max (curIndex, 0);

			int next = (curIndex + 1) % AvailableThemes.Count;
			Current = AvailableThemes [next];
		}
	}
}

