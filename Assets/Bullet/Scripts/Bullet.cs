using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

public class Bullet : NetworkBehaviour
{
  [SerializeField] private float speed = 10f;

  public override void OnNetworkSpawn()
  {
    base.OnNetworkSpawn();

    if (IsServer)
    {
      Rigidbody rb = GetComponent<Rigidbody>();
      if (rb != null)
      {
        rb.isKinematic    = false;
        rb.linearVelocity = this.transform.right * speed;
      }
    }
  }

  private void OnTriggerEnter(Collider other)
  {
    if (!other)
      return;
      
    if (other.CompareTag("Player"))
    {
      NetworkObject no = other.gameObject.GetComponent<NetworkObject>();
      Debug.Log($"Object : no == null => {no == null}");
      if (no != null)
      {
        Debug.Log($"Hit a network object with id {no.NetworkObjectId}");
        if(IsServer)
          ScoreManager.Instance.Score += 20;
        else
          ScoreManager.Instance.SetScoreServerRPC(ScoreManager.Instance.Score + 20);
      }
    }
  }
}
