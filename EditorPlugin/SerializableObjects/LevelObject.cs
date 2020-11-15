/// <summary>
/// This object is just for storing the deserialised data imported from JSON
/// </summary>
[System.Serializable]
public class LevelObject
{
    public string shape;
    public float rotation;
    public uint[] color;
    public string colorHex;
    public float scale;
    public float x;
    public float y;
}