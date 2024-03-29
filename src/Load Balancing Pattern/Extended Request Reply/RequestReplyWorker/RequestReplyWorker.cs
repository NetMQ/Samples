﻿const string WorkerEndpoint = "tcp://127.0.0.1:5560";

using var worker = new ResponseSocket();

worker.Connect(WorkerEndpoint);

while (true)
{
    var msg = worker.ReceiveMultipartMessage();

    Console.WriteLine("Processing Message {0}", msg.Last.ConvertToString());

    Thread.Sleep(500);

    worker.SendFrame(msg.Last.ConvertToString());
}
