const int RequestTimeout = 1000; // ms
const int MaxRetries = 50; // Before we abandon

var endpoints = new List<string>
{
    "tcp://*** Add your first endpoint ***",
    "tcp://*** Add your second endpoint ***"
};

if (endpoints.Count == 1)
{
    for (int i = 0; i < MaxRetries; i++)
    {
        if (TryRequest(endpoints[0], $"Hello from endpoint {endpoints[0]}"))
            break;
    }
}
else if (endpoints.Count > 1)
{
    foreach (string endpoint in endpoints)
    {
        if (TryRequest(endpoint, $"Hello from endpoint {endpoint}"))
            break;
    }
}
else
{
    Console.WriteLine("No endpoints");
}

Console.WriteLine("Press any key to exit...");
Console.ReadKey();

static bool TryRequest(string endpoint, string requestString)
{
    Console.WriteLine("Trying echo service at {0}", endpoint);

    using (var client = new RequestSocket())
    {
        client.Options.Linger = TimeSpan.Zero;

        client.Connect(endpoint);

        client.SendFrame(requestString);
        client.ReceiveReady += ClientOnReceiveReady;
        bool pollResult = client.Poll(TimeSpan.FromMilliseconds(RequestTimeout));
        client.ReceiveReady -= ClientOnReceiveReady;
        client.Disconnect(endpoint);
        return pollResult;
    }
}

static void ClientOnReceiveReady(object sender, NetMQSocketEventArgs args)
{
    Console.WriteLine("Server replied ({0})", args.Socket.ReceiveFrameString());
}