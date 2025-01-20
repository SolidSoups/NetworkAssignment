using Unity.Netcode.Components;
using UnityEngine;

namespace Network
{
  public class OwnedNetworkTransform : NetworkTransform
  {
    protected override bool OnIsServerAuthoritative()
    {
      return false;
    }
  }
  
}
