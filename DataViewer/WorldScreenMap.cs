using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TMOS_Romhack.DataViewer
{
    class WorldScreenMap
    {
        DataViewForm _parentForm;
        WorldScreenCollection _worldScreenCollection;
        bool[] _mapIndexUsed;
        public WorldScreen[,] _worldScreens { get; set; }
        public int[,] _worldScreenIds { get; set; }

        int farthestLeftTilePosition;
        int farthestRightTilePosition;
        int farthestTopTilePosition;
        int farthestBottomTilePosition;

        public WorldScreenMap(DataViewForm parent,WorldScreenCollection worldScreenCollection)
        {
            _parentForm = parent;
            _worldScreenCollection = worldScreenCollection;

            InitalizeData();
        }

        public void InitalizeData()
        {
            _worldScreens = new WorldScreen[32, 32];
            _worldScreenIds = new int[32, 32];
            _mapIndexUsed = new bool[255];

            farthestLeftTilePosition = 16;
            farthestRightTilePosition = 16;
            farthestTopTilePosition = 16;
            farthestBottomTilePosition = 16;
        }

      
        public void LoadWorldMap(int currentScreenIndex, int x, int y)
        {

            WorldScreen worldScreen = _worldScreenCollection.OriginalWorldScreens[currentScreenIndex];

          /*  if (worldScreen.IsWizardScreen())
            {
                _mapIndexUsed[currentScreenIndex] = true;
                WorldScreen wizardExitScreen;
                if (worldScreen.ScreenIndexRight != 0xFF)
                {
                    wizardExitScreen = _worldScreenCollection.OriginalWorldScreens[worldScreen.ScreenIndexRight];
                    LoadWorldMap(worldScreen.ScreenIndexRight, x , y);
                }
                else if(worldScreen.ScreenIndexLeft != 0xFF)
                {
                    wizardExitScreen = _worldScreenCollection.OriginalWorldScreens[worldScreen.ScreenIndexRight];
                    LoadWorldMap(worldScreen.ScreenIndexLeft, x, y);
                }
                else if (worldScreen.ScreenIndexDown != 0xFF)
                {
                    LoadWorldMap(worldScreen.ScreenIndexDown, x, y);
                }
                else if (worldScreen.ScreenIndexUp != 0xFF)
                {
                    LoadWorldMap(worldScreen.ScreenIndexUp, x, y);
                }
                return;
            }*/


             _mapIndexUsed[currentScreenIndex] = true;
            _parentForm.lv_worldScreens.Items[currentScreenIndex].ForeColor = Color.Green;
            _worldScreens[x,y] = worldScreen;
            _worldScreenIds[x, y] = currentScreenIndex;

            if (worldScreen.ScreenIndexRight < 0xF0 && !_mapIndexUsed[worldScreen.ScreenIndexRight]  && 
                _worldScreenCollection.OriginalWorldScreens[worldScreen.ScreenIndexRight].ParentWorld == worldScreen.ParentWorld)
            {
                int xRight = x + 1;
                if (farthestRightTilePosition < xRight) farthestRightTilePosition = xRight;
                LoadWorldMap(worldScreen.ScreenIndexRight, xRight, y);
               
            }
            if (worldScreen.ScreenIndexLeft < 0xF0 && !_mapIndexUsed[worldScreen.ScreenIndexLeft] &&
                _worldScreenCollection.OriginalWorldScreens[worldScreen.ScreenIndexLeft].ParentWorld == worldScreen.ParentWorld)
            {
                int xLeft = x - 1;
                if (farthestLeftTilePosition > xLeft) farthestLeftTilePosition = xLeft;
                LoadWorldMap(worldScreen.ScreenIndexLeft, xLeft, y);
              
            }
            if (worldScreen.ScreenIndexDown < 0xF0 && !_mapIndexUsed[worldScreen.ScreenIndexDown] &&
                _worldScreenCollection.OriginalWorldScreens[worldScreen.ScreenIndexDown].ParentWorld == worldScreen.ParentWorld)
            {
                int yDown = y - 1;
                if (farthestBottomTilePosition > yDown) farthestBottomTilePosition = yDown;
                LoadWorldMap(worldScreen.ScreenIndexDown, x, yDown);
               
            }
            if (worldScreen.ScreenIndexUp < 0xF0 && !_mapIndexUsed[worldScreen.ScreenIndexUp] &&
                _worldScreenCollection.OriginalWorldScreens[worldScreen.ScreenIndexUp].ParentWorld == worldScreen.ParentWorld)
            {
                int yUp = y + 1;
                if (farthestTopTilePosition < yUp)
                    farthestTopTilePosition = yUp;
                LoadWorldMap(worldScreen.ScreenIndexUp, x, yUp);
              
            }
      
        }

        public Dictionary<int,Rectangle> DrawWorldMap(int tileSizeX, int tileSizeY)
        {
            Dictionary<int, Rectangle> rects = new Dictionary<int, Rectangle>();
            int x = farthestLeftTilePosition;
            int y = farthestBottomTilePosition;

            int grid_position_x = 0;
          

            for (x = farthestLeftTilePosition; x <=farthestRightTilePosition; x++, grid_position_x++)
            {
                int grid_position_y = Math.Abs(farthestTopTilePosition - farthestBottomTilePosition);

                for (y = farthestBottomTilePosition; y <= farthestTopTilePosition; y++, grid_position_y--)
                {
                    
                    if (_worldScreens[x, y] != null)
                    {     
                        rects.Add(_worldScreenIds[x, y],new Rectangle(grid_position_x * tileSizeX,grid_position_y * tileSizeY, tileSizeX, tileSizeY));
                    }
                }
            }
            return rects;
        }

        public Point GetWorldScreenCoordsFromGrid(int tileX, int tileY)
        {
            Point p = new Point();
            p.X = farthestLeftTilePosition + tileX;
            p.Y = farthestTopTilePosition - tileY; /// n - tiley
            return p;
        }
    }
}
