using UnityEditor;
using Vexe.Editor.Types;
using Vexe.Runtime.Types;
using UnityObject = UnityEngine.Object;

namespace Vexe.Editor.Drawers
{
	public abstract class BaseRequirementDrawer<T> : CompositeDrawer<UnityObject, T> where T : RequiredAttribute
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

	public class RequiredFromThisDrawer : BaseRequirementDrawer<RequiredFromThisAttribute>
	{
		protected override string GetHelpMsg()
		{
			return "Couldn't resolve member from this gameObject";
		}
	}

	public class RequiredSingleDrawer : BaseRequirementDrawer<RequiredSingleAttribute>
	{
		protected override string GetHelpMsg()
		{
		  return "Couldn't find a single object in the scene to assign to member";
		}
	}

	public class RequiredDrawer : BaseRequirementDrawer<RequiredAttribute>
	{
		protected override string GetHelpMsg()
		{
		  return "Member assignment is required";
		}
	}

	public class RequiredFromChildrenDrawer : BaseRequirementDrawer<RequiredFromChildrenAttribute>
	{
		protected override string GetHelpMsg()
		{
			return "Couldn't resolve member from children";
		}
	}

	public class RequiredFromParentsDrawer : BaseRequirementDrawer<RequiredFromParentsAttribute>
	{
		protected override string GetHelpMsg()
		{
			return "Couldn't resolve member from parents";
		}
	}
}