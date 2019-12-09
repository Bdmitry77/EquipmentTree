using System;
using System.Linq;

namespace EquipmentTree
{
	class Program
	{
		static void Main(string[] args)
		{
			var root = new RootEquipment();
			root.Initialize();

			root.RandomActions(1);

			GetAllEquipment(root);

			Console.WriteLine("You're in user edit mode. Use command 'help' to get more useful info");

			bool showMenu = true;
			while (showMenu)
			{
				try
				{
					showMenu = MainMenu(root);
				}
				catch (Exception ex)
				{
					Console.WriteLine(ex.Message);
				}
			}
		}

		private static bool MainMenu(RootEquipment root)
		{
			string inputString = Console.ReadLine();
			string[] splittedString = inputString.Split(' ');

			var command = splittedString[0].Trim();
			var parameters = splittedString.Skip(1).ToArray();
			//TODO: Баг. Убрать этот метод, т.к. при добавлении новой группы, его имя с маленькой буквы...
			LowerParameters(parameters);

			//TODO: Обработать отсутствие параметров
			switch (command)
			{
				case "help":
					Console.WriteLine(GetInstrustion());
					return true;

				case "show":
					if (parameters.Length == 1)
					{
						//show all
						if (parameters[0] == "all")
							GetAllEquipment(root);
						else if (parameters[0] == "group")
						{
							var group = ChooseGroup(root);
							GetGroupInfo(group);
						}
					}
					else if (parameters.Length == 2)
					{
						if (parameters[0] == "device")
						{
							//show device <id>
							var equipmentId = parameters[1];
							var equipment = root.GetEquipmentById(equipmentId);

							Console.WriteLine(equipment.GetCurrentState());
						}
						else
							throw new Exception($"For 'show' second parameter expected: 'group' or 'device'");
					}
					else if (parameters.Length > 2)
						Console.WriteLine(ShowFormatError());
					else
						GetAllEquipment(root);
					return true;

				case "add":
					if (parameters.Length >= 1)
					{
						if (parameters[0] == "group")
						{
							if (parameters.Length > 1)
							{
								//add group [groupName]
								var newGroupName = parameters[1];
								if (!string.IsNullOrEmpty(newGroupName))
								{
									var newGroup = new GroupEquipment(newGroupName);
									root.AddGroup(newGroup);
									GetAllEquipment(root);
								}
								else
									throw new Exception($"Cant find parameter 'group name'");
							}
							else
								throw new Exception($"Cant find parameter 'group name'");
						}
						else if (parameters[0] == "device")
						{
							//add device
							Equipment newDevice = ChooseType();
							GroupEquipment group = ChooseGroup(root);

							group.AddEquipment(newDevice);

							GetAllEquipment(root);
						}
						else
							throw new Exception($"Expected first command parameter 'device' or 'group'.");
					}
					else
						throw new Exception($"Expected one command parameter. You can add 'group' or 'device'.");
					return true;

				case "edit":
					if (parameters.Length == 2)
					{
						if (parameters[0] == "device")
						{
							//edit device [deviceId]
							var equipmentId = parameters[1];
							var equipment = root.GetEquipmentById(equipmentId);

							EditDeviceProperty(equipment);
						}
						else
							throw new Exception($"Expected two command parameter 'device' and '<deviceId>'.");
					}
					else
						throw new Exception($"Expected two command parameter 'device' and '<deviceId>.");
					return true;

				case "delete":
					if (parameters[0] == "group")
					{
						//delete group
						GroupEquipment group = ChooseGroup(root);
						root.Groups.Remove(group);
						GetAllEquipment(root);
					}
					else if (parameters[0] == "device")
					{
						//delete device [deviceId]
						if (parameters.Length == 2)
						{
							var equipmentId = parameters[1];
							var equipment = root.GetEquipmentById(equipmentId);

							root.RemoveEquipment(equipment);

							GetAllEquipment(root);
						}
						else
							throw new Exception($"Expected two command parameter 'groupNumber' and 'equipmentNumber'.");
					}
					return true;

				case "cls":
					Console.Clear();
					return true;
				case "exit":
					return false;
				default:
					return true;
			}
		}

		private static void EditDeviceProperty(Equipment device)
		{
			var properties = ReflectionHelper.GetPropertyInfos(device);
			foreach (var propertyInfo in properties)
			{
				Console.WriteLine($"\t{propertyInfo.Name}");
			}

			while (true)
			{
				Console.Write("Choose device property:");

				var name = Console.ReadLine();
				var property = properties.FirstOrDefault(x => x.Name == name);

				if (property != null)
				{
					Console.WriteLine("Input new value:");
					var newValue = Console.ReadLine();

					if (property.PropertyType.IsEnum)
					{
						if (int.TryParse(newValue, out int newValueInt))
						{
							var enumVal = Enum.ToObject(property.PropertyType, newValueInt);
							if (Enum.IsDefined(property.PropertyType, enumVal))
								property.SetValue(device, Enum.ToObject(property.PropertyType, newValueInt));
						}
					}
					else if (property.PropertyType == typeof(double) 
						|| property.PropertyType == typeof(float) 
						|| property.PropertyType == typeof(decimal))
					{
						newValue = newValue.Replace(".", System.Globalization.NumberFormatInfo.CurrentInfo.NumberDecimalSeparator);
						newValue = newValue.Replace(",", System.Globalization.NumberFormatInfo.CurrentInfo.NumberDecimalSeparator);
						property.SetValue(device, Convert.ChangeType(newValue, property.PropertyType));
					}
					else
					{
						property.SetValue(device, Convert.ChangeType(newValue, property.PropertyType));
					}

					break;
				}
				else
					Console.WriteLine("Type not found");
			}
		}

		private static Equipment ChooseType()
		{
			Console.WriteLine($"Choose type:");

			var assemblies = ReflectionHelper.GetSubclassTypes<Equipment>();

			foreach (var assembly in assemblies)
			{
				Console.WriteLine($"\t{assembly.Name}");
			}

			while (true)
			{
				Console.Write("Choose device type:");

				var type = Console.ReadLine();
				var chosedType = assemblies.FirstOrDefault(x => x.Name == type);

				if (chosedType != null)
					return Activator.CreateInstance(chosedType) as Equipment;
				else
					Console.WriteLine("Type not found");
			}
		}

		private static GroupEquipment ChooseGroup(RootEquipment root)
		{
			Console.WriteLine($"Groups: ");
			foreach (var group in root.Groups)
			{
				Console.WriteLine($"\t{group.Name}");
			}

			while (true)
			{
				Console.Write("Choose group name:");

				var groupName = Console.ReadLine();
				var group = root.GetGroupByName(groupName);
				if (group != null)
					return group;
				else
					Console.WriteLine("Group not found");
			}
		}

		private static void LowerParameters(string[] parameters)
		{
			for (int i = 0; i < parameters.Length; i++)
			{
				parameters[i] = parameters[i].ToLower();
			}
		}

		private static string GetInstrustion()
		{
			string readme = @"Equipment Tree
Commands:
1) help
Help you to find every command
2) show [all]
Show all equipment
    2.1) show group
    Show concrete group of devices.
    2.2) show device [DeviceId]
    Same as 2.1, but show info about concrete device. 'DeviceId' is positive integer existing device id.
3) add group [group name]
    Add new group with a specific name
4) add device
    Command for adding new devices. Then you will choose device type and group for it.
5) edit device [deviceId]
6) delete group
7) delete device [deviceId]
8) cls
    Clear screen
9) exit
    quit
";
			return readme;
		}

		private static string ShowFormatError()
		{
			return "Wrong command format";
		}

		private static string ShowIndexError()
		{
			return "Out of index range";
		}

		private static void GetAllEquipment(RootEquipment root)
		{
			foreach (var group in root.Groups)
			{
				if (group != null)
					GetGroupInfo(group);
			}
		}

		private static void GetGroupInfo(GroupEquipment group)
		{
			Console.WriteLine($"Group: Name='{group.Name}':");

			if (group.Devices.Any())
			{

				foreach (var device in group.Devices)
				{
					if (device != null)
					{
						Console.WriteLine(device.GetCurrentState());
						Console.WriteLine();
					}
				}
			}
			else
				Console.WriteLine($"There are no devices");
		}
	}
}
