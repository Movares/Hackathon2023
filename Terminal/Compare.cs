using IMX.V500;
using System.Reflection;

namespace Terminal {
	public static class Compare {
		static readonly Dictionary<Type, List<PropertyInfo>> keys = new Dictionary<Type, List<PropertyInfo>> {
			[typeof(MicroNode)] = new List<PropertyInfo> { typeof(MicroNode).GetProperty("junctionRef")! },
			[typeof(MicroLink)] = new List<PropertyInfo> { typeof(MicroLink).GetProperty("implementationObjectRef")! },
			[typeof(FlankProtectionConfiguration)] = new List<PropertyInfo> {
					typeof(FlankProtectionConfiguration).GetProperty("switchMechanismRef")!,
					typeof(FlankProtectionConfiguration).GetProperty("position")!,
				},
			[typeof(ErtmsBaliseGroup)] = new List<PropertyInfo> { typeof(MicroNode).GetProperty("baliseGroupRef")! },
			[typeof(ErtmsLevelCrossing)] = new List<PropertyInfo> { typeof(MicroNode).GetProperty("levelCrossingRef")! },
			[typeof(ErtmsRoute)] = new List<PropertyInfo> { typeof(MicroNode).GetProperty("routeRef")! },
			[typeof(ErtmsSignal)] = new List<PropertyInfo> { typeof(MicroNode).GetProperty("signalRef")! },
			[typeof(EuroBalise)] = new List<PropertyInfo> { typeof(MicroNode).GetProperty("baliseRef")! },
		};

		public static bool IsEqual(object first, object second) {
			Type type = first.GetType();

			if (type != second.GetType()) {
				return false;
			}

			if (first is tBaseObject firstAsBase && second is tBaseObject secondAsBase) {
				return IsBaseObjectEqual(firstAsBase, secondAsBase);
			}

			if (HasRef(first)) {
				return IsNonBaseObjectWithRefEqual(first, second);
			}

			return IsNonBaseWithoutRefEqual(first, second);

		}

		public static bool HasRef(object obj) {
			return keys.ContainsKey(obj.GetType());
		}

		public static bool IsBaseObjectEqual(tBaseObject first, tBaseObject second) {
			return first.puic == second.puic;
		}

		public static bool IsNonBaseObjectWithRefEqual(object first, object second) {
			return first.GetType() == second.GetType() && keys[first.GetType()].All(prop => prop.GetValue(first)!.Equals(prop.GetValue(second)!));
		}

		public static bool IsNonBaseWithoutRefEqual(object first, object second) {
			return first.GetType() == second.GetType() && first.GetType().GetProperties().All(prop => prop.GetValue(first, null)!.Equals(prop.GetValue(second, null)!));
		}
	}
}
