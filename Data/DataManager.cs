using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class DataManager : MonoBehaviour
{
    public static DataManager Instance { get; private set; }

    public Dictionary<Type, object> initialDataDict = new();
    private Dictionary<Type, Dictionary<int, object>> dbData = new();

    [Header("TileHeightService Initial Settings")]
    [SerializeField] private TileHeightInitialData tileHeightInitialData;

    // Singleton Initialization
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            FillIntialDataDict();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void FillIntialDataDict()
    {
        Debug.Log("INITIALIZED DICT");
        initialDataDict[typeof(TileHeightInitialData)] = tileHeightInitialData;
    }

    public void SaveData<T>(T data, int key) where T : struct
    {
        if (!dbData.TryGetValue(typeof(T), out var typeDict))
        {
            typeDict = new();
            dbData[typeof(T)] = typeDict;
        }
        typeDict[key] = data;
    }

    public void DeleteData<T>(T data, int key) where T : struct
    {
        if (dbData.TryGetValue(typeof(T), out var typeDict))
        {
            typeDict.Remove(key);
        }
    }

    public T GetSavedData<T>(int key) where T : struct
    {
        if (dbData.TryGetValue(typeof(T), out Dictionary<int, object> typeDict) && typeDict.TryGetValue(key, out object value))
        {
            return (T)value;
        }
        return default;
    }

    public T GetInitialData<T>() where T : struct
    {
        if (initialDataDict.TryGetValue(typeof(T), out object value))
        {
            Debug.Log(value);
            return (T)value;
        }
        Debug.Log("HAD NO INITIAL DATA TO RETURN");
        return default;
    }
}
