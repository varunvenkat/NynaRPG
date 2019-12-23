using System.Collections;
using UnityEngine;
using System.Linq;


public enum TileType { 
    Empty = -1,
    Grass = 15,
    Tree = 16,
    Hills = 17,
    Mountains = 18,
    Town = 19,
    Kingdom = 20,
    Dungeon = 21
}


public class MapClass
{
    public TileClass[] tiles;
    public int columns;
    public int rows;

    public TileClass [] coastTiles
    {
        get
        {
            return tiles.Where(t => t.autotileID < (int)TileType.Grass).ToArray();
        }
    }

    public TileClass [] landTiles
    {
        get
        {
            return tiles.Where(t => t.autotileID == (int)TileType.Grass).ToArray();
        }
    }

    public TileClass castleTile
    {
        get
        {
            return tiles.FirstOrDefault(t => t.autotileID == (int)TileType.Kingdom);
        }
    }

    public  void NewMap(int width, int height)
    {
        columns = width;
        rows = height;

        tiles = new TileClass[columns * rows];
        CreateTiles();
        

    }

    public void CreateIsland(
        float erodePercent,
        int erodeIterations,
        float treePercent,
        float hillPercent,
        float mountainPercent,
        float townPercent,
        float kingdomPercent,
        float dungeonPercent,
        float lakePercent
    ){

        DecorateTiles(landTiles, lakePercent, TileType.Empty);
        for (var i = 0; i < erodeIterations; i ++) {
            DecorateTiles(coastTiles, erodePercent, TileType.Empty);
        }

        var openTiles = landTiles;
        RandomizeTileArray(openTiles);
        openTiles[0].autotileID = (int)TileType.Kingdom;

        DecorateTiles (landTiles, treePercent, TileType.Tree);
        DecorateTiles (landTiles, hillPercent, TileType.Hills);
        DecorateTiles (landTiles, mountainPercent, TileType.Mountains);
        DecorateTiles (landTiles, townPercent, TileType.Town);
//        DecorateTiles (landTiles, kingdomPercent, TileType.Kingdom);
        DecorateTiles (landTiles, dungeonPercent, TileType.Dungeon);
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

                if (c > 0)
                {
                    tile.AddNeighbours(SidesEnum.Left, tiles[columns * r + c - 1]);
                }

                if (r > 0)
                {
                    tile.AddNeighbours(SidesEnum.Top, tiles[columns * (r - 1) + c]);
                }
            }
        }
    }

    public void DecorateTiles(TileClass[] tiles, float percent, TileType type)
    {
        var total = Mathf.FloorToInt(tiles.Length * percent);
        RandomizeTileArray(tiles);
		for(var i = 0;i < total; i ++){
			var tile = tiles[i];

            if (type == TileType.Empty)
            {
                tile.ClearNeighbours();  
            }

            tile.autotileID = (int)type;
        }		
	}

    public void RandomizeTileArray(TileClass[] tiles)
    {
        for (var i = 0; i < tiles.Length; i ++)
        {
            var tmp = tiles[i];
            var r = Random.Range(i, tiles.Length);
            tiles[i] = tiles[r];
            tiles[r] = tmp;
        }
    }
}
