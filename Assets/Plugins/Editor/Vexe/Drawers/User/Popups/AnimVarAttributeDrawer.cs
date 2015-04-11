using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using Vexe.Runtime.Extensions;
using Vexe.Runtime.Types;

namespace Vexe.Editor.Drawers
{
	public class AnimatorVariableAttributeDrawer : AttributeDrawer<string, AnimVarAttribute>
	{
		private string[] _variables;
		private int _current;

		private Animator _animator;
		private Animator animator
		{
			get
			{
				if (_animator == null)
				{
					string getterMethod = attribute.GetAnimatorMethod;
					if (getterMethod.IsNullOrEmpty())
						_animator = gameObject.GetComponent<Animator>();
					else
						_animator = targetType.GetMethod(getterMethod, Flags.InstanceAnyVisibility)
											  .Invoke(rawTarget, null) as Animator;
				}
				return _animator;
			}
		}

		protected override void OnSingleInitialization()
		{
			if (memberValue == null)
				memberValue = "";

			if (animator != null && animator.runtimeAnimatorController != null)
				FetchVariables();
		}

		private void FetchVariables()
		{
			_variables = animator.parameters.Select(x => x.name).ToArray();
			if (_variables.IsEmpty())
				_variables = new[] { "N/A" };
			else
			{
				if (!attribute.AutoMatch.IsNullOrEmpty())
				{
					string match = niceName.Remove(niceName.IndexOf(attribute.AutoMatch));
					match = Regex.Replace(match, @"\s+", "");
					if (_variables.ContainsValue(match))
						memberValue = match;
				}
				_current = _variables.IndexOfZeroIfNotFound(memberValue);
			}
		}

		public override void OnGUI()
		{
			if (animator == null || animator.runtimeAnimatorController == null)
			{
				memberValue = gui.Text(niceName, memberValue);
			}
			else
			{
				if (_variables.IsNullOrEmpty())
					FetchVariables();

				var selection = gui.Popup(niceName, _current, _variables);
				{
					if (_current != selection || memberValue != _variables[selection])
						memberValue = _variables[_current = selection];
				}
			}
		}
	}
}