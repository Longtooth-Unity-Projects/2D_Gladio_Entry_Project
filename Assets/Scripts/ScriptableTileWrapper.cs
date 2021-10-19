using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu]
public class ScriptableTileWrapper : ScriptableObject
{
    [Tooltip("If true, then vehicles can traverse this path")]
    public bool isWalkable = true;

    [Tooltip("Movement Multiplier: < 1 slows movement, > 1 increases movement")]
    [Range(0f, 10f)]
    public float movementMultiplier = 1f;

    [Tooltip("List of tiles that will have these traits")]
    public TileBase[] tilesWithTheseTraits;
}
