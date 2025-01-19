using System;
using Unity.Netcode;
using UnityEngine;

public class DrawOwnerOnTop : NetworkBehaviour
{
  public override void OnNetworkSpawn()
  {
    base.OnNetworkSpawn();
    var spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    if (IsOwner)
      spriteRenderer.sortingOrder = 1;
    else
      spriteRenderer.sortingOrder = 0; 
  }
}
