using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TMOS_Romhack
{
    public class RandomEncounterLineup
	{

		public static int Size = 8;
		public byte[] Data;
		public byte StartByte { get { return Data[(int)DataContent.Startbyte]; } set { Data[(int)DataContent.Startbyte] = value; } }
		public byte Slot1 { get { return Data[(int)DataContent.Slot1]; } set { Data[(int)DataContent.Slot1] = value; } }

		public byte Slot2 { get { return Data[(int)DataContent.Slot2]; } set { Data[(int)DataContent.Slot2] = value; } }
		public byte Slot3 { get { return Data[(int)DataContent.Slot3]; } set { Data[(int)DataContent.Slot3] = value; } }
		public byte Slot4 { get { return Data[(int)DataContent.Slot4]; } set { Data[(int)DataContent.Slot4] = value; } }
		public byte Slot5 { get { return Data[(int)DataContent.Slot5]; } set { Data[(int)DataContent.Slot5] = value; } }
		public byte Slot6 { get { return Data[(int)DataContent.Slot6]; } set { Data[(int)DataContent.Slot6] = value; } }
		public byte Slot7 { get { return Data[(int)DataContent.Slot7]; } set { Data[(int)DataContent.Slot7] = value; } }

		public RandomEncounterLineup(byte[] data)
		{
			Data = data;
		}
		public RandomEncounterLineup GetDeepCopy()
		{
			return new RandomEncounterLineup(Data);
		}
		public enum DataContent
		{
			Startbyte, // always 00 - probably just to seperate
			Slot1,
			Slot2,
			Slot3,
			Slot4,
			Slot5,
			Slot6,
			Slot7
		}
	}
}
