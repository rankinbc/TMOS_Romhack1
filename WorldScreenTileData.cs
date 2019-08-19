using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TMOS_Romhack
{
    public class WorldScreenTileData
    {

        byte[] topTileChunk;
        byte[] bottomTileChunk;
        const int TILE_CHUNK_SIZE = 0x20;
        
        public static int TILES_Y_COUNT = 6;
        public static int TILES_X_COUNT = 8;

        public byte[,] Tiles;

        public WorldScreenTileData(byte[] RomTileData, byte dataPointer, byte topTilesByte, byte bottomTilesByte)
        {
            Tiles = new byte[8, 6];
            byte[] topTileChunk = new byte[32];
            byte[] bottomTileChunk = new byte[32];

            int topTileDataStartIndex = 0x0000;
            int bottomTileDataStartIndex = 0x0000;

            if (dataPointer >= 0x40 && dataPointer < 0x8f)
            {
                bottomTileDataStartIndex = 0x2000;
                topTileDataStartIndex = 0x0000;
            }

            else if (dataPointer >= 0x8f && dataPointer < 0xA0)
            {
                bottomTileDataStartIndex = 0x0000;
                topTileDataStartIndex = 0x2000;
            }
            else if (dataPointer >= 0xC0)
            {
                topTileDataStartIndex = 0x2000; 
                bottomTileDataStartIndex = 0x2000;
            }

            int topChunkIndex = topTileDataStartIndex + (topTilesByte * TILE_CHUNK_SIZE);
            int bottomChunkIndex = bottomTileDataStartIndex + (bottomTilesByte * TILE_CHUNK_SIZE);
            Array.Copy(RomTileData, topChunkIndex, topTileChunk, 0, TILE_CHUNK_SIZE);
            Array.Copy(RomTileData, bottomChunkIndex, bottomTileChunk, 0, TILE_CHUNK_SIZE);

            int i = 0;
            for (int y = 0; y < 4; y++ )
            {
                for (int x = 0; x < 8; x++)
                {
                    Tiles[x, y] = topTileChunk[i];
                    i++;
                }
            }

            i = 0;
            for (int y = 4; y < 6; y++)
            {
                for (int x = 0; x < 8; x++)
                {
                    Tiles[x, y] = bottomTileChunk[i];
                    i++;
                }
            }


            int b = 0;
            
        }
    }
}
