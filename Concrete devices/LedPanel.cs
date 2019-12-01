using System;
using System.ComponentModel;

namespace EquipmentTree
{
	public class LedPanel : Equipment, INotifyPropertyChanged
	{
		private string _message;

		public string Message
		{
			get => _message; set
			{
				_message = value;
				OnPropertyChanged(nameof(Message), value);
			}
		}

		public LedPanel() : base(EquipmentType.LedPanel)
		{

		}

		public LedPanel(string name, string message) : base(EquipmentType.LedPanel, name)
		{
			Message = message;
		}

		public LedPanel(string name) : base(EquipmentType.LedPanel, name)
		{

		}

		public override void RandomAction()
		{
			Message = Guid.NewGuid().ToString();
		}
	}
}
