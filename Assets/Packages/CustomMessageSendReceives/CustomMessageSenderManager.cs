using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;

public class CustomMessageSenderManager : SingletonMonoBehaviour<CustomMessageSenderManager>
{
    private NetworkManager _networkManager;
    private NetworkManager NetworkManager => _networkManager ??= NetworkManager.Singleton;
    private UnityTransport _unityTransport;
    private UnityTransport UnityTransport => NetworkManager.NetworkConfig.NetworkTransport as UnityTransport;

    public Dictionary<ulong, (string address, bool send)> ConnectedClients { get; } = new(); // <clientId, (Address, send)>

    private void Start()
    {
        NetworkManager.OnClientConnectedCallback += (clientId) =>
        {
            var endPoint = UnityTransport.GetEndpoint(clientId);
            ConnectedClients.TryAdd(clientId, (endPoint.Address, true));
        };

        NetworkManager.OnClientDisconnectCallback += (clientId) =>
        {
            ConnectedClients.Remove(clientId);
        };
    }

    // CAUTION:
    // NetworkDelivery.ReliableSequenced setting is required when the payload size gets over 1264 bytes.

    public void SendCustomDataToAll(string messageName, FastBufferWriter writer,
                                    NetworkDelivery delivery = NetworkDelivery.ReliableSequenced)
    {
        if (!NetworkManager.IsServer)
        {
            return;
        }

        NetworkManager.CustomMessagingManager.SendNamedMessageToAll(messageName, writer, delivery);
    }

    public void SendCustomData(string messageName, FastBufferWriter writer,
                               NetworkDelivery delivery = NetworkDelivery.ReliableSequenced)
    {
        if (!NetworkManager.IsServer)
        {
            return;
        }

        foreach (var clientId in ConnectedClients.Keys.Where(clientId => ConnectedClients[clientId].send))
        {
            NetworkManager.CustomMessagingManager.SendNamedMessage(messageName, clientId, writer, delivery);
        }
    }
}