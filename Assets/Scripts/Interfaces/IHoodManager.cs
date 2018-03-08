using System.Collections.Generic;
using UnityEngine;

public interface IHoodManager
{
    List<Vector3Int> Houses { get; }
    void NewGame();
    void StartNewHood(Vector3Int newCenter);
    void CreateNewHouse();

}
