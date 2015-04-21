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
		public ChecklistMenuPage()
		{
			Title = "Checklist Options";

			TableView view = new TableView();
			TableRoot root = new TableRoot();
			TableSection section = new TableSection();

			//New Inspection
			ViewCell newInspectionCell = new ViewCell();
			Label newInspectionLabel = new Label();
			newInspectionLabel.Text = "Breate a New Inspection";
			newInspectionCell.View = newInspectionLabel;

			//Existing Inspection
			ViewCell existingInspectionCell = new ViewCell();
			Label existingInspectionLabel = new Label();
			existingInspectionLabel.Text = "Breate a New Inspection";
			existingInspectionCell.View = existingInspectionLabel;

			//View References
			ViewCell viewReferencesCell = new ViewCell();
			Label viewReferencesLabel = new Label();
			newInspectionLabel.Text = "Breate a New Inspection";
			viewReferencesCell.View = viewReferencesLabel;

			//Full Reference List
			ViewCell referenceListCell = new ViewCell();
			Label referenceListLabel = new Label();
			referenceListLabel.Text = "Breate a New Inspection";
			referenceListCell.View = referenceListLabel;

			section.Add(newInspectionCell);
			section.Add(existingInspectionCell);
			section.Add(viewReferencesCell);
			section.Add(referenceListCell);

			root.Add(section);
			view.Root = root;

			Content = view;
		}
	}
}