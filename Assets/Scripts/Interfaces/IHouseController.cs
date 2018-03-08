using UnityEngine;

public interface IHouseController
{
    void SetConnected();
    void ClearHouse();
    void PaintHouse();
    void HouseInTempRegion();
    void HouseOutOfRegion();
    Vector3Int PositionInt();
}
