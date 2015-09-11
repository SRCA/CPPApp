using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Foundation;
using UIKit;
using Xamarin.Forms.Platform.iOS;
using Xamarin.Forms;
using CCPApp.iOS.Renderers;
using CoreGraphics;
using CoreAnimation;

[assembly: ExportRenderer(typeof(DatePicker), typeof(OutlineDatePickerRenderer))]
namespace CCPApp.iOS.Renderers
{
	public class OutlineDatePickerRenderer : DatePickerRenderer
	{
		protected override void OnElementChanged(ElementChangedEventArgs<DatePicker> e)
		{
			base.OnElementChanged(e);
			if (e.OldElement == null && Control != null)
			{
				CALayer layer = Control.Layer;
				layer.BorderWidth = (nfloat)1;
				layer.BorderColor = UIColor.Black.CGColor;
				layer.CornerRadius = (nfloat)8;
			}
		}
	}
}