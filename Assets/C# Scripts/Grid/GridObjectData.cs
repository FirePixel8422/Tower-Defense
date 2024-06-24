using UnityEngine;


public struct GridObjectData
{
    public Vector2Int gridPos;
    public Vector3 worldPos;

    public TowerCore tower;

    public bool full;
    public int coreType;
    public int type;

    public float scrap;
}