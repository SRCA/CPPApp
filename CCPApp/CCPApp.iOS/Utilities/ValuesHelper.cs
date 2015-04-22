using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using MonoTouch.Foundation;
using MonoTouch.UIKit;
using Xamarin.Forms;
using CCPApp.Utilities;
using CCPApp.iOS.Utilities;

[assembly: Dependency(typeof(ValuesHelper))]
namespace CCPApp.iOS.Utilities
{
	public class ValuesHelper : IValuesHelper
	{
		public string exportInstructions()
		{
			return "Please input the name you would like to give to the exported file in the box below.  " +
				"This file will be available to tranfer to a computer when you connect via iTunes";
		}

		public string deleteChecklistWarning(string title)
		{
			return "Are you sure you want to delete " + title + "?  All its data and inspections will be lost.  The only way to restore the checklist " +
				"will be to add it through iTunes again.";
		}
		public string outbriefingInstructions()
		{
			return "Please enter the name you would like to give this outbriefing file in the text box below.  "+
				"This file will be available to transfer to a computer via iTunes when you connect your device.  " +
				"To generate the PowerPoint outbriefing, please refer to the instructions provided separately.  "+
				"Should you require additional assistance, please contact SRCA at (334) 678-7722.";
		}
	}
}