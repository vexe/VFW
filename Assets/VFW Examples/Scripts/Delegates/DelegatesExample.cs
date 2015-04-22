using System.Collections.Generic;
using UnityEngine;
using VFWExamples;

namespace Vexe.Runtime.Types
{
	[DefineCategories("Actions", "Funcs", "Collections")]
	public class DelegatesExample : BetterBehaviour
	{
		const string Actions     = "Actions";
		const string Funcs       = "Funcs";
		const string Collections = "Collections";

		[Category(Actions)] public uAction action0;
		[Category(Actions)] public uAction<string> Action1 { get; set; } 
		[Category(Actions)] public uAction<GameObject, Vector3> action2;
		[Category(Actions)] public uAction<ITest, Transform, float> Action3 { get; set; }

		[Category(Funcs)] public uFunc<string> func0;
		[Category(Funcs)] public uFunc<int, bool> Func1 { get; set; }
		[Category(Funcs)] public uFunc<GameObject, Vector3, string> func2;
		[Category(Funcs)] public uFunc<ITest, Transform, float, string> Func3 { get; set; }

		[Category(Collections)] public uAction[] actions0;
		[Category(Collections)] public List<uAction<string>> actions1;
		[Category(Collections)] public Dictionary<string, List<uAction>> dict;

		public uDelegate kickass;

		[Hide]
		public void Action0Method0()
		{
			LogFormat("Action0Method0");

			action0.Add(Action0Method1);
			action0.Invoke();
		}

		public void Action0Method1()
		{
			LogFormat("Action0Method1");
		}

		public void Action1Method(string arg0)
		{
			LogFormat("Action1Method {0}", arg0);
		}

		public void Action2Method(GameObject arg0, Vector3 arg1)
		{
			LogFormat("Action0Method {0}, {1}", arg0, arg1);
		}

		public void Action3Method(ITest arg0, Transform arg1, float arg2)
		{
			LogFormat("Action1Method {0}, {1}, {2}", arg0, arg1, arg2);
		}

		public string Func0Method()
		{
			LogFormat("Func0Method");
			return string.Empty;
		}

		public bool Func1Method(int arg0)
		{
			LogFormat("Func1Method {0}", arg0);
			return arg0 > 0;
		}

		public string Func2Method(GameObject arg0, Vector3 arg1)
		{
			LogFormat("Func0Method {0}, {1}", arg0, arg1);
			return arg1.ToString();
		}

		public string Func3Method(ITest arg0, Transform arg1, float arg2)
		{
			LogFormat("Func1Method {0}, {1}, {2}", arg0, arg1, arg2);
			return arg2.ToString();
		}
	}
}