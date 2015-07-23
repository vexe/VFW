using System;
using Vexe.Runtime.Types;
using UnityEngine;

namespace Vexe.Runtime.Types
{
	/// <summary>
	/// Annotate a member with this attribute to display a comment on top of it.
    /// Use parameter helpButton to add a toggle button to show/hide the comment.
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
        public readonly bool helpButton;
       
		public CommentAttribute(string comment) : this(-1, comment)
		{
		}

		public CommentAttribute(int id, string comment) : this(id, comment, 1)
		{
		}

		public CommentAttribute(string comment, int type) : this(-1, comment, type)
		{
		}

        public CommentAttribute(string comment, bool helpButton) : this(-1, comment, 1, helpButton)
        {
        }

        public CommentAttribute(int id, string comment, int type) : this(id, comment, type, false) 
        { 
        }

        public CommentAttribute(string comment, int type, bool helpButton) : this(-1, comment, type, helpButton) 
        { 
        }

        public CommentAttribute(int id, string comment, bool helpButton) : this(id, comment, 1, helpButton)
        {
        }

        public CommentAttribute(int id, string comment, int type, bool helpButton) : base(id)
		{
			this.comment = comment;
            this.type = type;
            this.helpButton = helpButton;
		}
	}
}