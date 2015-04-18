using CCPApp.Items;
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
	public class CommentPage : ContentPage
	{
		public Inspection inspection;
		Label commentIndicatorLabel;
		Editor subjectTextEditor;
		GenericPicker<CommentType> commentTypePicker;
		GenericPicker<Question> questionPicker;
		Comment existingComment = null;
		Editor commentText;
		Editor discussionText;
		Editor recommendationText;
		//ResizingLayout layout;

		public CommentPage(Inspection inspection, Question initialQuestion)
		{
			this.inspection = inspection;
			ChecklistModel checklist = inspection.Checklist;
			this.Title = "Add/Edit Comment";

			StackLayout layout = new StackLayout
			{
				Padding = new Thickness(20, 28)
			};
			Page frontPage = App.Navigation.NavigationStack.First();
			double width = frontPage.Width;
			layout.WidthRequest = width * .95;

			//Choose comment type
			Label chooseCommentTypeLabel = new Label { Text = "Choose Comment Type:", FontAttributes = FontAttributes.Bold };
			commentTypePicker = new GenericPicker<CommentType>();
			commentTypePicker.AddItem(CommentType.Finding);
			commentTypePicker.AddItem(CommentType.Observation);
			commentTypePicker.AddItem(CommentType.Commendable);
			commentTypePicker.SelectedIndexChanged += SelectCommentType;
			commentTypePicker.Focused += genericPicker_Focused;
			commentTypePicker.Unfocused += genericPicker_Unfocused;

			//Choose question
			Label chooseQuestionLabel = new Label{Text="Choose Question:", FontAttributes = FontAttributes.Bold};
			questionPicker = new GenericPicker<Question>();
			foreach (Question question in checklist.GetAllQuestions().Where(q => !q.HasSubItems))
			{
				questionPicker.AddItem(question);
			}
			questionPicker.SelectedIndexChanged += SelectQuestion;
			questionPicker.Focused += genericPicker_Focused;
			questionPicker.Unfocused += genericPicker_Unfocused;

			//Comment description
			Label commentSubjectLabel = new Label { Text = "Subject:", FontAttributes = FontAttributes.Bold };
			subjectTextEditor = new Editor () ;
			subjectTextEditor.HeightRequest = 80;
			
			//Enter comment
			commentIndicatorLabel = new Label { Text = "Comment:", FontAttributes = FontAttributes.Bold };
			commentText = new Editor();
			commentText.HeightRequest = 80;

			//Discussion
			Label DiscussionLabel = new Label { Text = "Discussion:", FontAttributes = FontAttributes.Bold };
			discussionText = new Editor();
			discussionText.HeightRequest = 80;

			//Recommendation
			Label RecommendationLabel = new Label { Text = "Recommendation/Action Taken or Required:", FontAttributes = FontAttributes.Bold };
			recommendationText = new Editor();
			recommendationText.HeightRequest = 80;

			//Choose date
			Label chooseDateLabel = new Label { Text = "Date:", FontAttributes = FontAttributes.Bold };
			DatePicker date = new DatePicker() { Format = "MMMM dd, yyyy", HorizontalOptions = LayoutOptions.Start };
			date.Date = DateTime.Now;

			//Save button
			Button saveButton = new Button { Text = "Save Comment" };
			saveButton.Clicked += SaveComment;

			//Delete button
			Button deleteButton = new Button { Text = "Delete Comment" };
			deleteButton.Clicked += DeleteComment;

			//TODO: choose inspector
			//TODO more fields, I guess.

			//Perform the setup actions.
			commentTypePicker.SelectedIndex = 0;
			questionPicker.SelectedIndex = questionPicker.TItems.IndexOf(initialQuestion);

			double spaceBetweenQuestions = 2;
			layout.Children.Add(
				new StackLayout
				{
					Orientation = StackOrientation.Horizontal,
					Children = 
					{
						new StackLayout {
							WidthRequest = width * 2 / 3,
							Children = 
							{
								chooseCommentTypeLabel,
								commentTypePicker
							}
						},
						LayoutHelper.GetHorizontalSpacing(10),
						new StackLayout {
							WidthRequest = width * 1 / 3,
							Children = 
							{
								chooseDateLabel,
								date
							}
						},
					}
				});
			
			layout.Children.Add(LayoutHelper.GetVerticalSpacing(spaceBetweenQuestions));

			layout.Children.Add(chooseQuestionLabel);
			layout.Children.Add(questionPicker);
			layout.Children.Add(LayoutHelper.GetVerticalSpacing(spaceBetweenQuestions));

			layout.Children.Add(commentSubjectLabel);
			layout.Children.Add(subjectTextEditor);
			layout.Children.Add(LayoutHelper.GetVerticalSpacing(spaceBetweenQuestions));

			layout.Children.Add(commentIndicatorLabel);
			layout.Children.Add(commentText);
			layout.Children.Add(LayoutHelper.GetVerticalSpacing(spaceBetweenQuestions));

			layout.Children.Add(DiscussionLabel);
			layout.Children.Add(discussionText);
			layout.Children.Add(LayoutHelper.GetVerticalSpacing(spaceBetweenQuestions));

			layout.Children.Add(RecommendationLabel);
			layout.Children.Add(recommendationText);
			layout.Children.Add(LayoutHelper.GetVerticalSpacing(spaceBetweenQuestions));

			layout.Children.Add(
				new StackLayout{
					Orientation = StackOrientation.Horizontal,
					HorizontalOptions = LayoutOptions.Center,
					Children = 
					{
						saveButton, 
						LayoutHelper.GetHorizontalSpacing(30),
						deleteButton
					}
				});

			ScrollView scroll = new ScrollView();
			scroll.Content = layout;

			this.Content = scroll;
		}

		void genericPicker_Unfocused(object sender, FocusEventArgs e)
		{
			((Picker)sender).BackgroundColor = Color.Default;
		}

		void genericPicker_Focused(object sender, FocusEventArgs e)
		{
			((Picker)sender).BackgroundColor = App.VisualElementIsFocusedColor;
		}

		void SaveComment(object sender, EventArgs e)
		{
			if (existingComment == null)
			{
				existingComment = new Comment();
			}
			Question question = questionPicker.SelectedItem;
			existingComment.QuestionId = (int)question.Id;
			existingComment.question = question;
			existingComment.inspection = inspection;
			if (!inspection.comments.Contains(existingComment))
			{
				inspection.comments.Add(existingComment);
			}
			existingComment.CommentText = commentText.Text;
			existingComment.Subject = subjectTextEditor.Text;
			existingComment.type = commentTypePicker.SelectedItem;
			existingComment.Recommendation = recommendationText.Text;
			existingComment.Discussion = discussionText.Text;
			App.database.SaveComment(existingComment);
			Device.BeginInvokeOnMainThread(async () =>
			{
				await App.Navigation.PopAsync();
			});
		}
		void DeleteComment(object sender, EventArgs e)
		{
			if (existingComment == null)
			{
				//no comment to delete.
			}
			else
			{
				if (inspection.comments.Contains(existingComment))
				{
					inspection.comments.Remove(existingComment);
				}
				App.database.DeleteComment(existingComment);
			}
			Device.BeginInvokeOnMainThread(async () =>
			{
				await App.Navigation.PopAsync();
			});
		}

		private void SelectCommentType(object sender, EventArgs e)
		{
			//Update commentIndicatorLabel
			string commentType = commentTypePicker.Items[commentTypePicker.SelectedIndex];
			commentIndicatorLabel.Text = commentType+":";
			ProcessExistingComment();
		}
		private void SelectQuestion(object sender, EventArgs e)
		{
			Question question = questionPicker.SelectedItem;
			subjectTextEditor.Text = question.Text;
			ProcessExistingComment();
		}
		private void ProcessExistingComment()
		{
			if (questionPicker.SelectedIndex < 0 || questionPicker.SelectedIndex > questionPicker.Items.Count)
			{	//Things aren't initialized properly yet.
				return;
			}
			Question question = questionPicker.SelectedItem;
			List<Comment> commentsForQuestion = inspection.comments.Where(c => c.question == question).ToList();
			CommentType type = commentTypePicker.SelectedItem;
			existingComment = commentsForQuestion.SingleOrDefault(c => c.type == type);
			if (existingComment == null)
			{
				commentText.Text = "";
			}
			else
			{
				commentText.Text = existingComment.CommentText;
				discussionText.Text = existingComment.Discussion;
				recommendationText.Text = existingComment.Recommendation;
				subjectTextEditor.Text = existingComment.Subject;
			}
		}
		//private void CommentTextChanged(object sender, EventArgs e)
		//{
			//layout.OnChildChanged();
		//}
	}
}
