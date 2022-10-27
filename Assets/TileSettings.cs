using UnityEngine;

[CreateAssetMenu(fileName ="TileSettings", menuName ="Wemade2048/Tile Settings", order =0)]

public class TileSettings : ScriptableObject
{
    public float animationTime = .3f;
    public AnimationCurve animationCurve;
}