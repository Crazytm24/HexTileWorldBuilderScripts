using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class TilePiece : MonoBehaviour
{
    public Dictionary<int, TilePiece> neighbors = new Dictionary<int, TilePiece>(); // Store neighbors (0-5)
    public TileHeightService tileHeightService;
    private bool BLOCKED = false;

    private void Start()
    {
        tileHeightService = gameObject.AddComponent<TileHeightService>();
    }

    public void AddNeighbor(int direction, TilePiece neighbor)
    {
        if (!neighbors.ContainsKey(direction))
        {
            neighbors[direction] = neighbor;
        }
    }

    public List<TilePiece> GetNeighbors()
    {
        return new List<TilePiece>(neighbors.Values);
    }

    public void ClickOnTile(bool shiftClicked)
    {
        if (!BLOCKED)
        {
            RaiseTile();
            if (shiftClicked)
            {
                foreach (var neigbor in neighbors.Values)
                {
                    neigbor.RaiseTile();
                }
            }
        }
    }

    private void RaiseTile()
    {
        if (!BLOCKED)
        {
            tileHeightService.RaiseTile();
        }
    }

    public void AlertOfHover(bool shiftClicked)
    {
        if (tileHeightService && !BLOCKED)
        {
            tileHeightService.AlertAreaOfHighlight();
            if (shiftClicked)
            {
                foreach (var neigbor in neighbors.Values)
                {
                    neigbor.AlertOfHover(false);
                }
            }
        }
    }

    public void BlockTile()
    {
        BLOCKED = true;
    }
}
