using CCPApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using Xamarin.Forms;

namespace CCPApp.Views
{
	public class ReferenceListPage : ContentPage
	{
		ChecklistModel checklist;
		public ReferenceListPage(ChecklistModel checklist)
		{
			this.checklist = checklist;
			Title = "Reference List";

			TableView view = new TableView();
			TableRoot root = new TableRoot();
			TableSection section = new TableSection();

			List<Reference> references = new List<Reference>();

			foreach (List<Reference> questionReferences in checklist.GetAllQuestions().Select(q => q.References))
			{
				references.AddRange(questionReferences);
			}
			//folderName + "/" + reference.DocumentName;
			IEnumerable<string> fileNames = references.Select(r => r.DocumentName).Distinct();
			foreach (string fileName in fileNames)
			{
				ViewCell cell = new ViewCell();
				StackLayout layout = new StackLayout();
				layout.Padding = new Thickness(10, 0, 0, 0);

				Button button = new Button();
				button.Text = removeFileExtension(fileName);
				button.HorizontalOptions = LayoutOptions.StartAndExpand;
				button.Clicked += async (object Sender, EventArgs e) =>
				{
					string folderName = checklist.Id;
					string relativePath = folderName + "/" + fileName;
					ReferencePage page = new ReferencePage(relativePath, 1);
					await App.Navigation.PushAsync(page);
				};

				layout.Children.Add(button);
				cell.View = layout;
				section.Add(cell);
			}

			root.Add(section);
			view.Root = root;
			Content = view;
		}
		
		static string removeFileExtension(string file)
		{
			int lastDotIndex = file.LastIndexOf(".");
			return file.Substring(0, lastDotIndex);
		}
	}
}
