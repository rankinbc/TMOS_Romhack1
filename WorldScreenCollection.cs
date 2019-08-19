using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TMOS_Romhack
{
	public class WorldScreenCollection //name should be changed, this is using more than worldscreens
	{
		int world2screens = 137; //039EC5 - 03A745

		 int TOTAL_WORLDSCREEN_COUNT = 131;
		 int TOTAL_RANDOMENCOUNTERLINEUP_COUNT = 6; //this is a guess
		 int TOTAL_RANDOMENCOUNTERGROUP_COUNT = 58;

		 int RANDOMENCOUNTERGROUP_DATA_START_INDEX = 0x0C02A; //
		 int RANDOMENCOUNTERLINEUP_DATA_START_INDEX = 0x0C211; //?
		 int WORLD_SCREEN_DATA_START_INDEX = 0x39695;

        int TILE_DATA_START_INDEX = 0x03C4C7;

        byte[] TileData;


        public WorldScreen[] OriginalWorldScreens;
		public WorldScreen[] NewWorldScreens;

		public RandomEncounterLineup[] OriginalRandomEncounterLineups;
		public RandomEncounterLineup[] NewRandomEncounterLineups;

		public RandomEncounterGroup[] OriginalRandomEncounterGroups;
		public RandomEncounterGroup[] NewRandomEncounterGroups;

		Random mainRandom;
		int WorldIndex;
		public bool timeDoorProblem = false;

		public WorldScreenCollection (int worldScreenDataStartIndex, int worldscreenCount, int randomEncounterGroupDataStartIndex, int randomEncounterGroupCount, int randomEncounterLineupDataStartIndex, int randomEncounterLineupCount, int thisWorldIndex)
		{
			TOTAL_WORLDSCREEN_COUNT = worldscreenCount;
			TOTAL_RANDOMENCOUNTERLINEUP_COUNT = randomEncounterLineupCount; //this is a guess
			TOTAL_RANDOMENCOUNTERGROUP_COUNT = randomEncounterGroupCount;

			RANDOMENCOUNTERGROUP_DATA_START_INDEX = randomEncounterGroupDataStartIndex; //
			RANDOMENCOUNTERLINEUP_DATA_START_INDEX = randomEncounterLineupDataStartIndex; //?
			WORLD_SCREEN_DATA_START_INDEX = worldScreenDataStartIndex;

			WorldIndex = thisWorldIndex;

			NewWorldScreens = new WorldScreen[TOTAL_WORLDSCREEN_COUNT];
			OriginalWorldScreens = new WorldScreen[TOTAL_WORLDSCREEN_COUNT];

			OriginalRandomEncounterLineups = new RandomEncounterLineup[TOTAL_RANDOMENCOUNTERLINEUP_COUNT];
			NewRandomEncounterLineups = new RandomEncounterLineup[TOTAL_RANDOMENCOUNTERLINEUP_COUNT];

			OriginalRandomEncounterGroups = new RandomEncounterGroup[TOTAL_RANDOMENCOUNTERGROUP_COUNT];
			NewRandomEncounterGroups = new RandomEncounterGroup[TOTAL_RANDOMENCOUNTERGROUP_COUNT];
		}

        public int RandomFunctionTest()
        {
            return GetRandom().Next(0,9999);
        }



		public void LoadDataFromRomFile(ref FileStream fileStream)
		{
			using (BinaryReader reader = new BinaryReader(fileStream))
			{
				reader.BaseStream.Seek(WORLD_SCREEN_DATA_START_INDEX, SeekOrigin.Begin);
				for (int worldScreenIndex = 0; worldScreenIndex < TOTAL_WORLDSCREEN_COUNT; worldScreenIndex++)
				{
					byte[] block = new byte[16];
					reader.Read(block, 0, 16);
					OriginalWorldScreens[worldScreenIndex] = new WorldScreen(block);
				}

				reader.BaseStream.Seek(RANDOMENCOUNTERLINEUP_DATA_START_INDEX, SeekOrigin.Begin);
				for (int randomEncounterLineup = 0; randomEncounterLineup < TOTAL_RANDOMENCOUNTERLINEUP_COUNT; randomEncounterLineup++)
				{
					byte[] block = new byte[RandomEncounterLineup.Size];
					reader.Read(block, 0, RandomEncounterLineup.Size);
					OriginalRandomEncounterLineups[randomEncounterLineup] = new RandomEncounterLineup(block);
				}

				reader.BaseStream.Seek(RANDOMENCOUNTERGROUP_DATA_START_INDEX, SeekOrigin.Begin);
				for (int randomEncounterGroup = 0; randomEncounterGroup < TOTAL_RANDOMENCOUNTERGROUP_COUNT; randomEncounterGroup++)
				{
					byte[] block = new byte[RandomEncounterGroup.Size];
					reader.Read(block, 0, RandomEncounterGroup.Size);
					OriginalRandomEncounterGroups[randomEncounterGroup] = new RandomEncounterGroup(block);
				}

                //Load Tile Data
                
               int ROMTileDataSize = 0x3AC1;
              TileData = new byte[ROMTileDataSize];
                reader.BaseStream.Seek(TILE_DATA_START_INDEX, SeekOrigin.Begin);
                reader.Read(TileData, 0, ROMTileDataSize);
                for (int worldScreenIndex = 0; worldScreenIndex < TOTAL_WORLDSCREEN_COUNT; worldScreenIndex++)
                {
                    OriginalWorldScreens[worldScreenIndex].LoadTileData(TileData);
                }
            }
		}



		public void WriteDataToRom(ref FileStream fileStream)
		{
			fileStream.Position = WORLD_SCREEN_DATA_START_INDEX;
			for (int worldScreenIndex = 0; worldScreenIndex < TOTAL_WORLDSCREEN_COUNT; worldScreenIndex++)
			{
				fileStream.Write(NewWorldScreens[worldScreenIndex].Data, 0, NewWorldScreens[worldScreenIndex].Data.Length);
			}

			fileStream.Position = RANDOMENCOUNTERLINEUP_DATA_START_INDEX;
			for (int randomEncounterLineup = 0; randomEncounterLineup < TOTAL_RANDOMENCOUNTERLINEUP_COUNT; randomEncounterLineup++)
			{
				fileStream.Write(NewRandomEncounterLineups[randomEncounterLineup].Data, 0, NewRandomEncounterLineups[randomEncounterLineup].Data.Length);
			}
		   fileStream.Position = RANDOMENCOUNTERGROUP_DATA_START_INDEX;
			for (int randomEncounterGroup = 0; randomEncounterGroup < TOTAL_RANDOMENCOUNTERGROUP_COUNT; randomEncounterGroup++)
			{
				fileStream.Write(NewRandomEncounterGroups[randomEncounterGroup].Data, 0, NewRandomEncounterGroups[randomEncounterGroup].Data.Length);
			}
		}

		private bool WorldScreenIsTown(int screenIndex)
		{
			return OriginalWorldScreens[screenIndex].SpritesColor == 0x12;
		}

		public bool Modify(int worldNum, Random random)
		{
            /* dungeon screens
             W1 7A
                37
                2C
                59 00
                00  64sabaron   4Agoragora 
            */ 

            int[] W1ShuffleScreens = { 0x18, 0x1A, 0x3E, 0x40, 0x49, 0x62, 0x63, 0x6B,  0x6e, 0x6f, 0x70, 0x71,  0x61, 0x65, 0x67, 0x68, 0x66, 0x6C, 0x6A,  0x74, 0x75, 0x73, 0x77, 0x78, 0x79 };
            int[] W2ShuffleScreens = { 0x28, 0x18, 0x01, 0x2E, 0x1B, 0x72, 0x4F, 0x39, 0x5d, 0x6c, 0x7A, 0x1B, 0x72, 0x74, 0x75, 0x76, 0x7B, 0x7C, 0x78, 0x79, 0x7D, 0x7E, 0x80, 0x83, 0x84 };
            int[] W3ShuffleScreens = { 0x32, 0x06, 0x1E, 0x88, 0x89, 0x8A, 0x8B, 0x8E, 0x8F, 0x8D, 0x25, 0x3A, 0x4B, 0x52, 0x5A, 0x77, 0x84, 0x83, 0x92, 0x93, 0x91, 0x90, 0x96, 0x97, 0x95 };
            int[] W4ShuffleScreens = { 0x02, 0x17, 0x14, 0x38, 0x45, 0xA3, 0x64, 0x78, 0x7A, 0x87, 0x91, 0x92, 0x8F, 0x90, 0x93, 0x94, 0x97, 0x98, 0x96, 0x9A, 0x9D };
            int[] W5ShuffleScreens = { 0x02, 0x0E, 0x10, 0x29, 0x34, 0x5F, 0x51, 0x74, 0x6A, 0x68, 0x85, 0x86, 0x83, 0x84, 0x87, 0x88, 0x8D, 0x8E, 0x8C };
            int[][] shuffleSets = { W1ShuffleScreens, W2ShuffleScreens, W3ShuffleScreens, W4ShuffleScreens, W5ShuffleScreens };

          



            mainRandom = random;

            CopyOriginalData();

			ModifyObjectSets2();
			bool error = ModifyContents(shuffleSets[worldNum]);
			ModifyRandomEncounterLineups();
           // ModifyTownNPCs();
           // ModifySpriteColors();
            return error;
		}


		public void CopyOriginalData()
		{
			for (int i = 0; i < TOTAL_WORLDSCREEN_COUNT; i++)
			{
				NewWorldScreens[i] = OriginalWorldScreens[i].GetDeepCopy();
			}
			for (int i = 0; i < TOTAL_RANDOMENCOUNTERLINEUP_COUNT; i++)
			{
				NewRandomEncounterLineups[i] = OriginalRandomEncounterLineups[i].GetDeepCopy();
			}
			for (int i = 0; i < TOTAL_RANDOMENCOUNTERGROUP_COUNT; i++)
			{
				NewRandomEncounterGroups[i] = OriginalRandomEncounterGroups[i].GetDeepCopy();
			}
		}

  

        public void ModifyObjectSets2()
        {

            if (WorldIndex == 0)
            {
                Dictionary<byte, byte[]> DataPointerObjectSets = new Dictionary<byte, byte[]>();
                DataPointerObjectSets.Add(0x0E, new byte[] { 0x44, 0x11, 0x12, 0x46, 0x77, 0x84, 0x85, 0x86, 0x9e, 0xa0, 0xae });
                DataPointerObjectSets.Add(0x0F, new byte[] { 0x05, 0x07, 0x04, 0x08, 0x06, 0x03, 0x09, 0x0a, 0x0b, 0x0c, 0x0d, 0x0f, 0x0e, 0xaa, 0xa8, 0xa9,  0xac, 0xad, 0xaf, 0xb3, 0xb7});
                DataPointerObjectSets.Add(0x10, new byte[] {  0x0f, 0x10, 0x3b, 0x42, 0x43, 0x45, 0x47, 0x79, 0x83, 0xaf, 0xb5, 0xb8, 0xc2, 0xc9, 0xda, 0xd7, 0xd9, 0xdd, 0xdf, 0xf9, 0xfb });
                // DataPointerObjectSets.Add(0x11, new byte[] { 0xFA, 0xDC, 0xCC, 0xBB, 0xA3, 0xA1, 0xA7, 0xAB, 0xAE, 0x80, 0x86, 0x87, 0x85 });
                DataPointerObjectSets.Add(0x91, new byte[] { 0x11, 0x12, 0x34, 0x44, 0x46, 0x4d, 0x77, 0x84, 0x85, 0x86, 0x9E, 0xA1, 0xA7, 0xae, 0xb9, 0xbb, 0xc0, 0xcc, 0xdc, 0xe2, 0xe4, 0xe8 });
                DataPointerObjectSets.Add(0xD1, new byte[] { 0x11, 0x12, 0x34, 0x44, 0x46, 0x77, 0x84, 0x86, 0xa1, 0xa7, 0xae, 0xb9, 0xbb, 0xc0, 0xcc, 0xd9, 0xdc, 0xe0, 0xe4, 0xe8, 0xfa });
                DataPointerObjectSets.Add(0x16, new byte[] { 0x77, 0x11, 0x12, 0x34, 0x44, 0x46, 0x84, 0x85, 0x86, 0xa0, 0xa1, 0xa7, 0xae, 0xb9, 0xbb, 0xdc, 0xdd, 0xe2, 0xe4, 0xe8, 0xfa });
                DataPointerObjectSets.Add(0x17, new byte[] { 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x0b, 0x0c, 0x0d, 0x0e, 0x0f, 0x37, 0x3b, 0x3d, 0x3e, 0x3f, 0x40, 0x41, 0x42, 0x43, 0x47, 0x78, 0x79, 0x7a, 0x7b, 0x7c, 0x7d, 0x7e, 0x7f, 0x80, 0x82});
                DataPointerObjectSets.Add(0xD6, new byte[] { 0x11, 0x12, 0x34, 0x44, 0x46, 0x77, 0x84, 0x85, 0x86, 0xa0, 0xa1, 0xa7, 0xb9, 0xbb, 0xc0, 0xd3, 0xdc, 0xe2, 0xe4, 0xe8, 0xfa });
                DataPointerObjectSets.Add(0xD7, new byte[] { 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x0a, 0x0b, 0x0c, 0x0d, 0x0e, 0x0f, 0x37, 0x39, 0x3b, 0x3d, 0x3e, 0x3f, 0x40, 0x41, 0x42, 0x43, 0x47, 0x73, 0x78, 0x79, 0x7a, 0x7b, 0x7c, 0x7d});
                DataPointerObjectSets.Add(0xD8, new byte[] { 0x02, 0x03, 0x04, 0x07, 0x0b, 0x0c, 0x0f, 0x10, 0x3b, 0x41, 0x42, 0x43, 0x45, 0x47, 0x4d, 0x72, 0x79, 0x7a, 0x7e, 0x83, 0x9e, 0xa6, 0xaf, 0xb3, 0xb5, 0xb8, 0xc1, 0xc2, 0xc9, 0xcc });

                //overworld
                byte[] ScreenGroup1 = { 0x01, 0x04, 0x03, 0x02, 0x05, 0x06, 0x07, 0x0C, 0x11, 0x10, 0x0B, 0x0A, 0x0F, 0x09, 0x0E, 0x08, 0x0D, 0x12, 0x13, 0x14, 0x15, 0x18, 0x16, 0x19, 0x1A, 0x1B, 0x1C, 0x1D, 0x1E, 0x1F, 0x21, 0x22, 0x23, 0x40, 0x44, 0x43, 0x3F, 0x3A, 0x39, 0x3E, 0x38, 0x35, 0x2F, 0x29, 0x2A, 0x30, 0x31, 0x2B, 0x32, 0x2C, 0x25, 0x26, 0x27, 0x28, 0x2E, 0x2D, 0x33, 0x34, 0x37, 0x36, 0x3B, 0x3C, 0x41 };
                byte[] ScreenGroup1AllowedDataPointers = { 0x0F, 0x0E, 0x10 };

                //towns
                byte[] ScreenGroup2 = { 0x20, 0x17, 0x3D };
                byte[] ScreenGroup2AllowedDataPointers = { 0x91 };

                //towns
                byte[] ScreenGroup3 = { 0x00, 0x42 };
                byte[] ScreenGroup3AllowedDataPointers = { 0xD1 };

                //maze
                byte[] ScreenGroup4 = { 0x4A, 0x45, 0x47, 0x48 };
                byte[] ScreenGroup4AllowedDataPointers = { 0x16, 0x17 };

                //dungeon
                byte[] ScreenGroup5 = { 0x54, 0x53, 0x59, 0x5A, 0x5C, 0x5D, 0x5E, 0x5B, 0x55, 0x4F, 0x50, 0x57, 0x51, 0x4D, 0x4B, 0x4C, 0x4E, 0x52 };
                byte[] ScreenGroup5AllowedDataPointers = { 0xD6, 0xD7, 0xD8 };

                byte[][] ScreenGroups = { ScreenGroup1, ScreenGroup2, ScreenGroup3, ScreenGroup4, ScreenGroup5 };
                byte[][] ScreenGroupDataPointers = { ScreenGroup1AllowedDataPointers, ScreenGroup2AllowedDataPointers, ScreenGroup3AllowedDataPointers, ScreenGroup4AllowedDataPointers, ScreenGroup5AllowedDataPointers };

                ChangeObjectSets(DataPointerObjectSets, ScreenGroups, ScreenGroupDataPointers);


            }
            else if (WorldIndex == 1)
            {
                Dictionary<byte, byte[]> DataPointerObjectSets = new Dictionary<byte, byte[]>();
                //    DataPointerObjectSets.Add(0x0F, new byte[] { 0x03, 0x05, 0x07, 0x08, 0x09, 0x0a, 0x0a, 0x0b, 0x0c, 0x0d, 0x11, 0x14, 0x16, 0x3d, 0x42, 0x42, 0x43, 0x44, 0x45, 0x46, 0x47, 0x48, 0x49, 0x4a, 0x4c, 0x4e, 0x72, 0x73, 0x74, 0x74, 0x75, 0x76, 0x77 });
                //     DataPointerObjectSets.Add(0x0E, new byte[] { 0x41, 0x71, 0x0e, 0x10, 0x17, 0x4e, 0x4f, 0x50, 0x6a, 0x6b, 0x78, 0x83, 0x85, 0x8a, 0x96, 0xa1, 0xac, 0xae, 0xb2, 0xc4, });

                //past
                DataPointerObjectSets.Add(0x0F, new byte[] { 0x01, 0x02, 0x03, 0x05, 0x07, 0x08, 0x09, 0x16 });
                DataPointerObjectSets.Add(0x0E, new byte[] { 0x71, 0x15, 0x8a, 0x15, 0xac, 0x15, 0xae, 0x15, 0xb2, 0x15 });

                DataPointerObjectSets.Add(0x10, new byte[] { 0x05,0x0b,0x0e,0x0f,0x17,0x3c,0x4d,0x70});

               DataPointerObjectSets.Add(0x14, new byte[] { 0x02, 0x03, 0x07, 0x0d, 0x11, 0x42, 0x46, 0x47, 0x7d });
            DataPointerObjectSets.Add(0x16, new byte[] { 0x4e, 0x4f, 0x50, 0x6a, 0x6b, 0x71 });
            DataPointerObjectSets.Add(0x17, new byte[] { 0x01,0x05,0x07,0x08,0x09,0x3d,0x43,0x44,0x45,0x74,0x79});

                DataPointerObjectSets.Add(0xD1, new byte[] {0x0e,0x10, 0x4f,0x83,0x85,});

                DataPointerObjectSets.Add(0x91, new byte[] { 0x11, 0x12, 0x34, 0x44, 0x46, 0x4d, 0x77, 0x84, 0x85, 0x86, 0xA1, 0xA7, 0xae, 0xb9, 0xbb, 0xc0, 0xcc, 0xdc, 0xe2, 0xe4, 0xe8 });
                DataPointerObjectSets.Add(0xD6, new byte[] {0x00,0x17,0x41,0x50,0x6a,0x6b,0x78,0x83 });
                DataPointerObjectSets.Add(0xD7, new byte[] {0x09,0x0b,0x45,0x48,0x73,0x74,0x79 });
                DataPointerObjectSets.Add(0xD8, new byte[] {0x05,0x0c,0x0f,0x43,0x44,0x4d,0x70 });


                byte[] objectShuffleScreens = { 0x06, 0x07, 0x08, 0x09, 0x0A, 0x0B, 0x0D, 0x0E, 0x18, 0x16, 0x14, 0x1F, 0x21, 0x2B, 0x29, 0x27, 0x30, 0x32, 0x33, 0x2C, 0x23, 0x19, 0x1A, 0x10, 0x05, 0x03, 0x2A, 0x00, 0x02, 0x04, 0x0F };


               
                //overworld
                byte[] ScreenGroup1 = { 0x36, 0x2E, 0x35, 0x25, 0x1D, 0x06, 0x07, 0x08, 0x0A, 0x0B, 0x0C, 0x0D, 0x18, 0x17, 0x16, 0x15, 0x14, 0x1E, 0x1F, 0x20, 0x21, 0x22, 0x2B, 0x2A, 0x29, 0x28, 0x27, 0x2F, 0x30, 0x31, 0x32, 0x33, 0x2C, 0x2D, 0x23, 0x24, 0x19, 0x1A, 0x0F, 0x10, 0x04, 0x05, 0x02, 0x03, 0x00 };
                byte[] ScreenGroup1AllowedDataPointers = { 0x0F, 0x0E };

              
                //past
                byte[] ScreenGroup2 = { 0x50, 0x4F, 0x4B, 0x4C, 0x48, 0x47, 0x44, 0x43, 0x40, 0x3F, 0x3B, 0x3A, 0x3E, 0x42, 0x46, 0x4A, 0x4D, 0x49, 0x45, 0x41, 0x3D, 0x3C, 0x38, 0x39 };
                byte[] ScreenGroup2AllowedDataPointers = { 0x0F, 0x0E, 0x10 };


                //maze
                byte[] ScreenGroup3 = { 0x5B, 0x5C, 0x5D, 0x57, 0x58, 0x59, 0x5A, 0x54, 0x55, 0x56, 0x52, 0x53 };
                byte[] ScreenGroup3AllowedDataPointers = { 0x16, 0x17, 0X14 };

                //towns
           //     byte[] ScreenGroup4 = { 0x09, 0x4E };
            //    byte[] ScreenGroup4AllowedDataPointers = { 0x91 };

                //towns
             //   byte[] ScreenGroup5 = { 0x37, 0x26, 0x34 };
            //    byte[] ScreenGroup5AllowedDataPointers = { 0xD1 };

                //dungeon
                byte[] ScreenGroup6 = { 0x6E, 0x6F, 0x6A, 0x69, 0x64, 0x65, 0x6B, 0x60, 0x66, 0x5E, 0x5F, 0x62, 0x63, 0x68, 0x67, 0x6C };
                byte[] ScreenGroup6AllowedDataPointers = { 0xD6, 0xD7, 0xD8 };

                byte[][] ScreenGroups = { ScreenGroup1, ScreenGroup2, ScreenGroup3, ScreenGroup6 };
                byte[][] ScreenGroupDataPointers = { ScreenGroup1AllowedDataPointers , ScreenGroup2AllowedDataPointers, ScreenGroup3AllowedDataPointers,  ScreenGroup6AllowedDataPointers };

                ChangeObjectSets(DataPointerObjectSets, ScreenGroups, ScreenGroupDataPointers);

            }


            else if (WorldIndex == 2)
            {
                Dictionary<byte, byte[]> DataPointerObjectSets = new Dictionary<byte, byte[]>();

                //2 overworlds
                byte[] ScreenGroup1 = { 0x00, 0x02, 0x06, 0x03, 0x04, 0x05, 0x07, 0x08, 0x0C, 0x0F, 0x0E, 0x0B, 0x0A, 0x09, 0x0D, 0x10, 0x11, 0x13, 0x14, 0x15, 0x16, 0x1B, 0x1A, 0x19, 0x21, 0x22, 0x23, 0x2A, 0x29, 0x28, 0x27, 0x1F, 0x20, 0x1E, 0x1D, 0x18, 0x17, 0x2E, 0x2F, 0x30, 0x32, 0x31, 0x57, 0x58, 0x59, 0x5A, 0x56, 0x55, 0x54, 0x53, 0x52, 0x4D, 0x4E, 0x4F, 0x50, 0x51, 0x4A, 0x48, 0x47, 0x46, 0x3D, 0x3E, 0x3F, 0x42, 0x43, 0x40, 0x3C, 0x3B, 0x39, 0x38, 0x37, 0x36, 0x3A, 0x34, 0x24, 0x25, 0x26, 0x1C };
                byte[] ScreenGroup1AllowedDataPointers = { 0x0F, 0x0E, 0x10 };
                DataPointerObjectSets.Add(0x0e, new byte[] { 0x08, 0x15, 0x16, 0x17, 0x31, 0x32, 0x38 });
                DataPointerObjectSets.Add(0x0f, new byte[] { 0x04, 0x09, 0x0a, 0x0b, 0x0c, 0x0d, 0x0e, 0x0f, 0x10, 0x11, 0x13, 0x39, 0x3a, 0x3d, 0x3e, 0x47, 0x48 });
                DataPointerObjectSets.Add(0x10, new byte[] {  0x0a, 0x0b, 0x14 });

                //maze
                byte[] ScreenGroup2 = { 0x74, 0x78, 0x76, 0x79, 0x7C, 0x7D, 0x80, 0x81, 0x7E, 0x7F, 0x7B };
                byte[] ScreenGroup2AllowedDataPointers = { 0x14, 0x17, 0x16, 0x18 };
                DataPointerObjectSets.Add(0x14, new byte[] { 0x04, 0x09, 0x0a, 0x0b, 0x0c, 0x0d, 0x0e, 0x0f });
                DataPointerObjectSets.Add(0x16, new byte[] { 0x08, 0x15, 0x16, 0x17 });
                DataPointerObjectSets.Add(0x17, new byte[] { 0x3d, 0x3e, 0x47 });
                DataPointerObjectSets.Add(0x18, new byte[] { 0x3d, 0x3e, 0x47, 0x0a, 0x0b, 0x14, 0x49, 0x53, 0x5a });

                //dungeon
                byte[] ScreenGroup3 = { 0x5B, 0x5C, 0x5D, 0x64, 0x5C, 0x62, 0x61, 0x65, 0x68, 0x69, 0x66, 0x73, 0x6B, 0x6C, 0x6D, 0x5E };
                byte[] ScreenGroup3AllowedDataPointers = { 0xd4, 0xd7, 0xd8 };
                DataPointerObjectSets.Add(0xd4, new byte[] { 0x04, 0x09, 0x0a, 0x0b }); 
                DataPointerObjectSets.Add(0xd7, new byte[] { 0x0b, 0x13, 0x14, 0x37 });
                DataPointerObjectSets.Add(0xd8, new byte[] { 0x52, 0x53, 0x5a });

               
                byte[][] ScreenGroups = { ScreenGroup1, ScreenGroup2, ScreenGroup3 };
                byte[][] ScreenGroupDataPointers = { ScreenGroup1AllowedDataPointers, ScreenGroup2AllowedDataPointers, ScreenGroup3AllowedDataPointers };

                ChangeObjectSets(DataPointerObjectSets, ScreenGroups, ScreenGroupDataPointers);

            }

            else if (WorldIndex == 3)
            {
                Dictionary<byte, byte[]> DataPointerObjectSets = new Dictionary<byte, byte[]>();

                //2 overworlds
                //desert
                //desert/lava
               
                byte[] ScreenGroup1 = { 0x07, 0x06, 0x05, 0x11, 0x10, 0x0F, 0x0E, 0x0D, 0x13, 0x14, 0x15, 0x16, 0x1C, 0x1B, 0x1A, 0x19, 0x1F, 0x20, 0x21, 0x22, 0x27, 0x2B, 0x2A, 0x29, 0x2D, 0x2E, 0x34, 0x33, 0x32, 0x31, 0x30, 0x2F, 0x2B, 0x2A, 0x29, 0x2D, 0x2E, 0x34, 0x33, 0x32, 0x31, 0x30, 0x2F, 0x39, 0x3A, 0x35,  0x3C, 0x3D, 0x40, 0x3F, 0x3E, 0x44, 0x43, 0x42,0x4A, 0x49, 0x48, 0x46, 0x47, 0x45, 0x4B, 0x4C, 0x4D, 0x4E, 0x4F, 0x50, 0x51, 0x52, 0x58, 0x57, 0x56, 0x5C, 0x55, 0x5B, 0x5A, 0x54 };
                byte[] ScreenGroup1AllowedDataPointers = { 0x0F, 0x0E, 0x10 };
                DataPointerObjectSets.Add(0x0e, new byte[] { 0x03, 0x0a, 0x15, 0x1c, 0x17});
                DataPointerObjectSets.Add(0x0f, new byte[] { 0x04, 0x05, 0x08, 0x09, 0x0f, 0x12, 0x13, 0x3b, 0x3c });
                DataPointerObjectSets.Add(0x10, new byte[] { 0x02, 0x0f, 0x11, 0x14, 0x1e, 0x33 ,0x28,0x41});


                //cf:ce:0x03,0x15,0x17,0x1c,

                //bottompart 
                byte[] ScreenGroup2 = { 0x08, 0x09, 0x0A, 0x0B, 0x0C, 0x04, 0x03, 0x02, 0x01, 0x00 };
                byte[] ScreenGroup2AllowedDataPointers = { 0x1c, 0x11, 0x1a };
                DataPointerObjectSets.Add(0x1a, new byte[] { 0x05 });
                DataPointerObjectSets.Add(0x11, new byte[] { 0x03, 0x15, 0x17, 0x1c, 0x2f, 0x33, 0x3e, 0x40, 0x44 });
                DataPointerObjectSets.Add(0x1c, new byte[] { 0x05, 0x02, 0x14, 0x1d, 0x1e });

                //forest area  
                byte[] ScreenGroup3 = { 0x18, 0x1E, 0x1D, 0x23, 0x24 };
                byte[] ScreenGroup3AllowedDataPointers = { 0x10};
                DataPointerObjectSets.Add(0x09, new byte[] { 0x15, 0x17, 0x1c, 0x04, 0x06, 0x08, 0x09, 0x0b, 0x0f, 0x12, 0x13 });

                //dungeon d8
                byte[] ScreenGroup7 = { 0x63, 0x62, 0x64, 0x66, 0x6E, 0x6A, 0x68, 0x6B, 0x70, 0x73, 0x7A, 0x7B, 0x7C, 0x79, 0x78, 0x77, 0x75, 0x74, 0x76, 0x80, 0x7E, 0x7D, 0x81, 0x86, 0x89, 0x8A, 0x88 };
                byte[] ScreenGroup7AllowedDataPointers = { 0xd4, 0xd6, 0xd7, 0xd8 };
                DataPointerObjectSets.Add(0xd4, new byte[] { 0x04, 0x08, 0x09, 0x0f, 0x12, 0x13, 0x1d });
                DataPointerObjectSets.Add(0xd6, new byte[] { 0x04, 0x03, 0x15, 0x17, 0x1c, 0x28, 0x3e, 0x40 });
                DataPointerObjectSets.Add(0xd7, new byte[] { 0x04, 0x08, 0x09, 0x0f, 0x12, 0x13, 0x1d, 0x03, 0x15, 0x17, 0x1c, 0x28, 0x3e, 0x40, 0x05, 0x09, 0x0f, 0x13, 0x3c });
                DataPointerObjectSets.Add(0xd8, new byte[] { 0x02, 0x0f, 0x14, 0x25 });


                //dungeon 98
                byte[] ScreenGroup8 = { 0x69, 0x6F, 0x85, 0x82 };
                byte[] ScreenGroup8AllowedDataPointers = { 0x98,0x97,0x96 };
                DataPointerObjectSets.Add(0x96, new byte[] { 0x03, 0x15, 0x17, 0x1c });
                DataPointerObjectSets.Add(0x97, new byte[] { 0x04, 0x08, 0x09, 0x0f, 0x12, 0x13, 0x1d });
                DataPointerObjectSets.Add(0x98, new byte[] { 0x03, 0x15, 0x17, 0x1c, 0x04, 0x08, 0x09, 0x0f, 0x12, 0x13, 0x1d, 0x02, 0x0f, 0x14 });

                byte[][] ScreenGroups = { ScreenGroup1, ScreenGroup2, ScreenGroup3, ScreenGroup7, ScreenGroup8 };
                byte[][] ScreenGroupDataPointers = { ScreenGroup1AllowedDataPointers, ScreenGroup2AllowedDataPointers, ScreenGroup3AllowedDataPointers,ScreenGroup7AllowedDataPointers, ScreenGroup8AllowedDataPointers };

                ChangeObjectSets(DataPointerObjectSets, ScreenGroups, ScreenGroupDataPointers);

            }



            else if (WorldIndex == 4)
            {
                Dictionary<byte, byte[]> DataPointerObjectSets = new Dictionary<byte, byte[]>();

                //overworld
                byte[] ScreenGroup1 = { 0x1A, 0x1F, 0x1E, 0x23, 0x22, 0x21, 0x1D, 0x1B, 0x16, 0x17, 0x18, 0x19, 0x14, 0x13, 0x12, 0x11, 0x0D, 0x0E, 0x0F, 0x0B, 0x0C, 0x05 };
                byte[] ScreenGroup1AllowedDataPointers = { 0x0F, 0x0E ,0x10 };
                DataPointerObjectSets.Add(0x0e, new byte[] { 0x01, 0x07, 0x09, 0x0d, 0x1f });
                DataPointerObjectSets.Add(0x0f, new byte[] { 0x02, 0x05, 0x0a, 0x0b, 0x0c, 0x1e, 0x20 });
                DataPointerObjectSets.Add(0x10, new byte[] { 0x04, 0x06, 0x0c });

                //bottompart 
                byte[] ScreenGroup2 = { 0x07, 0x08, 0x02, 0x09, 0x03, 0x0A, 0x04, 0x01, 0x00 };
                byte[] ScreenGroup2AllowedDataPointers = { 0x1c,0x11 };
                DataPointerObjectSets.Add(0x1c, new byte[] { 0x04, 0x06, 0x0a, 0x0b });
                DataPointerObjectSets.Add(0x11, new byte[] { 0x01, 0x09, 0x0d, 0x1f });

                //maze
                byte[] ScreenGroup3 = { 0x39, 0x38, 0x2E, 0x29, 0x25, 0x2F, 0x30, 0x2B, 0x26, 0x2C, 0x31, 0x32, 0x33, 0x28 };
                byte[] ScreenGroup3AllowedDataPointers = { 0x16,0x17,0x18 };
                DataPointerObjectSets.Add(0x16, new byte[] { 0x01, 0x07, 0x09, 0x0d });
                DataPointerObjectSets.Add(0x17, new byte[] { 0x05, 0x07, 0x0b, 0x1e, 0x20 });
                DataPointerObjectSets.Add(0x18, new byte[] { 0x04, 0x06, 0x0a });

                //maze 54
                byte[] ScreenGroup4 = { 0x34 };
                byte[] ScreenGroup4AllowedDataPointers = { 0x54 };
                DataPointerObjectSets.Add(0x54, new byte[] { 0x02, 0x04, 0x05, 0x0b, 0x0c, 0x20 });

                //maze56
                byte[] ScreenGroup5 = { 0x35, 0x36 };
                byte[] ScreenGroup5AllowedDataPointers = { 0x56,0x57,0x58 };
                DataPointerObjectSets.Add(0x56, new byte[] { 0x07, 0x09, 0x0d });
                DataPointerObjectSets.Add(0x57, new byte[] { 0x05, 0x0a, 0x0c, 0x1e, 0x20 });
                DataPointerObjectSets.Add(0x58, new byte[] { 0x06, 0x0a, 0x0b, 0x0c });

                //maze96 
                byte[] ScreenGroup6 = { 0x2D, 0x27, 0x24, 0x3A, 0x37 };
                byte[] ScreenGroup6AllowedDataPointers = { 0x96, 0x98 };
                DataPointerObjectSets.Add(0x96, new byte[] { 0x07, 0x09, 0x0d });
                DataPointerObjectSets.Add(0x97, new byte[] { 0x04, 0x05, 0x0a, 0x0b, 0x0c, 0x1e, 0x20 });

                //dungeon d8
                byte[] ScreenGroup7 = { 0x3E, 0x3C, 0x3B, 0x44, 0x42, 0x46, 0x49, 0x48, 0x47, 0x4A, 0x4B, 0x4C, 0x4D, 0x4E, 0x4F, 0x54, 0x59, 0x58, 0x57, 0x56, 0x51, 0x52, 0x55, 0x5B, 0x5E, 0x5C, 0x61, 0x67, 0x65, 0x6E, 0x6C, 0x73, 0x7A, 0x7B, 0x7C, 0x6B,  0x69, 0x68, 0x70, 0x71, 0x72, 0x79, 0x78, 0x77, 0x7E, 0x7F, 0x80, 0x82 };
                byte[] ScreenGroup7AllowedDataPointers = { 0xd4,0xd6,0xd7,0xd8 };
                DataPointerObjectSets.Add(0xd4,new byte[] { 0x04, 0x05 });
                DataPointerObjectSets.Add(0xd6,new byte[] { 0x01, 0x07, 0x09, 0x0d, 0x1f });
                DataPointerObjectSets.Add(0xd7, new byte[] { 0x05 });
                DataPointerObjectSets.Add(0xd8, new byte[] { 0x04, 0x06, 0x0a, 0x0b });


                //dungeon 98
                byte[] ScreenGroup8 = { 0x7D, 0x81, 0x6F };
                byte[] ScreenGroup8AllowedDataPointers = { 0x98 };
                DataPointerObjectSets.Add(0x98, new byte[] { 0x06, 0x0c });

                byte[][] ScreenGroups = { ScreenGroup1, ScreenGroup2, ScreenGroup3, ScreenGroup4, ScreenGroup5, ScreenGroup6 , ScreenGroup7, ScreenGroup8};
                byte[][] ScreenGroupDataPointers = { ScreenGroup1AllowedDataPointers, ScreenGroup2AllowedDataPointers, ScreenGroup3AllowedDataPointers, ScreenGroup4AllowedDataPointers, ScreenGroup5AllowedDataPointers, ScreenGroup6AllowedDataPointers, ScreenGroup7AllowedDataPointers , ScreenGroup8AllowedDataPointers };


                ChangeObjectSets(DataPointerObjectSets, ScreenGroups, ScreenGroupDataPointers);
            }

        }

        private void ChangeObjectSets(Dictionary<byte, byte[]> DataPointerObjectSets, byte[][] ScreenGroups, byte[][] ScreenGroupDataPointers)
        {
            for (byte screenGroupIndex = 0; screenGroupIndex < ScreenGroups.Length; screenGroupIndex++) //FOR EACH SCREEN GROUP
            {
                for (int i = 0; i < ScreenGroups[screenGroupIndex].Length; i++) //FOR EACH SCREEN
                {
                    int screenIndex = ScreenGroups[screenGroupIndex][i];
                    if (!OriginalWorldScreens[screenIndex].isEnemyDoorScreen() &&
                        !OriginalWorldScreens[screenIndex].IsDemonScreen() &&
                        !OriginalWorldScreens[screenIndex].IsWizardScreen() &&
                        (OriginalWorldScreens[screenIndex].Event == 0x00 || OriginalWorldScreens[screenIndex].Event == 0x08))
                    {
                        byte[] DataPointerList = ScreenGroupDataPointers[screenGroupIndex];
                        byte randomDataPointer = DataPointerList.ElementAt(GetRandom().Next(0, DataPointerList.Length)); //Get random data pointer from allowed list
                        NewWorldScreens[screenIndex].DataPointer = randomDataPointer;

                        byte[] ObjectSetList = DataPointerObjectSets[randomDataPointer];
                        byte randomObjectSet = ObjectSetList[GetRandom().Next(0, ObjectSetList.Count())];
                        NewWorldScreens[screenIndex].ObjectSet = randomObjectSet;
                    }
                }
            }
        }

		public void ModifyTownNPCs() //OLD  using only for towns
		{
            //Dictionary<byte, List<byte>> enemyGroupScreens = new Dictionary<byte, List<byte>>();
            List<byte> enemyGroupScreens = new List<byte>();
            List<byte> oprinDoorValues = new List<byte>();

			//gather data
			for (byte i = 0x00; i < TOTAL_WORLDSCREEN_COUNT; i++)//foreach world screen
			{
				List<byte> currentWorldScreenList;

				if (
                    i != 0x63 &&
                    !NewWorldScreens[i].IsDemonScreen() && 
                    !NewWorldScreens[i].isEnemyDoorScreen() && 
                    !NewWorldScreens[i].IsWizardScreen() &&
                   (NewWorldScreens[i].Event == 0x00 || NewWorldScreens[i].Event == 0x08) &&
                     NewWorldScreens[i].IsTown()
                   )

                {
                    enemyGroupScreens.Add(i);


                    /*if (!enemyGroupScreens.TryGetValue(OriginalWorldScreens[i].DataPointer, out currentWorldScreenList))
					{
						currentWorldScreenList = new List<byte>();
						currentWorldScreenList.Add(i);
						enemyGroupScreens.Add(OriginalWorldScreens[i].DataPointer, currentWorldScreenList);
					}
					else
					{
						enemyGroupScreens[OriginalWorldScreens[i].DataPointer].Add(i);
					}*/
        }


    }

			//randomize
			List<byte>enemyGroupScreensRandomized = new  List<byte>();
			enemyGroupScreensRandomized = CopyList(enemyGroupScreens);
			Tasks.Shuffle(enemyGroupScreensRandomized, GetRandom());

			//modify
			
				for (int i = 0x00; i < enemyGroupScreensRandomized.Count; i++)
				{
                byte recieverWorldScreenIndex = enemyGroupScreensRandomized[i];
                byte giverWorldScreenIndex = enemyGroupScreens[i];
                    NewWorldScreens[recieverWorldScreenIndex].ObjectSet = OriginalWorldScreens[giverWorldScreenIndex].ObjectSet;
				}

		}

		public void ModifyRandomEncounterLineups() //shuffles NPC groups
		{
	
			List<int> occupiedSlots = new List<int>();
			byte[] monsterGroupRawData = new byte[TOTAL_RANDOMENCOUNTERLINEUP_COUNT * RandomEncounterLineup.Size];
			byte[] newMonsterGroupRawData;

			List<byte> monsters = new List<byte>();
			List<byte> monstersRandomized = new List<byte>();

			// Get indexes with monsters and shuffle the ones that arent ff or 00
			for (int i = 0; i < TOTAL_RANDOMENCOUNTERLINEUP_COUNT; i++)
			{
				for (int d = 0; d < RandomEncounterLineup.Size; d++)
				{
					monsterGroupRawData[i * RandomEncounterLineup.Size + d] = OriginalRandomEncounterLineups[i].Data[d];
				}
			}
			newMonsterGroupRawData = CopyByteArray(monsterGroupRawData);

			//gather data
			for (int i = 0; i < monsterGroupRawData.Length; i++)
			{
				if (monsterGroupRawData[i] != 0x00 && monsterGroupRawData[i] != 0xFF && monsterGroupRawData[i] != 0x01)
				{
					monsters.Add(monsterGroupRawData[i]);
					occupiedSlots.Add(i);
				}
			}

			//randomize
			monstersRandomized = CopyList(monsters);
			Tasks.Shuffle(monstersRandomized, GetRandom());

			//modify
			for (int i = 0; i < occupiedSlots.Count; i++)
			{
				newMonsterGroupRawData[occupiedSlots[i]] = monstersRandomized[i];
			}

			for(int i = 0; i < newMonsterGroupRawData.Length; i++)
			{
				int currentRandomEncounterLineup = i / RandomEncounterLineup.Size;
				int currentByte = i % RandomEncounterLineup.Size;
				NewRandomEncounterLineups[currentRandomEncounterLineup].Data[currentByte] = newMonsterGroupRawData[i];
			}	

			int a = 0;
		}




		public void ModifySpriteColors()
		{
            byte[] allowedColors = new byte[] { 0x00, 0x01, 0x03, 0x04, 0x05, 0x06, 0x09, 0x10, 0x12, 0x14, 0x15, 0x16, 0x1e, 0x1f, 0x20, 0x34, 0x35, 0x3c, 0x3d, 0x3e, 0x43, 0x44, 0x4a, 0x95, 0x4f, 0x50, 0x5a, 0x5e, 0x62, 0x64, 0x67, 0x83, 0x95, 0xa5, 0xb7, 0xba, 0xcc, 0xd3, 0xd7, 0xd8, 0xdf, 0xe5, 0xed, 0xf9 };

			for (int i = 0; i < TOTAL_WORLDSCREEN_COUNT; i++)//foreach world screen
			{
				if (NewWorldScreens[i].IsTown()) //63 is start screen
				{
                    NewWorldScreens[i].SpritesColor = allowedColors[GetRandom().Next(0, allowedColors.Length)];
				}
			}
		}

	
		public bool ModifyContents(int[] shuffleScreens)
		{

			int randomEncounterScreenCount = 0;
          

            //bool[] WorldContentIsRandomEncounter = new bool[TOTAL_WORLDSCREEN_COUNT];
            bool[] WorldContentShouldBeShuffled = new bool[TOTAL_WORLDSCREEN_COUNT];
			//bool[] ObjectNPCSetsShouldBeModified = new bool[TOTAL_WORLDSCREEN_COUNT];

			List<int> overWorldIndexes = new List<int>(); //to keep track of where random encounters can be placed

			//List<int> oprinDoorIndexes = new List<int>();

			//List<byte> oprinDoorObjectSets = new List<byte>();
			List<int> newTimeDoorWorldScreenIndexes = new List<int>();

		
			List<byte> filteredWorldScreenContents = new List<byte>();

			//
			//Gather Data	

		//	List<byte> temp = new List<byte>();
		//	List<WorldScreen> oprinDoorWorldScreens = new List<WorldScreen>();

			for (int i = 0; i < TOTAL_WORLDSCREEN_COUNT; i++)//foreach world screen
			{

                //	if (OriginalWorldScreens[i].Content > 0x34 && OriginalWorldScreens[i].Content != 0xFF && i != 0x63 && OriginalWorldScreens[i].Event < 0x40) //63 is start screen
                if (shuffleScreens.Contains(i) && !OriginalWorldScreens[i].IsWizardScreen())
                {
					WorldContentShouldBeShuffled[i] = true;
					filteredWorldScreenContents.Add(OriginalWorldScreens[i].Content);

					/*if (OriginalWorldScreens[i].HasOprinDoor())
					{
						oprinDoorIndexes.Add(i);
						oprinDoorObjectSets.Add(OriginalWorldScreens[i].ObjectSet);
					}*/
				
					
				}
				else if (OriginalWorldScreens[i].Content == 0xFF || OriginalWorldScreens[i].Content == 0x00)
				{
					
					if (OriginalWorldScreens[i].Content == 0xFF) randomEncounterScreenCount++;
					NewWorldScreens[i].Content = 0x00;
					overWorldIndexes.Add(i);
					WorldContentShouldBeShuffled[i] = false;
				}
				else
				{
					//World Screens under 0x34 except 0 are demons so don't edit these
					WorldContentShouldBeShuffled[i] = false;
				}
			}

			Tasks.Shuffle(filteredWorldScreenContents, GetRandom());
			//Tasks.Shuffle(oprinDoorIndexes, GetRandom());

			//ModifyRom
			for (int i = 0,contentIndex = 0; i < TOTAL_WORLDSCREEN_COUNT; i++)//foreach world screen
			{
				//NewWorldScreens[i] = OriginalWorldScreens[i];

				if (WorldContentShouldBeShuffled[i])
				{
					NewWorldScreens[i].Content = filteredWorldScreenContents.ElementAt(contentIndex);
					contentIndex++;
				}
			/*	if (NewWorldScreens[i].HasInaccessibleContent(oprinDoorObjectSets))
				{
					temp.Add(NewWorldScreens[i].Content);
					oprinDoorWorldScreens.Add(NewWorldScreens[i]);
				//	int oldOprinDoorIndex = oprinDoorIndexes[oprinDoorIterator];
		0990
        //	NewWorldScreens[oldOprinDoorIndex].Event = 0x00;
					oprinDoorIterator++;
				}*/
			}



		//	temp.Sort();
			//Add back Random Encounters and update the pointers for randomencountergroup data
			Tasks.Shuffle(overWorldIndexes, GetRandom());

		//	GetRandom().Next(0, randomEncounterScreenCount);
			for (int i = 0; i < randomEncounterScreenCount; i++)
			{

				//get screen that doesnt have ff or an oprin door
				 int worldScreenIndexToAddEncounter = GetRandom().Next(0, overWorldIndexes.Count);
				while (NewWorldScreens[worldScreenIndexToAddEncounter].Content == 0xFF || NewWorldScreens[worldScreenIndexToAddEncounter].Event != 0x00
                    && (NewWorldScreens[worldScreenIndexToAddEncounter].ScreenIndexDown != 0xFE &&
                    NewWorldScreens[worldScreenIndexToAddEncounter].ScreenIndexUp != 0xFE &&
                    NewWorldScreens[worldScreenIndexToAddEncounter].ScreenIndexLeft != 0xFE &&
                    NewWorldScreens[worldScreenIndexToAddEncounter].ScreenIndexRight != 0xFE)
                    )
				{
					worldScreenIndexToAddEncounter = GetRandom().Next(0, overWorldIndexes.Count);
				}
                if (NewWorldScreens[worldScreenIndexToAddEncounter].SpritesColor != 0x12) //possible change to originalworldscreens[
                {
                    NewWorldScreens[worldScreenIndexToAddEncounter].Content = 0xFF;
                    NewRandomEncounterGroups[i].WorldScreen = (byte)worldScreenIndexToAddEncounter;
                }
			}

            bool error = MakeSureTimeDoorsAreAccessible() || CheckForOtherProblems();
            return error;

		}

        public bool CheckForOtherProblems()
        {
            int[] w1UnderwaterScreens = { 0x7A, 0x77, 0x82, 0x79, 0x78 };
            foreach (int wsIndex in w1UnderwaterScreens)
            {
                if (NewWorldScreens[wsIndex].Content == 0x81 || NewWorldScreens[wsIndex].Content == 0xC0) //faruk cant be underwater
                {
                    return true;
                }
            }

            //Check that there arnet any content screens with a value of FF 
            for (int i = 0; i < NewWorldScreens.Length; i++)
            {
                if (NewWorldScreens[i].HasContentEntrance() && NewWorldScreens[i].Content == 0xFF)
                {
                    return true;
                }

                if (OriginalWorldScreens[i].IsWizardScreen() && !NewWorldScreens[i].IsWizardScreen())
                {
                    return true;
                }

            }

              

            return false;
        }

        bool[] problemAtWorld = new bool[5];
        public bool MakeSureTimeDoorsAreAccessible()
		{
            //sloppy time door fix
            //Make sure there arent 2 time doors in 1 time

            int[] w1PastScreens = { 0x40, 0x44, 0x43, 0x3F, 0x3A, 0x39, 0x3E, 0x3D, 0x38, 0x35, 0x2F, 0x30, 0x31, 0x32, 0x29, 0x2A, 0x2B, 0x2C, 0x25, 0x26, 0x27, 0x28, 0x2E, 0x2D, 0x33, 0x34, 0x36, 0x37, 0x3B, 0x3C, 0x41, 0x42, 0x6B, 0x69, 0x6A, 0x6C, 0x4A, 0x48, 0x46, 0x45, 0x47, 0x49, 0x6F, 0x6E, 0x6D, 0x70, 0x71 };
            int[] w2PastScreens = { 0x4F, 0x50, 0x51, 0x4B, 0x4C, 0x48, 0x47, 0x43, 0x44, 0x40, 0x3F, 0x3B, 0x3A, 0x3E, 0x42, 0x43, 0x46, 0x4A, 0x4E, 0x4D, 0x49, 0x45, 0x41, 0x3D, 0x39, 0x38, 0x3C, 0x70, 0x78, 0x79, 0x7C, 0x7B, 0x7A, 0x57, 0x5B, 0x58, 0x54, 0x5C, 0x5D, 0x5A, 0x56, 0x53, 0x52, 0x55, 0x59 };
            int[] w3PastScreens = { 0x4B, 0x4A, 0x4D, 0x4E, 0x52, 0x53, 0x57, 0x58, 0x59, 0x5A, 0x33, 0x56, 0x55, 0x51, 0x50, 0x4F, 0x54, 0x4C, 0x49, 0x48, 0x47, 0x45, 0x44, 0x46, 0x41, 0x3D, 0x3E, 0x3F, 0x42, 0x43, 0x40, 0x3C, 0x3B, 0x39, 0x38, 0x37, 0x36, 0x3A, 0x34, 0x35, 0x8D, 0x8C, 0x8E, 0x8F, 0x91, 0x90, 0x92, 0x93 };
            int[] w4PastScreens = { 0x38, 0x9A, 0x99, 0x9B, 0x9C, 0x9E, 0x9D, 0x37, 0x36, 0x35, 0x39, 0x3A, 0x3B, 0x3C, 0x3D, 0x40, 0x3F, 0x3E, 0x44, 0x43, 0x42, 0x41, 0x4A, 0x49, 0x48, 0x47, 0x46, 0x45, 0x4B, 0x4C, 0x4D, 0x4E, 0x4F, 0x50, 0x51, 0x52, 0x59, 0x58, 0x57, 0x56, 0x55, 0x54, 0x53, 0x5A, 0x5B, 0x5C, 0x5D, 0x68, 0x69, 0x6A, 0x6C, 0x6D, 0x6B, 0x6E, 0x6F, 0x70, 0x71, 0x72, 0x73, 0x7A, 0x7B, 0x7C, 0x79, 0x78, 0x77, 0x74, 0x75, 0x76, 0x89, 0x8A, 0x86, 0x85, 0x81, 0x7D, 0x7E, 0x82, 0x83, 0x87, 0x88,0x87, 0x7F, 0x84, 0x88, 0x84 };
            int[] w5PastScreens = { 0x68, 0x6F, 0x76, 0x7D, 0x81, 0x7E, 0x77, 0x70, 0x69, 0x6A, 0x6B, 0x72, 0x71, 0x78, 0x79, 0x80, 0x7F, 0x7E, 0x81, 0x82, 0x7A, 0x73, 0x6C, 0x6D, 0x6E, 0x75, 0x7C, 0x7B, 0x74 };


			List<int> timeDoorIndexes = new List<int>();
			if (WorldIndex == 0)
			{
				for (int i = 0; i < w1PastScreens.Length; i++ )
				{
                    int currentWorldScreen = w1PastScreens[i];
					if (NewWorldScreens[currentWorldScreen].HasTimeDoor())
					{
						timeDoorIndexes.Add(currentWorldScreen);
					}
				}

			}
			else if (WorldIndex == 1)
			{
                for (int i = 0; i < w2PastScreens.Length; i++)
                {
                    int currentWorldScreen = w2PastScreens[i];
                    if (NewWorldScreens[currentWorldScreen].HasTimeDoor())
					{
						timeDoorIndexes.Add(currentWorldScreen);
					}
				}		
			}
			else if (WorldIndex == 2)
			{
                for (int i = 0; i < w3PastScreens.Length; i++)
                {
                    int currentWorldScreen = w3PastScreens[i];
                    if (NewWorldScreens[currentWorldScreen].HasTimeDoor())
					{
						timeDoorIndexes.Add(currentWorldScreen);
					}
				}
			}
			else if (WorldIndex == 3)
			{
                for (int i = 0; i < w4PastScreens.Length; i++)
                {
                    int currentWorldScreen = w4PastScreens[i];
                    if (NewWorldScreens[currentWorldScreen].HasTimeDoor())
                    { 
                        timeDoorIndexes.Add(currentWorldScreen);
					}
				}
			}
			else //(WorldIndex == 4)
			{
                for (int i = 0; i < w5PastScreens.Length; i++)
                {
                    int currentWorldScreen = w5PastScreens[i];
                    if (NewWorldScreens[currentWorldScreen].HasTimeDoor())
                    {
                        timeDoorIndexes.Add(currentWorldScreen);
                    }
                }
            }

			if (timeDoorIndexes.Count != 1)
			{
               
                problemAtWorld[WorldIndex] = true;
				timeDoorProblem = true;
			}
			else
			{
				//no problem 
				timeDoorProblem = false;
			}
            return timeDoorProblem;
		}

		public Random GetRandom() //j and k need to be < 20
		{
			
			byte[] randomBytes = new byte[99];
			mainRandom.NextBytes(randomBytes);
			int seed = randomBytes[mainRandom.Next(0, 99)] * randomBytes[mainRandom.Next(0, 99)] + randomBytes[mainRandom.Next(0,99)];
           
			return new Random(seed);
		}

		public static Dictionary<byte, List<byte>> CopyDictionary(Dictionary<byte, List<byte>> original) // need deep copy
		{
			Dictionary<byte, List<byte>> newDictionary = new Dictionary<byte, List<byte>>();
			foreach (KeyValuePair<byte, List<byte>> entry in original)
			{
				List<byte> intList = new List<byte>();
				foreach (byte item in entry.Value)
				{
					intList.Add(item);
				}
				newDictionary.Add(entry.Key, intList);
			}
			return newDictionary;
		}

		public static List<byte> CopyList(List<byte> original)
		{
			List<byte> newArray = new List<byte>();
			foreach (byte b in original)
			{
				newArray.Add(b);
			}
			return newArray;
		}
		public static byte[] CopyByteArray(byte[]  original)
		{
			byte[] newArray = new byte[original.Length];
			for(int i = 0; i < original.Length;i++)
			{
				newArray[i] = original[i];

			}
			return newArray;
		}

	}
}
