using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;

namespace EquipmentTree
{
	public class RootEquipment
	{
		public List<GroupEquipment> Groups { get; set; }

		/// <summary>
		/// Initialize root random values
		/// </summary>
		/// <returns></returns>
		public void Initialize()
		{
			//Random groups (1-5)
			this.Groups = new List<GroupEquipment>();

			for (int i = 0; i < new Random().Next(1, 5); i++)
			{
				var group = new GroupEquipment()
				{
					Name = $"Group{i}"
				};

				var devices = new List<Equipment>();
				for (int k = 0; k < new Random().Next(1, 4); k++)
				{
					Equipment device = GetRandomEquipment(group);
					device.Name = $"Device_{i}_{k}";
					device.Group = group;

					devices.Add(device);
				}

				group.Devices = devices;

				this.Groups.Add(group);
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
                Random random = new Random(DateTime.Now.Millisecond);

				if (Groups.Count <= 0)
					return;

				var groupIndex = random.Next(0, Groups.Count);
				var randomGroup = Groups[groupIndex];

				if (randomGroup.Devices.Count <= 0)
					return;

				var deviceIndex = random.Next(0, randomGroup.Devices.Count);
				var randomDevice = randomGroup.Devices[deviceIndex];

				var randomIndex = CustomRandom.Next(0, 4);
				switch (randomIndex)
				{
					//Action depends on device type
					case 0:
						randomDevice.RandomAction();
						break;
					//Delete from group
					case 1:
						randomDevice.RemoveFromGroup();
						break;
					//Add new random device to group
					case 2:
						randomGroup.Devices.Add(GetRandomEquipment(randomGroup));
						break;
					//Move device to other group
					case 3:
						if (Groups.Count > 1)
						{
							var availableGroups = Groups.Where(x => x != randomDevice.Group).ToList();
							var randomGroupToMoveIndex = CustomRandom.Next(0, availableGroups.Count - 1);

							var destinationGroup = availableGroups.ElementAtOrDefault(randomGroupToMoveIndex);
							randomDevice.MoveToGroup(destinationGroup);
						}
						break;
					//Rename
					case 4:
						randomDevice.SetRandomName();
						break;
					default:
						break;
				}
			}
		}

		private Equipment GetRandomEquipment(GroupEquipment group)
		{
			var assemblies = Assembly.GetAssembly(typeof(Equipment))
				.GetTypes()
				.Where(myType => myType.IsClass
					&& !myType.IsAbstract
					&& myType.IsSubclassOf(typeof(Equipment)));

			var random = new Random(DateTime.Now.Millisecond);
			var randomIndex = random.Next(0, assemblies.Count());

			var randomEquipmentType = assemblies.ElementAt(randomIndex);

			Equipment equipment = Activator.CreateInstance(randomEquipmentType) as Equipment;
			equipment.Group = group;

			return equipment;
		}
	}
}
