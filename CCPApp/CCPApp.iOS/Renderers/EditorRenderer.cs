using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using MonoTouch.Foundation;
using MonoTouch.UIKit;
using Xamarin.Forms.Platform.iOS;
using Xamarin.Forms;
using CCPApp.iOS.Renderers;
using MonoTouch.CoreGraphics;
using MonoTouch.CoreAnimation;

[assembly: ExportRenderer(typeof(Editor), typeof(OutlineEditorRenderer))]
namespace CCPApp.iOS.Renderers
{
	public class OutlineEditorRenderer : EditorRenderer
	{
		protected override void OnElementChanged(ElementChangedEventArgs<Editor> e)
		{
			base.OnElementChanged(e);
			if (e.OldElement == null && Control != null)
			{
				CALayer layer = Control.Layer;
				layer.BorderWidth = 1F;
				layer.BorderColor = UIColor.Black.CGColor;
				layer.CornerRadius = 8;
			}
		}
	}
}