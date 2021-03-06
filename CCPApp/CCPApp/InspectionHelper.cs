﻿using CCPApp.Items;
using CCPApp.Models;
using CCPApp.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace CCPApp
{
	public class InspectionHelper
	{
		public static string EmptyCircleFileName = "EmptyCircle.png";
		public static string HalfCircleFileName = "HalfCircle.png";
		public static string CheckmarkFileName = "Checkmark2.png";

		public static async void CreateInspectionButtonClicked(object sender, EventArgs e)
		{
			CreateInspectionButton button = (CreateInspectionButton)sender;
			ChecklistModel checklist = button.checklist;
			EditInspectionPage page = new EditInspectionPage(null,checklist);
			VisualElement Parent = button.ParentView.ParentView;
			if (Parent.GetType() == typeof(InspectionListPage))
			{
				page.CallingPage = (InspectionListPage)button.ParentView.ParentView;
			}
			else
			{
				InspectionListPage ListPage = new InspectionListPage(checklist);
				page.CallingPage = ListPage;
			}
			await App.Navigation.PushAsync(page);
		}
		public static async void SelectInspectionButtonClicked(object sender, EventArgs e)
		{
			IInspectionContainer button = (IInspectionContainer)sender;
			Inspection inspection = button.inspection;
			//Device.BeginInvokeOnMainThread(async () =>
			//{
				InspectionPage page = new InspectionPage(inspection);
				await App.Navigation.PushAsync(page);
			//});
		}

		internal static List<QuestionPage> GenerateQuestionPages(List<Question> questions, Inspection inspection, ISectionPage sectionPage)
		{
			List<QuestionPage> pages = new List<QuestionPage>();
			string MasterQuestionText = string.Empty;
			List<Reference> MasterQuestionReferences = new List<Reference>();
			int MasterQuestionNumber = -2;
			double bottomSpace;
			if (sectionPage.GetType() == typeof(SectionNoPartsPage))
			{
				bottomSpace = 30;
			}
			else
			{
				bottomSpace = 60;
			}
			foreach (Question question in questions)
			{
				QuestionPage page = null;
				if (question.HasSubItems)
				{
					MasterQuestionNumber = question.Number;
					MasterQuestionText = question.Text.Trim();
					MasterQuestionReferences = question.References;
				}
				else if (question.Number == MasterQuestionNumber)
				{
					page = new QuestionPage(question, inspection, bottomSpace, MasterQuestionText + "\n" + question.Text.Trim(),MasterQuestionReferences);
				}
				else
				{
					page = new QuestionPage(question, inspection, bottomSpace);
				}
				if (page != null)
				{
					page.sectionPage = sectionPage;
					pages.Add(page);
				}
			}
			return pages;
		}

		public static async void InitializePages(List<QuestionPage> pages)
		{
			foreach (QuestionPage page in pages)
			{
				await Task.Delay(100).ContinueWith((task) =>{
					page.Initialize();
				});
			}
		}
	}

	public interface IInspectionContainer
	{
		Inspection inspection { get; set; }
	}

	public class InspectionGrid : Grid, IInspectionContainer
	{
		public InspectionGrid(Inspection inspection)
		{
			this.inspection = inspection;
			//Text = inspection.Name;
		}
		public Inspection inspection { get; set; }
	}
	public class InspectionButton : Button, IInspectionContainer
	{
		public InspectionButton(Inspection inspection)
		{
			this.inspection = inspection;
		}
		public Inspection inspection { get; set; }
	}
	public class CreateInspectionButton : Button
	{
		public ChecklistModel checklist { get; set; }
		public CreateInspectionButton(ChecklistModel checklist)
		{
			this.checklist = checklist;
		}
	}

	
	public class GoToQuestionButton : Button
	{
		//Question question { get; set; }
		InspectionPage inspectionPage { get; set; }
		public GoToQuestionButton(Question question, InspectionPage inspectionPage)
		{
			this.question = question;
			this.inspectionPage = inspectionPage;
			this.Clicked += clickQuestionButton;
		}
		public GoToQuestionButton(InspectionPage inspectionPage)
		{
			this.inspectionPage = inspectionPage;
			this.Clicked += clickQuestionButton;
		}

		private async static void clickQuestionButton(object sender, EventArgs e)
		{
			GoToQuestionButton button = (GoToQuestionButton)sender;
			ISectionPage sectionPage = button.inspectionPage.SetSectionPage(button.question.section);
			sectionPage.SetSelectedQuestion(button.question);
			button.inspectionPage.CheckActionMenu = false;

			await App.Navigation.PopAsync();
		}
		
		public Question question
		{
			get
			{
				return (Question)GetValue(QuestionProperty);
			}
			set
			{
				SetValue(QuestionProperty, value);
			}
		}

		public static readonly BindableProperty QuestionProperty =
			BindableProperty.Create<GoToQuestionButton, Question>
			(p => p.question, null);
	}
}
