using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAPI;
using MLAPI.Spawning;
using System;
using MLAPI.Transports.UNET;

public class ConnectionManager : MonoBehaviour
{
    public GameObject UIPanel;
    public Camera cameraUI;

    public string ipAdress = "127.0.0.1";

    UNetTransport transport;
    public void Host()
    {
        UIPanel.SetActive(false);
        NetworkManager.Singleton.ConnectionApprovalCallback += ApprovalCheck;
        cameraUI.gameObject.SetActive(false);
        NetworkManager.Singleton.StartHost(new Vector3(0, 2, 0), Quaternion.identity);
    }

    private void ApprovalCheck(byte[] connectionData, ulong clientID, NetworkManager.ConnectionApprovedDelegate callback)
    {
        Debug.Log("Password received : " + System.Text.Encoding.ASCII.GetString(connectionData));
        bool approve = System.Text.Encoding.ASCII.GetString(connectionData) == "Password1234";
        callback(true, null, approve, new Vector3(0, 2, 0), Quaternion.identity);
    }

    public void Join()
    {
        transport = NetworkManager.Singleton.GetComponent<UNetTransport>();
        transport.ConnectAddress = ipAdress;
        UIPanel.SetActive(false);
        NetworkManager.Singleton.NetworkConfig.ConnectionData = System.Text.Encoding.ASCII.GetBytes("Password1234");
        cameraUI.gameObject.SetActive(false);
        NetworkManager.Singleton.StartClient();

    }

    public void IpAdressChanged(string newAdress)
    {
        this.ipAdress = newAdress;
    }
}
