using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using Player;
using Unity.Netcode;

public class PlayerController : NetworkBehaviour
{
  [Header("Fire Settings")]
  [SerializeField] GameObject m_bulletPrefab;
  [SerializeField] private float m_gunDelay = 0.05f; // seconds
  [SerializeField] private float m_bulletDespawnTime = 5f; // seconds
  
  [Header("Move Settings")]
  [SerializeField] float m_speed = 10;
  Camera m_mainCamera;
  
  
  [SerializeField] private PlayerInputActions m_playerControls;
  private InputAction        move;
  private InputAction        fire;

  private void Awake()
  {
    m_mainCamera = Camera.main;
    m_playerControls = new PlayerInputActions();
    m_playerControls.Disable();
  }


  public override void OnNetworkSpawn()
  {
    base.OnNetworkSpawn();
    enabled = IsClient;
    if (!IsOwner) {
      enabled = false;
      m_playerControls.Disable();
      return;
    }
    enabled = true;
    m_playerControls.Enable();
  }


  void FixedUpdate(){
    HandleMovement();
  }
  
  // some movement ig
  public void HandleMovement()
  {
    // actually movement
    Vector3 position      = transform.position;
    Vector3 moveDirection = move.ReadValue<Vector2>();
    position.x         += moveDirection.x * m_speed * Time.deltaTime;
    position.y         += moveDirection.y * m_speed * Time.deltaTime;
    transform.position =  position;
     
    // croakation
    Vector3 mousePosition = Input.mousePosition;
    mousePosition.z = 10f;
    Vector3 worldPosition = m_mainCamera.ScreenToWorldPoint(mousePosition);
    Vector2 direction     = (worldPosition - transform.position).normalized;
    float   angle         = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
    transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
  }

  // GUNS AND STUFF
  public void ShootGun()
  {
    SpawnProjectileServerRpc(); 
  }
  [ServerRpc(RequireOwnership = false)]
  public void SpawnProjectileServerRpc()
  {
    GameObject bullet = Instantiate(m_bulletPrefab, transform.position, transform.rotation);
    bullet.GetComponent<NetworkObject>().SpawnWithOwnership(OwnerClientId, destroyWithScene: false);
    Destroy(bullet, m_bulletDespawnTime); 
  } 
  
  
  private void OnEnable()
  {
    move = m_playerControls.Player.Move;
    move.Enable();
    
    fire = m_playerControls.Player.Fire;
    fire.Enable();
    fire.performed += ctx => ShootGun();
  }
  
  private void OnDisable()
  {
    move.Disable();
    fire.Disable();
    m_playerControls.Disable();
  }
}
