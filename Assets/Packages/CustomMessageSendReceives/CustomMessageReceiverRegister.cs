using System.Linq;
using UnityEngine;

public class CustomMessageReceiverRegister : SingletonMonoBehaviour<CustomMessageReceiverRegister>
{
    private void Start()
    {
        var messageReceiverManager = FindAnyObjectByType<CustomMessageReceiverManager>();
        var monoBehaviours         = FindObjectsByType<MonoBehaviour>(FindObjectsInactive.Exclude);
        var iMessageReceivers      = monoBehaviours.OfType<ICustomMessageReceiver>();

        foreach (var iMessageReceiver in iMessageReceivers)
        {
            messageReceiverManager.AddMessageReceiver(iMessageReceiver.MessageName,
                                                      iMessageReceiver.OnReceiveCustomMessage);
        }
    }
}