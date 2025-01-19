using Player.Scripts;
using Unity.Netcode;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(PlayerSpawner))]
public class PlayerSpawnerEditor : Editor
{
  public override void OnInspectorGUI()
  {
    base.OnInspectorGUI();
    PlayerSpawner playerSpawner = (PlayerSpawner) target;


    if (
      Application.isPlaying && 
      NetworkManager.Singleton && 
      NetworkManager.Singleton.IsHost)
    {
      GUILayout.Space(8);
      EditorGUILayout.BeginVertical();
      GUILayout.Label("Host Controls", EditorStyles.boldLabel);
      if(GUILayout.Button("Spawn Player for Host")){
        OnSpawnHostButtonPressed(playerSpawner);
      }
      EditorGUILayout.EndVertical(); 
    }
      
  }

  private void OnSpawnHostButtonPressed(PlayerSpawner target)
  {
    target.SpawnPlayerForHost();
  }
}
