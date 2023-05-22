// Run both LazyPirate.Server and LazyPirate.Client simultaneously to see the demonstration of this pattern.

const int RequestTimeout = 2500;
const int RequestRetries = 10;
const string ServerEndpoint = "tcp://127.0.0.1:5555";

int sequence = 0;
bool expectReply = true;
int retriesLeft = RequestRetries;

Console.Title = "NetMQ LazyPirate Client";

RequestSocket client = CreateServerSocket();

while (retriesLeft > 0)
{
    sequence++;
    Console.WriteLine("C: Sending ({0})", sequence);
    client.SendFrame(Encoding.Unicode.GetBytes(sequence.ToString()));
    expectReply = true;

    while (expectReply)
    {
        bool result = client.Poll(TimeSpan.FromMilliseconds(RequestTimeout));

        if (result)
            continue;

        retriesLeft--;

        if (retriesLeft == 0)
        {
            Console.WriteLine("C: Server seems to be offline, abandoning");
            break;
        }

        Console.WriteLine("C: No response from server, retrying...");

        TerminateClient(client);

        client = CreateServerSocket();
        client.SendFrame(Encoding.Unicode.GetBytes(sequence.ToString()));
    }
}

TerminateClient(client);

void TerminateClient(NetMQSocket client)
{
    client.Disconnect(ServerEndpoint);
    client.Close();
}

RequestSocket CreateServerSocket()
{
    Console.WriteLine("C: Connecting to server...");

    var client = new RequestSocket();
    client.Connect(ServerEndpoint);
    client.Options.Linger = TimeSpan.Zero;
    client.ReceiveReady += ClientOnReceiveReady;

    return client;
}

void ClientOnReceiveReady(object sender, NetMQSocketEventArgs args)
{
    var reply = args.Socket.ReceiveFrameBytes();
    string strReply = Encoding.Unicode.GetString(reply);

    if (int.Parse(strReply) == sequence)
    {
        Console.WriteLine("C: Server replied OK ({0})", strReply);
        retriesLeft = RequestRetries;
        expectReply = false;
    }
    else
    {
        Console.WriteLine("C: Malformed reply from server: {0}", strReply);
    }
}
