
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace KinderChat
{
	class ItemAdapter : BaseAdapter
	{

		Activity context;
		string[] names;
		int[] icons;
		public ItemAdapter(Activity context)
		{
			this.context = context;
			icons = new [] {
				Resource.Drawable.ic_brocolli,
				Resource.Drawable.ic_toothbrush,
				Resource.Drawable.ic_cleanedroom,
				Resource.Drawable.ic_homework
			};
			names = new[]{ "Ate Broccoli", "Brushed Teeth", "Cleaned Room", "Finished Homework" };
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
			TaskAdapterViewHolder holder = null;

			if (view != null)
				holder = view.Tag as TaskAdapterViewHolder;

			if (holder == null)
			{
				holder = new TaskAdapterViewHolder();
				var inflater = context.GetSystemService(Context.LayoutInflaterService).JavaCast<LayoutInflater>();
				//replace with your item and your holder items
				//comment back in
				view = inflater.Inflate(Resource.Layout.item_task, parent, false);
				holder.Name = view.FindViewById<TextView>(Resource.Id.task_name);
				holder.Photo = view.FindViewById<ImageView>(Resource.Id.task_photo);
				view.Tag = holder;
			}

			holder.Photo.SetImageResource (icons [position]);
			holder.Name.Text = names [position];

			return view;
		}
			
		public override int Count
		{
			get
			{
				return names.Length;
			}
		}

	}
	class TaskAdapterViewHolder : Java.Lang.Object
	{
		public TextView Name { get; set; }
		public ImageView Photo { get; set; }
	}

}