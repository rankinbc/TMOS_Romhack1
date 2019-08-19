using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TMOS_Romhack
{
	public static class DataLibrary
	{
		public static class WorldContentData
		{
        
			public static byte BattleFlagSet = 0xFF;

			const byte None = 0x00;
			const byte FrozenPalaceMessage = 0x1D;
			const byte FirstMosque = 0x20;
			const byte ShopUnused7B = 0x7B;
			const byte ShopUnused7C = 0x7C;
			const byte ShopUnused7D = 0x7D;
			const byte Mosque = 0x7E;
			const byte Troopers = 0x7F;
			const byte Hotel10R = 0xA0;

			const byte Hotel169R = 0xB0;
			const byte Casino = 0xBE;
			const byte TimeDoor = 0xD7; //World dependent
										//0x1F warp to middle of screen with wizard music
										//0x20 black screen
			const byte Gilga = 0x21;
			const byte GilgaTrueColors = 0x22;
			const byte PrincessW1 = 0x2B;

		}

		public static class NPCGroupData
		{

			public static class World2
			{
				public static byte[] FallingRocks3 = new byte[] { 0x17, 0x0E };
				public static byte[] Robbers4 = new byte[] { 0x05, 0x0F };
				public static byte[] Claws4 = new byte[] { 0x02, 0x0F };
				public static byte[] JumpingClaws4 = new byte[] { 0x01, 0x0F };
			}


		}

		/*
			60 Shop B20 Mashroom Key horn
			61 Shop1
			62 Shop (Horen Past) b60 m60 c60 rseed100
			64 Shop2
			75 Shop Amaries Kaitos Fighter <blank>
			76 Shop Raincom <blank> Holyrobe Raincom
			77 Shop Spricom ? BasidoSquad?
			78 Shop Pukin Pukin Pukin Kebabu
			79 Shop mashroom key raincom holyrobe
			81 Faruk
			82 Dogos
			83 Kebabu
			84 Aqua Palace
			85 WiseMan Monecom										
			86 Achelato Princess
			87 Sabaron
			88 50 rupias
			89 gun meca
			90,1d Newborn cimaron tree
			40,50, 55 University (Cygnus, Monecom, Alalart)*/



	}
}
