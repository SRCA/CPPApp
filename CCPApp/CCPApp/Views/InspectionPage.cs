using CCPApp.Models;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace CCPApp.Views
{
	/// <summary>
	/// This class contains the design for the meat of the program, the answering of questions.
	/// </summary>
	public class InspectionPage : TabbedPage
	{
		private Inspection inspection;
		public bool CheckActionMenu = true;
		public InspectionPage(Inspection inspection)
		{
			this.inspection = inspection;
			Title = inspection.Name;

			/*ToolbarItem scoreButton = new ToolbarItem();
			scoreButton.Text = "Scores";
			//scoreButton.Icon = "ScoresIcon.png";
			scoreButton.Clicked += ClickScoresButton;

			ToolbarItem unansweredButton = new ToolbarItem();
			unansweredButton.Text = "Unanswered";
			unansweredButton.Clicked += ClickUnansweredButton;
			ToolbarItem disputedButton = new ToolbarItem();
			disputedButton.Text = "Disputed";
			disputedButton.Clicked += ClickDisputedButton;

			ToolbarItem reportButton = new ToolbarItem();
			reportButton.Text = "Report";
			reportButton.Clicked += ClickReportButton;*/

			ToolbarItem actionsButton = new ToolbarItem();
			actionsButton.Text = "Actions";
			actionsButton.Clicked += ClickActionsButton;

			//ToolbarItems.Add(scoreButton);
			//ToolbarItems.Add(unansweredButton);
			//ToolbarItems.Add(disputedButton);
			//ToolbarItems.Add(reportButton);
			ToolbarItems.Add(actionsButton);
			ChecklistModel checklist = inspection.Checklist;
			foreach (SectionModel section in checklist.Sections)
			{
				if (section.SectionParts.Count > 0)
				{
					SectionWithPartsPage page = new SectionWithPartsPage(section, inspection,this);
					Children.Add(page);
				}
				else
				{
					SectionNoPartsPage page = new SectionNoPartsPage(section, inspection,this);
					Children.Add(page);
				}
			}
			if (inspection.GetLastViewedQuestion() == null)
			{
				inspection.SetLastViewedQuestion(checklist.Sections.First().AllScorableQuestions().First());
			}
			this.CurrentPageChanged += this.PageChanged;
			Question targetQuestion = inspection.GetLastViewedQuestion();
			ISectionPage targetPage = SetSectionPage(targetQuestion.section);
			targetPage.Initialize();
			targetPage.SetSelectedQuestion(targetQuestion);
		}
		private void PageChanged(object sender, EventArgs e)
		{
			if (CurrentPage == null)
			{	//clicking "More" can yield a null page.
				return;
			}
			ISectionPage page = (ISectionPage)CurrentPage;
			page.Initialize();
			inspection.SetLastViewedQuestion(page.GetCurrentQuestion());
		}

		private async void ClickScoresButton()
		{
			Question question = ((ISectionPage)CurrentPage).GetCurrentQuestion();
			ScoresPage page = new ScoresPage(inspection, question);
			await App.Navigation.PushAsync(page);
		}
		private void ClickUnansweredButton()
		{
			Device.BeginInvokeOnMainThread(async () =>
			{
				UnansweredPage page = new UnansweredPage(inspection, this);
				await App.Navigation.PushAsync(page);
			});
		}
		private void ClickDisputedButton()
		{
			Device.BeginInvokeOnMainThread(async () =>
			{
				DisputedPage page = new DisputedPage(inspection, this);
				await App.Navigation.PushAsync(page);
			});
		}
		private void ClickReportButton()
		{
			Device.BeginInvokeOnMainThread(async () =>
			{
				//Pop up a loading page if this proves to be slow.
				//string generatedReport = ReportPage.GeneratePdf(inspection);
				//ReportPage page = new ReportPage(generatedReport);
				PrepareReportPage page = new PrepareReportPage(inspection);
				await App.Navigation.PushAsync(page);
			});
		}
		private void ClickOutbriefingButton()
		{
			Device.BeginInvokeOnMainThread(async () =>
			{
				OutbriefingPage page = new OutbriefingPage(inspection);
				await App.Navigation.PushAsync(page);
			});
		}
		private void ClickChecklistInfo()
		{
			DisplayAlert("Checklist Information", "You are viewing the checklist\n"+inspection.Checklist.Title, "OK");
		}
		private async void ClickActionsButton(object sender, EventArgs e)
		{
			string action = await DisplayActionSheet("Inspection Actions", "Cancel", null, "Generate Report", "Disputed Questions", "Unanswered Questions", "View Scores","Generate Outbriefing", "Checklist Information","Cancel");
			switch (action)
			{
			case "Generate Report":
				ClickReportButton();
				break;
			case "Disputed Questions":
				ClickDisputedButton();
				break;
			case "Unanswered Questions":
				ClickUnansweredButton();
				break;
			case "View Scores":
				ClickScoresButton();
				break;
			case "Generate Outbriefing":
				ClickOutbriefingButton();
				break;
			case "Checklist Information":
				ClickChecklistInfo();
				break;
			}
		}
		protected override void OnAppearing()
		{
			Type type = App.Navigation.NavigationStack.Last().GetType();
			if (CheckActionMenu && (type == typeof(PrepareReportPage) || type == typeof(DisputedPage) || type == typeof(UnansweredPage) || type == typeof(ScoresPage)
				|| type == typeof(OutbriefingPage)))
			{
				//They are returning from a page that comes from the actions menu.  They may expect the actions menu to still be there.
				ClickActionsButton(null, null);
			}
			CheckActionMenu = true;
			base.OnAppearing();
		}

		public ISectionPage SetSectionPage(SectionModel section)
		{
			this.CurrentPage = this.Children.Single(s => ((ISectionPage)s).GetCurrentSection().Id == section.Id);
			return (ISectionPage)CurrentPage;
		}
	}
	public interface ISectionPage
	{
		void Initialize();
		SectionModel GetCurrentSection();
		Question GetCurrentQuestion();
		void SetSelectedQuestion(Question question);
		/// <summary>
		/// Checks to see if the icon needs to be updated and, if so, updates it.
		/// </summary>
		/// <param name="answered">True if a question has just been answered, false if one has just been cleared</param>
		void UpdateIcon();
		void AutoAdvance(Question question);
	}
	internal class SectionWithPartsPage : TabbedPage, ISectionPage
	{
		SectionModel section;
		bool initialized = false;
		Inspection inspection;
		public InspectionPage parentPage;
		public SectionWithPartsPage(SectionModel section, Inspection inspection, InspectionPage parentPage)
		{
			this.section = section;
			this.inspection = inspection;
			Title = section.ShortTitle;
			this.parentPage = parentPage;
			int scoredQuestions = inspection.scores.Count(s => s.question.section == section);
			if (section.AllScorableQuestions().Count == scoredQuestions)
			{
				Icon = InspectionHelper.CheckmarkFileName;
			}
			else if (scoredQuestions > 0)
			{
				Icon = InspectionHelper.HalfCircleFileName;
			}
			else
			{
				Icon = InspectionHelper.EmptyCircleFileName;
			}
		}
		public void Initialize()
		{
			if (!initialized)
			{
				initialized = true;
				foreach (SectionPart part in section.SectionParts)
				{
					PartPage page = new PartPage(part, inspection,this);
					Children.Add(page);
				}
			}
		}
		public Question GetCurrentQuestion()
		{
			return ((QuestionPage)((PartPage)CurrentPage).CurrentPage).question;
		}
		public void SetSelectedQuestion(Question question)
		{
			this.CurrentPage = this.Children.Single(p => ((PartPage)p).GetPart().Id == question.part.Id);
			PartPage partPage = (PartPage)this.CurrentPage;
			ContentPage questionPage = partPage.Children.Single(q => ((QuestionPage)q).question.Id == question.Id);
			(questionPage as QuestionPage).Initialize();
			partPage.CurrentPage = questionPage;
		}
		public SectionModel GetCurrentSection()
		{
			return section;
		}
		public void UpdateIcon()
		{
			((PartPage)CurrentPage).UpdateIcon();
			int scoredQuestions = inspection.scores.Count(s => s.question.section == section);
			if (scoredQuestions == 0)
			{
				Icon = InspectionHelper.EmptyCircleFileName;
			}
			else if (scoredQuestions == section.AllScorableQuestions().Count)
			{
				Icon = InspectionHelper.CheckmarkFileName;
			}
			else
			{
				Icon = InspectionHelper.HalfCircleFileName;
			}
		}
		public void AutoAdvance(Question question)
		{
			((PartPage)CurrentPage).AutoAdvance(question);
		}
		protected override void OnCurrentPageChanged()
		{
			inspection.SetLastViewedQuestion(GetCurrentQuestion());
			base.OnCurrentPageChanged();
		}
	}
	public class SectionNoPartsPage : CarouselPage, ISectionPage
	{
		SectionModel section;
		bool initialized = false;
		Inspection inspection;
		InspectionPage parentPage;
		public SectionNoPartsPage(SectionModel section, Inspection inspection, InspectionPage parentPage)
		{
			this.section = section;
			this.inspection = inspection;
			this.parentPage = parentPage;
			Title = section.ShortTitle;
			int scoredQuestions = inspection.scores.Count(s => s.question.section == section);
			if (section.AllScorableQuestions().Count == scoredQuestions)
			{
				Icon = InspectionHelper.CheckmarkFileName;
			}
			else if (scoredQuestions > 0)
			{
				Icon = InspectionHelper.HalfCircleFileName;
			}
			else
			{
				Icon = InspectionHelper.EmptyCircleFileName;
			}
		}
		public void Initialize()
		{
			if (!initialized)
			{
				initialized = true;
				
				List<QuestionPage> pages = InspectionHelper.GenerateQuestionPages(section.Questions, inspection,this);
				foreach (ContentPage page in pages)
				{
					Children.Add(page);
				}
				//InspectionHelper.InitializePages(pages);
			}
		}
		public Question GetCurrentQuestion()
		{
			return ((QuestionPage)CurrentPage).question;
		}
		public void SetSelectedQuestion(Question question)
		{
			ContentPage page = this.Children.Single(q => ((QuestionPage)q).question.Id == question.Id);
			(page as QuestionPage).Initialize();
			this.CurrentPage = page;
		}
		public SectionModel GetCurrentSection()
		{
			return section;
		}
		public void UpdateIcon()
		{
			int scoredQuestions = inspection.scores.Count(s => s.question.section == section);
			if (scoredQuestions == 0)
			{
				Icon = InspectionHelper.EmptyCircleFileName;
			}
			else if (scoredQuestions == section.AllScorableQuestions().Count)
			{
				Icon = InspectionHelper.CheckmarkFileName;
			}
			else
			{
				Icon = InspectionHelper.HalfCircleFileName;
			}
			/*int scoredQuestions = Children.Cast<QuestionPage>().Count(p => p.HasScore);
			if (scoredQuestions == 0)
			{
				Icon = InspectionHelper.EmptyCircleFileName;
			}
			else if (scoredQuestions == Children.Count)
			{
				Icon = InspectionHelper.CheckmarkFileName;
			}
			else
			{
				Icon = InspectionHelper.HalfCircleFileName;
			}*/
		}
		public async void AutoAdvance(Question question)
		{
			List<Question> questions = section.AllScorableQuestions();
			if (question.Id == questions.Last().Id)
			{
				if (!(section.Id == inspection.Checklist.Sections.Last().Id))
				{
					bool accept = await DisplayAlert("End of Section", "You have reached the end of this section.  Would you like to continue to the next?", "Yes", "No");
					if (accept)
					{
						parentPage.CurrentPage = parentPage.Children.ElementAt(parentPage.Children.IndexOf(parentPage.CurrentPage) + 1);
					}
				}
				else
				{
					//report that this is the end of the checklist
					await DisplayAlert("End of Checklist", "You have reached the end of the checklist.", "OK");
				}
			}
			else
			{
				int index = questions.IndexOf(question);
				QuestionPage page = this.Children.ElementAt(index + 1) as QuestionPage;
				page.Initialize();
				this.CurrentPage = page;
			}
		}
		protected override void OnCurrentPageChanged()
		{
			QuestionPage page = CurrentPage as QuestionPage;
			if (page != null)
			{
				page.Initialize();
				int index = Children.IndexOf(CurrentPage);
				if (index > 0)
				{
					(Children.ElementAt(index - 1) as QuestionPage).Initialize();
				}
				if (index < Children.Count - 1)
				{
					(Children.ElementAt(index + 1) as QuestionPage).Initialize();
				}
			}
			inspection.SetLastViewedQuestion(GetCurrentQuestion());
			base.OnCurrentPageChanged();
		}
	}

	internal class PartPage : CarouselPage
	{
		SectionPart part;
		Inspection inspection;
		SectionWithPartsPage parentPage;
		public PartPage(SectionPart part, Inspection inspection, SectionWithPartsPage sectionPage)
		{
			this.part = part;
			this.inspection = inspection;
			parentPage = sectionPage;
			Title = "Part " + part.Label;
			List<QuestionPage> pages = InspectionHelper.GenerateQuestionPages(part.Questions, inspection, sectionPage);
			foreach (ContentPage page in pages)
			{
				Children.Add(page);
			}
			//InspectionHelper.InitializePages(pages);
			UpdateIcon();
			this.CurrentPageChanged += PartPage_CurrentPageChanged;
		}
		public SectionPart GetPart()
		{
			return part;
		}
		protected void PartPage_CurrentPageChanged(object sender, EventArgs e)
		{
			inspection.SetLastViewedQuestion(((QuestionPage)CurrentPage).question);
		}
		public void UpdateIcon()
		{
			int scoredQuestions = inspection.scores.Count(s => s.question.part == part);
			if (scoredQuestions == 0)
			{
				Icon = InspectionHelper.EmptyCircleFileName;
			}
			else if (scoredQuestions == part.ScorableQuestions.Count())
			{
				Icon = InspectionHelper.CheckmarkFileName;
			}
			else
			{
				Icon = InspectionHelper.HalfCircleFileName;
			}
		}
		public async void AutoAdvance(Question question)
		{
			List<Question> questions = part.ScorableQuestions.ToList();
			if (question.Id == questions.Last().Id)
			{
				SectionModel section = part.section;
				if (section.SectionParts.Last().Id == part.Id)
				{//this is the last part.  Offer to go to next section.
					if (!(section.Id == inspection.Checklist.Sections.Last().Id))
					{
						bool accept = await DisplayAlert("End of Section", "You have reached the end of this section.  Would you like to continue to the next?", "Yes", "No");
						if (accept)
						{
							parentPage.parentPage.CurrentPage = parentPage.parentPage.Children.ElementAt(parentPage.parentPage.Children.IndexOf(parentPage.parentPage.CurrentPage) + 1);
						}
					}
					else
					{
						//report that this is the end of the checklist
						await DisplayAlert("End of Checklist", "You have reached the end of the checklist.", "OK");
					}
				}
				else
				{//offer to go to the next part
					bool accept = await DisplayAlert("End of Part", "You have reached the end of this part.  Would you like to continue to the next?", "Yes", "No");
					if (accept)
					{
						parentPage.CurrentPage = parentPage.Children.ElementAt(parentPage.Children.IndexOf(parentPage.CurrentPage) + 1);
					}
				}
			}
			else
			{
				int index = questions.IndexOf(question);
				QuestionPage page = this.Children.ElementAt(index + 1) as QuestionPage;
				page.Initialize();
				this.CurrentPage = page;
			}
		}
		protected override void OnCurrentPageChanged()
		{
			QuestionPage page = CurrentPage as QuestionPage;
			if (page != null)
			{
				page.Initialize();
				int index = Children.IndexOf(CurrentPage);
				if (index > 0)
				{
					(Children.ElementAt(index - 1) as QuestionPage).Initialize();
				}
				if (index < Children.Count - 1)
				{
					(Children.ElementAt(index + 1) as QuestionPage).Initialize();
				}
			}
			base.OnCurrentPageChanged();
		}
	}
}
