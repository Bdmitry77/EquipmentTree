using System;
using System.Linq;
using System.Reflection;

namespace EquipmentTree
{
	class Program
    {
		static void Main(string[] args)
		{
			var root = new RootEquipment();
			root.Initialize();

			root.RandomActions(10);

			GetAllEquipment(root);

            Console.WriteLine("You're in user edit mode. Use command 'help' to get more useful info");

            bool showMenu = true;
            while (showMenu)
            {
                showMenu = MainMenu(root);
            }
        }

        private static bool MainMenu(RootEquipment root)
		{
			string inputString = Console.ReadLine();
			string[] splittedString = inputString.Split(' ');

			var command = splittedString[0].Trim();
			var parameters = splittedString.Skip(1).ToArray();
			LowerParameters(parameters);

			switch (command)
			{
				case "help":
					Console.WriteLine(GetInstrustion());
					return true;
				
				case "show":
					if (parameters.Length == 1)
					{
						if (parameters[0] == "all")
						{
							GetAllEquipment(root);
							return true;
						}
						else if (Int32.TryParse(parameters[0], out int groupNumber))
						{
							if (groupNumber >= 0 && groupNumber < root.Groups.Count)
								GetGroupInfo(root.Groups[groupNumber]);
							else
								Console.WriteLine(ShowIndexError());
						}
						else
							Console.WriteLine(ShowFormatError());
					}
					else if (parameters.Length == 2)
						if (Int32.TryParse(parameters[0], out int groupNumber) 
							&& Int32.TryParse(parameters[1], out int deviceNumber))
						{
							if (groupNumber >= 0 
								&& groupNumber < root.Groups.Count 
								&& deviceNumber >= 0 
								&& deviceNumber < root.Groups.ElementAt(groupNumber).Devices.Count)
							{
								var device = root.Groups.ElementAt(groupNumber).Devices.ElementAt(deviceNumber);
								Console.WriteLine(device.GetCurrentState());
							}
							else
								Console.WriteLine(ShowIndexError());
						}
						else
						{
							Console.WriteLine(ShowFormatError());
						}
					else if (parameters.Length > 2)
						Console.WriteLine(ShowFormatError());
					else
						GetAllEquipment(root);

					return true;

				case "add":
                    if (parameters.Length == 1)
                    {
                        if (parameters[0] == "group")
                        {
                            if (!string.IsNullOrEmpty(parameters[1]))
                            {
                                root.Groups.Add(new GroupEquipment() { Name = parameters[1] });
                                GetAllEquipment(root);
                            }
                            else
                                Console.WriteLine($"Cant find group name parameter");
                        }
                        else if (parameters[0] == "device")
                        {
                            Console.WriteLine($"Choose type:");
                            Equipment newDevice = ChooseType();
                            GroupEquipment group = ChooseGroup(root);

                            group.Devices.Add(newDevice);
                            newDevice.Group = group;

                            GetAllEquipment(root);
                        }
                    }
                    else
                        Console.WriteLine(ShowFormatError());
                    return true;

				case "edit":
					if (parameters.Length == 2)
					{
						if (Int32.TryParse(parameters[0], out int groupNumber)
							&& Int32.TryParse(parameters[1], out int deviceNumber))
						{
							if (groupNumber >= 0
								&& groupNumber < root.Groups.Count
								&& deviceNumber >= 0
								&& deviceNumber < root.Groups.ElementAt(groupNumber).Devices.Count)
							{
								var device = root.Groups.ElementAt(groupNumber).Devices.ElementAt(deviceNumber);

								EditDeviceProperty(device);
							}
							else
								Console.WriteLine(ShowIndexError());
						}
						else
						{
							Console.WriteLine(ShowFormatError());
						}
					}
					return true;

                case "delete":
                    if (parameters[0] == "group")
                    {
                        if (!string.IsNullOrEmpty(parameters[1]))
                        {
                            GroupEquipment group = ChooseGroup(root);
                            root.Groups.Remove(group);
                            GetAllEquipment(root);
                        }
                        else
                            Console.WriteLine($"Cant find group name parameter");
                    }
                    else if (parameters[0] == "device")
                    {
                        if (parameters.Length == 3)
                        {
                            if (Int32.TryParse(parameters[1], out int groupNumber)
                                && Int32.TryParse(parameters[2], out int deviceNumber))
                            {
                                if (groupNumber >= 0
                                    && groupNumber < root.Groups.Count
                                    && deviceNumber >= 0
                                    && deviceNumber < root.Groups.ElementAt(groupNumber).Devices.Count)
                                {
                                    var device = root.Groups.ElementAt(groupNumber).Devices.ElementAt(deviceNumber);
                                    device.RemoveFromGroup();
                                    GetAllEquipment(root);
                                }
                                else
                                    Console.WriteLine(ShowIndexError());
                            }
                            else
                            {
                                Console.WriteLine(ShowFormatError());
                            }
                        }
                        else
                            Console.WriteLine(ShowFormatError());
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
			var properties = device.GetPropertyInfos();
			foreach (var propertyInfo in properties)
			{
				Console.WriteLine($"\t{propertyInfo.Name}");
			}

			PropertyInfo property = null;
			while (true)
			{
				Console.Write("Choose device property:");

				var name = Console.ReadLine();
				property = properties.FirstOrDefault(x => x.Name == name);

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
					else
					{
						newValue = newValue.Replace(".", System.Globalization.NumberFormatInfo.CurrentInfo.NumberDecimalSeparator);
						newValue = newValue.Replace(",", System.Globalization.NumberFormatInfo.CurrentInfo.NumberDecimalSeparator);
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
			var assemblies = Assembly.GetAssembly(typeof(Equipment))
				.GetTypes()
				.Where(myType => myType.IsClass
					&& !myType.IsAbstract
					&& myType.IsSubclassOf(typeof(Equipment)));

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
				var group = root.Groups.FirstOrDefault(x => x.Name == groupName);
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
    2.1) show [group]
    Show concrete group of devices. 'Group' should be positive integer and group with this number is exist.
    2.2) show [group] [device]
    Same as 2.1, but show info about concrete device. 'Device' is positive integer existing device in concrete group.
3) add group [group name]
    Add new group with a specific name
4) add device
    Command for adding new devices. Then you will choose device type and group for it.
5) edit [group number] [device number]
6) delete group
7) delete device [group number] [device number]
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
            Console.WriteLine($"Group '{group.Name}':");

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
