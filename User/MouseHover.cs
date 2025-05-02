using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ClickType
{
    HeightChange,
    AddPath
}

public class MouseHover : MonoBehaviour
{
    public ClickType clickType;

    private void Update()
    {
        CheckForHighlighting();
    }

    private void CheckForHighlighting()
    {
        Vector3 mouse = Input.mousePosition;
        Ray castPoint = Camera.main.ScreenPointToRay(mouse);
        bool isShifted = Input.GetKey(KeyCode.LeftShift);
        RaycastHit hit;
        if (Physics.Raycast(castPoint, out hit, Mathf.Infinity, LayerMask.GetMask("Hover")))
        {
            hit.transform.GetComponent<TilePiece>().AlertOfHover(isShifted);
            if (Input.GetMouseButtonDown(0)) hit.transform.GetComponent<TilePiece>().ClickOnTile(clickType, true, isShifted);
            else if (Input.GetMouseButtonDown(1)) hit.transform.GetComponent<TilePiece>().ClickOnTile(clickType, false, isShifted);
        }
    }
}
