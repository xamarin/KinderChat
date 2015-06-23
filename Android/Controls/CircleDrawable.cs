using Android.Graphics;
using Android.Graphics.Drawables;
using System;

namespace KinderChat
{
	public class CircleDrawable : Drawable
	{
		Bitmap bmp;
		BitmapShader bmpShader;
		Paint paint;
		RectF oval;

		public CircleDrawable (Bitmap bmp)
		{
			this.bmp = bmp;
			bmpShader = new BitmapShader (bmp, Shader.TileMode.Clamp, Shader.TileMode.Clamp);
			paint = new Paint { AntiAlias = true };
			paint.SetShader (bmpShader);
			oval = new RectF ();
		}

		public override void Draw (Canvas canvas)
		{
			canvas.DrawOval (oval, paint);
		}

		protected override void OnBoundsChange (Rect bounds)
		{
			base.OnBoundsChange (bounds);
			if (oval == null)
				return;
			
			oval.Set (0, 0, bounds.Width (), bounds.Height ());
		}

		public override int IntrinsicWidth {
			get {
				if (bmp == null)
					return 0;

				var min = Math.Min (bmp.Width, bmp.Height);
				return min;
			}
		}

		public override int IntrinsicHeight {
			get {

				if (bmp == null)
					return 0;

				var min = Math.Min (bmp.Width, bmp.Height);
				return min;
			}
		}

		public override void SetAlpha (int alpha)
		{
		}

		public override int Opacity {
			get {
				return (int)Format.Opaque;
			}
		}

		public override void SetColorFilter (ColorFilter cf)
		{

		}
	}
}