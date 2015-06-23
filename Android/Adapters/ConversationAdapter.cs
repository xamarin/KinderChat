using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using KinderChat.Converters;

namespace KinderChat
{
	class ConverstationAdapter : BaseAdapter
	{

		Activity context;
		readonly ConversationsViewModel viewModel;
		public ConverstationAdapter(Activity context, ConversationsViewModel viewModel)
		{
			this.context = context;
			this.viewModel = viewModel;
			viewModel.Conversations.CollectionChanged += (sender, e) => context.RunOnUiThread (NotifyDataSetChanged);

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
			ConverstationAdapterViewHolder holder = null;

			if (view != null)
				holder = view.Tag as ConverstationAdapterViewHolder;

			if (holder == null)
			{
				holder = new ConverstationAdapterViewHolder();
				var inflater = context.GetSystemService(Context.LayoutInflaterService).JavaCast<LayoutInflater>();
				//replace with your item and your holder items
				//comment back in
				view = inflater.Inflate(Resource.Layout.item_conversation, parent, false);
				holder.Name = view.FindViewById<TextView>(Resource.Id.contact_name);
				holder.Message = view.FindViewById<TextView>(Resource.Id.subject);
				holder.Date = view.FindViewById<TextView>(Resource.Id.date);
				holder.Photo = view.FindViewById<ImageView>(Resource.Id.contact_photo);
				view.Tag = holder;
			}

			var conversation = viewModel.Conversations [position];

            holder.Name.Text = conversation.FriendName;
            holder.Message.Text = conversation.Thumbnail != null && conversation.Thumbnail.Length > 0 ? "Image message" : conversation.Text;  
			holder.Date.Text = new TimestampConverter().Convert(conversation.Date);
			Koush.UrlImageViewHelper.SetUrlDrawable (holder.Photo, conversation.FriendPhoto, Resource.Drawable.default_photo);

			return view;
		}
			
		public override int Count
		{
			get
			{
				return viewModel.Conversations.Count;
			}
		}

	}

	class ConverstationAdapterViewHolder : Java.Lang.Object
	{
		public TextView Name { get; set; }
		public TextView Message { get; set; }
		public TextView Date { get; set; }
		public ImageView Photo { get; set; }
	}
}