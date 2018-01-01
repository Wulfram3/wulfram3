// =====================================================================
// Copyright 2012-2013 FluffyUnderware
// All rights reserved
// =====================================================================
#if UNITY_3_3 || UNITY_3_4 || UNITY_3_5
#define UNITY_3
#endif
using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Class to manage a pool of prefabs
/// </summary>
public class SmartPool : MonoBehaviour {
    public const string Version = "1.02";
    static Dictionary<string, SmartPool> _Pools = new Dictionary<string, SmartPool>();
    /// <summary>
    /// Name of the Pool
    /// </summary>
    public string PoolName;
    /// <summary>
    /// The Prefab being managed by this pool
    /// </summary>
    public GameObject Prefab;
    /// <summary>
    /// Keep persistent between scene changes
    /// </summary>
    public bool DontDestroy=false;
    /// <summary>
    /// Automatically prepare when the game starts. If false, you'll need to call Prepare() by yourself
    /// </summary>
    public bool PrepareAtStart = true;
    /// <summary>
    /// Blocksize when the pool needs to grow or shrink
    /// </summary>
    public int AllocationBlockSize=1;
    /// <summary>
    /// Minimum pool size
    /// </summary>
    public int MinPoolSize=1;
    /// <summary>
    /// Maximum pool size
    /// </summary>
    public int MaxPoolSize=1;
    /// <summary>
    /// Behaviour when the maximum pool size is exceeded
    /// </summary>
    public PoolExceededMode OnMaxPoolSize = PoolExceededMode.Ignore;
    /// <summary>
    /// Automatically cull items until the pool reaches MaxPoolSize
    /// </summary>
    public bool AutoCull = true;
    /// <summary>
    /// Time in seconds between automatic culling occurs
    /// </summary>
    public float CullingSpeed = 1.0f;
    /// <summary>
    /// Whether all actions should be logged
    /// </summary>
    public bool DebugLog = false;

    Stack<GameObject> mStock=new Stack<GameObject>();
    List<GameObject> mSpawned=new List<GameObject>();
    float mLastCullingTime;
    
    public int InStock { get { return mStock.Count; } }
    public int Spawned { get { return mSpawned.Count; } }

    #region ### Unity Callbacks ###
    void Awake()
    {
        if (PoolName.Length == 0)
            Debug.LogWarning("SmartPool: Missing PoolName for pool belonging to '" + gameObject.name + "'!");
        if (DontDestroy)
            DontDestroyOnLoad(gameObject);
    }

    void OnEnable()
    {
        if (Prefab != null) {
            if (GetPoolByName(PoolName) == null) {
                _Pools.Add(PoolName, this);
                if (DebugLog)
                    Debug.Log("SmartPool: Adding '" + PoolName + "' to the pool dictionary!");
            }
        }
        else {
            Debug.LogError("SmartPool: Pool '" + PoolName + "' is missing it's prefab!");
        }
    }

    void Start()
    {
        if (PrepareAtStart)
            Prepare();
    }

    void LateUpdate()
    {
        if (AutoCull && Time.time - mLastCullingTime > CullingSpeed) {
            mLastCullingTime = Time.time;
            Cull(true);
        }
    }

    void OnDisable()
    {
        if (!DontDestroy) {
            Clear();
            if (_Pools.Remove(PoolName) && DebugLog)
                Debug.Log("SmartPool: Removing " + PoolName + " from the pool dictionary!");
        }
        
    }

    void Reset()
    {
        PoolName = "";
        Prefab = null;
        DontDestroy = false;
        AllocationBlockSize = 1;
        MinPoolSize = 1;
        MaxPoolSize = 1;
        OnMaxPoolSize = PoolExceededMode.Ignore;
        DebugLog = false;
        AutoCull = true;
        CullingSpeed = 1f;
        mLastCullingTime = 0;
    }

    #endregion

    #region ### Methods for the current pool ###

    /// <summary>
    /// Remove all instances held by this pool
    /// </summary>
    void Clear()
    {
        if (DebugLog)
            Debug.Log("SmartPool (" + PoolName + "): Clearing all instances of " + Prefab.name);
        foreach (GameObject go in mSpawned)
            Destroy(go);
        mSpawned.Clear();
        foreach (GameObject go in mStock)
            Destroy(go);
        mStock.Clear();
    }
    /// <summary>
    /// Shrink the stock to match MaxPoolSize
    /// </summary>
    public void Cull() { Cull(false); }
    
    /// <summary>
    /// Shrink the stock to match MaxPoolSize
    /// </summary>
    /// <param name="smartCull">if true a maximum of AllocationBlockSize items are culled</param>
    public void Cull(bool smartCull)
    {
        int toCull=(smartCull) ? Mathf.Min(AllocationBlockSize,mStock.Count-MaxPoolSize) : mStock.Count-MaxPoolSize;
        if (DebugLog && toCull>0)
            Debug.Log("SmartPool (" + PoolName + "): Culling "+(toCull)+" items");

        while (toCull-->0) {
            GameObject item = mStock.Pop();
            Destroy(item);
        }
    }
    /// <summary>
    /// Despawn a gameobject and add it to the stock
    /// </summary>
    /// <param name="item"></param>
    public void DespawnItem(GameObject item)
    {
        if (!item) {
            if (DebugLog)
                Debug.LogWarning("SmartPool (" + PoolName + ").DespawnItem: item is null!");
            return;
        }
        if (IsSpawned(item)) {
#if UNITY_3
            item.active = false;
#else
			item.SetActive(false);        
#endif
            item.name = Prefab.name + "_stock";
            mSpawned.Remove(item);
            mStock.Push(item);
            if (DebugLog)
                Debug.Log("SmartPool (" + PoolName + "): Despawning '" + item.name);
        }
        else {
            GameObject.Destroy(item);
            if (DebugLog)
                Debug.LogWarning("SmartPool (" + PoolName + "): Cant Despawn" + item.name + "' because it's not managed by this pool! However, SmartPool destroyed it!");
        }
    }
    /// <summary>
    /// Despawn all items of this pool
    /// </summary>
    public void DespawnAllItems()
    {
        while (mSpawned.Count > 0)
            DespawnItem(mSpawned[0]);
    }

    /// <summary>
    /// Destroys a spawned item instead of despawning it
    /// </summary>
    /// <param name="item">the spawned item to destroy</param>
    public void KillItem(GameObject item)
    {
        if (!item) {
            if (DebugLog)
                Debug.LogWarning("SmartPool (" + PoolName + ").KillItem: item is null!");
            return;
        }
        mSpawned.Remove(item);
        Destroy(item);
    }

    /// <summary>
    /// Whether a gameobject is managed by this pool
    /// </summary>
    /// <param name="item">an item</param>
    /// <returns>true if item is managed by this pool</returns>
    public bool IsManagedObject(GameObject item)
    {
        if (!item) {
            if (DebugLog)
                Debug.LogWarning("SmartPool (" + PoolName + ").IsManagedObject: item is null!");
            return false;
        }
        if (mSpawned.Contains(item) || mStock.Contains(item))
            return true;
        else
            return false;
    }
    /// <summary>
    /// Whether a gameobject is spawned by this pool
    /// </summary>
    /// <param name="item">an item</param>
    /// <returns>true if the item is spawned by this pool</returns>
    public bool IsSpawned(GameObject item)
    {
        if (!item) {
            if (DebugLog)
                Debug.LogWarning("SmartPool (" + PoolName + ").IsSpawned: item is null!");
            return false;
        }
        return (mSpawned.Contains(item));
    }
    /// <summary>
    /// Create instances and add them to the stock
    /// </summary>
    /// <param name="no"></param>
    void Populate(int no)
    {
        while (no > 0) {
            GameObject go = (GameObject)Instantiate(Prefab);
#if UNITY_3
			go.active=false;
#else
            go.SetActive(false);
#endif
            
            go.transform.parent = transform;
            go.name = Prefab.name + "_stock";
            mStock.Push(go);
            no--;
        }
        if (DebugLog)
            Debug.Log("SmartPool (" + PoolName + "): Instantiated " + mStock.Count + " instances of " + Prefab.name);
    }
    /// <summary>
    /// Clear all instances and repopulate the pool to match MinPoolSize
    /// </summary>
    public void Prepare()
    {
        Clear();
        mStock = new Stack<GameObject>(MinPoolSize);
        Populate(MinPoolSize);
    }
    /// <summary>
    /// Spawn an instance, make it active and add it to the Spawned list
    /// </summary>
    /// <returns>the instance spawned</returns>
    public GameObject SpawnItem()
    {
        GameObject item=null;
        // if we ran out of objects, create some
        if (InStock == 0) {
            if (Spawned < MaxPoolSize || OnMaxPoolSize==PoolExceededMode.Ignore)
                Populate(AllocationBlockSize);
        }
        // maybe we've got objects ready now
        if (InStock > 0) {
            item = mStock.Pop();
            if (DebugLog)
                Debug.Log("SmartPool (" + PoolName + "): Spawning item, taking it from the stock!");
            // or maybe we want to reuse the oldest spawned item
        }
        else if (OnMaxPoolSize == PoolExceededMode.ReUse) {
            item = mSpawned[0];
            mSpawned.RemoveAt(0);
            if (DebugLog)
                Debug.Log("SmartPool (" + PoolName + "): Spawning item, reusing an existing item!");
        } else if (DebugLog)
            Debug.Log("SmartPool (" + PoolName + "): MaxPoolSize exceeded, nothing was spawned!");
        if (item != null) {
            mSpawned.Add(item);
#if UNITY_3
			item.active = true;
#else
            item.SetActive(true);
#endif
            item.name = Prefab.name + "_clone";
            item.transform.localPosition = Vector3.zero;
        }
        return item;
    }

    #endregion

    #region ### Methods to access pools (static) ###

    /// <summary>
    /// Shrink a stock to match MaxPoolSize
    /// </summary>
    /// <param name="poolName"></param>
    public static void Cull(string poolName) { Cull(poolName, false); }

    /// <summary>
    /// Shrink a stock to match MaxPoolSize
    /// </summary>
    /// <param name="smartCull">if true a maximum of AllocationBlockSize items are culled</param>
    public static void Cull(string poolName, bool smartCull)
    {
        SmartPool P = GetPoolByName(poolName);
        if (P)
            P.Cull();
    }

    /// <summary>
    /// Despawn a managed item, returning it to the pool
    /// </summary>
    /// <param name="item">an item</param>
    /// <remarks>If the object is unmanaged, it will be destroyed</remarks>
    public static void Despawn(GameObject item)
    {
        if (item) {
            SmartPool P = GetPoolByItem(item);
            if (P != null)
                P.DespawnItem(item);
            else
                GameObject.Destroy(item);
        }
    }
    /// <summary>
    /// Despawn all items of a certain pool
    /// </summary>
    /// <param name="poolName">name of the pool</param>
    public static void DespawnAllItems(string poolName)
    {
        SmartPool P = GetPoolByName(poolName);
        if (P!=null)
            P.DespawnAllItems();
    }

    /// <summary>
    /// Find the pool an item belongs to
    /// </summary>
    /// <param name="item">an item</param>
    /// <returns>the corresponding pool or null if the item is unmanaged</returns>
    public static SmartPool GetPoolByItem(GameObject item)
    {
        foreach (SmartPool P in _Pools.Values)
            if (P.IsManagedObject(item))
                return P;
        return null;
    }

    /// <summary>
    /// Gets a pool by it's name
    /// </summary>
    /// <param name="poolName">name of the pool</param>
    /// <returns>a pool or null</returns>
    public static SmartPool GetPoolByName(string poolName)
    {
        SmartPool P;
        _Pools.TryGetValue(poolName, out P);
        return P;
    }

    /// <summary>
    /// Kill a spawned item instead of despawning it
    /// </summary>
    /// <param name="item">the spawned item to kill</param>
    /// <remarks>If the item is unmanaged, it will be destroyed anyway</remarks>
    public static void Kill(GameObject item)
    {
        if (item) {
            SmartPool P = GetPoolByItem(item);
            if (P != null)
                P.KillItem(item);
            else
                GameObject.Destroy(item);
        }
    }

    /// <summary>
    /// Clear all instances and repopulate a pool to match MinPoolSize
    /// </summary>
    public static void Prepare(string poolName)
    {
        SmartPool P = GetPoolByName(poolName);
        if (P != null)
            P.Prepare();
    }

    /// <summary>
    /// Spawn an item from a specific pool
    /// </summary>
    /// <param name="poolName">the pool's name</param>
    /// <returns>a gameobject or null if spawning failed</returns>
    public static GameObject Spawn(string poolName)
    {
        SmartPool P;
        if (_Pools.TryGetValue(poolName, out P))
            return P.SpawnItem();
        else {
            Debug.LogWarning("SmartPool: No pool with name '" + poolName + "' found!");
            return null;
        }
    }
    
    #endregion
}

/// <summary>
/// Determining reaction when MaxPoolSize is exceeded
/// </summary>
[System.Serializable]
public enum PoolExceededMode:int 
{
    /// <summary>
    /// MaxPoolSize will be ignored
    /// </summary>
    Ignore = 0,
    /// <summary>
    /// Spawning will fail when MaxPoolSize is exceeded
    /// </summary>
    StopSpawning = 1,
    /// <summary>
    /// Already spawned items will be returned when MaxPoolSize is exceeded
    /// </summary>
    ReUse = 2
}
