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


/// <summary>
/// Manager, that controls work with Lobby interface and give an ability
/// for players to connect to lobby;
/// </summary>
public class LobbyManager : MonoBehaviour
{
    public Action OnConnectingToGame;
    public Action<int> OnNewPlayers;
    public Action OnConnectedToLobby;
    public Action<string> OnCreatedLobby;
    public Action OnError;

    [SerializeField] private int _amountOfPlayers; // max amount of players in lobby
    [SerializeField] private float _lobbyKeeperDelayTime; 
    [SerializeField] private RelayController _relayController;
    [SerializeField] private String _loadingScene;
    private float _timerForAlive;
    private float _timerForPolling;
    private int _currentAmountOfPlayers = 0;
    private Lobby _lobby;
    private async void Start()
    {
        try
        {
            await UnityServices.InitializeAsync();

            AuthenticationService.Instance.SignedIn += () =>
            {

            };
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
        }
        catch(Exception e) 
        {
            Debug.Log(e);
        }
    }
    private void Update()
    {
        keepLobbyAlive();
        keepLobbyUpdated();
    }
    /// <summary>
    /// Updating lobby info once in _timerForPolling value
    /// 
    /// </summary>
    private async void keepLobbyUpdated()
    {
        if (_lobby != null)
        {
            _timerForPolling -= Time.deltaTime;
            if (_timerForPolling < 0)
            {
                try
                {
                    _timerForPolling = 1.1f;
                    _lobby = await LobbyService.Instance.GetLobbyAsync(_lobby.Id);
                    if (_lobby.Data["relay_ready"].Value != "0") //Check if connection info (Relay joinCode) for game is ready
                    {
                        if (AuthenticationService.Instance.PlayerId != _lobby.HostId) //Only clients can Join relay. Host already joined
                        {
                            _relayController.OnJoinedRelay = () => SceneManager.LoadScene(_loadingScene, LoadSceneMode.Single);
                            _relayController.JoinRelay(_lobby.Data["relay_ready"].Value);
                            OnConnectingToGame?.Invoke();
                        }
                    }
                    checkLobbyForNewPlayer();
                }
                catch
                {
                    _lobby = null;
                }
            }
        }
    }
    /// <summary>
    /// Check for income players and call an Action if any new players connected
    /// </summary>
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
    /// <summary>
    /// HOST method to avoid lobby deletion
    /// </summary>
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
    /// <summary>
    /// Host method to create lobby with defined name
    /// </summary>
    /// <param name="name"></param>
    public async void CreateLobby(string name)
    {
        try
        {
            _lobby = await LobbyService.Instance.CreateLobbyAsync(name, _amountOfPlayers, new CreateLobbyOptions() {
                IsPrivate = false,
                Data = new Dictionary<string, DataObject>
                {
                    {"relay_ready", new DataObject(DataObject.VisibilityOptions.Member, "0") } //Setting data for Relay Connection Info.
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
    /// <summary>
    /// CLIENT method to connect by name
    /// Using QuickJoinLobby method, that provides options for filtering lobbies
    /// In this script filter works by the name of Lobby
    /// </summary>
    /// <param name="lobbyName"></param>
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
    /// <summary>
    /// Leavng lobby and setting null for next connections
    /// </summary>
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
    /// <summary>
    /// HOST method, that calls RelayController logic for creating Relay joinCode
    /// </summary>
    public void StartGame()
    {
        if(_lobby.HostId == AuthenticationService.Instance.PlayerId)
        {
            _relayController.OnCreatedRelay = async (joinCode) =>
            {
                OnConnectingToGame?.Invoke();
                _lobby = await Lobbies.Instance.UpdateLobbyAsync(_lobby.Id, new UpdateLobbyOptions() //Updating lobby data with given joinCode
                {
                    Data = new Dictionary<string, DataObject>()
                    {
                         {"relay_ready", new DataObject(DataObject.VisibilityOptions.Member, joinCode) }
                    }
                });

                SceneManager.LoadScene(_loadingScene, LoadSceneMode.Single); //Moving to the Game Scene
            };
            _relayController.CreateRelay();
        }
    }
}
