using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Hoods Locations List", menuName = "New Hood Location List")]
public class HoodLocationsMock : ScriptableObject
{
    public List<Vector3Int> HoodsPoses;
}
