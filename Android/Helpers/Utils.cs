using System;
using Android.Graphics;
using Android.Graphics.Drawables;

namespace KinderChat
{
	public static class Utils
	{

		public static Bitmap DrawableToBitmap(Drawable drawable)
		{
			if (drawable is BitmapDrawable)
			{
				return ((BitmapDrawable)drawable).Bitmap;
			}

			if (drawable.IntrinsicHeight == -1 || drawable.IntrinsicHeight == -1)
				return null;

			Bitmap bitmap = Bitmap.CreateBitmap(drawable.IntrinsicWidth, drawable.IntrinsicHeight, Bitmap.Config.Argb8888);
			Canvas canvas = new Canvas(bitmap);
			drawable.SetBounds(0, 0, canvas.Width, canvas.Height);
			drawable.Draw(canvas);

			return bitmap;
		}
	}
}

