
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace KinderChat
{
	class FriendAdapter : BaseAdapter
	{

		Activity context;
		readonly FriendsViewModel viewModel;
		public FriendAdapter(Activity context, FriendsViewModel viewModel)
		{
			this.context = context;
			this.viewModel = viewModel;
			viewModel.Friends.CollectionChanged += (sender, e) => context.RunOnUiThread (NotifyDataSetChanged);

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
			FriendAdapterViewHolder holder = null;

			if (view != null)
				holder = view.Tag as FriendAdapterViewHolder;

			if (holder == null)
			{
				holder = new FriendAdapterViewHolder();
				var inflater = context.GetSystemService(Context.LayoutInflaterService).JavaCast<LayoutInflater>();
				//replace with your item and your holder items
				//comment back in
				view = inflater.Inflate(Resource.Layout.item_friend, parent, false);
				holder.Name = view.FindViewById<TextView>(Resource.Id.contact_name);
				holder.Photo = view.FindViewById<ImageView>(Resource.Id.contact_photo);
				view.Tag = holder;
			}




			var friend = viewModel.Friends [position];


			holder.Name.Text = friend.Name;
			Koush.UrlImageViewHelper.SetUrlDrawable (holder.Photo, friend.Photo, Resource.Drawable.default_photo);

			return view;
		}
			
		public override int Count
		{
			get
			{
				return viewModel.Friends.Count;
			}
		}

	}

	class FriendAdapterViewHolder : Java.Lang.Object
	{
		public TextView Name { get; set; }
		public ImageView Photo { get; set; }
	}
}