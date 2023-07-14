using System.Collections;
using System.Collections.Generic;
using Unity.Services.Relay.Models;
using UnityEngine;

[CreateAssetMenu(fileName = "Connection Info", menuName = "ScriptableObjects/ConnectionInfo", order = 2)]
public class ConnectionInfo : ScriptableObject
{
    public bool Host;
    public Allocation allocationData;
    public JoinAllocation joinAllocation;
    public string joinRelayCode;

}
