using Vexe.Editor.Types;
using Vexe.Runtime.Extensions;

namespace Vexe.Editor.Drawers
{
	public class NullableDrawer<T> : ObjectDrawer<T?> where T : struct
	{
		private EditorMember nullableMember;

		protected override void OnSingleInitialization()
		{
			nullableMember = EditorMember.WrapGetSet(
				@get          : member.Get,
				@set          : member.Set,
				@rawTarget    : member.RawTarget,
				@unityTarget  : unityTarget,
				@dataType     : typeof(T),
				@name         : niceName,
				@id           : id,
                @attributes   : attributes
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