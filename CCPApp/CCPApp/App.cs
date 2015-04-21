using CCPApp.Items;
using CCPApp.Models;
using CCPApp.Utilities;
using CCPApp.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xamarin.Forms;

namespace CCPApp
{
	public class App : Application
	{
		public static Color VisualElementIsFocusedColor { get; private set; }
		public static INavigation Navigation;
		public static DatabaseAccess database = new DatabaseAccess();
		public App()
		{
			List<ChecklistModel> checklists = new List<ChecklistModel>();

			checklists.AddRange(database.LoadAllChecklists());

			FrontPage frontPage = new FrontPage(checklists);

			MainPage = new NavigationPage(frontPage);
			Navigation = MainPage.Navigation;

			// Create styles for controls
			VisualElementIsFocusedColor = Color.FromHex("#E0F7E0");

			var editorStyle = new Style(typeof(Editor))
			{
				Triggers = {
					new Trigger(typeof(Editor)) 
					{ 
						Property = Editor.IsFocusedProperty, Value = true, 
						Setters = { 
							new Setter { Property = Editor.BackgroundColorProperty, Value = VisualElementIsFocusedColor }
						}
					}
				}
			};
			
			var datePickerStyle = new Style(typeof(DatePicker))
			{
				Triggers = {
					new Trigger(typeof(DatePicker)) 
					{ 
						Property = DatePicker.IsFocusedProperty, Value = true, 
						Setters = { 
							new Setter { Property = DatePicker.BackgroundColorProperty, Value = VisualElementIsFocusedColor }
						}
					}
				}
			};

			var entryStyle = new Style(typeof(Entry))
			{
				Triggers = {
					new Trigger(typeof(Entry)) 
					{ 
						Property = Entry.IsFocusedProperty, Value = true, 
						Setters = { 
							new Setter { Property = Entry.BackgroundColorProperty, Value = VisualElementIsFocusedColor }
						}
					}
				}
			};

			// Add styles to resource dictionary. Will be used by entire application.
			var resourceDictionary = new ResourceDictionary();
			resourceDictionary.Add(editorStyle);
			resourceDictionary.Add(datePickerStyle);
			resourceDictionary.Add(entryStyle);

			Application.Current.Resources = resourceDictionary;
		}
		public static Rectangle GetPageBounds()
		{
			return Navigation.NavigationStack.First().Bounds;
		}

		public static bool IsPortrait(Page page)
		{
			return page.Width < page.Height;
		}
	}
}
