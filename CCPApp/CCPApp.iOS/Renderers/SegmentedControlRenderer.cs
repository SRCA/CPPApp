using System;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;
using MonoTouch.UIKit;
using CCPApp.Utilities;
using CCPApp.iOS.Renderers;


[assembly:ExportRenderer(typeof(SegmentedControl), typeof(SegmentedControlRenderer))]
namespace CCPApp.iOS.Renderers
{
	public class SegmentedControlRenderer : ViewRenderer<SegmentedControl, UISegmentedControl>
	{
		public SegmentedControlRenderer ()
		{
		}

		protected override void OnElementChanged (ElementChangedEventArgs<SegmentedControl> e)
		{
			base.OnElementChanged (e);

			var segmentedControl = new UISegmentedControl ();

			for (var i = 0; i < e.NewElement.Children.Count; i++) {
				segmentedControl.InsertSegment (e.NewElement.Children [i].Text, i, false);
			}

			segmentedControl.ValueChanged += (sender, eventArgs) => {
				var selectedSegment = segmentedControl.SelectedSegment;

				e.NewElement.SelectedValue = segmentedControl.TitleAt(selectedSegment);
				e.NewElement.SelectedIndex = selectedSegment;
			};

			SetNativeControl (segmentedControl);
		}
	}
}