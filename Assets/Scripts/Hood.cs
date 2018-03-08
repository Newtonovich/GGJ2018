using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Hood
{
    public List<Vector3Int> Houses { get { return houses; } }

    private List<Vector3Int> houses;

    public Hood(Vector3Int firstHouseLocation)
    {
        houses = new List<Vector3Int>
        {
            firstHouseLocation
        };
    }

    public void PositionHouse(Vector3Int newHouse)
    {
        if(houses.Contains(newHouse))
        {
            Debug.LogError("trying to add new house that exists at " + newHouse);
            return;
        }

        houses.Add(newHouse);
    }

    public Vector3Int GetFreeAdjacentSpace()
    {
        Vector3Int borderHouse = GetRandomBorderHouse();

        int randNeigbor;

        List<Vector3Int> neigbors = GetNeighbors(borderHouse);

        do
        {
            randNeigbor = Random.Range(0, neigbors.Count);
        }
        while (houses.Contains(neigbors[randNeigbor]));

        return neigbors[randNeigbor];
    }

    public Vector3Int GetRandomBorderHouse()
    {
        int rnd;
        do
        {
            rnd = Random.Range(0, houses.Count);
        }
        while (!HasAtLeastOneFreeNeighbor(houses[rnd]));

        return houses[rnd];
    }

    public void MergeHood(Hood otherHood)
    {
        houses.AddRange(otherHood.Houses);
    }

    private bool HasAtLeastOneFreeNeighbor(Vector3Int center)
    {
        bool hasNNeigbor = houses.Contains(center + Vector3Int.up);
        bool hasWNeigbor = houses.Contains(center + Vector3Int.left);
        bool hasSNeigbor = houses.Contains(center + Vector3Int.down);
        bool hasENeigbor = houses.Contains(center + Vector3Int.right);

        return !(hasNNeigbor && hasSNeigbor && hasENeigbor && hasWNeigbor);
    }

    private List<Vector3Int> GetNeighbors(Vector3Int center)
    {
        List<Vector3Int> result = new List<Vector3Int>();

        result.Add(center + Vector3Int.up);
        result.Add(center + Vector3Int.left);
        result.Add(center + Vector3Int.down);
        result.Add(center + Vector3Int.right);

        return result;
    }
}
