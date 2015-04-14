using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace CCPApp.Utilities
{
	[AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
	public class EnumDescriptionAttribute : Attribute
	{
		private readonly string description;
		public string Description { get { return description; } }
		public EnumDescriptionAttribute(string description)
		{
			this.description = description;
		}

		public static string GetDescriptionFromEnumValue(Enum value)
		{
			var attribute = value.GetType()
				.GetRuntimeField(value.ToString())
				.GetCustomAttributes(typeof(EnumDescriptionAttribute), false)
				.SingleOrDefault() as EnumDescriptionAttribute;

			return attribute == null ? value.ToString() : attribute.Description;
		}
	}
}
