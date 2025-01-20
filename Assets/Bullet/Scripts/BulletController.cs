using System;
using System.Collections;
using System.Collections.Generic;
using Network;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

namespace Bullet
{
  public struct BulletSnapshot
  {
    public Vector2 position;
    public int tick;
  }
  
  public class BulletController : NetworkBehaviour
  {
    [SerializeField] private float speed = 10f;
    
    NetworkTimer m_networkTimer;
    List<BulletSnapshot> m_snapshotBuffer;
    const float k_serverTickRate =40f;
    const int k_interpolationTickDelay = 2;

    void Awake()
    {
      m_networkTimer = new NetworkTimer(k_serverTickRate);
      m_snapshotBuffer = new List<BulletSnapshot>(); 
    }

    void Update()
    {
      m_networkTimer.Update(Time.deltaTime); 
    }

    void FixedUpdate()
    {
      if (!IsOwner)
      {
        InterpolateRemoteBullet();
        return;
      }
      
      if (m_networkTimer.ShouldTick())
      {
        Move();
        if (IsServer)
        {
          UpdateTransformClientRpc(transform.position, m_networkTimer.CurrentTick);         
        }
        else
        {
          SyncTransformServerRpc(transform.position, m_networkTimer.CurrentTick);
        }
      } 
    }

    void InterpolateRemoteBullet()
    {
      int targetTick = m_networkTimer.CurrentTick - k_interpolationTickDelay;
      // remove old snapshots
      while(m_snapshotBuffer.Count >= 2 && m_snapshotBuffer[1].tick <= targetTick)
        m_snapshotBuffer.RemoveAt(0);

      if (m_snapshotBuffer.Count < 2) return;
      
      BulletSnapshot A = m_snapshotBuffer[0];
      BulletSnapshot B = m_snapshotBuffer[1];
      
      float t = (float)(m_networkTimer.CurrentTick - A.tick) / (B.tick - A.tick);

      transform.position = Vector2.Lerp(A.position, B.position, t);
    }
    
    [ServerRpc]
    void SyncTransformServerRpc(Vector2 position, int tick)
    {
      transform.position = position;
      UpdateTransformClientRpc(position, tick);
    }
    
    [ClientRpc]
    void UpdateTransformClientRpc(Vector2 position, int tick)
    {
      BulletSnapshot snapshot = new BulletSnapshot();
      snapshot.position = position;
      snapshot.tick = tick;
      m_snapshotBuffer.Add(snapshot);
    }

    void Move()
    {
      Vector3 position = transform.position;
      position += transform.right * (speed * Time.deltaTime);
      transform.position = position;
    }
    

    private void OnTriggerEnter(Collider other)
    {
      if (!other)
        return;
      
      if (other.CompareTag("Player"))
      {
        // Perform some kind of hit here 
      }
    }
  }
  
}
