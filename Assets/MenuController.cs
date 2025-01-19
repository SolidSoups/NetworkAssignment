using UnityEngine;
using Unity.Netcode;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
  public void StartHost()
  {
    NetworkManager.Singleton.StartHost();

    SceneManager.LoadScene(1);
  }
  
  public void StartClient()
  {
    NetworkManager.Singleton.StartClient();

    SceneManager.LoadScene(1);
  }
}
