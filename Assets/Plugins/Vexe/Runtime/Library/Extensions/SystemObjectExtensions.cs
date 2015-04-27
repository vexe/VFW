namespace Vexe.Runtime.Extensions
{
	public static class SystemObjectExtensions
	{
		public static bool GenericEquals<T>(this T x, T y)
		{
			if (typeof(T).IsValueType)
                return x.Equals(y);
			else
			{
				if (x != null)
                    return x.Equals(y);
				else if (y != null)
                    return y.Equals(x);
			}

			return true;
		}

		/// <summary>
		/// returns `obj == null || obj.Equals(null)` which is safer to use in generic methods
		/// if the object has an overriden == operator (useful when working with Unity types)
		/// </summary>
		public static bool IsObjectNull(this object obj)
		{
			return obj == null || obj.Equals(null);
		}
	}
}