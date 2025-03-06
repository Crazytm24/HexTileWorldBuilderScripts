using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseHover : MonoBehaviour
{
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
            if (Input.GetMouseButtonDown(0)) hit.transform.GetComponent<TilePiece>().ClickOnTile(isShifted);
        }
    }
}
