using System;
using UnityEditor;
using UnityEngine;
using Vexe.Editor.Helpers;
using UnityObject = UnityEngine.Object;

namespace Vexe.Editor.GUIs
{
	public abstract partial class BaseGUI
	{
		/// <summary>
		/// Creates a draggable label field (a label using a custom style making it resemble an object field)
		/// What if you wanted drop support? then you wouldn't be looking at this method, but one of the others above,
		/// like DraggablePropertyObjectField, and DragObjectField (they support dropping by default)
		/// or manually register your field for drop support via RegisterFieldForDrop, or even use DragAndDropObjectField.
		/// </summary>
		public void DraggableLabelField(string label, string field, UnityObject value, float labelWidth, GUIStyle style, MouseCursor cursor)
		{
			Label(label, labelWidth == 0 ? null : Layout.sWidth(labelWidth - 3.5f));
			Text(field);
			RegisterFieldForDrag(LastRect, value);
		}
		public void DraggableLabelField(string label, string field, UnityObject value, float labelWidth, GUIStyle style)
		{
			DraggableLabelField(label, field, value, labelWidth, style, MouseCursor.Link);
		}
		public void DraggableLabelField(string label, string field, UnityObject value, float labelWidth)
		{
			DraggableLabelField(label, field, value, labelWidth, null);
		}
		public void DraggableLabelField(string label, string field, UnityObject value)
		{
			DraggableLabelField(label, field, value, 0);
		}
		public void DraggableLabelField(string label, UnityObject value, float labelWidth)
		{
			DraggableLabelField(label, value == null ? "null" : value.name, value, labelWidth);
		}
		public void DraggableLabelField(string label, UnityObject value)
		{
			DraggableLabelField(label, value, 0);
		}
		public void DraggableLabelField(UnityObject value, float labelWidth = 0)
		{
			DraggableLabelField("", value, labelWidth);
		}
		public void DragDropArea<T>(
			string label, int labelSize, GUIStyle style,
			Predicate<UnityObject[]> canSetVisualModeToCopy, MouseCursor cursor,
			Action<T> onDrop, Action onMouseUp,
			float preSpace, float postSpace = 0f, float height = 0f) where T : UnityEngine.Object
		{
			using (Horizontal())
			{
				Space(preSpace);

				Box(label, style);

				var dropArea = LastRect;
				{
					// cache the current event
					Event currentEvent = Event.current;

					// if our mouse isn't contained within that box area, exit out
					if (dropArea.Contains(currentEvent.mousePosition))
					{
						GUIHelper.AddCursorRect(dropArea, cursor);

						if (onMouseUp != null && currentEvent.type == EventType.MouseUp)
							onMouseUp();

						if (onDrop != null)
						{
							if (currentEvent.type == EventType.DragUpdated ||
								currentEvent.type == EventType.DragPerform)
							{
								// set the visual mode to copy
								DragAndDrop.visualMode = canSetVisualModeToCopy(DragAndDrop.objectReferences) ?
									DragAndDropVisualMode.Copy : DragAndDropVisualMode.Rejected;

								// if we dropped something
								if (currentEvent.type == EventType.DragPerform)
								{
									// register that this drag-drop event has been handled by this control
									DragAndDrop.AcceptDrag();

									// loop over the dropped items and notify onDrop of each object
									for (int i = 0; i < DragAndDrop.objectReferences.Length; i++)
										onDrop(DragAndDrop.objectReferences[i] as T);
								}
								// since we've used the DragPerform event, we'll mark it as used
								// (its type will change to EventType.Used)
								// so that other Controls ignore it
								Event.current.Use();
							}
						}
					}
				}
				Space(postSpace);
			}
		}

		/* <<< Drag-n-Drop registration >>> */
		/// <summary>
		/// A couple of handy methods to register fields for drag-n-drop operations given what to drag/drop.
		/// </summary>
		#region
		/// <summary>
		/// Registers fieldRect for drag operations. dragObject is what's being dragged out of that field.
		/// </summary>
		public void RegisterFieldForDrag(Rect fieldRect, UnityObject dragObject)
		{
			if (dragObject == null) return;
			Event e = Event.current;
			if (fieldRect.Contains(e.mousePosition) && e.type == EventType.MouseDrag)
			{
				DragAndDrop.PrepareStartDrag();
				DragAndDrop.objectReferences = new[] { dragObject };
				DragAndDrop.StartDrag("drag");
				Event.current.Use();
			}
		}
		public void RegisterFieldForDrag(UnityObject dragObject)
		{
			RegisterFieldForDrag(LastRect, dragObject);
		}

        static Predicate<UnityObject[]> _alwaysAcceptDrop = objs => true;

		public T RegisterFieldForDrop<T>(Rect fieldRect, Func<UnityObject[], UnityObject> getDroppedObject) where T : UnityObject
        {
            return RegisterFieldForDrop<T>(fieldRect, getDroppedObject, _alwaysAcceptDrop);
        }

		/// <summary>
		/// Registers fieldRect for drop operations.
		/// Returns the dropped value
		/// </summary>
		public T RegisterFieldForDrop<T>(Rect fieldRect, Func<UnityObject[], UnityObject> getDroppedObject, Predicate<UnityObject[]> isDropAccepted) where T : UnityObject
		{
			Event currentEvent = Event.current;
			EventType eventType = currentEvent.type;
			T result = null;
			if (fieldRect.Contains(currentEvent.mousePosition) && (eventType == EventType.DragUpdated || eventType == EventType.DragPerform))
			{
                if (isDropAccepted(DragAndDrop.objectReferences))
                {
                    DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
                    if (eventType == EventType.DragPerform)
                    {
                        DragAndDrop.AcceptDrag();
                        result = getDroppedObject(DragAndDrop.objectReferences) as T;
                        currentEvent.Use();
                    }
                }
			}
			return result;
		}
		public T RegisterFieldForDrop<T>(Rect fieldRect) where T : UnityEngine.Object
		{
			return RegisterFieldForDrop<T>(fieldRect, objects => objects[0]);
		}
		#endregion
	}
}