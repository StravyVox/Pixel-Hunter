using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;

public class RelayController : MonoBehaviour
{
    public Action<string> OnCreatedRelay;
    public Action OnJoinedRelay;
    [SerializeField] ConnectionInfo _connectionInfoHandler;

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
