using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using Unity.Netcode;
using UnityEngine;
using Color = UnityEngine.Color;

public class NetworkPlayer : NetworkBehaviour
{
    private NetworkObject _networkObject;

    public ulong PlayerId;
    public ulong ClientId;

    private Camera _mainCam;
    private Renderer _renderer = null;

    private NetworkVariable<Color> _chosenColor = new NetworkVariable<Color>(
        default,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Owner
        );

    private NetworkVariable<Color> _currentColor = new NetworkVariable<Color>(
       default,
       NetworkVariableReadPermission.Everyone,
       NetworkVariableWritePermission.Owner
       );


    // Start is called before the first frame update
    void Start()
    {
        _networkObject = GetComponentInParent<NetworkObject>();

        PlayerId = _networkObject.NetworkObjectId;
        ClientId = _networkObject.OwnerClientId;

        int xPos = -5 + 5 * (int)PlayerId;

        transform.position = new Vector3(xPos, 0, 0);

        _mainCam = Camera.main;

        _renderer = GetComponent<Renderer>();

        if (IsOwner)
        {
            _chosenColor.Value = Random.ColorHSV(0f, 1f, 1f, 1f, 0.5f, 1f);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (IsLocalPlayer)
        {
            Debug.Log($"Player {PlayerId}s chosen color was {_chosenColor.Value}");

            if (CheckRaycast()) _currentColor.Value = _chosenColor.Value;
            else _currentColor.Value = Color.white;
        }

        if (IsServer)
        {
            SetColorClientRpc();
        }
    }


    [ClientRpc]
    private void SetColorClientRpc()
    {
        if (_renderer != null)
        {
            Debug.Log($"Player {PlayerId}s current color was {_currentColor.Value}");
            _renderer.material.color = _currentColor.Value;
        }
        else
            Debug.Log($"Player {PlayerId}s renderer was null");
    }

    private bool CheckRaycast()
    {


        if (!Input.GetMouseButton(0))
            return false;


        Ray ray = _mainCam.ScreenPointToRay(Input.mousePosition);

        Debug.DrawRay(ray.origin, ray.direction * 100f, _chosenColor.Value);

        RaycastHit hit;

        if (!Physics.Raycast(ray, out hit, 100)) return false;


        var hitPlayer = hit.collider.gameObject.GetComponent<NetworkPlayer>();

        return hitPlayer?.PlayerId == PlayerId && hitPlayer?.ClientId == ClientId;
    }

}
