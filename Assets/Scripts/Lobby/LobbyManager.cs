using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Text;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LobbyManager : MonoBehaviour
{
    public Action OnConnectingToGame;
    public Action<int> OnNewPlayers;
    public Action OnConnectedToLobby;
    public Action<string> OnCreatedLobby;
    public Action OnError;

    [SerializeField] private int _amountOfPlayers;
    [SerializeField] private float _lobbyKeeperDelayTime;
    [SerializeField] private RelayController _relayController;
    [SerializeField] private String _loadingScene;
    private float _timerForAlive;
    private float _timerForPolling;
    private int _currentAmountOfPlayers = 0;
    private Lobby _lobby;
    private async void Start()
    {
        await UnityServices.InitializeAsync();

        AuthenticationService.Instance.SignedIn += () =>
        {

        };
        await AuthenticationService.Instance.SignInAnonymouslyAsync();
    }
    private void Update()
    {
        keepLobbyAlive();
        keepLobbyUpdated();
    }
    private async void keepLobbyUpdated()
    {
        if (_lobby != null)
        {
            _timerForPolling -= Time.deltaTime;
            if (_timerForPolling < 0)
            {
                _timerForPolling = 1.1f;
                _lobby = await LobbyService.Instance.GetLobbyAsync(_lobby.Id);
                if (_lobby.Data["relay_ready"].Value != "0")
                {
                    if(AuthenticationService.Instance.PlayerId != _lobby.HostId)
                    {
                        _relayController.OnJoinedRelay = () => SceneManager.LoadScene(_loadingScene,LoadSceneMode.Single);
                        _relayController.JoinRelay(_lobby.Data["relay_ready"].Value);
                        OnConnectingToGame?.Invoke();
                    }
                }
                checkLobbyForNewPlayer();

            }
        }
    }
    private void checkLobbyForNewPlayer()
    {
        if (_lobby != null) {
            if (_currentAmountOfPlayers != _lobby.Players.Count)
            {
                OnNewPlayers?.Invoke(_lobby.Players.Count);
                _currentAmountOfPlayers = _lobby.Players.Count;
            }
        }
    }
    private async void keepLobbyAlive()
    {
       if(_lobby != null && _lobby.HostId == AuthenticationService.Instance.PlayerId)
        {
            _timerForAlive -= Time.deltaTime;
            if(_timerForAlive < 0)
            {
                _timerForAlive = _lobbyKeeperDelayTime;
                await LobbyService.Instance.SendHeartbeatPingAsync(_lobby.Id);
            }
        }
    }
    public async void CreateLobby(string name)
    {
        try
        {
            _lobby = await LobbyService.Instance.CreateLobbyAsync(name, _amountOfPlayers, new CreateLobbyOptions() {
                IsPrivate = false,
                Data = new Dictionary<string, DataObject>
                {
                    {"relay_ready", new DataObject(DataObject.VisibilityOptions.Member, "0") }
                } 
            });
            checkLobbyForNewPlayer();
            OnCreatedLobby?.Invoke(_lobby.Name);
        }
        catch (LobbyServiceException ex)
        {
            Debug.LogException(ex);
            OnError?.Invoke();
        }
    }
    
    public async void ConnectToLobby(string lobbyName)
    {
        try
        {
            lobbyName = lobbyName.Substring(0, lobbyName.Length-1);
            Lobby connectedlobby = await Lobbies.Instance.QuickJoinLobbyAsync(new QuickJoinLobbyOptions() { Filter = new List<QueryFilter>() { new QueryFilter(QueryFilter.FieldOptions.Name, lobbyName, QueryFilter.OpOptions.CONTAINS) } }); 
            _lobby = connectedlobby;
            OnConnectedToLobby?.Invoke();
        }
        catch (Exception ex)
        {
            Debug.LogError(ex);
            Debug.LogException(ex);
            OnError?.Invoke();
        }

    }
    public async void LeaveLobby()
    {
        try
        {
            await LobbyService.Instance.RemovePlayerAsync(_lobby.Id, AuthenticationService.Instance.PlayerId);
            _lobby = null;
        }
        catch (LobbyServiceException ex)
        {
            Debug.LogException(ex);
            OnError?.Invoke();
        }
    }
    public async void StartGame()
    {
        if(_lobby.HostId == AuthenticationService.Instance.PlayerId)
        {
            _relayController.OnCreatedRelay = async (joinCode) =>
            {
                OnConnectingToGame?.Invoke();
                _lobby = await Lobbies.Instance.UpdateLobbyAsync(_lobby.Id, new UpdateLobbyOptions()
                {
                    Data = new Dictionary<string, DataObject>()
                    {
                         {"relay_ready", new DataObject(DataObject.VisibilityOptions.Member, joinCode) }
                    }
                });

                SceneManager.LoadScene(_loadingScene, LoadSceneMode.Single);
            };
            _relayController.CreateRelay();
        }
    }
}
