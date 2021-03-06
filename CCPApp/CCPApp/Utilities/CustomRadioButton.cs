﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using CCPApp;

namespace CCPApp.Utilities
{
    public class CustomRadioButton : View
    {
        public static readonly BindableProperty CheckedProperty =
                   BindableProperty.Create<CustomRadioButton, bool>(
                       p => p.Checked, false);

        /// <summary>
        /// The default text property.
        /// </summary>
        public static readonly BindableProperty TextProperty =
            BindableProperty.Create<CustomRadioButton, string>(
                p => p.Text, string.Empty);

        /// <summary>
        /// The checked changed event.
        /// </summary>
        public EventHandler<EventArgs<bool>> CheckedChanged;


        /// <summary>
        /// Identifies the TextColor bindable property.
        /// </summary>
        /// 
        /// <remarks/>
        public static readonly BindableProperty TextColorProperty =
            BindableProperty.Create<CustomRadioButton, Color>(
                p => p.TextColor, Color.Black);

        /// <summary>
        /// Gets or sets a value indicating whether the control is checked.
        /// </summary>
        /// <value>The checked state.</value>
        public bool Checked
        {
            get
            {
                return this.GetValue<bool>(CheckedProperty);
            }

            set
            {
                this.SetValue(CheckedProperty, value);
                var eventHandler = this.CheckedChanged;
                if (eventHandler != null)
                {                   
                    eventHandler.Invoke(this, value);
                }
            }
        }

        public string Text
        {
            get
            {
                return this.GetValue<string>(TextProperty);
            }

            set
            {
                this.SetValue(TextProperty, value);
            }
        }

        public Color TextColor
        {
            get
            {
                return this.GetValue<Color>(TextColorProperty);
            }

            set
            {
                this.SetValue(TextColorProperty, value);
            }
        }

        new public int Id { get; set; }
    
		public void Highlight (bool highlight)
		{
			if(highlight)
			{
				Opacity = 1;
				BackgroundColor = Color.FromHex("#E0F7E0");
			}
			else
			{
				Opacity = 0.5;
				BackgroundColor = Color.White;
			}
		}
    }

  
}
