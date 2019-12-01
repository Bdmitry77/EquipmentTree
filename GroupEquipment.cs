using System.Collections.Generic;

namespace EquipmentTree
{
	public class GroupEquipment
	{
		public List<Equipment> Devices { get; set; } = new List<Equipment>();

		public string Name { get; set; }
	}
}
