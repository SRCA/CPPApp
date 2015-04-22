﻿using CCPApp.Models;
using CCPApp.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using Xamarin.Forms;

namespace CCPApp.Views
{
	public class InspectorsPage : ContentPage, IInspectorHolder
	{
		public InspectorsPage()
		{
			Title = "Inspectors";
			ResetInspectors();
		}

		public void ResetInspectors()
		{
			TableView inspectorsView = new TableView();
			inspectorsView.Intent = TableIntent.Menu;
			TableRoot root = new TableRoot();
			TableSection section = new TableSection();
			List<ViewCell> cells = new List<ViewCell>();

			foreach (Inspector inspector in App.database.LoadAllInspectors())
			{
				Button button = new Button();
				button.Text = inspector.Name;
				ViewCell cell = new ViewCell { View = button };
				BoundMenuItem<Inspector> Edit = new BoundMenuItem<Inspector> { Text = "Edit" , BoundObject = inspector };
				BoundMenuItem<Inspector> Delete = new BoundMenuItem<Inspector> { Text = "Delete", IsDestructive = true, BoundObject = inspector };
				Edit.Clicked += openEditInspectorPage;
				Delete.Clicked += deleteInspector;
				cell.ContextActions.Add(Edit);
				cell.ContextActions.Add(Delete);
				cells.Add(cell);
			}

			Button createInspectorsButton = new Button();
			createInspectorsButton.Text = "New Inspector";
			createInspectorsButton.Clicked += openCreateInspectorPage;
			cells.Add(new ViewCell { View = createInspectorsButton });

			section.Add(cells);
			root.Add(section);
			inspectorsView.Root = root;

			Content = inspectorsView;
		}
		public async void openCreateInspectorPage(object sender, EventArgs e)
		{
			EditInspectorPage page = new EditInspectorPage();
			page.CallingPage = this;

			await App.Navigation.PushAsync(page);
		}
		public void deleteInspector(object sender, EventArgs e)
		{
			BoundMenuItem<Inspector> item = (BoundMenuItem<Inspector>)sender;
			Inspector inspector = item.BoundObject;
			Inspector.DeleteInspector(inspector);
			ResetInspectors();
		}
		public async void openEditInspectorPage(object sender, EventArgs e)
		{
			BoundMenuItem<Inspector> item = (BoundMenuItem<Inspector>)sender;
			Inspector inspector = item.BoundObject;
			EditInspectorPage page = new EditInspectorPage(inspector);
			page.CallingPage = this;
			await App.Navigation.PushAsync(page);
		}
	}


	public class EditInspectorPage : ContentPage
	{
		public IInspectorHolder CallingPage { get; set; }
		private Inspector inspector;
		Entry NameEntry;
		//EntryCell NameCell;

		public EditInspectorPage(Inspector existingInspector = null)
		{
			if (existingInspector == null)
			{
				this.inspector = new Inspector();
				Title = "Create New Inspector";
			}
			else
			{
				this.inspector = existingInspector;
				Title = "Edit Inspector";
			}
			StackLayout layout = new StackLayout();
			layout.Padding = new Thickness(20, 0, 0, 0);
			layout.HorizontalOptions = LayoutOptions.StartAndExpand;
			//TableView view = new TableView();
			//view.Intent = TableIntent.Menu;
			//TableRoot root = new TableRoot(Title);
			//TableSection section = new TableSection();
			Padding = new Thickness(0, Device.OnPlatform(20, 0, 0), 0, 0);

			ViewCell NameCell = new ViewCell();
			StackLayout nameLayout = new StackLayout
			{
				Orientation = StackOrientation.Horizontal,
				HorizontalOptions = LayoutOptions.Center,
				Children = {
					new Label{
						Text = "Inspector Name: "
					}
				}
			};
			NameEntry = new Entry
			{
				BindingContext = inspector,
			};
			NameEntry.WidthRequest = 200;
			NameEntry.SetBinding(Entry.TextProperty, "Name");
			nameLayout.Children.Add(NameEntry);
			NameCell.View = nameLayout;

			//NameCell = new EntryCell
			//{
			//	BindingContext = inspector,
			//	Label = "Inspector Name:",
			//};
			//NameCell.SetBinding(EntryCell.TextProperty, "Name");
			ViewCell SaveCell = new ViewCell();
			ViewCell CancelCell = new ViewCell();
			Button saveButton = new Button();
			Button cancelButton = new Button();
			saveButton.HorizontalOptions = LayoutOptions.StartAndExpand;
			cancelButton.HorizontalOptions = LayoutOptions.StartAndExpand;
			saveButton.Clicked += SaveInspectorClicked;
			cancelButton.Clicked += CancelInspectorClicked;
			saveButton.Text = "Save";
			cancelButton.Text = "Cancel";
			SaveCell.View = saveButton;
			CancelCell.View = cancelButton;

			//section.Add(NameCell);
			//section.Add(SaveCell);
			//section.Add(CancelCell);
			//root.Add(section);
			//view.Root = root;
			layout.Children.Add(nameLayout);
			layout.Children.Add(saveButton);
			layout.Children.Add(cancelButton);
			Content = layout;
		}
		public async void SaveInspectorClicked(object sender, EventArgs e)
		{
			inspector.Name = NameEntry.Text;
			App.database.SaveInspector(inspector);
			CallingPage.ResetInspectors();

			await App.Navigation.PopAsync(true);
		}
		public async void CancelInspectorClicked(object sender, EventArgs e)
		{
			await App.Navigation.PopAsync(true);
		}
	}
}
