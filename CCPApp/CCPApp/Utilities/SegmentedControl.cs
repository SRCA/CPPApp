﻿using System;
using Xamarin.Forms;
using System.Collections.Generic;
using System.Linq;

namespace CCPApp.Utilities
{
	public class SegmentedControl : View, IViewContainer<SegmentedControlOption>
	{
		public IList<SegmentedControlOption> Children { get; set; }

		public SegmentedControl ()
		{
			Children = new List<SegmentedControlOption> ();
		}

		public event ValueChangedEventHandler ValueChanged;

		public delegate void ValueChangedEventHandler (object sender, EventArgs e);

		private string selectedValue;

		public string SelectedValue {
			get{ return selectedValue; }
			set {
				selectedValue = value;
				if (ValueChanged != null)
					ValueChanged (this, EventArgs.Empty);
			}
		}

		private int selectedIndex;
		public int SelectedIndex
		{
			get { return selectedIndex; }
			set
			{
				selectedIndex = value;
				if (ValueChanged != null)
					ValueChanged(this, EventArgs.Empty);
			}
		}
	}

	public class SegmentedControlOption:View
	{
		public static readonly BindableProperty TextProperty = BindableProperty.Create<SegmentedControlOption, string>(p => p.Text, "");
		
		public string Text {
			get{ return (string)GetValue (TextProperty); }
			set{ SetValue (TextProperty, value); }
		}

		public SegmentedControlOption ()
		{
		}
	}
}