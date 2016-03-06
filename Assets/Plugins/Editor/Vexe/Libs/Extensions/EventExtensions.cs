using UnityEngine;

namespace Vexe.Editor.Extensions
{
    public static class EventsExtensions
    {
        public static bool IsLMB(this Event e) { return e.button == 0; }
        public static bool IsRMB(this Event e) { return e.button == 1; }
        public static bool IsMMB(this Event e) { return e.button == 2; }
        public static bool IsLMBDown(this Event e) { return IsMouseDown(e) && IsLMB(e); }
        public static bool IsRMBDown(this Event e) { return IsMouseDown(e) && IsRMB(e); }
        public static bool IsMMBDown(this Event e) { return IsMouseDown(e) && IsMMB(e); }
        public static bool IsDoubleClick(this Event e) { return e.clickCount == 2; }
        public static bool IsMouseDown(this Event e) { return e.type == EventType.MouseDown; }
        public static bool IsMouseContained(this Event e, Rect rect) { return rect.Contains(e.mousePosition); }
    } 
}
