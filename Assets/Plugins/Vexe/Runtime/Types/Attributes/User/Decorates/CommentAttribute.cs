using System;
using Vexe.Runtime.Types;
using UnityEngine;

namespace Vexe.Runtime.Types
{
	/// <summary>
	/// Annotate a member with this attribute to display a comment on top of it
	/// The values for the type:
	/// 1: Info
	/// 2: Warning
	/// 3: Error
	/// </summary>
	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Method | AttributeTargets.Parameter)]
	public class CommentAttribute : CompositeAttribute
	{
		public readonly string comment;
		public readonly int type;

		public CommentAttribute(string comment) : this(-1, comment)
		{
		}

		public CommentAttribute(int id, string comment) : this(id, comment, 1)
		{
		}

		public CommentAttribute(string comment, int type) : this(-1, comment, type)
		{
		}

		public CommentAttribute(int id, string comment, int type) : base(id)
		{
			this.comment = comment;
			this.type    = type;
		}
	}
}