using System;
using Unity.Netcode;
using UnityEngine;

public class ScoreManager : NetworkBehaviour
{
  public static ScoreManager Instance { get; private set; }
  
  public NetworkVariable<int> m_score = new NetworkVariable<int>(
    0,
    NetworkVariableReadPermission.Everyone,
    NetworkVariableWritePermission.Server);

  public int Score
  {
    get => m_score.Value;
    set
    {
      if(IsServer)
        m_score.Value = value;
    }
  }
  
  [ServerRpc(RequireOwnership = false)]
  public void SetScoreServerRPC(int score)
  {
    m_score.Value = score;
  }

  private void Awake()
  {
    if (Instance != null)
    {
      Destroy(gameObject);
      return;
    }
    
    Instance = this; 
  }
  
  
}
