using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;

[InitializeOnLoad]
public static class PlayModeStateListener
{
    static PlayModeStateListener()
    {
        EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
    }

    private static void OnPlayModeStateChanged(PlayModeStateChange state)
    {
        if (state == PlayModeStateChange.EnteredPlayMode)
        {
            Debug.Log("Entered Play Mode");
        }
        else if (state == PlayModeStateChange.ExitingPlayMode)
        {
            
            PlayerPref_DatabaseManager.Instance.SaveInventory();
            PlayerPref_DatabaseManager.Instance.SaveProp();
            PlayerPref_DatabaseManager.Instance.SavePlayer();
            Debug.Log("Exiting Play Mode");
        }
        else if (state == PlayModeStateChange.EnteredEditMode)
        {
            Debug.Log("Entered Edit Mode");
        }
        else if (state == PlayModeStateChange.ExitingEditMode)
        {
            
            PlayerPref_DatabaseManager.Instance.SaveInventory();
            PlayerPref_DatabaseManager.Instance.SaveProp();
            PlayerPref_DatabaseManager.Instance.SavePlayer();
            Debug.Log("Exiting Edit Mode");
        }
    }
}
