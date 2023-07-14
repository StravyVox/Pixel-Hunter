using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SessionManager : NetworkBehaviour
{
    public static SessionManager Singleton; 
    private NetworkManager _networkManager;
    [SerializeField] private CoinsGenerator _coinsGenerator;
    [SerializeField] private ScoreManager _scoreManager;
    [SerializeField] private InGameUIController _gameUIController;
    [SerializeField] private int _amountOfConnectedToActivate;
    [SerializeField] private string _menuScene;
    private int _connectedPlayers;
    private int _alivePlayers;
    private void Start()
    {
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
        if (Singleton == null)
        {
            Singleton = this;
        }
        else if (Singleton == this)
        {
            Destroy(gameObject);
        }
    }
    [ServerRpc]
    private void DespawnServerRpc(ulong id)
    {
        _networkManager.ConnectedClients[id].PlayerObject.Despawn();
    }
    
    public void AddScore(ulong id)
    {
        if (_scoreManager != null)
        {
            _scoreManager.AddScore((int)id);
        }
    }
    private void ExitGame()
    {
        _networkManager.Shutdown();
        SceneManager.LoadScene(_menuScene);
    }
    private void EndGame()
    {
        Debug.Log("Endgame active");
        foreach (NetworkClient player in _networkManager.ConnectedClients.Values) {
            if (player.PlayerObject.gameObject)
            {
                ShowResultsClientRpc((int)player.ClientId, _scoreManager.GetScore((int)player.ClientId));
                DespawnServerRpc(player.ClientId);
            }  
        }
    }
    [ClientRpc] 
    private void ShowResultsClientRpc(int id, int score)
    {
        _coinsGenerator.StopGenerate();
        _gameUIController.SetWinResults(id, score);
        _gameUIController.ShowWinScoreBoard();
    }
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
