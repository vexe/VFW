using System;
using Vexe.Editor.Types;
using Vexe.Runtime.Extensions;
using Vexe.Runtime.Types;

namespace Vexe.Editor.Drawers 
{

    public abstract class AttributeDrawer<A> : BaseDrawer where A : DrawnAttribute 
    {
        protected A attribute { private set; get; }

        protected sealed override void InternalInitialize() 
        {
            attribute = attributes.GetAttribute<A>();
        }

    }


    public abstract class AttributeDrawer<T, A> : ObjectDrawer<T> where A : DrawnAttribute
	{
		protected A attribute { private set; get; }

		protected sealed override void InternalInitialize()
		{
			attribute = attributes.GetAttribute<A>();
		}

        public override bool CanHandle(Type memberType)
        {
            return memberType.IsA<T>() || memberType.IsSubclassOrImplementerOfRawGeneric(typeof(T));
        }
    }
}