using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using NamedMessageHandler = Unity.Netcode.CustomMessagingManager.HandleNamedMessageDelegate;

public class CustomMessageReceiverManager : SingletonMonoBehaviour<CustomMessageReceiverManager>
{
    // CAUTION:
    // NetworkManager.CustomMessagingManager will be null before OnClientStarted is called.

    private readonly Dictionary<string, NamedMessageHandler> _messageReceivers = new ();
    public IReadOnlyDictionary<string, NamedMessageHandler> MessageReceivers => _messageReceivers;

    private NetworkManager _networkManager;
    private NetworkManager NetworkManager => _networkManager ??= NetworkManager.Singleton;

    private void Start()
    {
        NetworkManager.OnClientStarted += AddMessageReceivers;
    }

    private void AddMessageReceivers()
    {
        var networkManager = NetworkManager.Singleton;

        if (networkManager.IsServer)
        {
            return;
        }

        var customMessagingManager = networkManager.CustomMessagingManager;

        foreach (var messageReceiver in _messageReceivers)
        {
            customMessagingManager.RegisterNamedMessageHandler(messageReceiver.Key, messageReceiver.Value);
        }
    }

    public void AddMessageReceiver(string messageName, NamedMessageHandler onReceiveMessage)
    {
        // NOTE:
        // onReceiveMessage sample.
        // private static void OnReceiveMessage(ulong senderId, FastBufferReader reader)
        // {
        //     reader.ReadValueSafe(out string receivedText);
        //     Debug.Log(receivedText);
        // }

        if (NetworkManager.IsServer)
        {
            Debug.LogWarning($"Cannot add a message receiver on the server. (messageName: {messageName})");
            return;
        }

        if (_messageReceivers.ContainsKey(messageName))
        {
            Debug.LogWarning($"{messageName} already exists. It will be overwritten.");
        }

        _messageReceivers.TryAdd(messageName, onReceiveMessage);

        NetworkManager.CustomMessagingManager?.RegisterNamedMessageHandler(messageName, onReceiveMessage);
    }
}