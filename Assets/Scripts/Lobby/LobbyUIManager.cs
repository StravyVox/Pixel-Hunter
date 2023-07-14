using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LobbyUIManager : MonoBehaviour
{
    [SerializeField] private LobbyManager _lobbyManager;
    [SerializeField] private TextMeshProUGUI _PlayersInfo;
    [SerializeField] private TextMeshProUGUI _LobbyNameTextArea;
    [SerializeField] private TextMeshProUGUI _LobbyEnterCode;
    [SerializeField] private TextMeshProUGUI _lobbyNameInput;
    [SerializeField] private GameObject _createLobbyUILayout;
    [SerializeField] private GameObject _startMenuUILayout;
    [SerializeField] private GameObject _connectToLobbyUILayout;
    [SerializeField] private GameObject _waitingScreenUILayout;
    [SerializeField] private GameObject _lobbyUILayout;
    [SerializeField] private GameObject _startButton;
    [SerializeField] private GameObject _backToMenuButton;
    private bool _isHost = false;
    private void Start()
    {
        _startButton.SetActive(false);
        _lobbyManager.OnError += () => leaveToMenu();
        _lobbyManager.OnNewPlayers += (newAmount) =>
        {
            _PlayersInfo.text = newAmount + " players has been connected";
            if(newAmount >= 2 && _isHost)
            {
                _startButton.SetActive(true);
            }
            else
            {
                _startButton.SetActive(false);
            }
        };
        _lobbyManager.OnConnectedToLobby += () =>
        {
            _waitingScreenUILayout.SetActive(false);
            _lobbyUILayout.SetActive(true);

            _backToMenuButton.SetActive(true);
        };
        _lobbyManager.OnCreatedLobby += (lobbyName) =>
        {
            _LobbyNameTextArea.text = lobbyName;
            _waitingScreenUILayout.SetActive(false);
            _lobbyUILayout.SetActive(true);
            _backToMenuButton.SetActive(true);
        };
        _lobbyManager.OnConnectingToGame += () =>
        {
            _waitingScreenUILayout.SetActive(true);
            _lobbyUILayout.SetActive(false);

        };
    }
    public void StartGame()
    {
        _lobbyManager.StartGame();    
    }
    public void OpenCreateLobbyMenu()
    {
        _startMenuUILayout.SetActive(false);
        _createLobbyUILayout.SetActive(true);

    }
    public void CreateLobby()
    {
        _createLobbyUILayout.SetActive(false);
        _startMenuUILayout.SetActive(false);
        _waitingScreenUILayout.SetActive(true);
        _lobbyManager.CreateLobby(_lobbyNameInput.text);
        _isHost = true;
    }
    public void OpenConnectLobby()
    {

        _backToMenuButton.SetActive(true);
        _connectToLobbyUILayout.SetActive(true);
        _startMenuUILayout.SetActive(false);
    }
    public void ConnectToLobby() {
        _connectToLobbyUILayout.SetActive(false);
        _waitingScreenUILayout.SetActive(true);
        _lobbyManager.ConnectToLobby(_LobbyEnterCode.text.ToString());
        _isHost = false;
    }
    public void leaveToMenu()
    {
        _lobbyManager.LeaveLobby();

        _backToMenuButton.SetActive(false);
        _connectToLobbyUILayout.SetActive(false);
        _waitingScreenUILayout.SetActive(false);
        _LobbyNameTextArea.gameObject.SetActive(true);
        _lobbyUILayout.gameObject.SetActive(false);
        _startMenuUILayout.SetActive(true);
    }
}
