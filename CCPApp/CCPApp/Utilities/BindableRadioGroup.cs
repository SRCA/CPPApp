using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;


namespace CCPApp.Utilities
{
    public class BindableRadioGroup: StackLayout
    {
       
        public List<CustomRadioButton> rads;

        public BindableRadioGroup()
        {

            rads = new List<CustomRadioButton>();
        }



        public static BindableProperty ItemsSourceProperty =
            BindableProperty.Create<BindableRadioGroup, IEnumerable>(o => o.ItemsSource, default(IEnumerable), propertyChanged: OnItemsSourceChanged);

     
        public static BindableProperty SelectedIndexProperty =
            BindableProperty.Create<BindableRadioGroup, int>(o => o.SelectedIndex, -1, BindingMode.TwoWay, propertyChanged:OnSelectedIndexChanged );

        public IEnumerable ItemsSource
        {
            get { return (IEnumerable)GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }

       
        public int SelectedIndex
        {
            get { return (int)GetValue(SelectedIndexProperty); }
            set { SetValue(SelectedIndexProperty, value); }
        }
      
        public event EventHandler<int> CheckedChanged;
		public event EventHandler<int> ItemUnchecked;
	

        private static void OnItemsSourceChanged(BindableObject bindable, IEnumerable oldvalue, IEnumerable newvalue)
        {
            var radButtons = bindable as BindableRadioGroup;
           
            radButtons.rads.Clear();
            radButtons.Children.Clear();
            if (newvalue != null)
            {
                int radIndex = 0;
                foreach (var item in newvalue)
                {
                    var rad = new CustomRadioButton();
                    rad.Text = item.ToString();
                    rad.Id = radIndex;

                    rad.CheckedChanged += radButtons.OnCheckedChanged;
					rad.Highlight(false);

                    radButtons.rads.Add(rad);
                                    
                    radButtons.Children.Add(rad);
                    radIndex++;
                }
            }
        }

        private void OnCheckedChanged(object sender, EventArgs<bool> e)
        {
			var selectedRad = sender as CustomRadioButton;

			if(e.Value == false)
			{
				if (ItemUnchecked != null && rads.All(x => x.Checked == false))
				{
					selectedRad.Highlight(false);
					ItemUnchecked.Invoke(sender, 0);
				}
				
				return;
			}

			foreach (var rad in rads)
            {
                if(!selectedRad.Id.Equals(rad.Id))
                {
                    rad.Checked = false;
					rad.Highlight(false);
                }
                else
                {
					rad.Highlight(true);
					if(CheckedChanged != null)
                    	CheckedChanged.Invoke(sender, rad.Id); 
                }
                
            }

        }

        private static void OnSelectedIndexChanged(BindableObject bindable, int oldvalue, int newvalue)
        {
            if (newvalue == -1) return;

            var bindableRadioGroup = bindable as BindableRadioGroup;


            foreach (var rad in bindableRadioGroup.rads)
            {
                if (rad.Id == bindableRadioGroup.SelectedIndex)
                {
                    rad.Checked = true;
                }
               
            }


        }
    
    }
}
