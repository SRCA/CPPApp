using CCPApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using Xamarin.Forms;

namespace CCPApp.Views
{
	public class ChecklistMenuPage : ContentPage
	{
		ChecklistModel checklist;
		public ChecklistMenuPage(ChecklistModel checklist)
		{
			this.checklist = checklist;
			Title = "Checklist Options";

			TableView view = new TableView();
			TableRoot root = new TableRoot();
			TableSection section = new TableSection();
			view.Intent = TableIntent.Menu;

			//New Inspection
			ViewCell newInspectionCell = new ViewCell();
			StackLayout newInspectionLayout = new StackLayout { Padding = new Thickness(20, 0, 0, 0) };
			CreateInspectionButton newInspectionButton = new CreateInspectionButton(checklist);
			newInspectionButton.HorizontalOptions = LayoutOptions.StartAndExpand;
			newInspectionButton.Clicked += InspectionHelper.CreateInspectionButtonClicked;
			newInspectionButton.Text = "Begin a New Inspection";
			newInspectionLayout.Children.Add(newInspectionButton);
			newInspectionCell.View = newInspectionLayout;

			//Existing Inspection
			ViewCell existingInspectionCell = new ViewCell();
			StackLayout existingInspectionLayout = new StackLayout { Padding = new Thickness(20, 0, 0, 0) };
			ChecklistButton existingInspectionButton = new ChecklistButton();
			existingInspectionButton.HorizontalOptions = LayoutOptions.StartAndExpand;
			existingInspectionButton.checklist = checklist;
			existingInspectionButton.Text = "Continue an Existing Inspection";
			existingInspectionButton.Clicked += ChecklistHelper.InspectionListButtonClicked;
			existingInspectionLayout.Children.Add(existingInspectionButton);
			existingInspectionCell.View = existingInspectionLayout;

			//View References
			ViewCell viewReferencesCell = new ViewCell();
			StackLayout viewReferencesLayout = new StackLayout { Padding = new Thickness(20, 0, 0, 0) };
			ChecklistButton viewReferencesButton = new ChecklistButton();
			viewReferencesButton.HorizontalOptions = LayoutOptions.StartAndExpand;
			viewReferencesButton.checklist = checklist;
			viewReferencesButton.Text = "View References";
			viewReferencesButton.Clicked += ChecklistHelper.ReferenceListButtonClicked;
			viewReferencesLayout.Children.Add(viewReferencesButton);
			viewReferencesCell.View = viewReferencesLayout;

			//Full Reference List
			/*ViewCell referenceListCell = new ViewCell();
			Label referenceListLabel = new Label();
			referenceListLabel.Text = "View Full Reference List";
			referenceListCell.View = referenceListLabel;*/

			section.Add(newInspectionCell);
			section.Add(existingInspectionCell);
			section.Add(viewReferencesCell);
			//section.Add(referenceListCell);

			root.Add(section);
			view.Root = root;

			Content = view;
		}
	}
}