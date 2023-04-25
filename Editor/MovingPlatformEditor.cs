using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

//TODO: Replace the using later with vexxlib utils for the clean nulls.

///<summary>
/// 
///</summary>
[CustomEditor(typeof(MovingPlatform))]
public class MovingPlatformEditor : Editor
{
    MovingPlatform targetScript;
    private void OnEnable()
    {
        EditorUtility.SetDirty(target);
        targetScript = (MovingPlatform)target;
    }

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();


        GUILayout.Label("Waypoint Buttons");

        if (GUILayout.Button("Add new waypoint"))
        {
            var point = AddNewWaypoint(); ;
            Selection.activeObject = point;
        }

        if (GUILayout.Button("Delete newest waypoint"))
        {
            DeleteNewestWaypoint();
        }

        if (GUILayout.Button("Clear waypoints"))
        {
            ClearAllWaypoints();
        }
    }

    #region Editor Methods
    /// <summary>
    /// Cleans the given list of null values
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="list"></param>
    private void CleanNullFromList<T>(List<T> list)
    {
        List<T> itemsToDelete = new List<T>();

        foreach (var item in list)
        {
            if (item == null)
            {
                itemsToDelete.Add(item);
            }
        }

        foreach (var item in itemsToDelete)
        {
            list.Remove(item);
        }
    }

    private void RegenerateNames(List<GameObject> list, string name)
    {
        int i = 0;

        foreach (var item in list)
        {
            i++;
            item.name = $"{name} {i}";
        }
    }

    private void RegenerateNames(List<WaypointData> list, string name)
    {
        int i = 0;

        foreach (var item in list)
        {
            i++;
            item.obj.name = $"{name} {i+1}";
        }
    }


    /// <summary>
    /// Creates a new respawn point.
    /// </summary>
    /// <returns>Gameobject just created.</returns>
    public GameObject AddNewWaypoint()
    {
        var instance = new GameObject();
        instance.transform.position = targetScript.transform.position;
        instance.transform.parent = targetScript.transform;
        targetScript.waypoints.Add(new WaypointData(instance));
        RegenerateNames(targetScript.waypoints, "Waypoint");
        CleanNullFromList(targetScript.waypoints);
        return instance;
    }

    public void DeleteNewestWaypoint()
    {
        // Validate list to not try and remove a null entry.
        CleanNullFromList(targetScript.waypoints);

        // Check if the list already has no points.
        if (targetScript.waypoints.Count <= 0)
        {
            // There are no points to delete.
            return;
        }

        var obj = targetScript.waypoints[targetScript.waypoints.Count - 1];
        DestroyImmediate(obj.obj);
        targetScript.waypoints.Remove(obj);

        // Check if the list has no points before trying to regenerate names.
        if (targetScript.waypoints.Count <= 0)
        {
            // There are no points to rename.
            return;
        }

        RegenerateNames(targetScript.waypoints, "Waypoint");
    }

    public void ClearAllWaypoints()
    {
        CleanNullFromList(targetScript.waypoints);

        List<GameObject> waypointsToDelete = new List<GameObject>();

        foreach (var child in targetScript.waypoints)
        {
            waypointsToDelete.Add(child.obj);
        }

        foreach (var child in waypointsToDelete)
        {
            DestroyImmediate(child);
        }

        targetScript.waypoints.Clear();
    }
    #endregion
}