using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Vexe.Editor.GUIs
{
	public abstract partial class BaseGUI
	{
		public class LabelWidthBlock : IDisposable
		{
			private float prevWidth;

			public LabelWidthBlock Begin(float newWidth)
			{
				prevWidth = EditorGUIUtility.labelWidth;
				EditorGUIUtility.labelWidth = newWidth;
				return this;
			}

			public void Dispose()
			{
				EditorGUIUtility.labelWidth = prevWidth;
			}
		}

		public class StateBlock : IDisposable
		{
			private bool prevState;

			public StateBlock Begin(bool newState)
			{
				prevState = GUI.enabled;
				GUI.enabled = newState;
				return this;
			}

			public void Dispose()
			{
				GUI.enabled = prevState;
			}
		}

		public class ContentColorBlock : IDisposable
		{
			private Color prevColor;

			public ContentColorBlock Begin(Color newColor)
			{
				prevColor = GUI.contentColor;
				GUI.contentColor = newColor;
				return this;
			}

			public void Dispose()
			{
				GUI.contentColor = prevColor;
			}
		}

		public class GUIColorBlock : IDisposable
		{
			private Color prevColor;

			public GUIColorBlock Begin(Color newColor)
			{
				prevColor = GUI.color;
				GUI.color = newColor;
				return this;
			}

			public void Dispose()
			{
				GUI.color = prevColor;
			}
		}

		public class IndentBlock : IDisposable
		{
			private float current;
			private readonly BaseGUI gui;
			private Stack<float> indents;

			public IndentBlock(BaseGUI gui)
			{
				this.gui = gui;
				indents = new Stack<float>();
			}

			public IndentBlock Begin(GUIStyle style, float amount)
			{
				indents.Push(current);

				current += amount;

				gui.Horizontal();
				gui.Space(current);
				gui.Vertical(style);
				return this;
			}

			public void Dispose()
			{
				current = indents.Pop();

				gui.EndVertical();
				gui.EndHorizontal();
			}
		}
	}
}
