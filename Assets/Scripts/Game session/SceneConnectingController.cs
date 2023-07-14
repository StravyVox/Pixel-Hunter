using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Controller, that will be activated as soon as scene will be loaded
/// Main purpose is get info from scriptable object about relay connection
/// and set connection and creation parameters for networkmanager 
/// </summary>
public class SceneConnectingController : MonoBehaviour
{
    [SerializeField] private ConnectionInfo _connectionInfo;//scriptable object
    [SerializeField] private string _connectionScene;
    private NetworkManager _networkManager;
    private void Start()
    {   if (_connectionInfo != null)
        {
            _networkManager = NetworkManager.Singleton;
            if (_connectionInfo.Host)
            {
                try
                {
                    _networkManager.GetComponent<UnityTransport>().SetHostRelayData(
                        _connectionInfo.allocationData.RelayServer.IpV4,
                        (ushort)_connectionInfo.allocationData.RelayServer.Port,
                        _connectionInfo.allocationData.AllocationIdBytes,
                        _connectionInfo.allocationData.Key,
                        _connectionInfo.allocationData.ConnectionData
                        );
                    _networkManager.StartHost();
                }
                catch(Exception ex)
                {
                    Debug.Log("Error on starting host");
                    Debug.Log(ex);
                }
                
            }
            else
            {
                try
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

                catch (Exception ex)
                {
                    Debug.Log("Error on starting client");
                    Debug.Log(ex);
                }

            }
        }
    }   
}
