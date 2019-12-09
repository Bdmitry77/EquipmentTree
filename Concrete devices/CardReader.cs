using System;
using System.Text;
using System.Text.RegularExpressions;

namespace EquipmentTree
{
	public class CardReader : Equipment
    {
        private string _accessCardNumber;
        public string AccessCardNumber
        {
            get { return _accessCardNumber; }
            set
            {
                if (value.Length % 2 == 0 && value.Length <= 16 && Regex.IsMatch(value, "[0-9A-Fa-f]+"))
				{
                    _accessCardNumber = ReverseBytesAndPad(value);
					OnPropertyChanged(nameof(AccessCardNumber), value);
				}
				else
					Console.WriteLine("Wrong format of the card number!");
            }
        }

		public CardReader() : base(EquipmentType.CardReader) { }

		public CardReader(string name) : base(EquipmentType.CardReader, name) { }

		public CardReader(string name, string accessCardNumber) : base(EquipmentType.CardReader, name)
		{
			AccessCardNumber = accessCardNumber;
		}

		private string ReverseBytesAndPad(string inputCardNumber)
        {
            StringBuilder sb = new StringBuilder(16);
            string result = "";
            for (int i = inputCardNumber.Length; i > 1; i = i - 2)
            {
                sb.Append(inputCardNumber.Substring(i - 2, 2));
            }
            result = sb.ToString();
            result = result.PadLeft(16, '0');
            return result;
        }

		public override void RandomAction() { }
	}
}
