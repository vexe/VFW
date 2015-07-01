using UnityEngine;
using Vexe.Runtime.Types;

namespace VFWExamples
{
    /// <summary>
    /// An example showing how to use the global event/messaging system
    /// The basic pattern is to subscribe/add handlers in OnEnable,
    /// unsubscribe/remove them in OnDisable,
    /// and raise them where the event/message should take place (when player dies, on item pickup etc)
    /// </summary>
	public class MessageExample : MonoBehaviour
	{
        // re-using a single event object
        private OnPlayerDied onPlayerDied = new OnPlayerDied();

		void OnEnable()
		{
			Message<OnPlayerDied>.Add(ReportPlayerDeath);
		}

		void OnDisable()
		{
			Message<OnPlayerDied>.Remove(ReportPlayerDeath);
		}

		void ReportPlayerDeath(OnPlayerDied e)
		{
			print(string.Format("Player {0} has died because of {1}", e.Player.name, e.CauseOfDeath));
		}

		void RunTest1(OnTest e)
		{
			print("Test1: Float: " + e.FloatValue + " Int: " + e.IntValue);
		}

		void RunTest2(OnTest e)
		{
			print("Test2: Float: " + e.FloatValue + " Int: " + e.IntValue);
		}

		void KillPlayer()
		{
			Message.Post(onPlayerDied.Set(transform, "JustBecause"));
		}

		void OnGUI()
		{
			if (GUILayout.Button("Kill player"))
				KillPlayer();

			if (GUILayout.Button("Sub ReportPlayerDeath"))
				Message<OnPlayerDied>.Add(ReportPlayerDeath);

			if (GUILayout.Button("Unsub ReportPlayerDeath"))
				Message<OnPlayerDied>.Remove(ReportPlayerDeath);

			if (GUILayout.Button("Is ReportPlayerDeath contained?"))
				print(Message<OnPlayerDied>.Contains(ReportPlayerDeath));

			if (GUILayout.Button("Clear OnPlayerDied"))
				Message<OnPlayerDied>.Clear();

			if (GUILayout.Button("Sub RunTest1"))
				Message<OnTest>.Add(RunTest1);

			if (GUILayout.Button("Unsub RunTest1"))
				Message<OnTest>.Remove(RunTest1);

			if (GUILayout.Button("Sub RunTest2"))
				Message<OnTest>.Add(RunTest2);

			if (GUILayout.Button("Unsub RunTest2"))
				Message<OnTest>.Remove(RunTest2);

			if (GUILayout.Button("Run tests"))
                Message.Post(new OnTest(10, 1.3f));

			if (GUILayout.Button("Is RunTest1 contained?"))
				print(Message<OnTest>.Contains(RunTest1));

			if (GUILayout.Button("Clear OnTest"))
				Message<OnTest>.Clear();
		}

        // for event objects, you could either use a mutable class and reuse it
		public class OnPlayerDied
		{
			public Transform Player;
			public string CauseOfDeath;

            public OnPlayerDied Set(Transform player, string causeOfDeath)
            {
                this.Player = player;
                this.CauseOfDeath = causeOfDeath;
                return this;
            }
        }

        // or an immutable struct (which I would personally recommend using instead)
		public struct OnTest
		{
			public readonly int IntValue;
			public readonly float FloatValue;

            public OnTest(int intValue, float floatValue)
            {
                this.IntValue = intValue;
                this.FloatValue = floatValue;
            }
		}
	}
}
