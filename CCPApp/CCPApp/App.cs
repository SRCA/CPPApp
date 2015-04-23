using CCPApp.Items;
using CCPApp.Models;
using CCPApp.Utilities;
using CCPApp.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

			//Begin embedded checklist code
			ChecklistMenuPage menuPage;
			if (checklists.Any())
			{//We no longer support multiple checklists.
				menuPage = new ChecklistMenuPage(checklists.First());
			}
			else
			{
				menuPage = new ChecklistMenuPage(ParseChecklist());
			}
			MainPage = new NavigationPage(menuPage);
			Navigation = MainPage.Navigation;
			//End embedded checklist code  Replace with instantiating FrontPage.

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

		private ChecklistModel ParseChecklist()
		{
			List<ChecklistModel> checklists = new List<ChecklistModel>();
			IEnumerable<string> zipFileNames = DependencyService.Get<IFileManage>().GetAllValidFiles();
			if (zipFileNames.Any())
			{
				List<ChecklistModel> newChecklists = new List<ChecklistModel>();
				foreach (string zipName in zipFileNames)
				{
					string unzippedDirectory = DependencyService.Get<IUnzipHelper>().Unzip(zipName);
					string xmlFile = DependencyService.Get<IFileManage>().GetXmlFile(unzippedDirectory);
					string checklistId = DependencyService.Get<IParseChecklist>().GetChecklistId(xmlFile);
					ChecklistModel model = ChecklistModel.Initialize(xmlFile);
					//move the files to a new folder.
					//DependencyService.Get<IFileManage>().MoveDirectoryToPrivate(unzippedDirectory, checklistId);
					newChecklists.Add(model);
					checklists.Add(model);
				}
				Task.Run(async () =>
				{
					await App.database.SaveChecklists(newChecklists);
				});
			}
			return checklists.Single();
		}
	}
}
