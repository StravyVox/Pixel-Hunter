using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;

/// <summary>
/// Relay Unity controller script provides functions for creating and joining relay
/// It used for creatind dedicated server
/// All connection info is stored in Scriptable Object
/// </summary>
public class RelayController : MonoBehaviour
{
    /// <summary>
    /// Actions for providing info about connection process
    /// </summary>
    public Action<string> OnCreatedRelay; 
    public Action OnJoinedRelay;

    [SerializeField] ConnectionInfo _connectionInfoHandler; //Scriptable object

    /// <summary>
    /// HOST method to create dedicated server and store data to connectionInfoHandler
    /// </summary>
    public async void CreateRelay()
    {
        try
        {
            Allocation allocation = await RelayService.Instance.CreateAllocationAsync(3);
            string joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);
            _connectionInfoHandler.allocationData = allocation;
            _connectionInfoHandler.joinRelayCode = joinCode;
            _connectionInfoHandler.Host = true;
            OnCreatedRelay?.Invoke(joinCode);
        }
        catch (RelayServiceException ex)
        {
            Debug.LogException(ex);
        }
    }
    /// <summary>
    /// CLIENT method to connect to dedicated server by relay joinCode and store data to connectionInfoHandler
    /// </summary>
    /// <param name="joinCode"></param>
    public async void JoinRelay(string joinCode)
    {
        try
        {
           JoinAllocation joinAllocation = await Relay.Instance.JoinAllocationAsync(joinCode);
            _connectionInfoHandler.Host = false;
            _connectionInfoHandler.joinAllocation = joinAllocation;
            OnJoinedRelay?.Invoke();
        }
        catch (RelayServiceException ex)
        {
            Debug.LogException(ex);
        }
    }
}
