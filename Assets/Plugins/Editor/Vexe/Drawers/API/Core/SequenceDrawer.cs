//#define PROFILE

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Vexe.Editor.Types;
using Vexe.Editor.Windows;
using Vexe.Runtime.Extensions;
using Vexe.Runtime.Helpers;
using Vexe.Runtime.Types;
using UnityObject = UnityEngine.Object;

namespace Vexe.Editor.Drawers
{
	public abstract class SequenceDrawer<TSequence, TElement> : ObjectDrawer<TSequence> where TSequence : IList<TElement>
	{
		private readonly Type elementType;
		private string sequenceName;
		private int advancedKey;
		private List<ElementMember<TElement>> elements;
		private SequenceOptions options;
		private bool perItemDrawing;
		private bool shouldDrawAddingArea;
		private int newSize;

		private bool isAdvancedChecked
		{
			get { return prefs.Bools.ValueOrDefault(advancedKey); }
			set { prefs.Bools[advancedKey] = value; }
		}

		protected abstract TSequence GetNew();

		public SequenceDrawer()
		{
			elementType = typeof(TElement);
			elements    = new List<ElementMember<TElement>>();
		}

		protected override void OnSingleInitialization()
		{
			var seqOptions   = attributes.GetAttribute<SeqAttribute>();
			options          = new SequenceOptions(seqOptions != null ? seqOptions.options : SeqOpt.None);
			options.Readonly = options.Readonly || attributes.AnyIs<ReadonlyAttribute>();

			// Sequence name
			{
				var formatMember = attributes.GetAttribute<FormatMemberAttribute>();
				if (formatMember != null)
				{
					sequenceName = formatMember.Format(niceName, memberType.GetNiceName());
				}
				else
				{
					sequenceName = niceName + " (" + memberType.GetNiceName() + ")";
				}

				if (options.Readonly)
					sequenceName += " (Readonly)";
			}

			advancedKey          = RTHelper.CombineHashCodes(id, sequenceName, "advanced");
			perItemDrawing       = attributes.AnyIs<PerItemAttribute>();
			shouldDrawAddingArea = !options.Readonly && elementType.IsA<UnityObject>();
		}

		public override void OnGUI()
		{
			if (memberValue == null)
				memberValue = GetNew();

			bool showAdvanced = options.Advanced && !options.Readonly;

			// header
			using (gui.Horizontal())
			{
				foldout = gui.Foldout(sequenceName, foldout, Layout.sExpandWidth());

				gui.FlexibleSpace();

				if (showAdvanced)
					isAdvancedChecked = gui.CheckButton(isAdvancedChecked, "advanced mode");

				if (!options.Readonly)
				{
					using (gui.State(memberValue.Count > 0))
					{
						if (gui.ClearButton("elements"))
							Clear();
						if (gui.RemoveButton("last element"))
							RemoveLast();
					}
					if (gui.AddButton("element", MiniButtonStyle.ModRight))
						AddValue();
				}
			}

			if (!foldout) return;

			if (memberValue.IsEmpty())
			{
				gui.HelpBox("Sequence is empty");
				return;
			}

			// body
			using (gui.Vertical(options.GuiBox ? GUI.skin.box : GUIStyle.none))
			{
				// advanced area
				if (isAdvancedChecked)
				{
					using (gui.Indent((GUI.skin.box)))
					{
						using (gui.Horizontal())
						{
							newSize = gui.Int("New size", newSize);
							if (gui.MiniButton("c", "Commit", MiniButtonStyle.ModRight))
							{
								if (newSize != memberValue.Count)
									memberValue.AdjustSize(newSize, RemoveAt, AddValue);
							}
						}

						using (gui.Horizontal())
						{
							gui.Label("Commands");

							if (gui.MiniButton("Shuffle", "Shuffle list (randomize the order of the list's elements", (Layout)null))
								Shuffle();

							if (gui.MoveDownButton())
								memberValue.Shift(true);

							if (gui.MoveUpButton())
								memberValue.Shift(false);

							if (!elementType.IsValueType && gui.MiniButton("N", "Filter nulls"))
							{
								for (int i = memberValue.Count - 1; i > -1; i--)
									if (memberValue[i] == null)
										RemoveAt(i);
							}
						}
					}
				}

				using (gui.Indent(options.GuiBox ? GUI.skin.box : GUIStyle.none))
				{
#if PROFILE
					Profiler.BeginSample("Sequence Elements");
#endif
					for (int iLoop = 0; iLoop < memberValue.Count; iLoop++)
					{
						var i = iLoop;
						var elementValue = memberValue[i];

						using (gui.Horizontal())
						{
							if (options.LineNumbers)
							{
								gui.NumericLabel(i);
							}

							var previous = elementValue;

							gui.BeginCheck();
							{
								using (gui.Vertical())
								{
									var element = GetElement(i);
									gui.Member(element, !perItemDrawing);
								}
							}

							if (gui.HasChanged())
							{
								if (options.Readonly)
								{
									memberValue[i] = previous;
								}
								else if (options.UniqueItems)
								{
									int occurances = 0;
									for (int k = 0; k < memberValue.Count; k++)
									{
										if (memberValue[i].GenericEqual(memberValue[k]))
										{
											occurances++;
											if (occurances > 1)
											{
												memberValue[i] = previous;
												break;
											}
										}
									}
								}
							}

							if (isAdvancedChecked)
							{
								var c = elementValue as Component;
								var go = c == null ? elementValue as GameObject : c.gameObject;
								if (go != null)
									gui.InspectButton(go);

								if (showAdvanced)
								{
									if (gui.MoveDownButton())
										MoveElementDown(i);
									if (gui.MoveUpButton())
										MoveElementUp(i);
								}
							}

							if (!options.Readonly && options.PerItemRemove && gui.RemoveButton("element", MiniButtonStyle.ModRight))
							{
								RemoveAt(i);
							}
						}
					}
#if PROFILE
					Profiler.EndSample();
#endif
				}
			}

			// footer
			if (shouldDrawAddingArea)
			{
				Action<UnityObject> addOnDrop = obj =>
				{
					var go = obj as GameObject;
					object value;
					if (go != null)
					{
						value = elementType == typeof(GameObject) ? (UnityObject)go : go.GetComponent(elementType);
					}
					else value = obj;
					AddValue((TElement)value);
				};

				using (gui.Indent())
				{
					gui.DragDropArea<UnityObject>(
						@label: "+Drag-Drop+",
						@labelSize: 14,
						@style: EditorStyles.toolbarButton,
						@canSetVisualModeToCopy: dragObjects => dragObjects.All(obj =>
						{
							var go = obj as GameObject;
							var isGo = go != null;
							if (elementType == typeof(GameObject))
							{
								return isGo;
							}
							return isGo ? go.GetComponent(elementType) != null : obj.GetType().IsA(elementType);
						}),
						@cursor: MouseCursor.Link,
						@onDrop: addOnDrop,
						@onMouseUp: () => SelectionWindow.Show(new Tab<UnityObject>(
							@getValues: () => UnityObject.FindObjectsOfType(elementType),
							@getCurrent: () => null,
							@setTarget: item =>
							{
								AddValue((TElement)(object)item);
							},
							@getValueName: value => value.name,
							@title: elementType.Name + "s")),
						@preSpace: 2f,
						@postSpace: 35f,
						@height: 15f
					);
				}
				gui.Space(3f);
			}
		}

		private ElementMember<TElement> GetElement(int index)
		{
			if (index >= elements.Count)
			{
				var element = new ElementMember<TElement>(
					@attributes  : attributes,
					@name        : string.Empty,
					@id          : id + index
				);

				element.Initialize(memberValue, index, rawTarget, unityTarget);
				elements.Add(element);
				return element;
			}

			var e = elements[index];
			e.Initialize(memberValue, index, rawTarget, unityTarget);
			return e;
		}

		// List ops
		#region
		protected abstract void Clear();
		protected abstract void RemoveAt(int atIndex);
		protected abstract void Insert(int index, TElement value);

		private void Shuffle()
		{
			memberValue.Shuffle();
		}

		private void MoveElementDown(int i)
		{
			memberValue.MoveElementDown(i);
		}

		private void MoveElementUp(int i)
		{
			memberValue.MoveElementUp(i);
		}

		private void RemoveLast()
		{
			RemoveAt(memberValue.Count - 1);
		}

		private void SetAt(int index, TElement value)
		{
			if (!memberValue[index].GenericEqual(value))
				memberValue[index] = value;
		}

		private void AddValue(TElement value)
		{
			Insert(memberValue.Count, value);
		}

		private void AddValue()
		{
			AddValue((TElement)elementType.GetDefaultValueEmptyIfString());
		}
		#endregion

		private struct SequenceOptions
		{
			public bool Readonly;
			public bool Advanced;
			public bool LineNumbers;
			public bool PerItemRemove;
			public bool GuiBox;
			public bool UniqueItems;

			public SequenceOptions(SeqOpt options)
			{
				Func<SeqOpt, bool> contains = value =>
					(options & value) != 0;

				Readonly      = contains(SeqOpt.Readonly);
				Advanced      = contains(SeqOpt.Advanced);
				LineNumbers   = contains(SeqOpt.LineNumbers);
				PerItemRemove = contains(SeqOpt.PerItemRemove);
				GuiBox        = contains(SeqOpt.GuiBox);
				UniqueItems   = contains(SeqOpt.UniqueItems);
			}
		}
	}

	public class ArrayDrawer<T> : SequenceDrawer<T[], T>
	{
		protected override T[] GetNew()
		{
			return new T[0];
		}

		protected override void RemoveAt(int atIndex)
		{
			memberValue = memberValue.ToList().RemoveAtAndGet(atIndex).ToArray();
		}

		protected override void Clear()
		{
			memberValue = memberValue.ToList().ClearAndGet().ToArray();
		}

		protected override void Insert(int index, T value)
		{
			memberValue = memberValue.ToList().InsertAndGet(index, value).ToArray();
			foldout = true;
		}
	}

	public class ListDrawer<T> : SequenceDrawer<List<T>, T>
	{
		protected override List<T> GetNew()
		{
			return new List<T>();
		}

		protected override void RemoveAt(int index)
		{
			memberValue.RemoveAt(index);
		}

		protected override void Clear()
		{
			memberValue.Clear();
		}

		protected override void Insert(int index, T value)
		{
			memberValue.Insert(index, value);
			foldout = true;
		}
	}
}
