using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomMapTester : MonoBehaviour
{
    [Header("Map Dimensions")]
    public int mapWidth = 20;
    public int mapHeight = 20;
    [Space]
    [Header("Visualise Map")]

    public GameObject mapContainer;
    public GameObject tilePrefab;
    public Vector2 tileSize = new Vector2(16,16);

    [Space]
    [Header("Map Sprites")]
    public Texture2D islandTexture;
    public Texture2D fowTexture;

    [Space]
    [Header("Player")]
    public GameObject playerPrefab;
    public GameObject player;
    public int onTileType = 20;
    public int distance = 3;    

    [Space]
    [Header("Decorate Map")]
    [Range(0, 0.9f)]
    public float erodePercent = 0.5f;
    [Range(0, 0.9f)]
    public float treePercent = 0.3f;
    [Range(0, 0.9f)]
    public float hillPercent = 0.2f;
    [Range(0, 0.9f)]
    public float mountainPercent = 0.1f;
    [Range(0, 0.9f)]
    public float townPercent = 0.05f;
    [Range(0, 0.9f)]
    public float kingdomPercent = 0.05f;
    [Range(0, 0.9f)]
    public float dungeonPercent = 0.05f;
    [Range(0, 0.9f)]
    public float lakePercent = 0.05f;

    public int erodeIterations = 2;

    public MapClass map;
    private int tmpX;
    private int tmpY;
    private Sprite[] islandTileSprites;
    private Sprite[] fowTileSprites;

    void Start()
    {
        islandTileSprites = Resources.LoadAll<Sprite>(islandTexture.name);
        fowTileSprites = Resources.LoadAll<Sprite>(fowTexture.name);

        Reset();   
    }

    public void Reset()
    {
        map = new MapClass();
        MakeMap();
        StartCoroutine(AddPlayer());
    }

    IEnumerator AddPlayer()
    {
        yield return new WaitForEndOfFrame();
        CreatePlayer();
    }

    void FixedUpdate()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            MakeMap();
        }
    }

    public void MakeMap()
    {
        map.NewMap(mapWidth, mapHeight);
        map.CreateIsland(
            erodePercent,
            erodeIterations,
            treePercent,
            hillPercent,
            mountainPercent,
            townPercent,
            kingdomPercent,
            dungeonPercent,
            lakePercent
        );
        CreateGrid();
        CenterMap(map.castleTile.id);
    }

    void CreateGrid()
    {
        ClearMapContainer();

        var total = map.tiles.Length;
        var maxColumns = map.columns;
        var column = 0;
        var row = 0;

        for (var i = 0; i < total; i++)
        {
            column = i % maxColumns;

            var newX = column * tileSize.x;
            var newY = -row * tileSize.y;

            var go = Instantiate(tilePrefab);
            go.name = "Tile " + i;
            go.transform.SetParent(mapContainer.transform);
            go.transform.position = new Vector3(newX, newY, 0);

            DecorateTile(i);

            if (column == (maxColumns - 1))
            {
                row++;
            }
        }
    }

    private void DecorateTile(int tileID)
    {
        var tile = map.tiles[tileID];
        var spriteID = tile.autotileID;
        var go = mapContainer.transform.GetChild(tileID).gameObject;

        if (spriteID >= 0)
        {
            var sr = go.GetComponent<SpriteRenderer>();
            if (tile.visited)
            {
                sr.sprite = islandTileSprites[spriteID];
            }
            else
            {
                tile.CalculateFoWAutotileID();
                sr.sprite = fowTileSprites[Mathf.Min(tile.fowAutotileID, fowTileSprites.Length - 1)];
            }
        }
        
    }

    public void CreatePlayer()
{
        player = Instantiate(playerPrefab);
        player.name = "Player";
        player.transform.SetParent(mapContainer.transform);

        var controller = player.GetComponent<MapMovementController>();
        controller.map = map;
        controller.tileSize = tileSize;
        controller.tileActionCallback += TileActionCallback;

        var moveScript = Camera.main.GetComponent<MoveCamera>();
        moveScript.target = player;

        controller.MoveTo(map.castleTile.id);
    }

    void TileActionCallback(int type)
    {
        onTileType = type;
        var tileID = player.GetComponent<MapMovementController>().currentTile;
        VisitTile(tileID);
    }

    void ClearMapContainer(){
        var children = mapContainer.transform.GetComponentsInChildren<Transform>();

            for (var i = children.Length - 1; i > 0; i--)
            {
                Destroy(children[i].gameObject);
            }
        }
    

    void CenterMap(int index)
    {
        var camPos = Camera.main.transform.position;
        var width = map.columns;

        PosUtil.CalculatePos(index, width, out tmpX, out tmpY);

        camPos.x = tmpX * tileSize.x;
        camPos.y = -tmpY * tileSize.y;         
        Camera.main.transform.position = camPos;
    }

    void VisitTile(int index)
    {
        int column, newX, newY, row = 0;

        PosUtil.CalculatePos(index, map.columns, out tmpX, out tmpY);

        var half = Mathf.FloorToInt(distance / 2f);
        tmpX -= half;
        tmpY -= half;

        var total = distance * distance;
        var maxColumns = distance - 1;

        for (int i = 0; i < total; i++)
        {
            column = i % distance;

            newX = column + tmpX;
            newY = row + tmpY;

            PosUtil.CalculateIndex(newX, newY, map.columns, out index);

            if (index > -1 && index < map.tiles.Length)
            {
                var tile = map.tiles[index];
                tile.visited = true;
                DecorateTile(index);

                foreach (var neighbour in tile.neighbours)
                {
                    if (neighbour != null)
                    {
                        if (!neighbour.visited)
                        {
                            neighbour.CalculateFoWAutotileID();
                            DecorateTile(neighbour.id);
                        }
                    }
                }
            }
            if (column == maxColumns)
            {
                row ++;
            }
        }

    }
}
