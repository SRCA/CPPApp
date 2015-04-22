using CCPApp.Models;
using CCPApp.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using Xamarin.Forms;

namespace CCPApp.Views
{
	public class EditInspectionPage : ContentPage, IInspectorHolder
	{
		public TableSection tableSection { get; set; }
		public Inspection inspection { get; set; }
		public InspectionListPage CallingPage { get; set; }
		//public GenericPicker<Inspector> inspectorPicker { get; set; }
		public List<Inspector> selectedInspectors;
		public List<Inspector> availableInspectors;
		public ListView availableListView;
		public ListView selectedListView;
		public EditInspectionPage(Inspection existingInspection = null, ChecklistModel checklist = null)
		{
			/*ToolbarItem inspectorButton = new ToolbarItem();
			inspectorButton.Text = "Inspectors";
			inspectorButton.Clicked += InspectorHelper.openInspectorsPage;
			ToolbarItems.Add(inspectorButton);*/

			Padding = new Thickness(0, 5, 0, 0);
			Title = "Inspection Information";
			if (existingInspection == null)
			{
				inspection = new Inspection();
				inspection.Checklist = checklist;
				inspection.ChecklistId = checklist.Id;
				selectedInspectors = new List<Inspector>();
			}
			else
			{
				inspection = existingInspection;
				selectedInspectors = new List<Inspector>(inspection.inspectors);
			}
			TableView view = new TableView();
			TableRoot root = new TableRoot("Inspection Information");
			TableSection section = new TableSection();
			StackLayout layout = new StackLayout();

			Grid valuesGrid = new Grid
			{
				Padding = new Thickness(5,0,0,15),
				ColumnDefinitions =
				{
					new ColumnDefinition { Width = GridLength.Auto},
					new ColumnDefinition { Width = GridLength.Auto}
				},
				RowDefinitions =
				{
					new RowDefinition { Height = GridLength.Auto},
					new RowDefinition { Height = GridLength.Auto},
					new RowDefinition { Height = GridLength.Auto},
					new RowDefinition { Height = GridLength.Auto},
					new RowDefinition { Height = GridLength.Auto},
					new RowDefinition { Height = GridLength.Auto},
				}
			};
			int rowNumber = 0;
			valuesGrid.Children.Add(new Label { Text = "Inspection Name:" }, 0, rowNumber);

			/*StackLayout nameLayout = new StackLayout
			{
				Orientation = StackOrientation.Horizontal,
				Children = {
					new Label{
						Text = "Inspection Name:"
					}
				}
			};*/
			Entry nameEntry = new Entry();
			nameEntry.WidthRequest = 300;
			nameEntry.BindingContext = inspection;
			nameEntry.SetBinding(Entry.TextProperty, "Name");
			valuesGrid.Children.Add(nameEntry, 1, rowNumber);
			rowNumber++;
			//nameLayout.Children.Add(nameEntry);
			/*EntryCell NameCell = new EntryCell
			{
				BindingContext = inspection,
				Label = "Inspection Name:",
			};
			NameCell.SetBinding(EntryCell.TextProperty, "Name");

			EntryCell OrganizationCell = new EntryCell
			{
				BindingContext = inspection,
				Label = "Organization:",
			};
			OrganizationCell.SetBinding(EntryCell.TextProperty, "Organization");*/
			/*StackLayout orgLayout = new StackLayout
			{
				Orientation = StackOrientation.Horizontal,
				Children = {
					new Label{
						Text = "Organization:"
					}
				}
			};*/
			Entry orgEntry = new Entry();
			orgEntry.BindingContext = inspection;
			orgEntry.SetBinding(Entry.TextProperty, "Organization");
			orgEntry.WidthRequest = 300;
			//orgLayout.Children.Add(orgEntry);
			valuesGrid.Children.Add(new Label { Text = "Organization Inspected:" },0,rowNumber);
			valuesGrid.Children.Add(orgEntry, 1, rowNumber);
			rowNumber++;

			Entry PocEntry = new Entry();
			PocEntry.BindingContext = inspection;
			PocEntry.SetBinding(Entry.TextProperty, "Poc");
			PocEntry.WidthRequest = 300;
			valuesGrid.Children.Add(new Label { Text = "POC:" }, 0, rowNumber);
			valuesGrid.Children.Add(PocEntry, 1, rowNumber);
			rowNumber++;

			Entry PocPhoneEntry = new Entry();
			PocPhoneEntry.BindingContext = inspection;
			PocPhoneEntry.SetBinding(Entry.TextProperty, "PocPhone");
			PocPhoneEntry.WidthRequest = 300;
			valuesGrid.Children.Add(new Label { Text = "POC Phone:" }, 0, rowNumber);
			valuesGrid.Children.Add(PocPhoneEntry, 1, rowNumber);
			rowNumber++;

			/*StackLayout dateLayout = new StackLayout
			{
				Orientation = StackOrientation.Horizontal,
				Children = {
					new Label{
						Text = "Inspection Date: "
					}
				}
			};*/
			if (inspection.Date == null)
			{
				inspection.Date = DateTime.Now;
			}
			DatePicker datePicker = new DatePicker
			{
				BindingContext = inspection,
			};
			datePicker.SetBinding(DatePicker.DateProperty, "Date");
			//dateLayout.Children.Add(datePicker);
			valuesGrid.Children.Add(new Label { Text = "Inspection Date:" }, 0, rowNumber);
			valuesGrid.Children.Add(datePicker, 1, rowNumber);
			rowNumber++;

			if (inspection.CompletedDate == null)
			{
				inspection.CompletedDate = DateTime.Now;
			}
			DatePicker completedPicker = new DatePicker
			{
				BindingContext = inspection
			};
			completedPicker.SetBinding(DatePicker.DateProperty, "CompletedDate");
			valuesGrid.Children.Add(new Label { Text = "Date Completed:" }, 0, rowNumber);
			valuesGrid.Children.Add(completedPicker, 1, rowNumber);
			rowNumber++;

			/*nameLayout.Padding = new Thickness(5, 0, 0, 0);
			orgLayout.Padding = new Thickness(5, 0, 0, 0);
			dateLayout.Padding = new Thickness(5, 0, 0, 0);*/

			//inspectorPicker = new GenericPicker<Inspector>();
			availableInspectors = App.database.LoadAllInspectors();
			availableInspectors.Add(Inspector.Null);
			foreach (Inspector selectedInspector in selectedInspectors)
			{
				availableInspectors.RemoveAll(i => i.Id == selectedInspector.Id);
			}
			selectedInspectors.Add(Inspector.Null);
			//foreach (Inspector inspector in availableInspectors)
			//{
			//	inspectorPicker.AddItem(inspector);
			//}
			/*if (inspection.inspectors.Any())
			{
				try
				{
					inspectorPicker.SelectedItem = inspection.inspectors.First();
				}
				catch (KeyNotFoundException) { }
			}*/
			//ViewCell inspectorCell = new ViewCell { View = inspectorPicker };

			//ViewCell inspectorsCell = new ViewCell();
			StackLayout inspectorsLayout = new StackLayout
			{
				Orientation = StackOrientation.Horizontal,
				Padding = new Thickness(5,0),
			};
			StackLayout availableLayout = new StackLayout();
			Rectangle bounds = App.GetPageBounds();
			availableLayout.WidthRequest = (bounds.Width / 2) -  5;
			availableLayout.HeightRequest = 400;
			StackLayout selectedLayout = new StackLayout();
			selectedLayout.WidthRequest = (bounds.Width / 2) - 5;
			selectedLayout.HeightRequest = 400;
			Label availableLabel = new Label { Text = "Available Inspectors" };
			Label selectedLabel  = new Label { Text = "Selected Inspectors" };
			availableLayout.Children.Add(availableLabel);
			selectedLayout.Children.Add(selectedLabel);
			inspectorsLayout.Children.Add(availableLayout);
			inspectorsLayout.Children.Add(selectedLayout);
			//inspectorsCell.View = inspectorsLayout;

			availableListView = CreateInspectorsListView(availableInspectors);
			selectedListView = CreateInspectorsListView(selectedInspectors);
			availableLayout.Children.Add(availableListView);
			selectedLayout.Children.Add(selectedListView);

			//ViewCell SaveCell = new ViewCell();
			//ViewCell CancelCell = new ViewCell();
			Button saveButton = new Button();
			//Button cancelButton = new Button();
			saveButton.Clicked += SaveInspectionClicked;
			//cancelButton.Clicked += CancelInspectionClicked;
			saveButton.Text = "Save";
			//cancelButton.Text = "Cancel";
			//SaveCell.View = saveButton;
			//CancelCell.View = cancelButton;

			Button createInspectorsButton = new Button();
			createInspectorsButton.Text = "New Inspector";
			createInspectorsButton.Clicked += openCreateInspectorPage;

			//section.Add(NameCell);
			//section.Add(OrganizationCell);
			//section.Add(inspectorCell);
			//section.Add(inspectorsCell);
			//section.Add(SaveCell);

			/*layout.Children.Add(nameLayout);
			layout.Children.Add(orgLayout);
			layout.Children.Add(dateLayout);*/
			layout.Children.Add(valuesGrid);
			layout.Children.Add(inspectorsLayout);
			layout.Children.Add(createInspectorsButton);
			layout.Children.Add(saveButton);
			//section.Add(CancelCell);
			root.Add(section);
			view.Root = root;
			//view.Intent = TableIntent.Form;
			//view.HasUnevenRows = true;
			Content = layout;
			//Content = view;
		}

		public async void SaveInspectionClicked(object sender, EventArgs e)
		{
			ChecklistModel checklist = inspection.Checklist;
			//inspection.Name = NameCell.Text;
			inspection.ChecklistId = checklist.Id;
			if (!checklist.Inspections.Contains(inspection))
			{
				checklist.Inspections.Add(inspection);
			}
			//if (inspectorPicker.SelectedIndex >= 0)
			//{
			inspection.inspectors = selectedInspectors;
			inspection.inspectors.Remove(Inspector.Null);
				//inspection.inspectors.Add(inspectorPicker.SelectedItem);
			//}
			App.database.SaveInspection(inspection);

			CallingPage.ResetInspections();
			if (!App.Navigation.NavigationStack.Contains(CallingPage))
			{
				App.Navigation.InsertPageBefore(CallingPage, this);
			}

			await App.Navigation.PopAsync(true);
		}
		/*public async void CancelInspectionClicked(object sender, EventArgs e)
		{
			await App.Navigation.PopModalAsync(true);
		}*/
		public ListView CreateInspectorsListView(List<Inspector> inspectors)
		{
			ListView view = new ListView();
			UpdateInspectorListView(inspectors, view, this);
			return view;
		}
		public static void UpdateInspectorListView(List<Inspector> inspectors, ListView view, EditInspectionPage page)
		{
			view.ItemsSource = inspectors;
			view.ItemTemplate = new DataTemplate(() =>
			{
				InspectorButton button = new InspectorButton(page);
				button.SetBinding(InspectorButton.InspectorProperty, "SelfReference");
				button.SetBinding(InspectorButton.TextProperty, "Name");
				ViewCell cell = new ViewCell();
				cell.View = button;
				return cell;
			});
		}

		public async void openCreateInspectorPage(object sender, EventArgs e)
		{
			EditInspectorPage page = new EditInspectorPage();
			page.CallingPage = this;

			await App.Navigation.PushAsync(page);
		}
		public void ResetInspectors()
		{
			foreach (Inspector inspector in App.database.LoadAllInspectors())
			{
				if (!selectedInspectors.Any(i => i.Id == inspector.Id) && !availableInspectors.Any(i => i.Id == inspector.Id))
				{
					//There shouldn't be more than 1 new inspector at a time.
					availableInspectors.Insert(availableInspectors.Count - 1, inspector);
					EditInspectionPage.UpdateInspectorListView(availableInspectors, availableListView, this);
				}
			}
		}
	}
	internal class InspectorButton : Button
	{
		EditInspectionPage page;
		public InspectorButton(EditInspectionPage page)
		{
			this.page = page;
			Clicked += InspectorButton_Clicked;
		}

		void InspectorButton_Clicked(object sender, EventArgs e)
		{
			Inspector inspector = ((InspectorButton)sender).inspector;
			if (inspector == Inspector.Null)
			{
				return;
			}
			List<Inspector> selected = page.selectedInspectors;
			List<Inspector> available = page.availableInspectors;
			if (available.Contains(inspector))
			{
				available.Remove(inspector);
				//selected.Add(inspector);
				selected.Insert(selected.Count - 1, inspector);
			}
			else if (page.selectedInspectors.Contains(inspector))
			{
				selected.Remove(inspector);
				//available.Add(inspector);
				available.Insert(available.Count - 1, inspector);
			}
			else
			{	//ya dun goofed.
				throw new DataMisalignedException();
			}
			EditInspectionPage.UpdateInspectorListView(selected, page.selectedListView, page);
			EditInspectionPage.UpdateInspectorListView(available, page.availableListView, page);
			//page.selectedListView = EditInspectionPage.CreateInspectorsListView(page.selectedInspectors, page);
			//page.availableListView = EditInspectionPage.CreateInspectorsListView(page.availableInspectors, page);
		}
		public Inspector inspector
		{
			get
			{
				return (Inspector)GetValue(InspectorProperty);
			}
			set
			{
				SetValue(InspectorProperty, value);
			}
		}

		public static readonly BindableProperty InspectorProperty =
			BindableProperty.Create<InspectorButton, Inspector>
			(p => p.inspector, null);
	}
}
