using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Match3/Tile type asset")]
public class TileTypeAsset : ScriptableObject
{
    public int id;
    public int value;
    public Sprite sprite;
}
