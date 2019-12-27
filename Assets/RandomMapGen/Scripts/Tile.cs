using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Text;



public enum SidesEnum
{
    Bottom,
    Right,
    Left,
    Top
}


public class TileClass
{
    public int id = 0;
    public TileClass[] neighbours = new TileClass[4];
    public int autotileID;
    public bool visited;
    public int fowAutotileID;

    public void AddNeighbours(SidesEnum side, TileClass tile){
        neighbours[(int)side] = tile;
        CalculateAutotileID();
    }

    public void RemoveNeighbour(TileClass tile)
    {
        var total = neighbours.Length;
        for (var i = 0; i < total; i ++)
        {
            if (neighbours[i] != null)
            {
                if (neighbours[i].id == tile.id)
                {
                    neighbours [i] = null;
                }
            }
        }
        CalculateAutotileID();
    }

    public void ClearNeighbours()
    {
        var total = neighbours.Length;
        for (var i = 0; i < total; i ++)
        {
            var tile = neighbours[i];
            if (tile != null)
            {
                tile.RemoveNeighbour(this);
                neighbours[i] = null;
            }
        }

        CalculateAutotileID();
    }

    private void CalculateAutotileID()
    {			
        var sideValues = new StringBuilder();

        foreach (TileClass tile in neighbours)
        {
            sideValues.Append(tile == null ? "0" : "1");
        }

        autotileID = Convert.ToInt32(sideValues.ToString(), 2);
    }
    private void CalculateFoWAutotileID()
    {
        var sideValues = new StringBuilder();

        foreach (TileClass tile in neighbours)
        {
            if(tile == null)
            {
                sideValues.Append("0");
            }
            else
            {
                sideValues.Append(tile.visited ? "0" : "1");
            }
        }

        fowAutotileID = Convert.ToInt32(sideValues.ToString(), 2);
    }
}
