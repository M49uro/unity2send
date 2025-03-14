using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UniRx;
using UnityEngine.UI;

public class UdpReceive : MonoBehaviour
{
    private UdpClient udpClient;
    private Subject<string> subject = new Subject<string>();
    [SerializeField] Text message;


    void Start()
    {
        udpClient = new UdpClient(35565);
        udpClient.BeginReceive(OnReceived, udpClient);

        subject
            .ObserveOnMainThread()
            .Subscribe(msg => {
                message.text = msg;
            }).AddTo(this);
    }

    private void OnReceived(System.IAsyncResult result)
    {
        UdpClient getUdp = (UdpClient)result.AsyncState;
        IPEndPoint ipEnd = null;

        byte[] getByte = getUdp.EndReceive(result, ref ipEnd);

        var message = Encoding.UTF8.GetString(getByte);
        subject.OnNext(message);

        getUdp.BeginReceive(OnReceived, getUdp);
    }

    private void OnDestroy()
    {
        udpClient.Close();
    }
}