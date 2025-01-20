using System;
using System.Collections;
using System.Collections.Generic;
using Network;
using UnityEngine;
using UnityEngine.InputSystem;
using Player;
using Player.Scripts;
using Unity.Netcode;



namespace Player
{
  public struct PlayerSnapshot // snapshot of player state to buffer and interpolate between
  {
    public Vector2 Position;
    public Quaternion Rotation;
    public int Tick;
  }
  public class PlayerController : NetworkBehaviour
  {
    [Header("Fire Settings")]
    [SerializeField] GameObject m_bulletPrefab;
    [SerializeField] private float m_gunDelay = 0.05f;       // seconds
    [SerializeField] private float m_bulletDespawnTime = 5f; // seconds
  
    [Header("Move Settings")]
    [SerializeField] float m_speed = 10;
    Camera m_mainCamera;

    NetworkTimer m_networkTimer;
    List<PlayerSnapshot> m_snapshotBuffer;
    const float k_serverTickRate = 60f;
    const int k_interpolationTickDelay = 2;
   
    // input stuff
    private PlayerInputActions m_playerControls;
    private InputAction        move;
    private InputAction        fire;

    private void Awake()
    {
      m_mainCamera = Camera.main;
      m_playerControls = new PlayerInputActions();
      m_playerControls.Disable();

      // [INTERPOLATION] initialize network timer and snapshot buffer 
      m_networkTimer = new NetworkTimer(k_serverTickRate);
      m_snapshotBuffer = new List<PlayerSnapshot>();
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

    void Update()
    {
      m_networkTimer.Update(Time.deltaTime);
    }

    void FixedUpdate()
    {
      while (m_networkTimer.ShouldTick())
      {
        // Here we handle the movement if we are the owner and then send new positions and rotations to the clients
        // if we are the server, or to the server if we are a client
        // Otherwise, we interpolate the unowned player position and rotation to simulate smooth movement
        if (IsOwner)
        {
          HandleMovement();

          if (IsServer)
          {
            UpdateTransformClientRpc(transform.position, transform.rotation, m_networkTimer.CurrentTick);
          }
          else
          {
            SyncTransformServerRpc(transform.position, transform.rotation, m_networkTimer.CurrentTick);
          }
        }
        else
        {
          InterpolateRemotePlayer(); 
        }
      }
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

    void InterpolateRemotePlayer()
    {
      int targetTick = m_networkTimer.CurrentTick - k_interpolationTickDelay;

      while (m_snapshotBuffer.Count >= 2 && m_snapshotBuffer[1].Tick <= targetTick) 
      {
        m_snapshotBuffer.RemoveAt(0);
      }

      if (m_snapshotBuffer.Count < 2) return;
      
      PlayerSnapshot first = m_snapshotBuffer[0];
      PlayerSnapshot second = m_snapshotBuffer[1];
      
      float t = (float)(m_networkTimer.CurrentTick - first.Tick) / (second.Tick - first.Tick);
      
      transform.position = Vector2.Lerp(first.Position, second.Position, t);
      transform.rotation = Quaternion.Slerp(first.Rotation, second.Rotation, t);
    }
  
    [ServerRpc] void SyncTransformServerRpc(Vector2 position, Quaternion rotation, int tick) {
      transform.position= position;
      transform.rotation = rotation;
      UpdateTransformClientRpc(position, rotation, tick);
    }

    [ClientRpc]
     void UpdateTransformClientRpc(Vector3 position, Quaternion rotation, int tick)
    {
      if (IsOwner) return;
      m_snapshotBuffer.Add(new PlayerSnapshot() {
          Position = position,
          Rotation = rotation,
          Tick     = tick
      });
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
}
