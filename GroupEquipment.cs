using System;
using System.Collections.Generic;
using System.Linq;

namespace EquipmentTree
{
	public class GroupEquipment
	{
		public List<Equipment> Devices { get; } = new List<Equipment>();

		public string Name { get;  }

		public GroupEquipment(string name)
		{
			Name = name;
		}

		public void AddEquipment(Equipment equipment)
		{
			if (equipment == null)
				throw new ArgumentNullException(nameof(equipment));

			if (Devices.Any(x => x.Id == equipment.Id))
				throw new Exception($"Equipment with Id '{equipment.Id}' exists");

			Devices.Add(equipment);
		}

		public Equipment GetEquipmentById(int targetId)
		{
			return Devices.FirstOrDefault(x => x.Id == targetId);
		}

		public Equipment GetRandomEquipment()
		{
			Random random = new Random(DateTime.Now.Millisecond);

			var deviceIndex = random.Next(0, Devices.Count);

			return Devices[deviceIndex];
		}

		public Equipment GetEquipmentByIndex(string index)
		{
			if (int.TryParse(index, out int equipmentNumber))
			{
				var equipment = Devices.ElementAtOrDefault(equipmentNumber);
				if (equipment == null)
					throw new Exception($"Equipment with index '{index}' not found.");

				return equipment;
			}
			else
				throw new Exception($"Wrong equipment index format '{index}'. Expected number.");
		}

		public void RemoveEquipment(Equipment equipment)
		{
			Devices.Remove(equipment);
		}
	}
}
