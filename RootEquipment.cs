using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace EquipmentTree
{
	public class RootEquipment
	{
		public List<GroupEquipment> Groups { get; } = new List<GroupEquipment>();

		/// <summary>
		/// Initialize root random values
		/// </summary>
		/// <returns></returns>
		public void Initialize()
		{
			//Random groups (1-5)
			for (int i = 0; i < new Random().Next(1, 5); i++)
			{
				var group = new GroupEquipment(name: $"Group{i}");

				for (int k = 0; k < new Random().Next(1, 4); k++)
				{
					Equipment equipment = Equipment.CreateRandomEquipment();
					equipment.Name = $"Device_{i}_{k}";

					group.AddEquipment(equipment);
				}

				AddGroup(group);
			}
		}

		/// <summary>
		/// Random actions
		/// </summary>
		/// <param name="actionCount"></param>
		public void RandomActions(int actionCount)
		{
			for (int i = 0; i < actionCount; i++)
			{
				Thread.Sleep(2000);

				if (Groups.Count <= 0)
					return;

				var randomGroup = GetRandomGroup();

				if (randomGroup.Devices.Count <= 0)
					return;

				var randomEquipment = randomGroup.GetRandomEquipment();

				var randomIndex = CustomRandom.Next(0, 4);
				switch (randomIndex)
				{
					//Action depends on device type
					case 0:
						randomEquipment.RandomAction();
						break;
					//Delete from group
					case 1:
						RemoveEquipment(randomEquipment);
						break;
					//Add new random device to group
					case 2:
						var newEquipment = Equipment.CreateRandomEquipment();
						randomGroup.AddEquipment(newEquipment);
						break;
					//Move device to other group
					case 3:
						if (Groups.Count > 1)
						{
							var availableGroups = Groups.Where(x => x != null && !x.Devices.Any(y => y?.Id == randomEquipment.Id)).ToList();
							var randomGroupToMoveIndex = CustomRandom.Next(0, availableGroups.Count - 1);

							var destinationGroup = availableGroups.ElementAtOrDefault(randomGroupToMoveIndex);

							MoveToGroup(randomEquipment, destinationGroup);
						}
						break;
					//Rename
					case 4:
						randomEquipment.SetRandomName();
						break;
					default:
						break;
				}
			}
		}

		public void AddGroup(GroupEquipment groupEquipment)
		{
			if (groupEquipment == null)
				throw new ArgumentNullException(nameof(groupEquipment));

			this.Groups.Add(groupEquipment);
		}

		public GroupEquipment GetGroupByName(string groupName)
		{
			return Groups.FirstOrDefault(x => x.Name == groupName);
		}

		public GroupEquipment GetRandomGroup()
		{
			Random random = new Random(DateTime.Now.Millisecond);

			var groupIndex = random.Next(0, Groups.Count);
			return Groups[groupIndex];
		}

		public GroupEquipment GetGroupByIndex(string index)
		{
			if (int.TryParse(index, out int groupNumber))
			{
				var group = Groups.ElementAtOrDefault(groupNumber);
				if (group == null)
					throw new Exception($"Group with index '{index}' not found.");

				return group;
			}
			else
				throw new Exception($"Wrong group index format '{index}'. Expected number.");
		}

		public void RemoveEquipment(Equipment equipment)
		{
			var group = Groups.FirstOrDefault(x => x != null && x.Devices.Contains(equipment));
			group?.RemoveEquipment(equipment);
		}

		private void MoveToGroup(Equipment equipment, GroupEquipment destinationGroup)
		{
			var equipmentGroup = Groups.FirstOrDefault(x => x.Devices.Contains(equipment));

			equipmentGroup.RemoveEquipment(equipment);
			destinationGroup.AddEquipment(equipment);
		}

		public Equipment GetEquipmentById(string id)
		{
			if (int.TryParse(id, out int equipmentId))
			{
				var group = Groups.FirstOrDefault(x => x.Devices.Any(y => y.Id == equipmentId));
				if (group == null)
					throw new Exception($"Group with equipment id='{equipmentId}' not found.");

				var equipment = group.GetEquipmentById(equipmentId);

				return equipment;
			}
			else
				throw new Exception($"Wrong equipment id format '{id}'. Expected number.");
		}
	}
}
