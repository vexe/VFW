using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using Vexe.Runtime.Extensions;
using Vexe.Runtime.Types;
using MembersList = System.Collections.Generic.List<System.Reflection.MemberInfo>;

namespace Vexe.Editor.Internal
{
    /// <summary>
    /// Responsible for the resolution of a category definition (what members are categorized in that cateogry)
    /// and determining how the members are combined in that category (united or intersected)
    /// </summary>
	public class CategoryDefinitionResolver
	{
		readonly MembersList _excluded;
		readonly Func<MembersList, DefineCategoryAttribute, MembersList>[] _defres;
		readonly Func<MembersList, DefineCategoryAttribute, MembersList> _memres;

		public CategoryDefinitionResolver()
		{
			_excluded = new MembersList();

			// member resolver (when including members to a certain category via attributes)
			_memres = (input, def) =>
			{
				var output = new MembersList();
				output.AddRange(input.Where(m =>
				{
					var memberDef = m.GetCustomAttribute<CategoryAttribute>();
					return memberDef != null && memberDef.name == def.FullPath;
				}));
				return output;
			};

			_defres = new Func<MembersList, DefineCategoryAttribute, MembersList>[]
			{
				// regex pattern resolver
				(input, def) =>
				{
					var output = new MembersList();
					var pattern = def.Pattern;
					if (!pattern.IsNullOrEmpty())
						output.AddRange(input.Where(member => Regex.IsMatch(member.Name, pattern)));
					return output;
				},

				// return type resolver
				(input, def) =>
				{
					var output = new MembersList();
					var returnType = def.DataType;
					if (returnType != null)
						output.AddRange(input.Where(m => m.GetDataType().IsA(returnType)));
					return output;
				},

				// member type resolver
				(input, def) =>
				{
					var output = new MembersList();
					Predicate<CategoryMemberType> isMemberTypeDefined = mType => (def.MemberType & mType) > 0;
					output.AddRange(input.Where(m => isMemberTypeDefined((CategoryMemberType)m.MemberType)));
					return output;
				},

				// explicit members resolver
				(input, def) =>
				{
					var output = new MembersList();
					var explicitMembers = def.ExplicitMembers;
					output.AddRange(input.Where(m => explicitMembers.Contains(m.Name)));
					return output;
				},
			};
		}

		public MembersList Resolve(MembersList input, DefineCategoryAttribute definition)
		{
			var result = new MembersList();

			var defMembers = _defres.Select(r => r.Invoke(input, definition))
								   .Where(g => !g.IsEmpty())
								   .Cast<IEnumerable<MemberInfo>>().ToArray();

			if (!defMembers.IsEmpty())
			{
				switch (definition.Grouping)
				{
					case CategorySetOp.Intersection:
						result.AddRange(defMembers.Aggregate((g1, g2) => g1.Intersect(g2)));
						break;
					case CategorySetOp.Union:
						result.AddRange(defMembers.UnionAll());
						break;
				}
			}

			// Solve members annotated with CategoryAttribute
			_memres.Invoke(input, definition).Foreach(result.Add);

			// Filter out excluded members
			result.RemoveAll(_excluded.Contains);

			// If this definition's members are exclusive (doesn't allow dups)
			// we maintain a ref to its members to exclude them from other defs
			if (definition.Exclusive)
				_excluded.AddRange(result);

			return result;
		}
	}
}