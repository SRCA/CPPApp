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

		public QuestionPage(Question question, Inspection inspection, string textOverride = null, List<Reference> extraReferences = null)
		{
			this.question = question;
			this.inspection = inspection;
			StackLayout layout = new StackLayout
			{
				Padding = new Thickness(20,20),
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
						}
					}
				}
			);

			// Part
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

			layout.Children.Add(GetSpacing(20));
			layout.Children.Add(GetHorizontalLine());

			// Question text
			layout.Children.Add(
				new StackLayout
				{
					BackgroundColor = Color.Yellow,
					Padding = new Thickness(10, 20, 10, 20),
					Children =
					{
						new Label {
							Text = (textOverride == null) ? question.Text.Trim() : textOverride,
							FontAttributes = FontAttributes.Italic
						}
					}
				});

			//Add Edit Comment Button
			Button commentButton = new Button();
			commentButton.Text = "Add/Edit Comment For Question";
			commentButton.Clicked += openCommentPage;
			commentButton.HorizontalOptions = LayoutOptions.End;
			layout.Children.Add(commentButton);

			layout.Children.Add(GetSpacing(10));

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
			answerRadioGroup.CheckedChanged += answerRadioGroup_CheckedChanged;
			layout.Children.Add(answerRadioGroup);

			//Answer buttons
			List<AnswerButton> answerButtons = new List<AnswerButton>();
			foreach (Answer answer in Enum.GetValues(typeof(Answer)))
			{
				AnswerButton button = new AnswerButton(answer);
				button.Text = EnumDescriptionAttribute.GetDescriptionFromEnumValue(answer);
				button.HorizontalOptions = LayoutOptions.Start;
				button.Clicked += AnswerQuestion;
				layout.Children.Add(button);
			}

			//Clear scores button
			Button clearScoresButton = new Button
			{
				Text = "Clear Scores"
			};
			clearScoresButton.Clicked += clearScores;
			clearScoresButton.HorizontalOptions = LayoutOptions.Start;
			layout.Children.Add(clearScoresButton);

			// References label
			layout.Children.Add(new Label
			{
				Text = "References:",
				FontAttributes = FontAttributes.Bold
			});
			layout.Children.Add(GetSpacing(5));
			layout.Children.Add(GetHorizontalLine());

			//References buttons
			List<Reference> references = question.References;
			if (extraReferences != null)
			{		//Creates a copy of the list so we aren't adding to the original.
				references = references.ToList();
				references.AddRange(extraReferences);
			}
			foreach (Reference reference in references)
			{
				ReferenceButton referenceButton = new ReferenceButton(reference);
				referenceButton.folderName = inspection.ChecklistId;
				referenceButton.BackgroundColor = Color.Red;
				referenceButton.HorizontalOptions = LayoutOptions.Start;
				layout.Children.Add(referenceButton);
			}

			//Remarks label
			Label remarksLabel = new Label();
			remarksLabel.Text = "Remarks:";
			layout.Children.Add(remarksLabel);

			//Remarks box
			remarksBox = new Editor();
			remarksBox.Text = question.Remarks;
			remarksBox.HeightRequest = 175;
			question.OldRemarks = question.Remarks;
			remarksBox.TextChanged += SaveRemarksText;
			layout.Children.Add(remarksBox);

			ScrollView scroll = new ScrollView();
			scroll.Content = layout;

			Content = scroll;
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
			sectionPage.UpdateIcon(true);
			sectionPage.AutoAdvance(question);
		}


		private void AnswerQuestion(object sender, EventArgs e)
		{
			AnswerButton button = (AnswerButton)sender;
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
			score.answer = button.answer;
			App.database.SaveScore(score);
			HasScore = true;
			sectionPage.UpdateIcon(true);
			sectionPage.AutoAdvance(question);
		}
		private void clearScores(object sender, EventArgs e)
		{
			if (score != null)
			{
				inspection.scores.Remove(score);
				App.database.DeleteScore(score);
			}
			HasScore = false;
			sectionPage.UpdateIcon(false);
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

		private BoxView GetHorizontalLine()
		{
			return new BoxView
				{
					HeightRequest = 1,
					Color = Color.Black
				};
		}

		private BoxView GetSpacing(double heightRequest)
		{
			return new BoxView
			{
				HeightRequest = heightRequest
			};
		}
	}

	internal class AnswerChoice
	{
		public Answer Answer { get; set; }
		public string Text { get; set; }
	}

	internal class AnswerButton : Button
	{
		public Answer answer;
		public AnswerButton(Answer answer)
		{
			this.answer = answer;
		}
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
