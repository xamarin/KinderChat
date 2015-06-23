using SuperWebSocket.SubProtocol;

namespace KinderChat.WorkerRole.SocketServer.Infrastructure.Transport.Sockets.Commands
{
    public class Ping : JsonSubCommand<LiveSession, bool>
    {
        protected override void ExecuteJsonCommand(LiveSession session, bool arg)
        {
            SendJsonMessage(session, arg);
        }
    }
}
