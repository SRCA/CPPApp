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
		public static INavigation Navigation;
		public static DatabaseAccess database = new DatabaseAccess();
		public App()
		{
			List<ChecklistModel> checklists = new List<ChecklistModel>();

			checklists.AddRange(database.LoadAllChecklists());

			FrontPage frontPage = new FrontPage(checklists);

			MainPage = new NavigationPage(frontPage);
			Navigation = MainPage.Navigation;
		}
		public static Rectangle GetPageBounds()
		{
			return Navigation.NavigationStack.First().Bounds;
		}
	}
}
