using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using TMOS_Romhack.DataViewer;
using System.IO;

namespace TMOS_Romhack
{
    public partial class DataViewForm : Form
    {
        WorldScreenCollection _worldScreens;
        WorldScreenMap _map;

        Dictionary<int, Rectangle> rectangles;
        int MAP_TILE_SIZE_X =56;
        int MAP_TILE_SIZE_Y = 56;

        Dictionary<string, string> KnownContents;
        Dictionary<string, string> KnownEvents;
        Dictionary<string, string> KnownScreenExits;
        Dictionary<string, string> KnownObjectSets;

        Dictionary<string, string> TileImagePaths;

        int selectedIndex;

        public DataViewForm(int worldIndex, WorldScreenCollection worldScreens)
        {
            _worldScreens = worldScreens;
            _map = new WorldScreenMap(this,worldScreens);


            LoadContentFiles(worldIndex);


            InitializeComponent();
            pictureBox1.Image = new Bitmap(pictureBox1.Width, pictureBox1.Height);
            pb_tiles.Image = new Bitmap(pb_tiles.Width, pb_tiles.Height);
        }

        public void LoadContentFiles(int worldIndex)
        {
            KnownContents = new Dictionary<string, string>();
            KnownEvents = new Dictionary<string, string>();
            KnownScreenExits = new Dictionary<string, string>();
            KnownObjectSets = new Dictionary<string, string>();

            TileImagePaths = new Dictionary<string, string>();
            string line = "";

            string worldSpecificPath = "";
            switch(worldIndex)
            {
                case 0: worldSpecificPath = "World1";break;
                case 1: worldSpecificPath = "World2"; break;
                case 2: worldSpecificPath = "World3"; break;
                case 3: worldSpecificPath = "World4"; break;
                case 4: worldSpecificPath = "World5"; break;
                default: worldSpecificPath = "World1";break;
            }

   
                System.IO.StreamReader contentFile = new System.IO.StreamReader(@"DataFiles/" + worldSpecificPath + "/content.txt");
                while ((line = contentFile.ReadLine()) != null)
                {
                    string key = line.Split(' ')[0];
                    string value = line.Substring(3);
                    KnownContents.Add(key, value);
                }
                contentFile.Close();

                System.IO.StreamReader objectSetFile = new System.IO.StreamReader(@"DataFiles/" + worldSpecificPath + "/objectsets.txt");
                while ((line = objectSetFile.ReadLine()) != null)
                {
                    string key = line.Split(' ')[0];
                    string value = line.Substring(3);
                    KnownObjectSets.Add(key, value);
                }
                objectSetFile.Close();
        
            

            System.IO.StreamReader eventFile = new System.IO.StreamReader(@"DataFiles/events.txt");
            while ((line = eventFile.ReadLine()) != null)
            {
                string key = line.Split(' ')[0];
                string value = line.Substring(3);
                KnownEvents.Add(key, value);
            }
            eventFile.Close();

            System.IO.StreamReader screenExitsFile = new System.IO.StreamReader(@"DataFiles/screenexits.txt");
            while ((line = screenExitsFile.ReadLine()) != null)
            {
                string key = line.Split(' ')[0];
                string value = line.Substring(3);
                KnownScreenExits.Add(key, value);
            }
            screenExitsFile.Close();


            System.IO.StreamReader tileImagesFile = new System.IO.StreamReader(@"DataFiles/tiles.txt");
            while ((line = tileImagesFile.ReadLine()) != null)
            {
                string key = line.Split(' ')[0];
                string value = line.Substring(3);
                TileImagePaths.Add(key, value);
            }
            tileImagesFile.Close();




        }

        public static int HexToInt(string hexValue)
        {
            return int.Parse(hexValue, System.Globalization.NumberStyles.HexNumber);
        }

        public static Color getGroundColor(WorldScreen ws)
        {
            Color gr;
            switch (ws.WorldScreenColor.ToString("X2"))
            {
                case "21":
                case "2A":
                case "32":
                case "45":
                    gr = Color.FromArgb(255, 0, 60, 20);        //past
                    break;
                case "30":
              //  case "38":
                case "3B":
                   // gr = Color.FromArgb(255, 36, 24, 140);
                    gr = Color.FromArgb(255, 0, 112, 236);      //water
                    break;
                case "25":
                case "41":
                case "47":
                    gr = Color.FromArgb(255, 252, 228, 160);    //desert
                    break;
                case "1A":
                    gr = Color.FromArgb(255, 0, 80, 0);         //Dark palace
                    break;
                case "3C":
                case "31":
              //case "34":
                    gr = Color.FromArgb(255, 164, 0, 0);        //red
                    break;
                case "23":
                case "2B":
                case "39":
                    gr = Color.FromArgb(255, 188, 188, 188);    //winter
                    break;
              /*  case "1B":
                    gr = Color.FromArgb(255, 60, 188, 252);     //ice
                    break;*/
                case "11":
                case "27":
                case "43":
                case "44":
                case "4A":
                case "34":
                case "1F":
                case "20":
                    gr = Color.FromArgb(255, 0, 0, 0);          //black
                    break;
                case "1C":
                //case "27":
                //case "31":
                //case "34":
                //case "44":
                case "46":
                case "48":
                    gr = Color.FromArgb(255, 216, 40, 0);       //lava
                    break;
               /* case "1D":
                    gr = Color.FromArgb(255, 68, 0, 156);       //Sabaron's palace
                    break;*/
                default:
                    gr = Color.FromArgb(255, 0, 148, 0);
                    // Console.WriteLine(ws.WorldScreenColor);
                    break;
            }
            return gr;
        }

        private void DataViewForm_Load(object sender, EventArgs e)
        {
            LoadWorldScreensListbox();

            
        }

        private void LoadWorldScreensListbox()
        {
            
            for(int i = 0; i < _worldScreens.OriginalWorldScreens.Count(); i++)
            {
                WorldScreen ws = _worldScreens.OriginalWorldScreens[i];
                string[] data = new string[] {
            
                 ws.ParentWorld.ToString("X2"),
                 ws.AmbientSound.ToString("X2"),
                 ws.Content.ToString("X2"),
                 ws.ObjectSet.ToString("X2"),
                 ws.ScreenIndexRight.ToString("X2"),
                 ws.ScreenIndexLeft.ToString("X2"),
                 ws.ScreenIndexDown.ToString("X2"),
                 ws.ScreenIndexUp.ToString("X2"),
                 ws.DataPointer.ToString("X2"),
                 ws.ExitPosition.ToString("X2"),
                 ws.TopTiles.ToString("X2"),
                ws.BottomTiles.ToString("X2"),
                ws.WorldScreenColor.ToString("X2"),
                ws.SpritesColor.ToString("X2"),
                ws.Unknown.ToString("X2"),
                ws.Event.ToString("X2")
                    };
            lv_worldScreens.Items.Add(i.ToString("X2") + " (" + i.ToString() + ")").SubItems.AddRange(data);
            }
        }

        private void ResetCurrentlyViewingListBoxItems()
        {
            foreach (ListViewItem lvw in lv_worldScreens.Items)
            {
                lvw.ForeColor = Color.Black;
                
            }
        }


        private void btn_updateMap_Click(object sender, EventArgs e)
        {
            _map.InitalizeData();
            ResetCurrentlyViewingListBoxItems();
            _map.LoadWorldMap(lv_worldScreens.SelectedIndices[0], 16, 16);
           
            using (var g = Graphics.FromImage(pictureBox1.Image))
            {
                g.Clear(Color.LightGray);
                
                rectangles = _map.DrawWorldMap(MAP_TILE_SIZE_X, MAP_TILE_SIZE_Y);

                foreach (KeyValuePair<int, Rectangle> item in rectangles)
                {
                    int wsIndex = item.Key;
                    WorldScreen ws = _worldScreens.OriginalWorldScreens[item.Key];
                    Rectangle rect = item.Value;
                    Brush bgbrush;
                    if (ws.IsWizardScreen())
                    {
                        bgbrush = new SolidBrush(Color.FromArgb(40, 40, 40, 40));
                    }
                    else { bgbrush = new SolidBrush(Color.FromArgb(255, 255, 255, 255)); }
                    
                   // g.FillRectangle(bgbrush, rect);
                   // g.DrawRectangle(Pens.LightGreen, rect);


                    //Draw Tiles on map
                    if (cb_fill_map_tiles.Checked)
                    {
                        RectangleF tileRect;
                        WorldScreenTileData tileData = ws.TileData;

                        Color ground = getGroundColor(ws);

                        float TILEVIEW_SIZE_X = (float) rect.Width / WorldScreenTileData.TILES_X_COUNT;
                        float TILEVIEW_SIZE_Y = (float) rect.Height / WorldScreenTileData.TILES_Y_COUNT;
                        for (int y = 0; y < WorldScreenTileData.TILES_Y_COUNT; y++)
                        {
                            for (int x = 0; x < WorldScreenTileData.TILES_X_COUNT; x++)
                            {
                                byte tileValue = tileData.Tiles[x, y];
                                tileRect = new RectangleF(rect.Left + (x * TILEVIEW_SIZE_X), rect.Top + (y * TILEVIEW_SIZE_Y), TILEVIEW_SIZE_X, TILEVIEW_SIZE_Y);

                              //  Rectangle tileRect = new Rectangle(rect.Left + (x * TILEVIEW_SIZE_X), rect.Top + (y * TILEVIEW_SIZE_Y), TILEVIEW_SIZE_X, TILEVIEW_SIZE_Y);

                                Brush brush = new SolidBrush(ground);   //here
                                g.FillRectangle(brush, tileRect);

                                //grid
                                //  g.DrawRectangle(Pens.Black, rect);

                                if (TileImagePaths.ContainsKey(tileValue.ToString("X2")))
                                {
                                    Image image = new Bitmap(@"Images/TileImages/" + TileImagePaths[tileValue.ToString("X2")]);
                                    g.DrawImage(image, tileRect);
                                }

                            }
                        }
                    }


                        /*if (selectedIndex == wsIndex)
                        {
                            Brush brush = new SolidBrush(Color.FromArgb(40, 50, 50, 50));
                            g.FillRectangle(brush, rect);
                        }*/

                        Font drawFont = new Font("Arial", 7);



                    PointF drawIdPoint = new PointF(rect.X + 2, rect.Y + 2);
                    //g.DrawString(item.Key.ToString("X2"), drawFont, Pens.Black.Brush, drawIdPoint);

                    PointF drawContentPoint = new PointF(rect.X + rect.Width - 14, rect.Y + 2);
                    if (ws.Content != 0x00 && ws.Content != 0xFF)
                    {
                        // if (ws.Content == 0xFE) g.DrawString(ws.Content.ToString("X2"), drawFont, Pens.Orange.Brush, drawContentPoint);
                       //g.DrawString(ws.WorldScreenColor.ToString("X2"), drawFont, Pens.Blue.Brush, drawContentPoint);
                    }
                    if (ws.Content == 0xFF)
                    {
                        Brush encScreen = new SolidBrush(Color.FromArgb(110, 255, 0, 210));
                        g.FillRectangle(encScreen, rect);
                        //g.FillRectangle(Pens.WhiteSmoke.Brush, rect.Left, rect.Top, MAP_TILE_SIZE_X, MAP_TILE_SIZE_Y);
                    }


                    PointF drawDataPointerPoint = new PointF(rect.X + rect.Width - 14, rect.Y + rect.Height - 24);
                    PointF drawObjectSetPoint = new PointF(rect.X + rect.Width - 14, rect.Y + rect.Height - 14);
                    /*if (ws.ObjectSet != 0x00)
                    {
                       
                        g.DrawString(ws.DataPointer.ToString("X2"), drawFont, Pens.Purple.Brush, drawDataPointerPoint);
                        g.DrawString(ws.ObjectSet.ToString("X2"), drawFont, Pens.Red.Brush, drawObjectSetPoint);
                    }*/
                    

                    int worldExitRectangleScaleX = (MAP_TILE_SIZE_X / 3);
                    int worldExitRectangleScaleY = (MAP_TILE_SIZE_Y / 3);

                    //DOWN
                    if (ws.ScreenIndexDown == 0xFF)
                    {
                       // g.DrawLine(Pens.Black, new Point(rect.Left, rect.Bottom - 1), new Point(rect.Right, rect.Bottom - 1));
                    }
                    else if (ws.ScreenIndexDown == 0xFE)
                    {
                        //g.DrawLine(Pens.Blue, new Point(rect.Left, rect.Bottom - 1), new Point(rect.Right, rect.Bottom - 1));
                        //g.DrawString(ws.Content.ToString("X2"), drawFont, Pens.Blue.Brush, drawContentPoint);
                    }
                    else if (_worldScreens.OriginalWorldScreens[ws.ScreenIndexDown].ParentWorld != ws.ParentWorld)
                    {
                        //g.DrawRectangle(Pens.DeepSkyBlue, new Rectangle(rect.Left + worldExitRectangleScaleX, rect.Bottom - 5, worldExitRectangleScaleX, 5));
                        //g.DrawString(ws.ScreenIndexDown.ToString("X2"), drawFont, Pens.DeepSkyBlue.Brush, new Point(rect.Left + 3 + rect.Width / 3, rect.Bottom - 15));
                    }
                    else if (_worldScreens.OriginalWorldScreens[ws.ScreenIndexDown].IsWizardScreen())
                    {
                       // g.DrawLine(Pens.Orange, new Point(rect.Left, rect.Bottom - 1), new Point(rect.Right, rect.Bottom - 1));
                       // g.DrawLine(Pens.Orange, new Point(rect.Left + Left+ rect.Width / 2, rect.Bottom), new Point(rect.Left + rect.Width / 2, rect.Bottom - 7));
                    }

                    //UP
                    if (ws.ScreenIndexUp == 0xFF)
                    {
                        //g.DrawLine(Pens.Black, new Point(rect.Left, rect.Top + 1), new Point(rect.Right, rect.Top + 1));
                    }
                    else if (ws.ScreenIndexUp == 0xFE)
                    {
                        //g.DrawLine(Pens.Blue, new Point(rect.Left, rect.Top + 1), new Point(rect.Right, rect.Top + 1));
                        //g.DrawString(ws.Content.ToString("X2"), drawFont, Pens.Blue.Brush, drawContentPoint);
                    }
                    else if (_worldScreens.OriginalWorldScreens[ws.ScreenIndexUp].ParentWorld != ws.ParentWorld)
                    {
                        //g.DrawRectangle(Pens.DeepSkyBlue, new Rectangle(rect.Left + worldExitRectangleScaleX, rect.Top, worldExitRectangleScaleX, 5));
                        //g.DrawString(ws.ScreenIndexUp.ToString("X2"), drawFont, Pens.DeepSkyBlue.Brush, new Point(rect.Left + 3 +  rect.Width /3, rect.Top + 5));
                    }
                    else if (_worldScreens.OriginalWorldScreens[ws.ScreenIndexUp].IsWizardScreen())
                    {
                       // g.DrawLine(Pens.Orange, new Point(rect.Left, rect.Top + 1), new Point(rect.Right, rect.Top + 1));
                       // g.DrawLine(Pens.Orange, new Point(rect.Left + rect.Width / 2, rect.Top), new Point(rect.Left + rect.Width / 2, rect.Top + 7));
                    }

                    //RIGHT
                    if (ws.ScreenIndexRight == 0xFF)
                    {
                        //g.DrawLine(Pens.Black, new Point(rect.Right - 1, rect.Top), new Point(rect.Right - 1, rect.Bottom));
                    }
                    else if (ws.ScreenIndexRight == 0xFE)
                    {
                        //g.DrawLine(Pens.Blue, new Point(rect.Right - 1, rect.Top), new Point(rect.Right - 1, rect.Bottom));
                        //g.DrawString(ws.Content.ToString("X2"), drawFont, Pens.Blue.Brush, drawContentPoint);
                    }
                    else if (_worldScreens.OriginalWorldScreens[ws.ScreenIndexRight].ParentWorld != ws.ParentWorld)
                    {
                        //g.DrawRectangle(Pens.DeepSkyBlue, new Rectangle(rect.Right, rect.Top + worldExitRectangleScaleY, 5, worldExitRectangleScaleY));
                        //g.DrawString(ws.ScreenIndexRight.ToString("X2"), drawFont, Pens.DeepSkyBlue.Brush, new Point(rect.Right - 10, rect.Bottom - 30));
                    }
                    else if (_worldScreens.OriginalWorldScreens[ws.ScreenIndexRight].IsWizardScreen())
                    {
                      //  g.DrawLine(Pens.Orange, new Point(rect.Right - 1, rect.Top), new Point(rect.Right - 1, rect.Bottom));
                      //  g.DrawLine(Pens.Orange, new Point(rect.Right, rect.Bottom -  rect.Height / 2), new Point(rect.Right + 7, rect.Bottom - rect.Height / 2));
                    }

                    //LEFT
                    if (ws.ScreenIndexLeft == 0xFF)
                    {
                        //g.DrawLine(Pens.Black, new Point(rect.Left + 1, rect.Top), new Point(rect.Left + 1, rect.Bottom));
                    }
                    else if (ws.ScreenIndexLeft == 0xFE)
                    {
                        //g.DrawLine(Pens.Blue, new Point(rect.Left + 1, rect.Top), new Point(rect.Left + 1, rect.Bottom));
                        //g.DrawString(ws.Content.ToString("X2"), drawFont, Pens.Blue.Brush, drawContentPoint);
                    }
                    else if (_worldScreens.OriginalWorldScreens[ws.ScreenIndexLeft].ParentWorld != ws.ParentWorld)
                    {
                        //g.DrawRectangle(Pens.DeepSkyBlue, new Rectangle(rect.Left, rect.Top + worldExitRectangleScaleY, 5, worldExitRectangleScaleY));
                        //g.DrawString(ws.ScreenIndexLeft.ToString("X2"), drawFont, Pens.DeepSkyBlue.Brush, new Point(rect.Left + 5, rect.Bottom - 30));
                    }
                    else if (_worldScreens.OriginalWorldScreens[ws.ScreenIndexLeft].IsWizardScreen())
                    {
                      //  g.DrawLine(Pens.Orange, new Point(rect.Left + 1, rect.Top), new Point(rect.Left + 1, rect.Bottom));
                      //  g.DrawLine(Pens.Orange, new Point(rect.Left, rect.Bottom - rect.Height / 2), new Point(rect.Left - 7, rect.Bottom - rect.Height / 2));
                    }

                    g.DrawRectangle(Pens.Black, rect.Left, rect.Top, rect.Width, rect.Height);
                }

                pictureBox1.Refresh();

                DrawTileGrid(selectedIndex);
            }
        }

        private void DrawTileGrid(int screenIndex)
        {
            WorldScreen ws = _worldScreens.OriginalWorldScreens[selectedIndex];
            WorldScreenTileData tileData = ws.TileData;
            const int TILEVIEW_SIZE_X = 64;
            const int TILEVIEW_SIZE_Y = 64;

            using (var g = Graphics.FromImage(pb_tiles.Image))
            {
                    Color ground = getGroundColor(ws);
                    g.Clear(Color.White);

                    for (int y = 0; y < WorldScreenTileData.TILES_Y_COUNT; y++)
                    {
                        for (int x = 0; x < WorldScreenTileData.TILES_X_COUNT; x++)
                        {
                            byte tileValue = tileData.Tiles[x, y];

                            Rectangle rect = new Rectangle(x * TILEVIEW_SIZE_X, y * TILEVIEW_SIZE_Y, TILEVIEW_SIZE_X, TILEVIEW_SIZE_Y);

                            Brush brush = new SolidBrush(ground);   //and here
                            g.FillRectangle(brush, rect);
                        
                        //grid
                        //  g.DrawRectangle(Pens.Black, rect);
                        if (cb_show_tile_image.Checked)
                        {
                            if (TileImagePaths.ContainsKey(tileValue.ToString("X2")))
                            {
                                Image image = new Bitmap(@"Images/TileImages/" + TileImagePaths[tileValue.ToString("X2")]);
                                g.DrawImage(image, rect);
                            }
                        }
                        else
                        {
                        
                            g.DrawRectangle(Pens.Black, rect);
                        }


                        if (cb_show_tile_id.Checked)
                        {
                            Font drawFont = new Font("Arial", 9);
                            g.DrawString(tileValue.ToString("X2"), drawFont, Pens.White.Brush, rect.Left + 25, rect.Top + 25);
                        }
                        }
                    }
                    
                }
                pb_tiles.Refresh();
            
        }



        private void pictureBox1_Click(object sender, EventArgs e)
        {


            Point mousePosition = pictureBox1.PointToClient(Cursor.Position);

            int tileX = mousePosition.X / MAP_TILE_SIZE_X;
            int tileY = mousePosition.Y / MAP_TILE_SIZE_Y;

            Point worldScreenCoords = _map.GetWorldScreenCoordsFromGrid(tileX, tileY);

            if (lv_worldScreens.Items.Count > 0)
            {
                int selectedWorldIndex = _map._worldScreenIds[worldScreenCoords.X, worldScreenCoords.Y];
                lv_worldScreens.Items[selectedWorldIndex].Selected = true;
                lv_worldScreens.Items[selectedWorldIndex].Focused = true;
                lv_worldScreens.TopItem = lv_worldScreens.Items[selectedWorldIndex];
                lv_worldScreens.Select();

                MouseEventArgs me = (MouseEventArgs)e;
                if (me.Button == System.Windows.Forms.MouseButtons.Right)
                {
                    tb_screencollect.Text += "0x" + selectedWorldIndex.ToString("X2") + ",";
                }
            }
            //MessageBox.Show(worldScreenCoords.X.ToString() + "  " + worldScreenCoords.Y.ToString() + "   " + _map._worldScreenIds[worldScreenCoords.X, worldScreenCoords.Y]);

        }

        


        private void lv_worldScreens_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lv_worldScreens.SelectedIndices.Count > 0)
            {
               
                selectedIndex = lv_worldScreens.SelectedIndices[0];
                WorldScreen selectedScreen = _worldScreens.OriginalWorldScreens[selectedIndex];
                label1.Text = "World Screen 0x" + selectedIndex.ToString("X2") + " (" + selectedIndex + ")";

                selectedIndex = lv_worldScreens.SelectedIndices[0];
                lbl_grid_vars.Text = "0x" + selectedIndex.ToString("X2") + " (" + selectedIndex + ")\r\n" +
                    "DataPointer:" + selectedScreen.DataPointer.ToString("X2") + "\r\n" + 
                "Top: 0x" + selectedScreen.TopTiles.ToString("X2") + " (" + selectedScreen.TopTiles + ")\r\n" +
                "Bottom: 0x" + selectedScreen.BottomTiles.ToString("X2") + " (" + selectedScreen.BottomTiles + ")\r\n";
                ;

                updateVariableListBox(_worldScreens.OriginalWorldScreens[selectedIndex]);
                btn_updateMap.PerformClick();
            }





        }



        private void updateVariableListBox(WorldScreen ws)
        {
         

            lv_variables.Items[(int)WorldScreen.DataContent.ParentWorld].SubItems[1].Text = ws.ParentWorld.ToString("X2");
            lv_variables.Items[(int)WorldScreen.DataContent.AmbientSound].SubItems[1].Text = ws.AmbientSound.ToString("X2");
            lv_variables.Items[(int)WorldScreen.DataContent.Content].SubItems[1].Text = ws.Content.ToString("X2");
            lv_variables.Items[(int)WorldScreen.DataContent.ObjectSet].SubItems[1].Text = ws.ObjectSet.ToString("X2");
            lv_variables.Items[(int)WorldScreen.DataContent.ScreenIndexRight].SubItems[1].Text = ws.ScreenIndexRight.ToString("X2");
            lv_variables.Items[(int)WorldScreen.DataContent.ScreenIndexLeft].SubItems[1].Text = ws.ScreenIndexLeft.ToString("X2");
            lv_variables.Items[(int)WorldScreen.DataContent.ScreenIndexDown].SubItems[1].Text = ws.ScreenIndexDown.ToString("X2");
            lv_variables.Items[(int)WorldScreen.DataContent.ScreenIndexUp].SubItems[1].Text = ws.ScreenIndexUp.ToString("X2");
            lv_variables.Items[(int)WorldScreen.DataContent.DataPointer].SubItems[1].Text = ws.DataPointer.ToString("X2");
            lv_variables.Items[(int)WorldScreen.DataContent.ExitPosition].SubItems[1].Text = ws.ExitPosition.ToString("X2");
            lv_variables.Items[(int)WorldScreen.DataContent.TopTiles].SubItems[1].Text = ws.TopTiles.ToString("X2");
            lv_variables.Items[(int)WorldScreen.DataContent.BottomTiles].SubItems[1].Text = ws.BottomTiles.ToString("X2");
            lv_variables.Items[(int)WorldScreen.DataContent.WorldScreenColor].SubItems[1].Text = ws.WorldScreenColor.ToString("X2");
            lv_variables.Items[(int)WorldScreen.DataContent.SpritesColor].SubItems[1].Text = ws.SpritesColor.ToString("X2");
            lv_variables.Items[(int)WorldScreen.DataContent.Unknown].SubItems[1].Text = ws.Unknown.ToString("X2");
            lv_variables.Items[(int)WorldScreen.DataContent.Event].SubItems[1].Text = ws.Event.ToString("X2");

            //hints
            //content
            if (KnownContents.ContainsKey(ws.Content.ToString("X2"))) lv_variables.Items[(int)WorldScreen.DataContent.Content].SubItems[2].Text = KnownContents[ws.Content.ToString("X2")];
            else lv_variables.Items[(int)WorldScreen.DataContent.Content].SubItems[2].Text = "?";

            //objectSets
            if (KnownObjectSets.ContainsKey(ws.ObjectSet.ToString("X2"))) lv_variables.Items[(int)WorldScreen.DataContent.ObjectSet].SubItems[2].Text = KnownObjectSets[ws.ObjectSet.ToString("X2")];
            else lv_variables.Items[(int)WorldScreen.DataContent.ObjectSet].SubItems[2].Text = "?";

            //events
            if (KnownEvents.ContainsKey(ws.Event.ToString("X2"))) lv_variables.Items[(int)WorldScreen.DataContent.Event].SubItems[2].Text = KnownEvents[ws.Event.ToString("X2")];
            else lv_variables.Items[(int)WorldScreen.DataContent.Event].SubItems[2].Text = "?";

            //screenexits
            if (KnownScreenExits.ContainsKey(ws.ScreenIndexLeft.ToString("X2"))) lv_variables.Items[(int)WorldScreen.DataContent.ScreenIndexLeft].SubItems[2].Text = KnownScreenExits[ws.ScreenIndexLeft.ToString("X2")];
            else lv_variables.Items[(int)WorldScreen.DataContent.ScreenIndexLeft].SubItems[2].Text = "enter screen " + ws.ScreenIndexLeft.ToString("X2");

            if (KnownScreenExits.ContainsKey(ws.ScreenIndexRight.ToString("X2"))) lv_variables.Items[(int)WorldScreen.DataContent.ScreenIndexRight].SubItems[2].Text = KnownScreenExits[ws.ScreenIndexRight.ToString("X2")];
            else lv_variables.Items[(int)WorldScreen.DataContent.ScreenIndexRight].SubItems[2].Text = "enter screen " + ws.ScreenIndexRight.ToString("X2");

            if (KnownScreenExits.ContainsKey(ws.ScreenIndexUp.ToString("X2"))) lv_variables.Items[(int)WorldScreen.DataContent.ScreenIndexUp].SubItems[2].Text = KnownScreenExits[ws.ScreenIndexUp.ToString("X2")];
            else lv_variables.Items[(int)WorldScreen.DataContent.ScreenIndexUp].SubItems[2].Text = "enter screen " + ws.ScreenIndexUp.ToString("X2");

            if (KnownScreenExits.ContainsKey(ws.ScreenIndexDown.ToString("X2"))) lv_variables.Items[(int)WorldScreen.DataContent.ScreenIndexDown].SubItems[2].Text = KnownScreenExits[ws.ScreenIndexDown.ToString("X2")];
            else lv_variables.Items[(int)WorldScreen.DataContent.ScreenIndexDown].SubItems[2].Text = "enter screen " + ws.ScreenIndexDown.ToString("X2");

        }

        private void cb_fill_map_tiles_CheckedChanged(object sender, EventArgs e)
        {
            btn_updateMap.PerformClick();
        }

        private void cb_show_tile_id_CheckedChanged(object sender, EventArgs e)
        {
            DrawTileGrid(selectedIndex);
        }

        private void cb_show_tile_image_CheckedChanged(object sender, EventArgs e)
        {
            DrawTileGrid(selectedIndex);
        }

    }
}
