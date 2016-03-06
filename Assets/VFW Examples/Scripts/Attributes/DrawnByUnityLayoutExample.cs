using UnityEngine.Events;
using Vexe.Runtime.Types;

namespace VFWExamples
{
	public class DrawnByUnityLayoutExample : BaseBehaviour
	{
        public Index2D drawnByVfwLayout;

        [DrawByUnity]
        public Index2D drawnByUnityLayout;

        // Unity events are by default drawn by Unity's layout
        public UnityEvent callback;
	}
}