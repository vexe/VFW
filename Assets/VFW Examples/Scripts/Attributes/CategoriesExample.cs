using UnityEngine;
using Vexe.Runtime.Types;

namespace VFWExamples
{
	[DefineCategory("NonExclusiveStrings", 2.5f, DataType = typeof(string), Pattern = "^nonEx", Exclusive = false)]
	[DefineCategory("ExclusiveInts", 2.6f, DataType = typeof(int), Pattern = "^ex")]
	[DefineCategory("Unite", 2.7f, DataType = typeof(void), MemberType = CategoryMemberType.Property, Pattern = "^nonEx", Grouping = CategorySetOp.Union, Exclusive = false)]
	[DefineCategory("Explicit", 2.8f, "otherString1", "exInt1", "floatProp1", "CustomMethod")]
	[DefineCategory("Fields/Custom")]
	public class CategoriesExample : BaseBehaviour
	{
		private const string Custom = "Fields/Custom";

		public string nonExString1;
		public string nonExString2;
		public string nonExString3;

		public string otherString1;
		public string otherString2;

		public int exInt1;
		public int exInt2;
		public int exInt3;

		public float floatProp1 { get; set; }
		public float floatProp2 { get; set; }

		[Show]
		private void TestMethod1()
		{
			print("TestMethod1");
		}
		[Show]
		private void TestMethod2()
		{
			print("TestMethod2");
		}

		[Show, Category(Custom)]
		public float CustomProp { get; set; }

		[Show, Category(Custom)]
		public void CustomMethod()
		{
			LogFormat("CustomMethod");
		}

		public Color dbgColor;
		public float dbgFloat;
	}

	// <<< Q/A >>> //
	/* Q: What is a category?
	 * A: It's a logical way of combining members (fields, properties and methods) together in one group in the inspector
	 *		You can use filters to customly add members to a category
	 *		You can filter by DataType, MemberType, Pattern, Grouping and ExplicitMembers
	 * 
	 * Q: I didn't define a "Fields", "Properties" nor "Methods" categories, where did they come from?
	 * A: When you inherit BetterBehaviour, you automatically get 4 non-exclusive categories:
	 *		a) "Fields": For all visible fields
	 *		b) "Properties": For all visible properties
	 *		c) "Methods" For all visible methods
	 *		d) "Debug" For any member whose name start with "dbg"
	 *		
	 * Q: What defines a visible member?
	 * A:	a) Fields: A visible field is defined as a field that's either public or non public annotated with Serialize whose type is supported by Unity's serialization system
	 *		b) Property: Any property annotated with [Show] regardless of access modifier
	 *		c) Method: Any method annotated with [Show] regardless of access modifier
	 *		
	 * Q: What is an exclusive category?
	 * A: By default a category is exlusive unless the Exclusive flag is set to false
	 *		When a category is exclusive its members won't be included in other categories even if they're defined in them
	 *		So for ex, "Fields" is non exclusive, for ex "ExclusiveInts" is an exclusive category for ints starting with "ex"
	 *		all ints included in "ExclusiveInts" won't be available for the "Fields" category.
	 *		Change the definition of "ExclusiveInts" to be non exclusive to have its members visible in other categories who require them
	 *		
	 * Q: How is a category definition resolved?
	 * A: Like you've probably seen, there's many ways you could filter and categorize your fields
	 *		If you don't specify a "Grouping" - then the results of all the filter/resolver will get intersected to get the final result
	 *		Ex: A category with `MemberType = MemberType.Field, DataType = typeof(string), Pattern = "^test"`
	 *		Let's take it one by one:
	 *		1- `MemberType = MemberType.Field`: The result is 'all' the visible fields
	 *		2- `DataType = typeof(string)`:		The result is 'all' members whose 'data type' is of type string (i.e. string fields, string properties and methods with string return)
	 *		3- `Pattern = "^test"`:				The result is 'all' members whose name start with the word "test"
	 *		The final result:					All fields whose data type is string, and whose name start with the word "test"
	 *		If you specify SetOp.Union for your Grouping, the final result would be different, basically combine the results and pick the distinct members
	 *		So the result of a union would be:	All fields and all members whose data type is string and all members whose name start with "test"
	 *		
	 * Q: What is the order of resolution? (resolving categories)
	 * A: Exclusive ones come first, then the non exclusive come after
	 * 
	 * Q: Can you talk a bit about how are things defined above?
	 * A: Sure:
	 *		1- We already talked about "Fields", "Properties" and "Methods"
	 *		2- NonExclusiveStrings:
	 *			a) Has a display order of 2.5, which means it will appear right after "Methods" but before "Debug"
	 *			b) It's non-exclusive meaning any member included in it could appear in other categories (thus they're not 'exclusive' to this category)
	 *			c) No grouping is defined, so "Intersection" is used
	 *			d) Uses a data type of `string` and a pattern of "^nonEx" so it will capture any string member whose name start with "nonEx"
	 *		3- ExclusiveInts:
	 *			a) No need to talk about the display order anymore, you got the idea...
	 *			b) It's exclusive, meaning any member defined in it won't appear in any other category
	 *			c) Intersection grouping is used as well
	 *			d) Uses a data type of `int` and a pattern of "^ex" so it will capture any ints whose name start with "ex"
	 *		4- Unite:
	 *			c) Union grouping is specified which means the end result is the distintion of the combination of all the resolvers/filters (i.e. the combination of the filters, without duplicates)
	 *			d) Uses a data type of void, member type of Property, and a pattern of "^nonEx" which means all members whose data type is void (methods), all properties and all members whose name start with "nonEx"
	 *		5- Explicit:
	 *			d) Uses explicit member adding by name
	 *		6- Fields/Custom:
	 *			Doesn't specify anything - instead members include themselves by using CategoryMember
	 *	
	 * Q: Looking at the results in the inspector, some things are different from what you described,
	 *		for ex you said the "Unite" should contain all visible properties, but it doesn't, it only has "floatProp2" what about "floatProp1" and "CustomProp"?
	 *		And where's "exInt1" in "Explicit"?
	 * A: The answer to all of these questions lies in the "Exclusive" flag:
	 *		1- "CustomProp" is exclusive to "Fields/Custom", "floatProp1" is exclusive to "Explicit" that's why they didn't appear in "Unite"
	 *		2- "exInt1" is exclusive to "ExclusiveInts" that's why it didn't appear in "Explicit"
	 */
}
