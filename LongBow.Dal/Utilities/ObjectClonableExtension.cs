namespace LongBow.Dal.Utilities
{
	public static class ObjectClonableExtension
	{
		public static T Clone<T>(this T myObject) where T : new()
		{
			var myObject2 = new T();

			var props = typeof(T).GetProperties(); // on récupère toutes les propriétés de la classe TradeField

			foreach (var prop in props)
			{
				var getMethod = prop.GetGetMethod(); // on récupère la méthode GET de la propriété
				var setMethod = prop.GetSetMethod(); // on récupère la méthode SET de la propriété

				if (getMethod == null || !getMethod.IsPublic || getMethod.IsStatic || setMethod == null || !setMethod.IsPublic ||
					setMethod.IsStatic) continue;

				var value = getMethod.Invoke(myObject, null); // on appelle la méthode GET sur baseField pour récupérer la valeur
				prop.SetValue(myObject2, value, null); // on appelle la méthode SET sur l'instance courante pour lui attribuer la valeur
			}

			return myObject2;
		}
	}
}
