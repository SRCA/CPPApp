using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace CCPApp.Utilities
{
	public class LayoutHelper
	{
		public static BoxView GetHorizontalLine()
		{
			return new BoxView
			{
				HeightRequest = 1,
				Color = Color.Black
			};
		}

		public static BoxView GetVerticalSpacing(double heightRequest)
		{
			return new BoxView
			{
				HeightRequest = heightRequest
			};
		}

		public static BoxView GetHorizontalSpacing(double widthRequest)
		{
			return new BoxView
			{
				WidthRequest = widthRequest
			};
		}

	}
}
