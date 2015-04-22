﻿using SQLite.Net.Attributes;
using SQLiteNetExtensions.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CCPApp.Models
{
	public class Question : IdModel
	{
		[PrimaryKey,AutoIncrement]
		public override int? Id { get; set; }
		[ForeignKey(typeof(SectionModel))]
		public int? SectionId { get; set; }
		[ManyToOne(CascadeOperations= CascadeOperation.All)]
		public SectionModel section { get; set; }
		[ForeignKey(typeof(SectionPart))]
		public int? SectionPartId { get; set; }
		[ManyToOne(CascadeOperations = CascadeOperation.All)]
		public SectionPart part { get; set; }


		public int Number { get; set; }
		public string Subqualifier { get; set; }
		public bool Critical { get; set; }
		public string CriticalApplication = "";
		public bool InvertScore { get; set; }
		public bool IsLastQuestion = false;
		public bool HasSubItems { get; set; }
		public bool Updated { get; set; }
		public string Text { get; set; }
		public string PrintedText { get; set; }
		public string OldText { get; set; }
		//public string Remarks { get; set; }
		public string OldRemarks;

		[OneToMany(CascadeOperations = CascadeOperation.All)]
		public List<Reference> References { get; set; }

		public override string ToString()
		{
			return section.Label + stringWithoutSection;
			/*string prefix;
			if (part != null)
			{
				prefix = section.Label + "-" + part.Label;
			}
			else
			{
				prefix = section.Label;
			}
			return prefix + "-" + numberString;*/
		}
		public string stringWithoutSection
		{
			get
			{
				if (part == null)
				{
					return numberString;
				}
				else
				{
					return part.Label + "-" + numberString;
				}
			}
		}
		public string numberString
		{
			get
			{
				return Number.ToString() + Subqualifier;
			}
		}
		public bool IsScorable()
		{
			return !HasSubItems;
		}

		public Question()
		{
			References = new List<Reference>();
		}

		public string ToStringForListing
		{
			get
			{
				Question master = MasterQuestion;
				if (master == null)
				{
					return "Section " + section.Label + ": " + section.Title + ", Question: "+stringWithoutSection;
					//return ToString() + "Section " + section.Label + ": " + Text + ", ";
				}
				else
				{
					return "Section " + section.Label + ": " + section.Title + ", Question: " + stringWithoutSection;
					//return ToString() + "Section " + master.section.Label + ": " + Text + ", ";
				}
			}
		}

		public string FullString
		{
			get
			{
				Question master = MasterQuestion;
				if (master == null)
				{
					return ToString() + " " + Text;
				}
				else
				{
					return ToString() + " " + master.Text + " " + Text;
				}
			}
		}
		public Question SelfReference
		{
			get
			{
				return this;
			}
		}
		public Question MasterQuestion
		{
			get
			{
				if (Subqualifier == null || Subqualifier == string.Empty)
				{
					return null;
				}
				return section.AllQuestions().Single(q => q.SectionId == SectionId && q.SectionPartId == SectionPartId && q.Number == Number && q.Subqualifier == string.Empty);
			}
		}
	}
	/// <summary>
	/// Represents a reference for a question.
	/// </summary>
	public class Reference
	{
		[PrimaryKey, AutoIncrement]
		public int? Id { get; set; }

		[ForeignKey(typeof(Question))]
		public int? QuestionId { get; set; }
		[ManyToOne(CascadeOperations = CascadeOperation.All)]
		public Question question { get; set; }

		public string Document { get; set; }
		public string DocumentName { get; set; }
		public string Bookmark { get; set; }
		public string Description { get; set; }
		public Reference() { }
		public Reference(Reference r)
		{
			Document = r.Document;
			DocumentName = r.DocumentName;
			Bookmark = r.Bookmark;
			Description = r.Description;
		}

		public override string ToString()
		{
			return Description;
		}
		public string NameWithoutExt
		{
			get
			{
				return DocumentName.Substring(0, DocumentName.LastIndexOf('.'));
			}
			set
			{
				DocumentName = value + Extension;
			}

		}
		public string Extension
		{
			get
			{
				return DocumentName.Substring(DocumentName.LastIndexOf('.'));
			}
			set
			{
				DocumentName = NameWithoutExt + value;
			}
		}
	}
}
