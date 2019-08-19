using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TMOS_Romhack
{
    class RandomizeScript
    {
        int _SEED;


        WorldScreenCollection[] _WorldScreenCollections;
        string filePath = "";
        string hex = "";

        public string OutputText { get; set; }

        //string path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\tmos_goodseeds.txt";
        // File outputFile = new File(path);

        //File RomFile;
        public RandomizeScript(FileStream romFileStream, RandomizerSettings randomizerSettings)
        {
          




        }
        private void LoadRom(string path)
        {
            LoadWorldScreenDataFromRomFile(path);




        }

        private void SaveRom(string path)
        {


            byte[] startScreenTileData1 = new byte[] { 0x4D, 0x20, 0xB8, 0xC4, 0xC6, 0xB8, 0x20, 0x4D, 0x4D, 0x20, 0xB9, 0xC5, 0xC7, 0xB9, 0x20, 0x4D, 0x4D, 0xB8, 0xB8, 0xC0, 0xC3, 0xB8, 0xB8, 0x4D, 0x4D, 0xB9, 0xB9, 0xC0, 0xC3, 0xB9, 0xB9, 0x4D };
            byte[] startScreenTileData2 = new byte[] { 0x73, 0x73, 0x73, 0x73, 0x73, 0x73, 0x73, 0x73, 0xBF, 0x73, 0xBE, 0xBF, 0x73, 0x73, 0x73, 0xBE, 0x4D };


            FileStream fs = new FileStream(path, FileMode.OpenOrCreate);


            foreach (WorldScreenCollection wc in _WorldScreenCollections)
            {
                wc.WriteDataToRom(ref fs);
            }

            //Screen1 tile change

            WriteBytes(fs, 0x03F687, startScreenTileData1);
            WriteBytes(fs, 0x03F7C7, startScreenTileData2);

            //GILGA
            WriteByte(fs, 0x1743f, 0x06); //eye hp
            WriteByte(fs, 0x17447, 0x1C); //stage 2 hp damage
            WriteByte(fs, 0x18751, 0x20); //thunder damage

            WriteByte(fs, 0x17248, 0x0d); //projectile damage
            WriteByte(fs, 0x174c6, 0x04); //projectile speed


            //CURLY
            WriteByte(fs, 0x17450, 0x1D); //Arm HP
            WriteByte(fs, 0x1724c, 0x22); //Projectile damage
            WriteByte(fs, 0x1724f, 0x01); //projectile cooldown
            WriteBytes(fs, 0x1156e, new byte[] { 0xAA, 0x66, 0x03 }); //Color

            //TROLL 
            WriteByte(fs, 0x17A24, 0x1b); //Switch position delay time
            WriteByte(fs, 0x17459, 0x80); //HP (parts 1 and 2)
            WriteByte(fs, 0x18759, 0x24); //thunder damage
            WriteByte(fs, 0x18759, 0x24); //thunder damage
                                          //color (blue ones) 0x11571
                                          //color (red one): 0x1154a

            //TROLL part 2
            WriteByte(fs, 0x17250, 0x24); //Projectile damage
            WriteByte(fs, 0x17251, 0x05); //projectile behavior
            WriteByte(fs, 0x17253, 0x01); //projectile cooldown
            WriteByte(fs, 0x17455, 0x05); //collision damage

            //SALAMANDER
            WriteByte(fs, 0x17462, 0xE0); // HP
            WriteByte(fs, 0x17257, 0x00); //projectile cooldown
            WriteByte(fs, 0x17255, 0x03); //projectile speed
            WriteByte(fs, 0x1875d, 0x38); //fire magic dmg
            WriteByte(fs, 0x18a2e, 0xF6); //fire field animation

            //Part before goragora
            WriteByte(fs, 0x3c458, 0x1f);
            WriteByte(fs, 0x3c448, 0x0d);
            WriteByte(fs, 0x3c478, 0x0d);
            WriteByte(fs, 0x3c468, 0x0d);
            WriteByte(fs, 0x03c488, 0x0f);
            WriteByte(fs, 0x3c418, 0x2f);
            WriteByte(fs, 0x03c498, 0x2f);

            //GORAGORA

            // ----

            //Cut exp given by world enemies by half
            WriteBytes(fs, 0x174AA, new byte[] { 0x01, 0x00, 0x02, 0x00, 0x05, 0x00, 0x0A, 0x00, 0x0F, 0x00, 0x14, 0x00, 0x19, 0x00, 0x02, 0x00, 0x04 });

            //Make max breads/mashrooms 25
            /*    WriteByte(fs, 0x156, 0x19);
             //   WriteByte(fs, 0x42e5, 0x19);
             //   WriteByte(fs, 0x4729, 0x19);
                WriteByte(fs, 0x47b1, 0x19);
                */
            //make troopers cost 200
            WriteByte(fs, 0x4577, 0xc8);

            //change university costs
            WriteByte(fs, 0x52b2, 0x05);
            WriteByte(fs, 0x52b4, 0x14);
            WriteByte(fs, 0x52b6, 0x14);
            WriteByte(fs, 0x51b8, 0x14);

            WriteByte(fs, 0x52c1, 0x16);
            WriteByte(fs, 0x52c3, 0x28);
            WriteByte(fs, 0x52c5, 0x28);

            WriteByte(fs, 0x52d0, 0x25);
            WriteByte(fs, 0x52d2, 0x3c);
            WriteByte(fs, 0x52d4, 0x3c);
            WriteByte(fs, 0x51d6, 0x3c);

            WriteByte(fs, 0x52df, 0x32);
            WriteByte(fs, 0x52e1, 0x50);
            WriteByte(fs, 0x52e3, 0x50);

            WriteByte(fs, 0x52ee, 0x40);
            WriteByte(fs, 0x52f0, 0x64);
            WriteByte(fs, 0x52f2, 0x64);


            //Player clothes color
            WriteByte(fs, 0x1ed07, 0x02); //normal
            WriteByte(fs, 0xca72, 0x02); //battle
            WriteByte(fs, 0x1ed0a, 0x03); //r armor 
            WriteByte(fs, 0xca75, 0x03); //r armor battle



            //StartScreen Title Color
            fs.Seek(0x38890, SeekOrigin.Begin);
            fs.Write(new byte[] { 0x72 }, 0, 1);
            fs.Seek(0x38892, SeekOrigin.Begin);
            fs.Write(new byte[] { 0x02 }, 0, 1);
            fs.Seek(0x38894, SeekOrigin.Begin);
            fs.Write(new byte[] { 0x12 }, 0, 1);

            //StartScreen Text mod
            byte[] modText = new byte[] { 0x41, 0x30, 0x3D, 0x33, 0x3E, 0x3C, 0x38, 0x49, 0x34, 0x33, 0x2C, 0x23, 0x64, 0x18, 0x3C, 0x3E, 0x33, 0x2C, 0x31, 0x48, 0x2C, 0x32, 0x43, 0x01, 0x08, 0x07, 0x2C, 0x42, 0x34, 0x34, 0x33, 0x2C };
            fs.Seek(0x038473, SeekOrigin.Begin);
            fs.Write(modText, 0, modText.Length);

            //Seed text
            fs.Seek(0x038493, SeekOrigin.Begin);
            fs.Write(GetSeedTextBytes(_SEED.ToString()), 0, 6);

            //first mosque dialog change  - removed: changes all mosque screens
            /* byte[] mosque1DialogScreen = new byte[] { 0x0E, 0x2F, 0x03, 0x4A };
             fs.Seek(0x4FC9, SeekOrigin.Begin);
             fs.Write(mosque1DialogScreen, 0, mosque1DialogScreen.Length);*/

            //first mosque talk text change
            //  fs.Seek(0x36e7F, SeekOrigin.Begin);
            // fs.Write(new byte[] { 0xFF }, 0, 1);

            /*  byte[] mosque1DialogText = new byte[] { 0x43, 0x3C, 0x3E, 0x42, 0x7D, 0xFA, 0x7F, 0xD2, 0x2C, 0xCD, 0x2C, 0x33, 0x34, 0x3F, 0x30, 0x41, 0x43, 0xFA, 0x43, 0x3E, 0x2C, 0x33, 0x34, 0x35, 0x34, 0x30, 0x43, 0x2C, 0xBA, 0x79, 0xFA, 0x46, 0x3E, 0x3D, 0x4C, 0x43, 0x2C, 0xD2, 0x7C, 0xFA, 0xFA, 0xFE };
              fs.Seek(0x36e9e, SeekOrigin.Begin);
              fs.Write(mosque1DialogText, 0, mosque1DialogText.Length);*/


            //First Screen cahracter dialog
            byte[] characterDialogText = new byte[] { 0x80, 0xC9, 0x2C, 0x46, 0x3E, 0x41, 0x3B, 0x33, 0x2C, 0x20, 0x10, 0x42, 0x2C, 0x31, 0x12, 0x12, 0x3D, 0x2C, 0x41, 0x10, 0x3D, 0x33, 0x3E, 0x21, 0x38, 0x49, 0x12, 0x33, 0x4F, 0x2E, 0x12, 0x3D, 0x39, 0x3E, 0x48, 0x7D, 0x2C, 0x4B, 0x2F };
            fs.Seek(0x0215B5, SeekOrigin.Begin);
            fs.Write(characterDialogText, 0, characterDialogText.Length);

            //first guy sprite
            //fs.Seek(0x1253F, SeekOrigin.Begin);
            //fs.Write(new byte[] { 0xFC }, 0, 1);
            //center him
            fs.Seek(0x013c70, SeekOrigin.Begin);
            fs.Write(new byte[] { 0xF1 }, 0, 1);
            fs.Seek(0x013c74, SeekOrigin.Begin);
            fs.Write(new byte[] { 0xf9 }, 0, 1);

            //starting screen
            fs.Seek(0x039cc8, SeekOrigin.Begin);
            fs.Write(new byte[] { 0x21 }, 0, 1);

            fs.Seek(0x039cc6, SeekOrigin.Begin);
            fs.Write(new byte[] { 0x04 }, 0, 1);

            fs.Seek(0x039cd1, SeekOrigin.Begin);
            fs.Write(new byte[] { 0xA3 }, 0, 1);

            //color
            fs.Seek(0x39cd2, SeekOrigin.Begin);
            fs.Write(new byte[] { 0xDF }, 0, 1);


            //RISKY - change someting that causes town sprites to be shuffled
            // WriteByte(fs, 0x10953, 0x05); 

            fs.Close();

            OutputText += "Saved To Rom.\n";
        }

        private void WriteByte(FileStream fs, int address, byte value)
        {
            fs.Seek(address, SeekOrigin.Begin);
            fs.Write(new byte[] { value }, 0, 1);

        }

        private void WriteBytes(FileStream fs, int address, byte[] value)
        {
            fs.Seek(address, SeekOrigin.Begin);
            fs.Write(value, 0, value.Length);
        }

        private string ModifyRom(int SEED)
        {

            bool[] error = new bool[5];
            Random random = new Random(Convert.ToInt32(SEED.ToString()));
            bool timeDoorProblem = false;
            for (int i = 0; i < _WorldScreenCollections.Length; i++)
            {
                WorldScreenCollection wc = _WorldScreenCollections[i];


                error[i] = wc.Modify(i, random);

                if (wc.timeDoorProblem)
                {
                    timeDoorProblem = true;

                }
            }

            if (timeDoorProblem)
            {

                return "TimeDoor problem";
            }
            else
            {
                if (CheckThatAllRequiredScreenContentsArePresent())
                {
                    return "success";
                }
                else
                {
                    return "***Requred content missing***";
                }


            }



        }
        private bool CheckThatAllRequiredScreenContentsArePresent()
        {
            bool allRequiredScreensExist = true;

            byte[] requiredContentsW1 = new byte[]
            {
                0x81, //Faruk
				0x83, //Kebabu
				0x84 //Aqua Palace
			};

            byte[] requiredContentsW2 = new byte[]
            {
                0x83 //Epin
			};
            byte[] requiredContentsW3 = new byte[]
        {
                0x81, //Cimaron Tree
				0x82, //Supapa
				0x84, //Mustafa
				0x85 //Frozen Palace
		};
            byte[] requiredContentsW4 = new byte[]
    {
                0x80, //Gubibi
				0x81, //Rainy
				0x82 //Yufla Palace
				
	};
            byte[] requiredContentsW5 = new byte[]
                {
                0x80, //Hasan
				0x82, //Legend Sword
				0x83, //Armor of light
				0x84, //palace entrance
				0x85 //sabaron
				};

            WorldScreen matchingItem = null;
            //bool[] exists = new bool[requiredContents.Length];

            //W1
            for (int requiredContentIndex = 0; requiredContentIndex < requiredContentsW1.Length; requiredContentIndex++)
            {
                byte requiredContent = requiredContentsW1[requiredContentIndex];
                matchingItem = _WorldScreenCollections[0].NewWorldScreens.FirstOrDefault(WorldScreen => WorldScreen.Content == requiredContent);
                if (matchingItem == null)
                {
                    allRequiredScreensExist = false;
                }
                else { int asdasd = 0; }
            }

            //W2
            for (int requiredContentIndex = 0; requiredContentIndex < requiredContentsW2.Length; requiredContentIndex++)
            {
                byte requiredContent = requiredContentsW2[requiredContentIndex];
                matchingItem = _WorldScreenCollections[1].NewWorldScreens.FirstOrDefault(WorldScreen => WorldScreen.Content == requiredContent);
                if (matchingItem == null)
                {
                    allRequiredScreensExist = false;
                }
                else { int asdasd = 0; }
            }

            //W3
            for (int requiredContentIndex = 0; requiredContentIndex < requiredContentsW3.Length; requiredContentIndex++)
            {
                byte requiredContent = requiredContentsW3[requiredContentIndex];
                matchingItem = _WorldScreenCollections[2].NewWorldScreens.FirstOrDefault(WorldScreen => WorldScreen.Content == requiredContent);
                if (matchingItem == null)
                {
                    allRequiredScreensExist = false;
                }
                else { int asdasd = 0; }
            }

            //W4
            for (int requiredContentIndex = 0; requiredContentIndex < requiredContentsW4.Length; requiredContentIndex++)
            {
                byte requiredContent = requiredContentsW4[requiredContentIndex];
                matchingItem = _WorldScreenCollections[3].NewWorldScreens.FirstOrDefault(WorldScreen => WorldScreen.Content == requiredContent);
                if (matchingItem == null)
                {
                    allRequiredScreensExist = false;
                }
                else { int asdasd = 0; }
            }

            //W5
            for (int requiredContentIndex = 0; requiredContentIndex < requiredContentsW5.Length; requiredContentIndex++)
            {
                byte requiredContent = requiredContentsW5[requiredContentIndex];
                matchingItem = _WorldScreenCollections[4].NewWorldScreens.FirstOrDefault(WorldScreen => WorldScreen.Content == requiredContent);
                if (matchingItem == null)
                {
                    allRequiredScreensExist = false;
                }
                else { int asdasd = 0; }
            }

            return allRequiredScreensExist;
        }

        private byte[] GetSeedTextBytes(string seedString)
        {//bad naming
            byte[] blankTextBytes = new byte[6] { 0x2C, 0x2C, 0x2C, 0x2C, 0x2C, 0x2C };
            byte[] seedTextBytes = seedString.Select(t => byte.Parse(t.ToString())).ToArray();

            for (int i = 0; i < seedTextBytes.Length; i++)
            {
                blankTextBytes[i] = seedTextBytes[i];
            }

            return blankTextBytes;
        }

        public void Input_LoadROM(string filePath)
        {
            if (filePath != "")
            {
                LoadRom(filePath);
            }
        }

        
        public void Input_SaveROM(string filePath)
        {
              SaveRom(filePath);
        }

        private void LoadWorldScreenDataFromRomFile(string filePath)
        {
            _WorldScreenCollections = new WorldScreenCollection[5];
            _WorldScreenCollections[0] = new WorldScreenCollection(0x39695, 131, 0xC02A, 15, 0xC211, 6, 0);
            _WorldScreenCollections[1] = new WorldScreenCollection(0x39EC5, 137, 0xC058, 16, 0xC241, 6, 1);
            _WorldScreenCollections[2] = new WorldScreenCollection(0x3A755, 153, 0xC089, 17, 0xC271, 6, 2); //w3
            _WorldScreenCollections[3] = new WorldScreenCollection(0x3B0E5, 164, 0xC0BD, 22, 0xC2C1, 6, 3); //w3
            _WorldScreenCollections[4] = new WorldScreenCollection(0x3BB25, 154, 0xC100, 19, 0xC301, 8, 4); //w3

            //	_WorldScreenCollection[2] = new WorldScreenCollection(0x39695, 131, 0xC02A, 15, 0xC211, 10);
            //	_WorldScreenCollection[3] = new WorldScreenCollection(0x39695, 131, 0xC02A, 15, 0xC211, 10);
            //	_WorldScreenCollection[4] = new WorldScreenCollection(0x39695, 131, 0xC02A, 15, 0xC211, 10);
            foreach (WorldScreenCollection wc in _WorldScreenCollections)
            {
                FileStream fileStream = new FileStream(filePath, FileMode.Open);

                wc.LoadDataFromRomFile(ref fileStream);
                fileStream.Close();
            }


        }


        public void Initialize()
        {
            Random random = new Random();
            Initialize(random.Next(0, 99999));
        }
        public void Initialize(int SEED)
        {
            _SEED = SEED;
        }

        private void btn_viewData_Click(object sender, EventArgs e)
        {
            int selectedWorldIndex = (int)num_worldToView.Value - 1;
            DataViewForm dv = new DataViewForm(selectedWorldIndex, _WorldScreenCollections[selectedWorldIndex]);
            dv.Show();
        }

        private void btn_modify_Click(object sender, EventArgs e)
        {
            string status = ModifyRom();

            if (status == "success")
            {
                tb_output.Text += "Rom Modified with seed " + tb_seed.Text + ".\n";
                btn_save_rom.Enabled = true;
                //  WriteGoodSeedToFile(Convert.ToInt32(tb_seed.Text));
                //get a bunch of good seeds
                /*  int seed = new Random().Next(0, 99999);
                  tb_seed.Text = seed.ToString();

                  timer1.Start();*/

            }
            else
            {
                //    tb_output.Text += status + ". Trying with a new seed.\n";
                int seed = new Random().Next(0, 99999);
                tb_seed.Text = seed.ToString();
                //  WriteGoodSeedToFile(seed);
                timer1.Start();
            }


        }

        private void WriteGoodSeedToFile(int seed)
        {
            using (StreamWriter outputFile = File.AppendText(path))
            {
                outputFile.WriteLine(seed);
            }
        }

        private void btn_load_default_rom_Click(object sender, EventArgs e)
        {
            filePath = @"TMOS.nes";
            Start(true);
        }

        private void timer1_Tick(object sender, EventArgs e)
        {

            timer1.Stop();
            btn_load_rom.PerformClick();
            btn_modify.PerformClick();
        }

        private void button1_Click(object sender, EventArgs e)
        {

        }

        /*void ShuffleEnemyGroups()
		{
			Random randomseed = new Random();
			Random random = new Random(randomseed.Next(0,99999));
			List<byte[]> enemyGroupCollection = _WorldScreenCollection.GetAllOriginalEnemyGroups();
			Tasks.Shuffle(enemyGroupCollection, random);

			_WorldScreenCollection.SetEnemyGroups(enemyGroupCollection);
		}
		void ShuffleWorldScreenContents()
		{
			Random random = new Random();
			List<byte> worldContents = _WorldScreenCollection.GetAllOriginalWorldScreenContents();
			List<byte> newWorldContents = _WorldScreenCollection.GetAllOriginalWorldScreenContents();
			Tasks.Shuffle(newWorldContents, random);

			_WorldScreenCollection.SetAllWorldScreenContents(newWorldContents);

		}*/


    }
}
