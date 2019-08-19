using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TMOS_Romhack
{
    public class RandomEncounterGroup
	{
		public static int Size = 3;
		public byte[] Data;
		public byte WorldScreen { get { return Data[(int)DataContent.WorldScreen]; } set { Data[(int)DataContent.WorldScreen] = value; } }
		public byte MonsterGroup { get { return Data[(int)DataContent.MonsterGroup]; } set { Data[(int)DataContent.MonsterGroup] = value; } }

		public byte Unknown { get { return Data[(int)DataContent.Unknown]; } set { Data[(int)DataContent.Unknown] = value; } }

		public RandomEncounterGroup(byte[] data)
		{
			Data = data;
		}
		public RandomEncounterGroup GetDeepCopy()
		{
			return new RandomEncounterGroup(Data);
		}
		public enum DataContent
		{
			WorldScreen,
			MonsterGroup,
			Unknown
		}
	}
}
