using Unity.Netcode;
using UnityEditor;
using UnityEngine;

namespace Player.Scripts
{


  [DisallowMultipleComponent]
  public class PlayerSpawner : NetworkBehaviour
  {
    [Header("References")] [SerializeField]
    private GameObject playerPrefab;

    
    private void Start()
    {
      if (NetworkManager.Singleton.IsServer || NetworkManager.Singleton.IsHost)
      {
        Debug.Log("Spawning PlayerSpawner as NetworkObject on server or host");
        GetComponent<NetworkObject>().Spawn();
    
        if (NetworkManager.Singleton.IsHost)
        {
          SpawnPlayerForHost();
        }
      
        NetworkManager.Singleton.OnClientConnectedCallback += SpawnPlayerForClient;
      }
      Debug.Log($"PlayerSpawner Start called on {(IsHost ? "Host" : IsClient ? "Client" : "Server")}");
    
    }

    public override void OnNetworkSpawn()
    {
      SpawnPlayer();
    }

    public void SpawnPlayer()
    {
      if (!IsServer)
      {
        SpawnPlayerServerRpc(NetworkManager.Singleton.LocalClientId);
      }
      if (IsHost)
      {
        SpawnPlayerForHost();
      }
      Debug.Log($"PlayerSpawner OnNetworkSpawn called on {(IsHost ? "Host" : IsClient ? "Client" : "Server")}");
    }

    [ServerRpc(RequireOwnership = false)]
    private void SpawnPlayerServerRpc(ulong clientId)
    {
      Debug.Log("Spawning player for client: " + clientId);
      GameObject    newPlayer     = Instantiate(playerPrefab);
      NetworkObject networkObject = newPlayer.GetComponent<NetworkObject>();

      if (networkObject == null)
      {
        Debug.LogError("Prefab does not have a network object component");
        return;
      }
    
      newPlayer.SetActive(true);
      networkObject.SpawnAsPlayerObject(clientId, true);
    }

    public void SpawnPlayerForHost()
    {
      ulong hostId = NetworkManager.Singleton.LocalClientId;
      Debug.Log($"Spawning player for host [cliendId:{hostId}]");
      GameObject    hostPlayer    = Instantiate(playerPrefab);
      NetworkObject networkObject = hostPlayer.GetComponent<NetworkObject>();

      if (networkObject == null)
      {
        Debug.LogError("Host Player Prefab does not have a network object component");
        return;
      }
    
      hostPlayer.SetActive(true);
      networkObject.SpawnAsPlayerObject(hostId, true);
    }

    private void SpawnPlayerForClient(ulong clientId)
    {
      Debug.Log($"Client [clientId:{clientId}] connected. Spawning Player...");
      SpawnPlayerServerRpc(clientId);
    }

    public override void OnDestroy()
    {
      if (NetworkManager.Singleton != null)
      {
        NetworkManager.Singleton.OnClientConnectedCallback -= SpawnPlayerForClient;
      }
    }
  }
}