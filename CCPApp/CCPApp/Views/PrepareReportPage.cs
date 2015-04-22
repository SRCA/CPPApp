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
	public class PrepareReportPage : ContentPage
	{
		Inspection inspection;
		ReportOptionsModel model = new ReportOptionsModel();

		Button GenerateButton;
		Label pleaseWaitLabel;
		ActivityIndicator indicator = new ActivityIndicator();
		Grid grid;
		int rowNumber = 0;

		public PrepareReportPage(Inspection inspection)
		{
			this.inspection = inspection;
			Padding = new Thickness(10, 0, 10, 5);
			Title = "Report Options";
			StackLayout layout = new StackLayout();
			layout.VerticalOptions = LayoutOptions.Center;
			//layout.HorizontalOptions = LayoutOptions.End;

			Label ScoredOnlyLabel = new Label { Text = "Scored Sections Only" };
			Switch ScoredOnlySwitch = new Switch { IsToggled = false, BindingContext = model };
			ScoredOnlySwitch.SetBinding(Switch.IsToggledProperty, "ScoredSectionsOnly");

			Label CommentLabel = new Label { Text = "Comments" };
			Switch CommentSwitch = new Switch { IsToggled = true, BindingContext = model };
			CommentSwitch.SetBinding(Switch.IsToggledProperty, "Comments");
			//StackLayout CommentLayout = new StackLayout { Children = { CommentLabel, CommentSwitch }, Orientation = StackOrientation.Horizontal, HorizontalOptions = LayoutOptions.End };

			Label QuestionsLabel = new Label { Text = "Questions List" };
			Switch QuestionsSwitch = new Switch { IsToggled = true, BindingContext = model };
			QuestionsSwitch.SetBinding(Switch.IsToggledProperty, "Questions");
			//StackLayout QuestionsLayout = new StackLayout { Children = { QuestionsLabel, QuestionsSwitch }, Orientation = StackOrientation.Horizontal, HorizontalOptions = LayoutOptions.End };

			Label StructureLabel = new Label { Text = "Checklist Structure" };
			Switch StructureSwitch = new Switch { IsToggled = true, BindingContext = model };
			StructureSwitch.SetBinding(Switch.IsToggledProperty, "Structure");
			//StackLayout StructureLayout = new StackLayout { Children = { StructureLabel, StructureSwitch }, Orientation = StackOrientation.Horizontal, HorizontalOptions = LayoutOptions.End };

			Label TotalsLabel = new Label { Text = "Section Totals" };
			Switch TotalsSwitch = new Switch { IsToggled = true, BindingContext = model };
			TotalsSwitch.SetBinding(Switch.IsToggledProperty, "Totals");
			//StackLayout TotalsLayout = new StackLayout { Children = { TotalsLabel, TotalsSwitch }, Orientation = StackOrientation.Horizontal, HorizontalOptions = LayoutOptions.End };

			Label ScoreSheetLabel = new Label { Text = "Score Sheet" };
			Switch ScoreSheetSwitch = new Switch { IsToggled = true, BindingContext = model };
			ScoreSheetSwitch.SetBinding(Switch.IsToggledProperty, "ScoreSheet");
			//StackLayout ScoreSheetLayout = new StackLayout { Children = { ScoreSheetLabel, ScoreSheetSwitch }, Orientation = StackOrientation.Horizontal, HorizontalOptions = LayoutOptions.End };

			Label GraphLabel = new Label { Text = "Graph Sheet" };
			Switch GraphSwitch = new Switch { IsToggled = true, BindingContext = model };
			GraphSwitch.SetBinding(Switch.IsToggledProperty, "GraphSheet");
			//StackLayout GraphLayout = new StackLayout { Children = { GraphLabel, GraphSwitch }, Orientation = StackOrientation.Horizontal, HorizontalOptions = LayoutOptions.End };

			GenerateButton = new Button { Text = "Generate Report" };
			GenerateButton.Clicked += GenerateButtonClicked;			

			grid = new Grid
			{
				VerticalOptions = LayoutOptions.CenterAndExpand,
				ColumnDefinitions =
				{
					new ColumnDefinition{Width = App.GetPageBounds().Width / 2},
					new ColumnDefinition{Width = App.GetPageBounds().Width / 2}
				}
			};

			grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
			CommentLabel.XAlign = TextAlignment.End;
			grid.Children.Add(CommentLabel, 0, rowNumber);
			grid.Children.Add(CommentSwitch,1,rowNumber);
			rowNumber++;

			grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
			QuestionsLabel.XAlign = TextAlignment.End;
			grid.Children.Add(QuestionsLabel, 0, rowNumber);
			grid.Children.Add(QuestionsSwitch, 1, rowNumber);
			rowNumber++;

			grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
			ScoredOnlyLabel.XAlign = TextAlignment.End;
			grid.Children.Add(ScoredOnlyLabel, 0, rowNumber);
			grid.Children.Add(ScoredOnlySwitch, 1, rowNumber);
			rowNumber++;

			grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
			StructureLabel.XAlign = TextAlignment.End;
			grid.Children.Add(StructureLabel, 0, rowNumber);
			grid.Children.Add(StructureSwitch, 1, rowNumber);
			rowNumber++;

			grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
			TotalsLabel.XAlign = TextAlignment.End;
			grid.Children.Add(TotalsLabel, 0, rowNumber);
			grid.Children.Add(TotalsSwitch, 1, rowNumber);
			rowNumber++;

			grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
			ScoreSheetLabel.XAlign = TextAlignment.End;
			grid.Children.Add(ScoreSheetLabel, 0, rowNumber);
			grid.Children.Add(ScoreSheetSwitch, 1, rowNumber);
			rowNumber++;

			grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
			GraphLabel.XAlign = TextAlignment.End;
			grid.Children.Add(GraphLabel, 0, rowNumber);
			grid.Children.Add(GraphSwitch, 1, rowNumber);
			rowNumber++;

			grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
			grid.Children.Add(GenerateButton, 0, 2, rowNumber, rowNumber + 1);
			rowNumber++;

			//extra row for the loading indicator.
			grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
			pleaseWaitLabel = new Label { Text = "Generating report...", HorizontalOptions = LayoutOptions.Center, VerticalOptions = LayoutOptions.Center };
			indicator.HorizontalOptions = LayoutOptions.Center;

			/*layout.Children.Add(CommentLayout);
			layout.Children.Add(QuestionsLayout);
			layout.Children.Add(StructureLayout);
			layout.Children.Add(TotalsLayout);
			layout.Children.Add(ScoreSheetLayout);
			layout.Children.Add(GraphLayout);
			layout.Children.Add(GenerateButton);
			Content = layout;*/

			Content = grid;
		}

		public void GenerateButtonClicked(object sender, EventArgs e)
		{
			//Device.BeginInvokeOnMainThread(() =>
			//{
			double height = GenerateButton.Height;
			grid.Children.Remove(GenerateButton);
			pleaseWaitLabel.HeightRequest = height;
			grid.Children.Add(pleaseWaitLabel, 0, 2, rowNumber - 1, rowNumber);
			indicator.IsRunning = true;
			grid.Children.Add(indicator, 0, 2, rowNumber, rowNumber + 1);
			//});

			Device.BeginInvokeOnMainThread(async () =>
			{
				string generatedReport = ReportPage.GeneratePdf(inspection, model);
				ReportPage page = new ReportPage(generatedReport);
				await App.Navigation.PushAsync(page);
				grid.Children.Remove(pleaseWaitLabel);
				grid.Children.Remove(indicator);
				grid.Children.Add(GenerateButton, 0, 2, rowNumber - 1, rowNumber);
				indicator.IsRunning = false;
			});
		}
	}
}
