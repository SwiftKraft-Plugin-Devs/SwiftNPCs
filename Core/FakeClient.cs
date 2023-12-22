using Mirror;

namespace SwiftNPCs.Core
{
    public class FakeClient(int connectionId) : NetworkConnectionToClient(connectionId)
    {
        public override string address => "127.0.0.1";

        public override void Send(ArraySegment<byte> segment, int channelId = 0) { }

        public override void Disconnect() { }
    }
}
