using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoadingController : MonoBehaviour
{
    [SerializeField] private NetworkSceneManager _networkSceneManager;
    [SerializeField] private NetworkManager _networkManager;
    [SerializeField] private ConnectionInfo _connectionInfo;
    [SerializeField] private string _connectionScene;
    private void Start()
    {
        Debug.Log("Network Spawned");
        if (_connectionInfo != null)
        {
            _networkManager = NetworkManager.Singleton;
            _networkSceneManager = _networkManager.SceneManager;
            try
            {
                UnityTransport transport = _networkManager.GetComponent<UnityTransport>();
                
            }
            catch
            {
                Debug.Log("Failed to get Unity Transport");
            }
            if (_connectionInfo.Host)
            {
                Debug.Log(_connectionInfo.allocationData);
                Debug.Log(_networkManager);
                _networkManager.GetComponent<UnityTransport>().SetHostRelayData(
                    _connectionInfo.allocationData.RelayServer.IpV4,
                    (ushort)_connectionInfo.allocationData.RelayServer.Port,
                    _connectionInfo.allocationData.AllocationIdBytes,
                    _connectionInfo.allocationData.Key,
                    _connectionInfo.allocationData.ConnectionData
                    );
                _networkManager.StartHost();

                
            }
            else
            {
                _networkManager.GetComponent<UnityTransport>().SetClientRelayData(
                    _connectionInfo.joinAllocation.RelayServer.IpV4,
                    (ushort)_connectionInfo.joinAllocation.RelayServer.Port,
                    _connectionInfo.joinAllocation.AllocationIdBytes,
                    _connectionInfo.joinAllocation.Key,
                    _connectionInfo.joinAllocation.ConnectionData,
                    _connectionInfo.joinAllocation.HostConnectionData
                    );
                _networkManager.StartClient();

            }
        }
    }
    private void StartScene()
    {
        if(_networkSceneManager == null)
        {
            _networkSceneManager = _networkManager.SceneManager;
        }
        Debug.Log(_networkSceneManager);
        _networkSceneManager.ActiveSceneSynchronizationEnabled = true;
        _networkSceneManager.LoadScene(_connectionScene, UnityEngine.SceneManagement.LoadSceneMode.Single);
       
        
    }

    
}
