using System;
using UnityEngine;

namespace Vexe.Runtime.Helpers
{
    public static class EventsHelper
    {
        /// <summary>
        /// Returns a Rename event (keyCode as F2 and a KeyDown event type)
        /// </summary>
        public static Event GetRenameEvent() { return new Event() { keyCode = KeyCode.F2, type = EventType.KeyDown }; }

        /// <summary>
        /// Performs the specified action when a Unity Undo/Redo has been performed
        /// </summary>
        /// <param name="action"></param>
        public static void OnUndoRedoPerformed(Action action)
        {
            if (Event.current == null) return;
            if (Event.current.type == EventType.ValidateCommand)
                if (Event.current.commandName == "UndoRedoPerformed")
                    action();
        }

        public static Event GetLMBMouseDownEvent() { return new Event { type = EventType.MouseDown, keyCode = KeyCode.Mouse0 }; }
        public static bool IsDoubleClick() { return Event.current.clickCount == 2; }
        public static bool IsMouseDown() { return Event.current.type == EventType.MouseDown; }
        public static bool IsLMB() { return Event.current.button == 0; }
        public static bool IsRMB() { return Event.current.button == 1; }
        public static bool IsMMB() { return Event.current.button == 2; }
        public static bool IsLMBMouseDown() { return IsMouseDown() && IsLMB(); }
        public static bool IsRMBMouseDown() { return IsMouseDown() && IsRMB(); }
        public static bool IsMMBMouseDown() { return IsMouseDown() && IsMMB(); }
    }
}