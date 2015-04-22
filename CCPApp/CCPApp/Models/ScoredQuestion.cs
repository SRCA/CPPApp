using CCPApp.Utilities;
using SQLite.Net.Attributes;
using SQLiteNetExtensions.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CCPApp.Models
{
	public class ScoredQuestion
	{
		[PrimaryKey, AutoIncrement]
		public int? Id { get; set; }

		[ForeignKey(typeof(Inspection))]
		public int? InspectionId { get; set; }
		[ManyToOne(CascadeOperations=CascadeOperation.CascadeRead)]
		public Inspection inspection { get; set; }
		[ForeignKey(typeof(Question))]
		public int QuestionId { get; set; }
		[ManyToOne(CascadeOperations=CascadeOperation.All)]
		public Question question { get; set; }
		public Answer answer { get; set; }
	}
	public enum Answer
	{
		Yes,
		No,
		[EnumDescription("N/A")]
		NA,
		Disputed
	}

	public class Remark
	{
		[PrimaryKey, AutoIncrement]
		public int? Id { get; set; }
		[ForeignKey(typeof(Inspection))]
		public int? InspectionId { get; set; }
		[ManyToOne(CascadeOperations = CascadeOperation.CascadeRead)]
		public Inspection inspection { get; set; }
		[ForeignKey(typeof(Question))]
		public int QuestionId { get; set; }
		[ManyToOne(CascadeOperations = CascadeOperation.All)]
		public Question question { get; set; }

		public String remark { get; set; }

		public void UpdateRemark()
		{
			if (remark.Length > 0)
			{
				App.database.SaveRemark(this);
			}
			else
			{
				inspection.remarks.Remove(this);
				App.database.DeleteRemark(this);
			}
		}
	}
}
