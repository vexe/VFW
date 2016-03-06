using UnityEngine;
using System.Collections;

namespace Vexe.Runtime.Types
{
	public static class DelegateSettings
	{
		/// <summary>
		/// Any method name defined in this array will be ignored in the delegates methods popup
		/// </summary>
		public static string[] IgnoredMethods =
		{
			"CancelInvoke",
			"StopAllCoroutines",
		};
	}
}