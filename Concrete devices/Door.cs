using System;

namespace EquipmentTree
{
	public class Door : Equipment
	{
		private DoorState _state;

		public DoorState State
		{
			get => _state; set
			{
				_state = value;
				OnPropertyChanged(nameof(State), value.ToString());
			}
		}

		public Door() : base(EquipmentType.Door)
		{
			State = GetRandomState();
		}

		public Door(string name) : base(EquipmentType.Door, name)
		{
			State = GetRandomState();
		}

		public override void RandomAction()
		{
			GetRandomState();
		}

		private DoorState GetRandomState()
		{
			var values = Enum.GetValues(typeof(DoorState));
			Random random = new Random();
			return (DoorState)values.GetValue(random.Next(1, values.Length));
		}
	}

	[Flags]
    public enum DoorState
    {
        Locked = 1,
        Open = 2,
        OpenForTooLong = 4,
        OpenedForcibly = 8
    }
}
