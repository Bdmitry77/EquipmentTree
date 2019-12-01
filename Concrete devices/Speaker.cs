using System;
using System.ComponentModel;

namespace EquipmentTree
{
	public class Speaker : Equipment, INotifyPropertyChanged
	{
		private const double _maxVolume = 100;

		private SoundType _sound = SoundType.None;
		public SoundType Sound
		{
			get => _sound; set
			{
				_sound = value;
				OnPropertyChanged(nameof(Sound), value.ToString());
			}
		}

		private double _volume = 0;
		public double Volume
		{
			get => _volume;
			set
			{
				if (value <= 0)
					_volume = 0;
				else
					_volume = value;
				OnPropertyChanged(nameof(Volume), value.ToString());
			}
		}

		public Speaker() : base(EquipmentType.Speaker)
		{
		}

		public Speaker(string name) : base(EquipmentType.Speaker, name) { }

		public Speaker(string name, SoundType sound) : base(EquipmentType.Speaker, name)
		{
			Sound = sound;
		}

		public Speaker(string name, SoundType sound, double volume) : base(EquipmentType.Speaker, name)
		{
			Sound = sound;
			Volume = volume;
		}

		public override void RandomAction()
		{
			Volume = new Random().NextDouble() * _maxVolume;

			var values = Enum.GetValues(typeof(SoundType));
			Random random = new Random();
			Sound = (SoundType)values.GetValue(random.Next(values.Length));
		}
	}

	public enum SoundType : int
    {
        None,
        Music,
        Alarm
    }
}
