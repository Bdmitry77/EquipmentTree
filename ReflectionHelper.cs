using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace EquipmentTree
{
	public static class ReflectionHelper
	{
		public static IEnumerable<Type> GetSubclassTypes<T>() where T : class
		{
			var assemblies = Assembly.GetAssembly(typeof(T))
				.GetTypes()
				.Where(myType => myType.IsClass
					&& !myType.IsAbstract
					&& myType.IsSubclassOf(typeof(T)));

			return assemblies;
		}

		public static IEnumerable<PropertyInfo> GetPropertyInfos<T>(T obj) where T : class
		{
			var type = obj.GetType();
			var properties = type.GetProperties();
			return properties;
		}
	}
}
