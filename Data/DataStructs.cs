using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct TileHeightInitialData
{
    //Click Data
    [HideInInspector] public float originalHeight;
    public float clickedHeightIncrease;
    public float stiffness;
    public float damping;
    public float popForce;
    [HideInInspector] public float velocity;
    //Hover Data
    public float hoverHeightIncrease;
    public float hoverChangeSpeed;
    [HideInInspector] public float velPast;
    [HideInInspector] public bool shouldBeHighlighted;
    [HideInInspector] public bool switchGate;
    [HideInInspector] public bool lastSwitchGate;
    public Color activeColor;
    public Color inactiveColor;
    [HideInInspector] public Material sourceMat;
    [HideInInspector] public Material newMat;
    [HideInInspector] public List<MeshRenderer> childMeshRends;
}
