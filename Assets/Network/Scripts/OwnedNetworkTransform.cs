using Unity.Netcode.Components;
using UnityEngine;

public class OwnedNetworkTransform : NetworkTransform
{
  protected override bool OnIsServerAuthoritative()
  {
    return false;
  }
}
