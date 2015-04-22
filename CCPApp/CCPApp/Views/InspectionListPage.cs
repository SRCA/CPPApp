using CCPApp.Models;
using CCPApp.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace CCPApp.Views
{
	public class InspectionListPage : ContentPage
	{
		ChecklistModel checklist;
		//public string ChecklistTitle { get; set; }
		public InspectionListPage(ChecklistModel checklist)
		{
			ToolbarItem inspectorButton = new ToolbarItem();
			inspectorButton.Text = "Inspectors";
			inspectorButton.Clicked += InspectorHelper.openInspectorsPage;
			ToolbarItems.Add(inspectorButton);
			Title = "Inspection Listing";
			NavigationPage.SetBackButtonTitle(this, "Inspection Listing");
			this.checklist = checklist;
			ResetInspections();
		}
		public void ResetInspections()
		{
			TableView view = new TableView();
			view.Intent = TableIntent.Menu;
			TableRoot root = new TableRoot("Inspections list");
			TableSection section = new TableSection();
			List<ViewCell> cells = new List<ViewCell>();
			double columnWidth = (App.GetPageBounds().Width - 20) / 3;

			if (checklist.Inspections.Count > 0)
			{	//Header cell
				ViewCell cell = new ViewCell();
				Grid grid = new Grid();
				grid.Padding = new Thickness(20, 0, 0, 0);
				grid.VerticalOptions = LayoutOptions.Center;
				grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
				grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(columnWidth * 2, GridUnitType.Absolute) });
				grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(columnWidth / 2, GridUnitType.Absolute) });
				grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(columnWidth / 2, GridUnitType.Absolute) });

				grid.Children.Add(new Label
				{
					Text = "Description",
					HorizontalOptions = LayoutOptions.StartAndExpand,
					VerticalOptions = LayoutOptions.Center
				}, 0, 0);
				grid.Children.Add(new Label
				{
					Text = "Unit",
					HorizontalOptions = LayoutOptions.StartAndExpand,
					VerticalOptions = LayoutOptions.Center
				}, 1, 0);
				grid.Children.Add(new Label
				{
					Text = "Date Started",
					HorizontalOptions = LayoutOptions.StartAndExpand,
					VerticalOptions = LayoutOptions.Center
				}, 2, 0);
				cell.View = grid;
				cells.Add(cell);
			}

			foreach (Inspection inspection in checklist.Inspections)
			{
				ViewCell cell = new ViewCell();
				BoundMenuItem<Inspection> Edit = new BoundMenuItem<Inspection> { Text = "Edit", BoundObject = inspection };
				BoundMenuItem<Inspection> Delete = new BoundMenuItem<Inspection> { Text = "Delete", BoundObject = inspection, IsDestructive = true };
				Edit.Clicked += openEditPage;
				Delete.Clicked += deleteInspection;
				cell.ContextActions.Add(Edit);
				cell.ContextActions.Add(Delete);

				//Button button = new InspectionButton(inspection);
				//button.Clicked += InspectionHelper.SelectInspectionButtonClicked;
				//cell.View = button;

				InspectionGrid grid = new InspectionGrid(inspection);
				TapGestureRecognizer recognizer = new TapGestureRecognizer();
				recognizer.Tapped += InspectionHelper.SelectInspectionButtonClicked;
				grid.GestureRecognizers.Add(recognizer);
				grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
				
				grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(columnWidth * 2, GridUnitType.Absolute) });
				grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(columnWidth / 2, GridUnitType.Absolute) });
				grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(columnWidth / 2, GridUnitType.Absolute) });

				InspectionButton nameButton = new InspectionButton(inspection);
				nameButton.Text = inspection.Name;
				nameButton.HorizontalOptions = LayoutOptions.StartAndExpand;
				nameButton.Clicked += InspectionHelper.SelectInspectionButtonClicked;
				InspectionButton orgButton = new InspectionButton(inspection);
				orgButton.Text = inspection.Organization;
				orgButton.HorizontalOptions = LayoutOptions.StartAndExpand;
				orgButton.Clicked += InspectionHelper.SelectInspectionButtonClicked;
				InspectionButton dateButton = new InspectionButton(inspection);
				dateButton.HorizontalOptions = LayoutOptions.StartAndExpand;
				dateButton.Clicked += InspectionHelper.SelectInspectionButtonClicked;
				if (inspection.Date == null)
					dateButton.Text = string.Empty;
				else
					dateButton.Text = ((DateTime)inspection.Date).ToString("MM/dd/yyyy");

				grid.Children.Add(
					nameButton,
					0,0);
				grid.Children.Add(
					orgButton,
					1, 0);
				grid.Children.Add(
					dateButton,
					2, 0);
				grid.Padding = new Thickness(20, 0, 0, 0);

				cell.View = grid;
				cells.Add(cell);
			}
			CreateInspectionButton createInspectionButton = new CreateInspectionButton(checklist);
			createInspectionButton.Text = "Begin a New Inspection";
			createInspectionButton.Clicked += InspectionHelper.CreateInspectionButtonClicked;
			ViewCell createInspectionButtonView = new ViewCell();
			createInspectionButtonView.View = createInspectionButton;

			section.Add(cells);
			section.Add(createInspectionButtonView);
			root.Add(section);
			view.Root = root;

			Content = view;
		}

		public async void openEditPage(object sender, EventArgs e)
		{
			BoundMenuItem<Inspection> button = (BoundMenuItem<Inspection>)sender;
			Inspection inspection = button.BoundObject;
			EditInspectionPage page = new EditInspectionPage(inspection);
			page.CallingPage = this;
			await App.Navigation.PushAsync(page);
		}
		public async void deleteInspection(object sender, EventArgs e)
		{
			//Warn that this is permanent.  Ask if they're sure.
			BoundMenuItem<Inspection> button = (BoundMenuItem<Inspection>)sender;
			Inspection inspection = button.BoundObject;
			bool answer = await DisplayAlert("Confirm Deletion", "Are you sure you want to delete "+inspection.Name+"?  All its data and scores will be lost.", "Yes", "No");
			if (!answer)
			{
				return;
			}			
			Inspection.DeleteInspection(inspection);
			if (checklist.Inspections.Contains(inspection))
			{	//This is supposed to be removed in the DeleteInspection method, but there appear to be multiple copies of som
					//objects in memory.
				checklist.Inspections.Remove(inspection);
			}
			ResetInspections();
		}
	}
}
