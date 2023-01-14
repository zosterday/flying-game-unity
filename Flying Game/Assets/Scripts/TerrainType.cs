using UnityEngine;

[System.Serializable]
public struct TerrainType
{
    [SerializeField]
    private string name;

    [SerializeField]
    private float height;

    [SerializeField]
    private Color color;

    public float Height => height;

    public Color Color => color;
}