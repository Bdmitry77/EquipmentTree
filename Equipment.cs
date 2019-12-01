using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace EquipmentTree
{
	public abstract class Equipment : INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler PropertyChanged;

		Guid Id { get; set; } = Guid.NewGuid();

		EquipmentType Type { get; }

		private string _name;
		public string Name
		{
			get { return _name; }
			set
			{
				_name = value;
				OnPropertyChanged(nameof(Name), value);
			}
		}
		
		public GroupEquipment Group { get; set; }

		public virtual string GetCurrentState()
		{
			var currentState = $"Id is {Id}\nEquipment type is '{Type}'";

			foreach (var propertyInfo in GetPropertyInfos())
			{
				currentState += "\n\t" + propertyInfo.Name + " = " + propertyInfo.GetValue(this);
			}

			return currentState + "\n";
		}
		
		public IEnumerable<PropertyInfo> GetPropertyInfos()
		{
			var deviceType = this.GetType();
			var properties = deviceType.GetProperties().Where(x => x.Name != "Group");
			return properties;
		}

		public void RemoveFromGroup()
		{
			Group.Devices.Remove(this);
		}

		public void MoveToGroup(GroupEquipment destinationGroup)
		{
			destinationGroup.Devices.Add(this);
			this.Group.Devices.Remove(this);
		}

		public Equipment(EquipmentType type)
		{
			Type = type;
			Name = RandomName();
		}

		public Equipment(EquipmentType type, string name)
		{
			Type = type;
			Name = name;
		}

		protected void OnPropertyChanged(string name, string value)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(value));
			Console.WriteLine($"The property '{name}' was changed to '{value}' for device:");
			Console.WriteLine(this.GetCurrentState());
		}

		public abstract void RandomAction();

		public void SetRandomName()
		{
			Name = Guid.NewGuid().ToString().Substring(0, 5);
		}

		private string RandomName()
		{
			return Guid.NewGuid().ToString().Substring(0, 5);
		}
	}

	public enum EquipmentType
    {
        LedPanel,
        Door,
        Speaker,
        CardReader
    }
}