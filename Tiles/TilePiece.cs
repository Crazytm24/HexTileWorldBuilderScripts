using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

// This is the main component that dictates the state of the tile
public class TilePiece : MonoBehaviour
{
    public Dictionary<int, TilePiece> neighbors = new Dictionary<int, TilePiece>(); // Store neighbors (0-5)
    public TileHeightService tileHeightService;
    private bool BLOCKED = false;
    private bool isPath = false;
    public GameObject pathTiles;
    public GameObject grassTile;

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

    public void ClickOnTile(ClickType type, bool isAdding, bool shiftClicked)
    {
        if (!BLOCKED)
        {
            switch (type)
            {
                case ClickType.HeightChange:
                    ChangeTileHeight(isAdding);
                    break;
                case ClickType.AddPath:
                    ChangePath(isAdding, 0);
                    break;
            }
            if (shiftClicked)
            {
                foreach (var neigbor in neighbors.Values)
                {
                    neigbor.ClickOnTile(type, isAdding, false);
                }
            }
        }
    }

    private void ChangeTileHeight(bool isRaising)
    {
        if (!BLOCKED)
        {
            tileHeightService.ChangeTileHeight(isRaising);
        }
    }

    private void ChangePath(bool isAdding, int tagType)
    {
        if (!BLOCKED)
        {
            isPath = isAdding;
            neighbors.Select(x => x.Value).ToList().ForEach(neighbor => neighbor.ChangeTile());
            ChangeTile();
        }
    }

    // Hover over tile effects
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

    public void ChangeTile()
    {
        TurnOffAllTiles();
        if (!isPath){
            grassTile.SetActive(true);
            return;
        }
        grassTile.SetActive(false);
        RotationData rotData = new();
        List<int> neighborsWithPaths = neighbors.Where(x => x.Value.isPath).Select(x => x.Key).ToList();
        var data = rotData.FindConnectionRotation(neighborsWithPaths);
        if (data.connectionType == 0){
            pathTiles.transform.GetChild(0).GetChild(0).gameObject.SetActive(true);
        }
        else {
            var groupNum = (int)Math.Truncate((float)data.connectionType / 10);
            var tileNum = (int)data.connectionType - (groupNum * 10);
            var tile = pathTiles.transform.GetChild(groupNum).GetChild(tileNum);
            tile.rotation = Quaternion.Euler(tile.rotation.x, data.rotationAmount, tile.rotation.z);
            tile.gameObject.SetActive(true);
        }
    }

    // turn off refering to disabling the visual tile topper piece
    private void TurnOffAllTiles(){
        for(int i = 0; i < pathTiles.transform.childCount; i++){
            var group = pathTiles.transform.GetChild(i);
            for(int t = 0; t < group.transform.childCount; t++){
                var tile = group.transform.GetChild(t);
                tile.rotation = Quaternion.Euler(tile.rotation.x, 0, tile.rotation.z);
                tile.gameObject.SetActive(false);
            }
        }
    }
}
