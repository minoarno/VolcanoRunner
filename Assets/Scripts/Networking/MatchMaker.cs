using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;

using Object = UnityEngine.Object;

public class MatchMaker : MonoBehaviour
{
    public int maxPlayers = 2;
    public string id;
    public string lobbyName;
    public const string joinKey = "j";
    public static string PlayerId { get; private set; }

    public TextMeshProUGUI updateText;

    public int playerLevel = 10;

    private static UnityTransport _transport;


    public void Host()
    {
        NetworkManager.Singleton.StartHost();
        updateText.text = "I am host";
    }


    public void Join()
    {
        NetworkManager.Singleton.StartClient();
        updateText.text = "I am client";
    }


    public async void Play()
    {
        updateText.text = "Logging in";
        _transport = Object.FindAnyObjectByType<UnityTransport>();

        await Login();

        CheckForLobbies();

    }

    private async void CheckForLobbies()
    {
        updateText.text = "Finding Game";

        var queryOptions = new QueryLobbiesOptions
        {
            Filters = new List<QueryFilter>
            {
                new QueryFilter (
                    field: QueryFilter.FieldOptions.AvailableSlots,
                    op: QueryFilter.OpOptions.GT,
                    value: "0")
            }
        };

        var response = await LobbyService.Instance.QueryLobbiesAsync(queryOptions);
        var lobbies = response.Results;


        if (lobbies.Count > 0)
        {
            foreach ( var lobby in lobbies )
            {
                JoinLobby(lobby);
            }
        }
        else
        {
            CreateLobby();
        }
    }

    private async void JoinLobby(Lobby lobby)
    {
        var allocation = await RelayService.Instance.JoinAllocationAsync(lobby.Data[joinKey].Value);

        id = lobby.Id;

        SetTransformAsClient(allocation);

        NetworkManager.Singleton.StartClient();

        updateText.text = $"In a lobby";
    }

    public void SetTransformAsClient(JoinAllocation allocation)
    {
        _transport.SetClientRelayData(allocation.RelayServer.IpV4, (ushort)allocation.RelayServer.Port,
                allocation.AllocationIdBytes, allocation.Key, allocation.ConnectionData, allocation.HostConnectionData);
    }


    private async void CreateLobby()
    {
        updateText.text = "Creating Lobby";

        try
        {
            var allocation = await RelayService.Instance.CreateAllocationAsync(maxPlayers);

            var joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);

            var options = new CreateLobbyOptions
            {
                Data = new Dictionary<string, DataObject>
                {
                    { joinKey, new DataObject(DataObject.VisibilityOptions.Public, joinCode) }
                }
            };

            var lobby = await Lobbies.Instance.CreateLobbyAsync(lobbyName, maxPlayers, options);

            _transport.SetHostRelayData(allocation.RelayServer.IpV4, (ushort)allocation.RelayServer.Port,
                allocation.AllocationIdBytes, allocation.Key, allocation.ConnectionData);


            id = lobby.Id;
            StartCoroutine(HeartBeat(lobby.Id, 15f));


            NetworkManager.Singleton.StartHost();
            updateText.text = $"I am lobby Host {playerLevel}";
        }
        catch (Exception e)
        {
            Debug.LogError(e);
            updateText.text = "Failed Creating Lobby";
        }
    }

    public static async Task Login()
    {
        if (UnityServices.State == ServicesInitializationState.Uninitialized)
        {
            var options = new InitializationOptions();

            await UnityServices.InitializeAsync(options);
        }

        if (!AuthenticationService.Instance.IsSignedIn)
        {
            await AuthenticationService.Instance.SignInAnonymouslyAsync();

            PlayerId = AuthenticationService.Instance.PlayerId;
        }



    }

    public static IEnumerator HeartBeat(string lobbyId, float waitTimeSeconds)
    {
        var delay = new WaitForSeconds(waitTimeSeconds);

        while (true)
        {
            Lobbies.Instance.SendHeartbeatPingAsync(lobbyId);

            print("Beat");

            yield return delay;
        }
    }


    private void OnDestroy()
    {
        try
        {
            StopAllCoroutines();
            Lobbies.Instance.RemovePlayerAsync(id, PlayerId);
            Debug.LogFormat($"Logged out player from lobby: {id}, with playerId{PlayerId}");
        }
        catch
        {
            Debug.LogFormat("Failed to destroy");
        }
    }

}
