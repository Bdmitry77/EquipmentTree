using System;

namespace EquipmentTree
{
	/// <summary>
	/// Simple System.Random with inclusive maxValue
	/// </summary>
	public static class CustomRandom
	{
		public static int Next(int minValue, int maxValue)
		{
			return new Random(DateTime.Now.Millisecond).Next(minValue, maxValue + 1);
		}
	}
}
