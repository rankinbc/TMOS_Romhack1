using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TMOS_Romhack
{
	public class WorldScreen
	{
		public static int Size = 16;
		public byte[] Data;
		public byte ParentWorld { get { return Data[(int)DataContent.ParentWorld]; } set { Data[(int)DataContent.ParentWorld] = value; } }
		public byte AmbientSound { get { return Data[(int)DataContent.AmbientSound]; } set { Data[(int)DataContent.AmbientSound] = value; } }
		public byte Content { get { return Data[(int)DataContent.Content]; } set { Data[(int)DataContent.Content] = value; } }
		public byte ObjectSet { get { return Data[(int)DataContent.ObjectSet]; } set { Data[(int)DataContent.ObjectSet] = value; } }
		public byte ScreenIndexRight { get { return Data[(int)DataContent.ScreenIndexRight]; } set { Data[(int)DataContent.ScreenIndexRight] = value; } }
		public byte ScreenIndexLeft { get { return Data[(int)DataContent.ScreenIndexLeft]; } set { Data[(int)DataContent.ScreenIndexLeft] = value; } }
		public byte ScreenIndexDown { get { return Data[(int)DataContent.ScreenIndexDown]; } set { Data[(int)DataContent.ScreenIndexDown] = value; } }
		public byte ScreenIndexUp { get { return Data[(int)DataContent.ScreenIndexUp]; } set { Data[(int)DataContent.ScreenIndexUp] = value; } }
		public byte DataPointer { get { return Data[(int)DataContent.DataPointer]; } set { Data[(int)DataContent.DataPointer] = value; } }
		public byte ExitPosition { get { return Data[(int)DataContent.ExitPosition]; } set { Data[(int)DataContent.ExitPosition] = value; } }
		public byte TopTiles { get { return Data[(int)DataContent.TopTiles]; } set { Data[(int)DataContent.TopTiles] = value; } }
		public byte BottomTiles { get { return Data[(int)DataContent.BottomTiles]; } set { Data[(int)DataContent.BottomTiles] = value; } }
		public byte WorldScreenColor { get { return Data[(int)DataContent.WorldScreenColor]; } set { Data[(int)DataContent.WorldScreenColor] = value; } }
		public byte SpritesColor { get { return Data[(int)DataContent.SpritesColor]; } set { Data[(int)DataContent.SpritesColor] = value; } }
		public byte Unknown { get { return Data[(int)DataContent.Unknown]; } set { Data[(int)DataContent.Unknown] = value; } }
		public byte Event { get { return Data[(int)DataContent.Event]; } set { Data[(int)DataContent.Event] = value; } }

      
        public WorldScreenTileData TileData;


		public WorldScreen(byte[] data)
		{
			Data = data;
		}

		public WorldScreen GetDeepCopy()
		{
            byte[] newData = new byte[Data.Length];
           for (int i = 0; i < Data.Length; i++)
            {
                newData[i] = Data[i];
            }

            WorldScreen ws = new WorldScreen(newData);
            
			return new WorldScreen(Data);
		}

        public void LoadTileData(byte[] ROMTileData)
        {
            TileData = new WorldScreenTileData(ROMTileData,DataPointer, TopTiles, BottomTiles);
            int a = 0;
        }

		public bool IsDemonScreen()
		{
			if (Content >= 0x21 && Content <= 0x2A)
			{
				return true;
			}
			else
				return false;
		}

        public bool IsWizardScreen()
        {
            if (Content == 0x01)
            {
                return true;
            }
            else
                return false;
        }

        public bool IsTown()
        {
            if (SpritesColor == 0x12)
            {
                return true;
            }
            else
                return false;
        }

        public bool isEnemyDoorScreen()
		{
			if (
                (ParentWorld == 0x61 && ObjectSet == 0x10) ||
                (ParentWorld == 0x64 && ObjectSet == 0x0F) ||
                (ParentWorld == 0x67 && ObjectSet == 0x14) ||
                (ParentWorld == 0x67 && ObjectSet == 0x15) ||
                (ParentWorld == 0x69 && ObjectSet == 0x14) ||
                (ParentWorld == 0x69 && ObjectSet == 0x15) ||
                (ParentWorld == 0x69 && ObjectSet == 0x15) ||
                (ParentWorld == 0x6C && ObjectSet == 0x0D) ||
                (ParentWorld == 0x6A && ObjectSet == 0x14) ||
                (ParentWorld == 0x6A && ObjectSet == 0x15) ||
                (ParentWorld == 0x6E && ObjectSet == 0x0D) ||
                (ParentWorld == 0x9F && ObjectSet == 0x0D)
                )
			{
				return true;
			}
			else
				return false;
		}

		public bool HasTimeDoor()
		{
            if (Content == 0xC0)
            {
                return true;
            }
            else
                return false;
		//	return Content == 0xC0;

		}
		public bool HasOprinDoor()
		{
			return Event == 0x22 || HasInaccessibleContent();
		}
		
		public bool HasInaccessibleContent()
		{
			if (Content > 0x34 && Content != 0xFF && Event != 0x40 )
			{
				if (ScreenIndexLeft != 0xFE && ScreenIndexRight != 0xFE && ScreenIndexUp != 0xFE && ScreenIndexDown != 0xFE)
				{
					return true;
				}
				else
				{
					return false;
				}
			}
			return false;
		}

        public bool HasContentEntrance()
        {
            if (ScreenIndexLeft == 0xFE || ScreenIndexRight == 0xFE || ScreenIndexUp == 0xFE || ScreenIndexDown == 0xFE)
            {
                return true;
            }
            else return false;
        }

        public override string ToString()
        {
            return ParentWorld + " " +
                AmbientSound + " " +
                Content + " " +
                ObjectSet + " " +
                ScreenIndexRight + " " +
                ScreenIndexLeft + " " +
                ScreenIndexDown + " " +
                ScreenIndexUp + " " +
                DataPointer + " " +
                ExitPosition + " " +
                TopTiles + " " +
                BottomTiles + " " +
                WorldScreenColor + " " +
                SpritesColor + " " +
                Unknown + " " +
                Event + " ";
        }

        public enum DataContent
		{
			ParentWorld, //music and some other things
			AmbientSound,
			Content,
			ObjectSet, //includes doors
			ScreenIndexRight,
			ScreenIndexLeft,
			ScreenIndexDown,
			ScreenIndexUp,
			DataPointer,
			ExitPosition,
			TopTiles,
			BottomTiles,
			WorldScreenColor,
			SpritesColor,
			Unknown,
			Event //dialog
		}
	}
}
