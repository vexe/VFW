using UnityEditor;
using Vexe.Editor.Types;
using Vexe.Runtime.Types;
using UnityObject = UnityEngine.Object;

namespace Vexe.Editor.Drawers
{
	public abstract class BaseRequirementAttributeDrawer<T> : CompositeDrawer<UnityObject, T> where T : RequiredAttribute
	{
		protected abstract string GetHelpMsg();
		
		public override void OnUpperGUI()
		{
			if (member.IsNull())
			{
				gui.HelpBox(GetHelpMsg(), MessageType.Warning);
			}
		}
	}

	public class RequiredFromThisAttributeDrawer : BaseRequirementAttributeDrawer<RequiredFromThisAttribute>
	{
		protected override string GetHelpMsg()
		{
			return "Couldn't resolve member from this gameObject";
		}
	}

	public class RequiredSingleAttributeDrawer : BaseRequirementAttributeDrawer<RequiredSingleAttribute>
	{
		protected override string GetHelpMsg()
		{
		  return "Couldn't find a single object in the scene to assign to member";
		}
	}

	public class RequiredAttributeDrawer : BaseRequirementAttributeDrawer<RequiredAttribute>
	{
		protected override string GetHelpMsg()
		{
		  return "Member assignment is required";
		}
	}

	public class RequiredFromChildrenAttributeDrawer : BaseRequirementAttributeDrawer<RequiredFromChildrenAttribute>
	{
		protected override string GetHelpMsg()
		{
			return "Couldn't resolve member from children";
		}
	}

	public class RequiredFromParentsAttributeDrawer : BaseRequirementAttributeDrawer<RequiredFromParentsAttribute>
	{
		protected override string GetHelpMsg()
		{
			return "Couldn't resolve member from parents";
		}
	}
}