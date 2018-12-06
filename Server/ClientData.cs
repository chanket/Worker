using Common.Frames.Information;
using System.Net.Sockets;

namespace Server
{
    public class ClientData : Common.Server.ClientDataBase
    {
        public AnswerFrame Information { get; set; }

        public ClientData(Server server, TcpClient client, AnswerFrame information)
            : base(server, client)
        {
            Information = information;
        }
    }
}
