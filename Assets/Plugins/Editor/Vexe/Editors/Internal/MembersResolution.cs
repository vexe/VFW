using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using Vexe.Runtime.Extensions;
using Vexe.Runtime.Types;
using MembersGroup = System.Collections.Generic.List<System.Reflection.MemberInfo>;

namespace Vexe.Editor.Internal
{
	public class MembersResolution
	{
		private readonly MembersGroup excluded;
		private DefineCategoryAttribute definition;

		readonly Func<MembersGroup, DefineCategoryAttribute, MembersGroup>[] defres;
		readonly Func<MembersGroup, DefineCategoryAttribute, MembersGroup> memres;

		public MembersResolution()
		{
			excluded = new MembersGroup();

			// member resolver (when including members to a certain category via attributes)
			memres = (input, def) =>
			{
				var output = new MembersGroup();
				output.AddRange(input.Where(m =>
				{
					var memberDef = m.GetCustomAttribute<CategoryAttribute>();
					return memberDef != null && memberDef.name == def.FullPath;
				}));
				return output;
			};

			defres = new Func<MembersGroup, DefineCategoryAttribute, MembersGroup>[]
			{
				// regex pattern resolver
				(input, def) =>
				{
					var output = new MembersGroup();
					var pattern = def.Pattern;
					if (!pattern.IsNullOrEmpty())
						output.AddRange(input.Where(member => Regex.IsMatch(member.Name, pattern)));
					return output;
				},

				// return type resolver
				(input, def) =>
				{
					var output = new MembersGroup();
					var returnType = def.DataType;
					if (returnType != null)
						output.AddRange(input.Where(m => m.GetDataType().IsA(returnType)));
					return output;
				},

				// member type resolver
				(input, def) =>
				{
					var output = new MembersGroup();
					Predicate<MemberType> isMemberTypeDefined = mType => (def.MemberType & mType) > 0;
					output.AddRange(input.Where(m => isMemberTypeDefined((MemberType)m.MemberType)));
					return output;
				},

				// explicit members resolver
				(input, def) =>
				{
					var output = new MembersGroup();
					var explicitMembers = def.ExplicitMembers;
					output.AddRange(input.Where(m => explicitMembers.Contains(m.Name)));
					return output;
				},
			};
		}

		public MembersGroup Resolve(MembersGroup input, DefineCategoryAttribute definition)
		{
			this.definition = definition;
			return Resolve(input);
		}

		public MembersGroup Resolve(MembersGroup input)
		{
			var result = new MembersGroup();

			var defMembers = defres.Select(r => r.Invoke(input, definition))
								   .Where(g => !g.IsEmpty())
								   .Cast<IEnumerable<MemberInfo>>().ToArray();

			if (!defMembers.IsEmpty())
			{
				switch (definition.Grouping)
				{
					case SetOp.Intersection:
						result.AddRange(defMembers.Aggregate((g1, g2) => g1.Intersect(g2)));
						break;
					case SetOp.Union:
						result.AddRange(defMembers.UnionAll());
						break;
				}
			}

			// Solve members annotated with CategoryAttribute
			memres.Invoke(input, definition).Foreach(result.Add);

			// Filter out excluded members
			result.RemoveAll(excluded.Contains);

			// If this definition's members are exclusive (doesn't allow dups)
			// we maintain a ref to its members to exclude them from other defs
			if (definition.Exclusive)
				excluded.AddRange(result);

			return result;
		}
	}
}