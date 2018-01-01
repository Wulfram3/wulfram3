using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(SmartPool))]
public class SmartPoolInspector : Editor {

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        
        SmartPool pool=target as SmartPool;
        if (pool!=null) {
            GUILayout.Label("In Stock: " + pool.InStock.ToString());
            GUILayout.Label("Spawned: " + pool.Spawned.ToString());
            Repaint();
        }
        
    }
}
