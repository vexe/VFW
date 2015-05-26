using System;
using System.Linq;
using UnityEngine;
using Vexe.Editor.Types;
using Vexe.Runtime.Extensions;
using Vexe.Runtime.Types;

namespace Vexe.Editor.Drawers
{
	public abstract class CompositeDrawer<T, A> : BaseDrawer where A : CompositeAttribute
	{
		protected A attribute { private set; get; }

		protected T memberValue
		{
			get { return (T)member.Value; }
			set { member.Value = value; }
		}

		protected sealed override void InternalInitialize()
		{
            attribute = attributes.OfType<A>()
                                  .OrderBy(x => x.id)
                                  .FirstOrDefault(x => !member.InitializedComposites.Contains(x));

            if (attribute == null)
            { 
                Debug.LogError("Requesting a composite attribute ({0}) from attributes that all have been initialized! This should not happen, please report it"
                     .FormatWith(typeof(T).GetNiceName()));

                attribute = attributes.GetAttribute<A>();
            }

            member.InitializedComposites.Add(attribute);
		}

		public sealed override void OnGUI()
		{
		}

        public override bool CanHandle(Type memberType)
        {
            return memberType.IsA<T>() || memberType.IsSubclassOrImplementerOfRawGeneric(typeof(T));
        }

        protected EditorMember FindRelativeMember(string memberName)
        {
            return EditorMember.WrapMember(memberName, typeof(T), memberValue, unityTarget, id);
        }
	}
}