const int RequestTimeout = 2500;
const int RequestRetries = 10;
const string ServerEndpoint = "tcp://localhost:5555";

string strSequenceSent = "";
bool expectReply = true;
int retriesLeft = 0;

retriesLeft = RequestRetries;

var client = CreateServerSocket();

int sequence = 0;

while (retriesLeft > 0)
{
    sequence++;
    strSequenceSent = sequence.ToString() + " HELLO";
    Console.WriteLine("C: Sending ({0})", strSequenceSent);
    client.SendFrame(Encoding.Unicode.GetBytes(strSequenceSent));
    expectReply = true;

    while (expectReply)
    {
        bool result = client.Poll(TimeSpan.FromMilliseconds(RequestTimeout));

        if (!result)
        {
            retriesLeft--;

            if (retriesLeft == 0)
            {
                Console.WriteLine("C: Server seems to be offline, abandoning");
                break;
            }
            else
            {
                Console.WriteLine("C: No response from server, retrying..");

                TerminateClient(client);

                client = CreateServerSocket();
                client.SendFrame(Encoding.Unicode.GetBytes(strSequenceSent));
            }
        }
    }
}

TerminateClient(client);

void TerminateClient(RequestSocket client)
{
    client.Disconnect(ServerEndpoint);
    client.Close();
}

RequestSocket CreateServerSocket()
{
    Console.WriteLine("C: Connecting to server...");

    var guid = Guid.NewGuid();
    var client = new RequestSocket
    {
        Options =
        {
            Linger = TimeSpan.Zero,
            Identity = Encoding.Unicode.GetBytes(guid.ToString())
        }
    };
    client.Connect(ServerEndpoint);
    client.ReceiveReady += ClientOnReceiveReady;

    return client;
}

void ClientOnReceiveReady(object sender, NetMQSocketEventArgs socket)
{
    var reply = socket.Socket.ReceiveFrameBytes();

    if (Encoding.Unicode.GetString(reply) == (strSequenceSent + " WORLD!"))
    {
        Console.WriteLine("C: Server replied OK ({0})", Encoding.Unicode.GetString(reply));
        retriesLeft = RequestRetries;
        expectReply = false;
    }
    else
    {
        Console.WriteLine("C: Malformed reply from server: {0}", Encoding.Unicode.GetString(reply));
    }
}
