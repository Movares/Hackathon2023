using IMX.V500;
using System.Collections;
using System.Reflection;

namespace Terminal {
	internal static class IMXObjects {
		public static object[] Get(ImSpoor imx) {
			return GetSubObjects(imx, new Stack<Type>()).ToArray();
		}

		public static object[] GetNonIMX(ImSpoor imx) {
			return Get(imx).Where(obj => obj is not tBaseObject).ToArray();
		}

		public static tBaseObject[] GetBase(ImSpoor imx) {
			return Get(imx).OfType<tBaseObject>().ToArray();
		}

		public static string[] GetPuics(ImSpoor model) {
			return GetBase(model).Select(obj => obj.puic).ToArray();
		}

		private static List<object> GetSubObjects(object obj, Stack<Type> types) {
			PropertyInfo[] properties = obj.GetType().GetProperties();
			List<object> result = new();

			if (types.Contains(obj.GetType())) {
				return result;
			}

			types.Push(obj.GetType());

			foreach (PropertyInfo property in properties) {
				object value = property.GetValue(obj, null)!;

				if (value == null) {
					continue;
				}

				if (IsImxObject(value)) {
					result.Add(value);
				}

				if (value is IEnumerable enumerable) {
					foreach (var item in enumerable) {
						if (IsImxObject(item)) {
							result.Add(item);
						}

						foreach (object tIMXObject in GetSubObjects(item, types)) {
							result.Add(tIMXObject);
						}
					}
				} else {
					result.AddRange(GetSubObjects(value, types));
				}
			}

			types.Pop();

			return result;
		}

		private static bool IsImxObject(object obj) {
			Type[] special = new[] {
				typeof(tSwitchMechanismPosition),
				typeof(tErtmsExtension),
				typeof(tProjectArea),
			};

			return obj is tIMXObject || special.Any(t => obj.GetType().IsSubclassOf(t));
		}
	}
}
