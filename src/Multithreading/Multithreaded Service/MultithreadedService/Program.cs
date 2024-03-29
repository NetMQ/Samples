﻿using NetMQ.Devices;

CancellationToken token;

Console.Title = "NetMQ Multi-threaded Service";

var queue = new QueueDevice("tcp://localhost:5555", "tcp://localhost:5556", DeviceMode.Threaded);

var source = new CancellationTokenSource();
token = source.Token;

for (int threadId = 0; threadId < 10; threadId++)
{
    _ = Task.Factory.StartNew(WorkerRoutine, token);
}

queue.Start();

var tasks = new List<Task>();
for (int i = 0; i < 1000; i++)
{
    int clientId = i;
    tasks.Add(Task.Factory.StartNew(() => ClientRoutine(clientId)));
}

await Task.WhenAll(tasks.ToArray());

source.Cancel();

queue.Stop();

Console.WriteLine("Press ENTER to exit...");
Console.ReadLine();

void ClientRoutine(object clientId)
{
    try
    {
        using var req = new RequestSocket();
        req.Connect("tcp://localhost:5555");

        byte[] message = Encoding.Unicode.GetBytes($"{clientId} Hello");

        Console.WriteLine("Client {0} sent \"{0} Hello\"", clientId);
        req.SendFrame(message, message.Length);

        var response = req.ReceiveFrameString(Encoding.Unicode);
        Console.WriteLine("Client {0} received \"{1}\"", clientId, response);
    }
    catch (Exception ex)
    {
        Console.WriteLine("Exception on ClientRoutine: {0}", ex.Message);
    }
}

void WorkerRoutine()
{
    try
    {
        using ResponseSocket rep = new();
        rep.Options.Identity = Encoding.Unicode.GetBytes(Guid.NewGuid().ToString());
        rep.Connect("tcp://localhost:5556");
        //rep.Connect("inproc://workers");
        rep.ReceiveReady += RepOnReceiveReady;
        while (!token.IsCancellationRequested)
        {
            rep.Poll(TimeSpan.FromMilliseconds(100));
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine("Exception on WorkerRoutine: {0}", ex.Message);
        throw;
    }
}

void RepOnReceiveReady(object sender, NetMQSocketEventArgs args)
{
    try
    {
        NetMQSocket rep = args.Socket;

        byte[] message = rep.ReceiveFrameBytes();

        //Thread.Sleep(1000); //  Simulate 'work'

        byte[] response =
            Encoding.Unicode.GetBytes(Encoding.Unicode.GetString(message) + " World from worker " + Encoding.Unicode.GetString(rep.Options.Identity));

        rep.TrySendFrame(response, response.Length);
    }
    catch (Exception ex)
    {
        Console.WriteLine("Exception on RepOnReceiveReady: {0}", ex.Message);
        throw;
    }
}
