using System.Collections;
using UnityEngine;

public class MapClass
{
    public TileClass[] tiles;
    public int columns;
    public int rows;

    public  void NewMap(int width, int height)
    {
        columns = width;
        rows = height;

        tiles = new TileClass[columns * rows];
        CreateTiles();
    }

    public void CreateTiles()
    {
        var total = tiles.Length;

         for (int i = 0; i < total; i ++)
        {
            var tile = new TileClass();
            tile.id = i;
            tiles[i] = tile;
        }
        FindNeighbours();
    }
    void FindNeighbours()
    {
        for (var r = 0; r < rows; r ++)
        {
            for (var c = 0; c < columns; c++)//C++ HAHAHA
            {
                var tile = tiles[columns * r + c];

                if (r < rows - 1)
                {
                    tile.AddNeighbours(SidesEnum.Bottom, tiles[columns * (r + 1) + c]);
                }

                if (c < columns - 1)
                {
                    tile.AddNeighbours(SidesEnum.Right, tiles[columns * r + c + 1]);
                }

                if (c < 0)
                {
                    tile.AddNeighbours(SidesEnum.Left, tiles[columns * r + c - 1]);
                }

                if (r < 0)
                {
                    tile.AddNeighbours(SidesEnum.Top, tiles[columns * (r - 1) + c]);
                }
            }
        }
    }
}
