using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Manager, that controls  main game session, which includes
/// work with current scores
/// setting behaviour when some of the players die
/// 
/// </summary>
public class SessionManager : NetworkBehaviour
{

    public static SessionManager Singleton; //Only on SessionManager on scene 
    [SerializeField] private CoinsGenerator _coinsGenerator;
    [SerializeField] private ScoreManager _scoreManager;
    [SerializeField] private InGameUIController _gameUIController;
    [SerializeField] private int _amountOfConnectedToActivate; //Check if at least (value) players are on scene to generate coins
    [SerializeField] private string _menuScene; //Start offline menu
    private NetworkManager _networkManager;
    private int _connectedPlayers;
    private int _alivePlayers;
    private void Start()
    {
        if (Singleton == null)
        {
            Singleton = this;
        }
        else if (Singleton == this)
        {
            Destroy(gameObject);
        }
        _networkManager = NetworkManager.Singleton;
        _networkManager.OnClientConnectedCallback += (id) => {
            if (!IsHost) return;
            _connectedPlayers = _networkManager.ConnectedClients.Count;
            _alivePlayers = _networkManager.ConnectedClients.Count;
            _scoreManager.AddObjects(_networkManager.ConnectedClientsIds.ToList<ulong>());
            SetonZeroHPActions();
            if (_connectedPlayers >= _amountOfConnectedToActivate)
            {
                _coinsGenerator.StartGenerate();
            }

        };
        _networkManager.OnClientDisconnectCallback += (id) =>
        {
            if (!IsHost) return;
            _connectedPlayers--;
            _alivePlayers--;
            if (_connectedPlayers <= _amountOfConnectedToActivate)
            {
                _coinsGenerator.StopGenerate();
            }
        };
        _gameUIController.OnExitButton = () => ExitGame();
    }
    /// <summary>
    /// Despawn player by server
    /// </summary>
    /// <param name="id"></param>
    [ServerRpc]
    private void DespawnServerRpc(ulong id)
    {
        _networkManager.ConnectedClients[id].PlayerObject.Despawn();
    }
    
    public void AddScore(ulong id)
    {   
        if (_scoreManager != null)
        {
            Debug.Log("Adding score for id: " + id);
            _scoreManager.AddScore((int)id);
        }
    }
    /// <summary>
    /// Shutdown connection and load start scene
    /// </summary>
    private void ExitGame()
    {
        _networkManager.Shutdown();
        Destroy(gameObject,2);
        SceneManager.LoadScene(_menuScene);
    }
    /// <summary>
    /// Checking for alive players and getting their score
    /// after that calling info to despawn them;
    /// </summary>
    private void EndGame()
    {
        foreach (NetworkClient player in _networkManager.ConnectedClients.Values) {
            if (player.PlayerObject.GetComponent<HP>().HealthPoints>0)
            {
                ShowResultsClientRpc((int)player.ClientId, _scoreManager.GetScore((int)player.ClientId));
                DespawnServerRpc(player.ClientId);
            }  
        }
    }
    /// <summary>
    /// Created on client side and activating end game UI scoreboard with all info
    /// </summary>
    /// <param name="id"></param>
    /// <param name="score"></param>
    [ClientRpc] 
    private void ShowResultsClientRpc(int id, int score) 
    {
        _coinsGenerator.StopGenerate();
        _gameUIController.SetWinResults(id, score);
        _gameUIController.ShowWinScoreBoard();
    }
    /// <summary>
    /// Server is setting actions, when players get zero hp
    /// </summary>
    private void SetonZeroHPActions()//Done by server side
    {
        foreach(NetworkClient player in _networkManager.ConnectedClients.Values)
        {
            player.PlayerObject.GetComponent<HP>().OnZeroHP = () =>
            {
                    DespawnServerRpc(player.ClientId);
                    
                    _alivePlayers--;
                    if (_alivePlayers == 1)
                    {
                        this.EndGame();
                    }
            };
        }
    }
}
