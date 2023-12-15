using IMX.V500;
using System.Reflection;
using System.Globalization;

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
			const double tolerance = 2.0;
			Type type = first.GetType();

			if (type != second.GetType()) {
				return false;
			}

			PropertyInfo? atMeasureProperty = type.GetProperty("atMeasure");

			if (atMeasureProperty != null && atMeasureProperty.PropertyType == typeof(double)) {
				return Math.Abs(((double)atMeasureProperty.GetValue(first)! - (double)atMeasureProperty.GetValue(second)!)) < tolerance;
			}

			PropertyInfo? pointProperty = type.GetProperties().FirstOrDefault(prop => prop.PropertyType.IsSubclassOf(typeof(tPoint)));

			if (pointProperty != null) {
				tPoint p = (tPoint)pointProperty.GetValue(first)!;
				tPoint q = (tPoint)pointProperty.GetValue(second)!;

				(double, double, double) pPoints = Points(p.Point);
				(double, double, double) qPoints = Points(q.Point);

				double distance = Math.Sqrt(
					pPoints.Item1 * qPoints.Item1 +
					pPoints.Item2 * qPoints.Item2 +
					pPoints.Item3 * qPoints.Item3
				);

				return distance < tolerance;
			}

			return first.GetType().GetProperties().All(prop => prop.GetValue(first, null)?.Equals(prop.GetValue(second, null)) ?? false);
		}

		private static (double, double, double) Points(Point point) {
			string[] values = point.coordinates.Split(',');

			return (
				double.Parse(values[0], CultureInfo.InvariantCulture),
				double.Parse(values[1], CultureInfo.InvariantCulture),
				double.Parse(values[2], CultureInfo.InvariantCulture)
			);
		}
	}
}
