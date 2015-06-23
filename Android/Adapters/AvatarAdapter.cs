
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace KinderChat
{
	class AvatarAdapter : BaseAdapter
	{

		Activity context;
		readonly ProfileViewModel viewModel;
		public AvatarAdapter(Activity context, ProfileViewModel viewModel)
		{
			this.context = context;
			this.viewModel = viewModel;
			viewModel.Avatars.CollectionChanged += (sender, e) => context.RunOnUiThread (NotifyDataSetChanged);

		}


		public override Java.Lang.Object GetItem(int position)
		{
			return position;
		}

		public override long GetItemId(int position)
		{
			return position;
		}
			
		public override View GetView(int position, View convertView, ViewGroup parent)
		{
			var view = convertView;
			AvatarAdapterViewHolder holder = null;

			if (view != null)
				holder = view.Tag as AvatarAdapterViewHolder;

			if (holder == null)
			{
				holder = new AvatarAdapterViewHolder();
				var inflater = context.GetSystemService(Context.LayoutInflaterService).JavaCast<LayoutInflater>();
				//replace with your item and your holder items
				//comment back in
				view = inflater.Inflate(Resource.Layout.item_avatar, parent, false);
				holder.Photo = view.FindViewById<ImageView>(Resource.Id.photo);
				view.Tag = holder;
			}




			var avatar = viewModel.Avatars [position];

			Koush.UrlImageViewHelper.SetUrlDrawable (holder.Photo, avatar.ImageUrl, Resource.Drawable.ic_launcher);

			return view;
		}
			
		public override int Count
		{
			get
			{
				return viewModel.Avatars.Count;
			}
		}

	}

	class AvatarAdapterViewHolder : Java.Lang.Object
	{
		public ImageView Photo { get; set; }
	}
}