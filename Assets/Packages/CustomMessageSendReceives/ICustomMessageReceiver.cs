using Unity.Netcode;

public interface ICustomMessageReceiver
{
    string MessageName { get; }
    void OnReceiveCustomMessage(ulong senderId, FastBufferReader reader);
}