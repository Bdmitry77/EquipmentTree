using System;
using System.ComponentModel;
using System.Linq;
using System.Threading;

namespace EquipmentTree
{
	public abstract class Equipment : INotifyPropertyChanged
	{
		private static int _counter = 0;

		public int Id { get; }

		public event PropertyChangedEventHandler PropertyChanged;

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

		public virtual string GetCurrentState()
		{
			var currentState = $"Equipment: Type='{Type}', Id='{Id}'";
			foreach (var propertyInfo in ReflectionHelper.GetPropertyInfos(this))
			{
				currentState += "\n\t" + propertyInfo.Name + " = " + propertyInfo.GetValue(this);
			}

			return currentState + "\n";
		}

		public Equipment(EquipmentType type)
		{
			Type = type;
			Name = RandomName();
			Id = Interlocked.Increment(ref _counter);
		}

		public Equipment(EquipmentType type, string name)
		{
			Type = type;
			Name = name;
			Id = Interlocked.Increment(ref _counter);
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

		public static Equipment CreateRandomEquipment()
		{
			var assemblies = ReflectionHelper.GetSubclassTypes<Equipment>();

			var random = new Random(DateTime.Now.Millisecond);
			var randomIndex = random.Next(0, assemblies.Count());

			var randomEquipmentType = assemblies.ElementAt(randomIndex);

			Equipment equipment = Activator.CreateInstance(randomEquipmentType) as Equipment;

			return equipment;
		}

		public static Guid ParseId(string id)
		{
			if (Guid.TryParse(id, out Guid guidId))
				throw new Exception($"Cant parse equipment id '{id}'");

			return guidId;
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