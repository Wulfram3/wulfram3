// =====================================================================
// Copyright 2012 FluffyUnderware
// All rights reserved
// http://www.fluffyunderware.com
// =====================================================================
// SMARTPOOL 1.02 
// =====================================================================
SmartPool home: http://fluffyunderware.com/pages/unity-plugins/smartpool.php
Support forum: http://forum.fluffyunderware.com


============= GETTING STARTED ==========================================

* Add a SmartPool component to a gameObject of your choice
* Define a Pool Name. This Name will be used to interact with the pool
* Link to a Prefab. This Prefab will be used when spawning items
* Use SmartPool.Spawn("PoolName") to retrieve and SmartPool.Despawn(gameObject) to store objects in the pool

Reference available at our product website, also checkout the example scene

============= PACKAGE CONTENT ==========================================

SmartPool/SmartPool.cs	SmartPool component (add this to a gameObject)
SmartPool/Editor		SmartPool inspector
SmartPool/Example		Example scene (can be deleted in your projects)

============= ATTENTION: PLAYMAKER USERS ================================
Download the playMaker additions (custom actions and example) from our website:

SmartPoolPlayMaker.unitypackage content:

SmartPool/pmExample		playMaker Example (can be deleted in your projects)
SmartPool/pmActions		playMaker Actions

============= HISTORY ==================================================
v1.02 Despawn() and Kill() will now destroy unmanaged objects
v1.01 Fixed a small typo
v1.00 Initial release