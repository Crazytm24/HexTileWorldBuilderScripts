using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[ExecuteInEditMode]
public class GridMaker : MonoBehaviour
{
    [Header("Grid Settings")]
    public Vector2Int gridSize = new Vector2Int(5, 5);

    [Header("Tile Settings")]
    public float outerSize = 2.8864f;
    public bool isFlatTopped = true;
    public GameObject tilePiece;

    private Dictionary<Vector2Int, TilePiece> tileDictionary = new Dictionary<Vector2Int, TilePiece>();

    private Vector2Int lastGridSize;
    private float lastOuterSize;
    private bool lastIsFlatTopped;

    private List<Vector2Int> oddNeighborNodes = new()
    {
        new Vector2Int(1, 0),
        new Vector2Int(0, -1),
        new Vector2Int(-1, 0),
        new Vector2Int(-1, 1),
        new Vector2Int(0, 1),
        new Vector2Int(1, 1),
    };

    private List<Vector2Int> evenNeighborNodes = new()
    {
        new Vector2Int(1, 0),
        new Vector2Int(0, -1),
        new Vector2Int(-1, 0),
        new Vector2Int(-1, -1),
        new Vector2Int(0, 1),
        new Vector2Int(1, -1),
    };

    private void Start()
    {
        GenerateGrid();
    }

    private void Update()
    {
        if (gridSize != lastGridSize || outerSize != lastOuterSize || isFlatTopped != lastIsFlatTopped)
        {
            ClearGrid();
            GenerateGrid();

            lastGridSize = gridSize;
            lastOuterSize = outerSize;
            lastIsFlatTopped = isFlatTopped;
        }
    }

    private void GenerateGrid()
    {
        tileDictionary.Clear();

        if (tilePiece == null)
        {
            Debug.LogError("Tile Piece Prefab is missing!");
            return;
        }

        for (int y = 0; y < gridSize.y; y++)
        {
            for (int x = 0; x < gridSize.x; x++)
            {
                Vector2Int coord = new Vector2Int(x, y);
                Vector3 hexPosition = GetPositionForHexFromCoordinate(coord);

                GameObject tileObj = Instantiate(tilePiece, hexPosition, Quaternion.identity, transform);
                tileObj.name = $"Hex {x}, {y}";

                TilePiece tile = tileObj.GetComponentInChildren<TilePiece>();
                if (tile != null)
                {
                    tileDictionary[coord] = tile;
                }
            }
        }

        AssignNeighbors();
    }

    private void AssignNeighbors()
    {
        foreach (var entry in tileDictionary)
        {
            Vector2Int coord = entry.Key;
            TilePiece tile = entry.Value;

            var neighborNodes = (coord.x % 2 == 0) ? evenNeighborNodes : oddNeighborNodes;

            neighborNodes
                .Select((neighbor, index) => new { index, neighborCoord = coord + neighbor })
                .Where(n => tileDictionary.ContainsKey(n.neighborCoord))
                .ToList()
                .ForEach(n => tile.AddNeighbor(n.index, tileDictionary[n.neighborCoord]));

            if (tile.GetNeighbors().Count < 6)
            {
                tile.BlockTile();
            }
        }
    }

    private Vector3 GetPositionForHexFromCoordinate(Vector2Int coord)
    {
        float xOffset = isFlatTopped ? 1.5f : Mathf.Sqrt(3);
        float yOffset = isFlatTopped ? Mathf.Sqrt(3) : 1.5f;

        float xPos = coord.x * xOffset * outerSize;
        float yPos = coord.y * yOffset * outerSize;

        if (!isFlatTopped)
        {
            xPos += (coord.y % 2) * (xOffset * outerSize / 2f);
        }
        else
        {
            yPos += (coord.x % 2) * (yOffset * outerSize / 2f);
        }

        return new Vector3(xPos, 0, yPos);
    }

    private void ClearGrid()
    {
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            DestroyImmediate(transform.GetChild(i).gameObject);
        }
        tileDictionary.Clear();
    }
}
