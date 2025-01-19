using System;
using Codice.Client.ChangeTrackerService;
using PlasticPipe.PlasticProtocol.Messages;
using Player.Scripts;
using Unity.Netcode;
using UnityEditor;
using UnityEngine;

public class NetworkEditorWindow : EditorWindow
{
  bool m_buttonPressed = false;
  
  [MenuItem("===^-(ツ)-^===> Custom Tools <===^-(ツ)-^===/Network Editor")]
  public static void Initialize(){
    var window = GetWindow<NetworkEditorWindow>("Network Editor");

    window.minSize = new Vector2(400, 200);
    window.maxSize = new Vector2(400, 200);
  }

  void OnEnable()
  {
    Application.quitting += ResetButtonPressed;
  }

  void OnDisable()
  {
    Application.quitting -= ResetButtonPressed;
  }

  void ResetButtonPressed() => m_buttonPressed = false;

  void OnGUI(){
    GUILayout.Label("Network Editor", EditorStyles.boldLabel);
    
    // store some local states

    // Spawn hosts and clients
    if (Application.isPlaying)
    {
      DrawNetworkConnector();
      if (m_buttonPressed && NetworkManager.Singleton)
      {
        if(NetworkManager.Singleton.IsHost)
          GUILayout.Label("Currently playing as Host");
        else if(NetworkManager.Singleton.IsClient)
          GUILayout.Label("Currently playing as Client");
      }
    }
    else
    {
      DrawNetworkConnector();
      GUILayout.Label("Please start the game to use the network editor"); 
    }
    
    GUILayout.Space(8);
    
    // DrawPlayerSpawner();     
    
  }

  void DrawPlayerSpawner()
  {
    var           spawner  = GameObject.FindGameObjectWithTag("Spawner");
    PlayerSpawner target = null;
    if(spawner)
      target   = spawner.GetComponent<PlayerSpawner>();
    bool disabled = !m_buttonPressed || !spawner;
    
    // display
    EditorGUI.BeginDisabledGroup(disabled);
    GUILayout.Label("Player Spawner", EditorStyles.boldLabel);
    GUILayout.BeginVertical();
    if(GUILayout.Button("Spawn Player")){
      Debug.Log("Spawning player depending on host or client");
      target.SpawnPlayer();
    }
    GUILayout.EndVertical();
    EditorGUI.EndDisabledGroup();
  }

  void DrawNetworkConnector()
  {
    bool disabled = m_buttonPressed || !Application.isPlaying || !NetworkManager.Singleton;
    EditorGUI.BeginDisabledGroup(disabled); 
    GUILayout.BeginVertical();
    if(GUILayout.Button("Connect Host")){
      if (NetworkManager.Singleton)
      {
        NetworkManager.Singleton.StartHost();
        m_buttonPressed = true;
      }
      else
        Debug.LogError("NetworkManager not found");
    }
    if(GUILayout.Button("Connect Client")){
      if (NetworkManager.Singleton)
      {
        NetworkManager.Singleton.StartClient();
        m_buttonPressed = true;
      }
      else
        Debug.LogError("NetworkManager not found");
    }
    GUILayout.EndVertical();
    EditorGUI.EndDisabledGroup();
  }
}
