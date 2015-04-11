using Vexe.Editor.Types;
using Vexe.Runtime.Extensions;

namespace Vexe.Editor.Drawers
{
	public class NullableDrawer<T> : ObjectDrawer<T?> where T : struct
	{
		private EditorMember nullableMember;

		protected override void OnSingleInitialization()
		{
			nullableMember = new ArgMember(
				@getter        : member.Get,
				@setter        : member.Set,
				@target        : member.Target,
				@unityTarget   : unityTarget,
				@dataType      : typeof(T),
				@attributes    : attributes,
				@name          : niceName,
				@id            : id
			);
		}

		public override void OnGUI()
		{
			if (!memberValue.HasValue)
				memberValue = (T)typeof(T).GetDefaultValue();

			MemberField(nullableMember);
		}
	}
}