using System;

using UIKit;
using CoreGraphics;
using Foundation;

namespace KinderChat.iOS
{
	[Register("TaskCell")]
	public class TaskCell : UICollectionViewCell, IThemeable
	{
		public static readonly NSString CellId = new NSString ("TaskCell");

		static readonly CGSize IconMaxArea =  new CGSize(124, 124);
		static readonly nfloat SelectionRadius = 52;

		UIView iconContainer;
		UIView selectionCircle;
		UIImageView taskIcon;
		UILabel taskNameLbl;

		public UIImage TaskIcon {
			get {
				return taskIcon.Image;
			}
			set {
				taskIcon.Image = value;
			}
		}

		public string TaskName {
			get {
				return taskNameLbl.Text;
			}
			set {
				taskNameLbl.Text = value;
			}
		}

		public override bool Selected {
			get {
				return base.Selected;
			}
			set {
				base.Selected = value;
				selectionCircle.Hidden = !value;
			}
		}

		public TaskCell (IntPtr handle)
			: base(handle)
		{
			Initialize ();
		}

		public TaskCell ()
		{
			Initialize ();
		}

		void Initialize()
		{
			iconContainer = new UIView {
				TranslatesAutoresizingMaskIntoConstraints = false,
			};
			taskIcon = new UIImageView {
				TranslatesAutoresizingMaskIntoConstraints = false,
				ContentMode = UIViewContentMode.ScaleAspectFit,
			};
			taskNameLbl = new UILabel {
				TranslatesAutoresizingMaskIntoConstraints = false,
				TextAlignment = UITextAlignment.Center,
				Lines = 2,
			};

			iconContainer.AddSubview (taskIcon);
			ContentView.AddSubviews (iconContainer, taskNameLbl);

			iconContainer.Height (IconMaxArea.Height);
			taskIcon.CenterXY ();

			NSLayoutConstraint constraint = null;
			constraint = NSLayoutConstraint.Create (iconContainer, NSLayoutAttribute.Left, NSLayoutRelation.Equal, ContentView, NSLayoutAttribute.Left, 1, 10);
			ContentView.AddConstraint (constraint);
			constraint = NSLayoutConstraint.Create (iconContainer, NSLayoutAttribute.Right, NSLayoutRelation.Equal, ContentView, NSLayoutAttribute.Right, 1, -10);
			ContentView.AddConstraint (constraint);

			ContentView.AddConstraint (Layout.VerticalSpacing (iconContainer, taskNameLbl));
			taskNameLbl.CenterX ();
			ContentView.AddConstraints (Layout.PinLeftRightEdges(taskNameLbl, 10, 10));

			SetupSelectedView ();
		}

		public void SetCellSize (CGSize size)
		{
			taskNameLbl.PreferredMaxLayoutWidth = size.Width - 20;
		}

		public void ApplyCurrentTheme ()
		{
			taskNameLbl.Font = Theme.Current.TextTitleFont;
			taskNameLbl.TextColor = Theme.Current.TitleTextColor;
		}

		void SetupSelectedView ()
		{
			selectionCircle = new UIView {
				TranslatesAutoresizingMaskIntoConstraints = false,
				BackgroundColor = Theme.Current.MainColor,
				Hidden = true,
			};
			iconContainer.AddSubview (selectionCircle);
			iconContainer.SendSubviewToBack (selectionCircle);

			selectionCircle.CenterXY ();
			selectionCircle.Height (2 * SelectionRadius);
			selectionCircle.Width (2 * SelectionRadius);
			selectionCircle.Layer.CornerRadius = SelectionRadius;
		}

		public void AnimateIconAppearance ()
		{
			iconContainer.Alpha = 0;
			UIView.Animate (1, () => {
				iconContainer.Alpha = 1;
			});
		}
	}
}

