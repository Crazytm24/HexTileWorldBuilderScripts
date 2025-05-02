using System.Collections.Generic;
using System;
using UnityEngine;

// Inorder for hover to work correctly, Hover Layer must be applied to the GameObject
public class TileHeightService : MonoBehaviour
{
    private TileHeightInitialData tileData;
    private float targetHeight;

    // I dont think this should ever need to reference the parent
    //private TilePiece tilePiece;

    //public TileHeightService(TilePiece FatherTile)
    //{
    //    tilePiece = FatherTile;
    //}

    private void Start()
    {
        tileData = DataManager.Instance.GetInitialData<TileHeightInitialData>();
        targetHeight = tileData.originalHeight;
        if (tileData.sourceMat == null)
        {
            tileData.sourceMat = gameObject.GetComponent<MeshRenderer>().material;
        }
        tileData.newMat = new(tileData.sourceMat);

        if (gameObject.GetComponent<MeshRenderer>()) gameObject.GetComponent<MeshRenderer>().material = tileData.newMat;
        tileData.childMeshRends = new List<MeshRenderer>(GetComponentsInChildren<MeshRenderer>());

        if (tileData.childMeshRends.Count > 0)
        {
            foreach (MeshRenderer meshie in tileData.childMeshRends)
            {
                meshie.material = tileData.newMat;
            }
        }
    }

    private void Update()
    {
        ManageHoverHeight();
        ManageClickHeight();    // ClickHeight and HoverHeight are fighting each other, looks fine on faster computers but needs improvement
        ColorTransitions();
    }

    private void LateUpdate()
    {
        if (tileData.lastSwitchGate != tileData.switchGate) tileData.shouldBeHighlighted = true;
        else tileData.shouldBeHighlighted = false;
        tileData.lastSwitchGate = tileData.switchGate;
    }

    private void ManageHoverHeight()
    {
        var adjustedPosition = new Vector3(transform.position.x, tileData.shouldBeHighlighted ? targetHeight + tileData.hoverHeightIncrease : targetHeight, transform.position.z);
        transform.position = Vector3.Lerp(transform.position, adjustedPosition, Time.deltaTime * tileData.hoverChangeSpeed);
    }

    private void ManageClickHeight()
    {
        float displacement = targetHeight - transform.position.y;
        float springForce = displacement * tileData.stiffness;
        float dampingForce = tileData.velocity * tileData.damping;

        tileData.velocity += (springForce - dampingForce) * Time.deltaTime;
        transform.position += new Vector3(0, tileData.velocity * Time.deltaTime, 0);

        if (Mathf.Abs(tileData.velocity) < 0.05 && Mathf.Abs(displacement) < 0.05)
        {
            transform.position = new Vector3(transform.position.x, targetHeight, transform.position.z);
            tileData.velocity = 0;
        }
    }

    public void AlertAreaOfHighlight()
    {
        tileData.switchGate = !tileData.switchGate;
    }

    public void ColorTransitions()
    {
        if (tileData.shouldBeHighlighted)
        {
            tileData.newMat.color = Color.Lerp(tileData.newMat.color, tileData.activeColor, Time.deltaTime * tileData.hoverChangeSpeed);
        }
        else
        {
            tileData.newMat.color = Color.Lerp(tileData.newMat.color, tileData.inactiveColor, Time.deltaTime * tileData.hoverChangeSpeed);
        }
    }

    public void ChangeTileHeight(bool isRaising)
    {
        var newHeight = Mathf.Clamp(
            isRaising ? targetHeight + tileData.clickedHeightChange : targetHeight - tileData.clickedHeightChange,
            tileData.clickedHeightMin,
            tileData.clickedHeightMax
        );
        if (targetHeight != newHeight)
        {
            targetHeight = newHeight;
            tileData.velocity = tileData.popForce;
        }
        else // Some sort of feedback showing that it cant be changed that direction any more
        {

        }

    }
}
