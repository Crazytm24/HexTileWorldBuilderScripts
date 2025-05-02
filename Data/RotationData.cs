using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

// This has the calculations for figuring out path connections and rotation of tiles along with what type of tile should be used
public class RotationData : MonoBehaviour
{
    public enum ConnectionTypeEnum
    {
        Connection0 = 0,
        Connection1 = 10,
        Connection20 = 20,
        Connection21 = 21,
        Connection22 = 22,
        Connection30 = 30,
        Connection31 = 31,
        Connection32 = 32,
        Connection33 = 33,
        Connection40 = 40,
        Connection41 = 41,
        Connection42 = 42,
        Connection43 = 50,
        Connection60 = 60,

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
                return new RotationDataClass(0, 0);
            case 1:
                return new RotationDataClass(rotationMultiplier * nodes[0], (ConnectionTypeEnum)10);
            case 2:
                return GetDifferentialAndTypeOfTwoConnections(nodes[0], nodes[1], 20);
            case 3:
                return GetDifferentialAndTypeOfThreeConnections(nodes[0], nodes[1], nodes[2], 30);
            case 4:
                nodes = InvertNodes(nodes);
                return GetDifferentialAndTypeOfTwoConnections(nodes[0], nodes[1], 40);
            case 5:
                nodes = InvertNodes(nodes);
                return new RotationDataClass(rotationMultiplier * nodes[0], (ConnectionTypeEnum)50);
            case 6:
                return new RotationDataClass(0, (ConnectionTypeEnum)60);
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
        int connectionType = Mathf.Min(diff, 6 - diff) - 1 + conStart;
        ConnectionTypeEnum connection = (ConnectionTypeEnum)connectionType;
        RotationDataClass data = new(rotation, connection);
        return data;
    }

    public RotationDataClass GetDifferentialAndTypeOfThreeConnections(int one, int two, int three, int conStart)
    {
        List<ThreeConnectionDiffs> diffs = new() {
            new ThreeConnectionDiffs(one, two),
            new ThreeConnectionDiffs(one, three),
            new ThreeConnectionDiffs(two, three)
        };

        ConnectionTypeEnum connection = (ConnectionTypeEnum)conStart;
        int rotation = 0;

        if (diffs.Where(x => x.diff == 3).Any())
        {
            var importantDiffs = diffs.Where(x => x.diff == 3 || x.diff == 2 || x.diff == 4).ToList();
            var highestSum = importantDiffs
                .OrderByDescending(x => x.pointA + x.pointB)
                .FirstOrDefault();
            if (diffs.Where(x => x.diff == 3).Any() && diffs.Where(x => x.diff == 5).Any() && diffs.Where(x => x.diff == 2).Any())
            {
                highestSum = importantDiffs
                        .OrderByDescending(x => x.pointA + x.pointB)
                        .LastOrDefault();
            }
            connection = (ConnectionTypeEnum)(highestSum.diff == 3 ? 2 + conStart : 3 + conStart);
            int sharedPoint = importantDiffs
                .SelectMany(d => new[] { d.pointA, d.pointB })
                .GroupBy(p => p)
                .Where(g => g.Count() > 1)
                .Select(g => g.Key)
                .First();
            rotation = sharedPoint * rotationMultiplier;
        }
        else if (!diffs.Where(x => x.diff == 1).Any())
        {
            connection = (ConnectionTypeEnum)(conStart + 1);
            var randomObject = diffs[Random.Range(0, diffs.Count)];
            rotation = ((Random.Range(0, 1) == 0) ? randomObject.pointA : randomObject.pointB) * rotationMultiplier;
        }
        else
        {
            var importantDiffs = diffs.Where(x => x.diff == 1 || x.diff == 5).ToList();
            int sharedPoint = importantDiffs
                .SelectMany(d => new[] { d.pointA, d.pointB })
                .GroupBy(p => p)
                .Where(g => g.Count() > 1)
                .Select(g => g.Key)
                .First();
            rotation = sharedPoint * rotationMultiplier;
        }

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

    private class ThreeConnectionDiffs
    {
        public int pointA;
        public int pointB;
        public int diff;

        public ThreeConnectionDiffs(int a, int b)
        {
            pointA = a;
            pointB = b;
            diff = Mathf.Abs(a - b);
        }
    }
}
