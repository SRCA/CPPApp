using CCPApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using CCPApp.Utilities;

namespace CCPApp.Views
{
	public class QuestionPage : ContentPage
	{
		public Question question;
		Inspection inspection;
		ScoredQuestion score;
		Editor remarksBox;
		public ISectionPage sectionPage { get; set; }
		public bool HasScore = false;
		bool initialized = false;
		string textOverride = null;
		List<Reference> extraReferences = null;

		public  void Initialize()
		{
			if (!initialized)
			{
				Setup();
				initialized = true;
			}
		}

		public QuestionPage(Question question, Inspection inspection, string textOverride = null, List<Reference> extraReferences = null)
		{
			this.question = question;
			this.inspection = inspection;
			this.textOverride = textOverride;
			this.extraReferences = extraReferences;
		}

		private void Setup()
		{
			StackLayout layout = new StackLayout
			{
				Padding = new Thickness(20, 28),
				Spacing = 0
				//VerticalOptions = LayoutOptions.Center,
			};

			layout.Children.Add(
				new StackLayout
				{
					Orientation = StackOrientation.Horizontal,
					Children = 
					{
						// Section
						new Label {
							Text = "Section " + question.section.Label + ": " + question.section.Title,
							FontAttributes = FontAttributes.Bold,
							HorizontalOptions = LayoutOptions.StartAndExpand
						},
						// Question Number
						new Label {
							Text = "Question " + question.numberString,
							HorizontalOptions = LayoutOptions.EndAndExpand,
							XAlign = TextAlignment.End,
							FontAttributes = FontAttributes.Bold
						}
					}
				}
			);

			// Part
			if (question.SectionPartId != null)
			{
				layout.Children.Add(
					new StackLayout
					{
						Orientation = StackOrientation.Horizontal,
						Children =
					{
						new Label {
							Text = "Part " + question.part.Label + ": " + question.part.Description
						}
					}
					});
			}

			layout.Children.Add(LayoutHelper.GetVerticalSpacing(20));
			layout.Children.Add(LayoutHelper.GetHorizontalLine());

			// Question text
			layout.Children.Add(
				new StackLayout
				{
					BackgroundColor = Color.FromHex("#ffd758"),
					Padding = new Thickness(10, 20, 10, 20),
					Children =
					{
						new Label {
							Text = (textOverride == null) ? question.Text.Trim() : textOverride,
							FontAttributes = FontAttributes.Italic
						}
					}
				});
			layout.Children.Add(LayoutHelper.GetHorizontalLine());

			//Add Edit Comment Button
			Button commentButton = new Button();
			commentButton.Text = "Add/Edit Comment For Question";
			commentButton.FontAttributes = FontAttributes.Italic;
			commentButton.Clicked += openCommentPage;
			commentButton.HorizontalOptions = LayoutOptions.End;
			layout.Children.Add(commentButton);

			layout.Children.Add(LayoutHelper.GetVerticalSpacing(5));

			//Answer
			score = inspection.GetScoreForQuestion(question);
			if (score != null)
			{
				HasScore = true;
				var answer = EnumDescriptionAttribute.GetDescriptionFromEnumValue(score.answer);
			}
			else
			{
				HasScore = false;
			}

			// Answers - Radio Group
			var answers = Enum.GetValues(typeof(Answer)).Cast<Answer>().ToList();
			var answerRadioGroup = new BindableRadioGroup
			{
				ItemsSource = answers.Select(x => EnumDescriptionAttribute.GetDescriptionFromEnumValue(x)),
				SelectedIndex = HasScore == true ? answers.FindIndex(x => x.Equals(score.answer)) : -1
			};
			answerRadioGroup.Spacing = 12;
			answerRadioGroup.CheckedChanged += answerRadioGroup_CheckedChanged;
			answerRadioGroup.ItemUnchecked += answerRadioGroup_Unchecked;

			layout.Children.Add(new StackLayout
			{
				Children = { answerRadioGroup }
			});

			layout.Children.Add(LayoutHelper.GetVerticalSpacing(25));

			// References label
			layout.Children.Add(new Label
			{
				Text = "References:",
				FontAttributes = FontAttributes.Bold
			});
			layout.Children.Add(LayoutHelper.GetVerticalSpacing(5));
			layout.Children.Add(LayoutHelper.GetHorizontalLine());

			//References buttons
			List<Reference> references = question.References;
			if (extraReferences != null)
			{		//Creates a copy of the list so we aren't adding to the original.
				references = references.ToList();
				references.AddRange(extraReferences);
			}

			foreach (Reference reference in references)
			{
				var referenceButton = new ReferenceButton(reference) { folderName = inspection.ChecklistId, FontAttributes = FontAttributes.Italic, HorizontalOptions = LayoutOptions.StartAndExpand };
				layout.Children.Add(
					new StackLayout
					{
						Orientation = StackOrientation.Horizontal,
						Padding = new Thickness(24, 0),
						HeightRequest = 30,
						Children = 
						{
							new Label { TextColor = Color.FromHex("#2b90ff"), FontSize = 40, XAlign = TextAlignment.Center, Text = "\u2022"},
							LayoutHelper.GetHorizontalSpacing(8),
							referenceButton
						}
					});
			}
			layout.Children.Add(LayoutHelper.GetVerticalSpacing(25));


			//Remarks label
			layout.Children.Add(new Label
			{
				Text = "Remarks:",
				FontAttributes = FontAttributes.Bold
			});

			//Remarks box
			remarksBox = new Editor();
			remarksBox.Text = question.Remarks;
			remarksBox.HeightRequest = 100;
			question.OldRemarks = question.Remarks;
			remarksBox.TextChanged += SaveRemarksText;
			layout.Children.Add(remarksBox);

			ScrollView scroll = new ScrollView();
			scroll.Content = layout;

			Content = scroll;
		}

		void answerRadioGroup_Unchecked(object sender, int e)
		{
			clearScores();
		}

		protected async void SaveRemarksText(object Sender, EventArgs e)
		{
			await Task.Run(() => question.Remarks = remarksBox.Text);
		}
		protected override void OnDisappearing()
		{
			if (question.Remarks != question.OldRemarks)
			{
				App.database.SaveQuestion(question);
			}
			base.OnDisappearing();
		}

		void answerRadioGroup_CheckedChanged(object sender, int e)
		{
			var radio = sender as CustomRadioButton;

			if (radio == null) return;

			var answer = Enum.GetValues(typeof(Answer)).Cast<Answer>().ElementAt(radio.Id);

			if (score == null)
			{
				score = new ScoredQuestion();
			}
			score.QuestionId = (int)question.Id;
			score.question = question;
			score.inspection = inspection;
			if (!inspection.scores.Contains(score))
			{
				inspection.scores.Add(score);
			}
			score.answer = answer;
			App.database.SaveScore(score);
			HasScore = true;
			sectionPage.UpdateIcon();

			Task.Delay(100).ContinueWith((task) =>
			{
				Device.BeginInvokeOnMainThread(() =>
				{
					sectionPage.AutoAdvance(question);
				});
			});
		}

		private void clearScores()
		{
			if (score != null)
			{
				inspection.scores.Remove(score);
				App.database.DeleteScore(score);
			}
			HasScore = false;
			sectionPage.UpdateIcon();
		}

		private void openCommentPage(object sender, EventArgs e)
		{
			Device.BeginInvokeOnMainThread(async () =>
			{
				CommentPage page = new CommentPage(inspection, question);
				await App.Navigation.PushAsync(page);
			});
		}
		private void openReferencePage(object sender, EventArgs e)
		{
			Device.BeginInvokeOnMainThread(async () =>
			{
				string referenceName = inspection.ChecklistId + "/" + question.References.First().DocumentName;
				string bookmark = question.References.First().Bookmark;
				int pageNumber = 1;
				if (bookmark != string.Empty)
				{
					pageNumber = int.Parse(bookmark);
				}
				ReferencePage page = new ReferencePage(referenceName, pageNumber);
				await App.Navigation.PushAsync(page);
			});
		}

	}

	internal class AnswerChoice
	{
		public Answer Answer { get; set; }
		public string Text { get; set; }
	}

	internal class ReferenceButton : Button
	{
		public Reference reference { get; set; }
		public string folderName { get; set; }
		public ReferenceButton(Reference reference)
		{
			this.reference = reference;
			this.Text = reference.Description;
			this.Clicked += openReferencePage;
		}

		void openReferencePage(object sender, EventArgs e)
		{
			Device.BeginInvokeOnMainThread(async () =>
			{
				string referenceName = folderName + "/" + reference.DocumentName;
				string bookmark = reference.Bookmark;
				int pageNumber = 1;
				if (bookmark != string.Empty)
				{
					pageNumber = int.Parse(bookmark);
				}
				ReferencePage page = new ReferencePage(referenceName, pageNumber);
				await App.Navigation.PushAsync(page);
			});
		}
	}
}
