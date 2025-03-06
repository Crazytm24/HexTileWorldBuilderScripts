using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConnectionData
{
    public int slot;
    public int connectionOneDiff;
    public int connectionTwoDiff;

    public ConnectionData(int _slot, int _cod, int _ctd)
    {
        slot = _slot;
        connectionOneDiff = _cod;
        connectionTwoDiff = _ctd;
    }

    public bool Contains(int diff)
    {
        return connectionOneDiff == diff || connectionTwoDiff == diff;
    }

    public bool bothConnectionsAre(int diff)
    {
        return connectionOneDiff == diff && connectionTwoDiff == diff;
    }
}

public class RotationData : MonoBehaviour
{
    public enum ConnectionTypeEnum
    {
        Connection0 = 0,
        Connection1 = 1,
        Connection21 = 21,
        Connection22 = 22,
        Connection23 = 23,
        Connection32 = 32,
        Connection33 = 33,
        Connection34 = 34,

    }

    private int rotationMultiplier = 60;

    public class RotationDataClass
    {
        public int rotationAmount;
        public ConnectionTypeEnum connectionType;

        public RotationDataClass(int _rotationAmount, ConnectionTypeEnum _connectionType)
        {
            rotationAmount = _rotationAmount;
            connectionType = _connectionType;
        }
    }

    public RotationDataClass FindConnectionRotation(List<int> nodes)
    {
        nodes.Sort();
        switch (nodes.Count)
        {
            case 0:

            case 1:
                return new RotationDataClass(rotationMultiplier * nodes[0], 0);
            case 2:
                return GetDifferentialAndTypeOfTwoConnections(nodes[0], nodes[1], 20);
            case 3:
                break;
            case 4:
                nodes = InvertNodes(nodes);
                return GetDifferentialAndTypeOfTwoConnections(nodes[0], nodes[1], 20);
            case 5:
                nodes = InvertNodes(nodes);
                return new RotationDataClass(rotationMultiplier * nodes[0], 0);
            case 6:
                break;
            default:
                break;
        }
        return new(0, 0);
    }

    public RotationDataClass GetDifferentialAndTypeOfTwoConnections(int one, int two, int conStart)
    {
        int diff = Mathf.Abs(one - two);
        int rotation = 0;
        switch (diff)
        {
            case 1:
            case 2:
                rotation = two * rotationMultiplier;
                break;
            case 3:
                rotation = ((Random.Range(0, 1) == 0) ? one : two) * rotationMultiplier;
                break;
            case 4:
            case 5:
                rotation = one * rotationMultiplier;
                break;
        }
        int connectionType = Mathf.Min(diff, 6 - diff) + conStart;
        ConnectionTypeEnum connection = (ConnectionTypeEnum)connectionType;
        RotationDataClass data = new(rotation, connection);
        return data;
    }

    public List<int> InvertNodes(List<int> nodes)
    {
        List<int> baseSet = new()
        {
            0,
            1,
            2,
            3,
            4,
            5
        };

        baseSet.RemoveAll(slot => nodes.Contains(slot));
        return baseSet;
    }
}
