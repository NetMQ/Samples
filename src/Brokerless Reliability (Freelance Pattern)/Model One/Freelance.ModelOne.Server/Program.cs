using System.Net.Sockets;

const uint PortNumber = 5555;

using (var response = new ResponseSocket())
{
    string address = GetComputerLanIP();

    if (!string.IsNullOrEmpty(address))
    {
        Console.WriteLine("Binding tcp://{0}:{1}", address, PortNumber);
        response.Bind($"tcp://{address}:{PortNumber}");

        while (true)
        {
            bool hasMore;
            string msg = response.ReceiveFrameString(out hasMore);
            if (string.IsNullOrEmpty(msg))
            {
                Console.WriteLine("No msg received.");
                break;
            }

            Console.WriteLine("Msg received! {0}", msg);
            response.SendFrame(msg, hasMore);

            Thread.Sleep(1000);
        }

        response.Options.Linger = TimeSpan.Zero;
    }
    else
    {
        Console.WriteLine("Wrong IP address");
    }

    Console.WriteLine("Press any key to exit...");
    Console.ReadKey();
}

static string GetComputerLanIP()
{
    string strHostName = Dns.GetHostName();
    IPHostEntry ipEntry = Dns.GetHostEntry(strHostName);

    foreach (var ipAddress in ipEntry.AddressList)
    {
        if (ipAddress.AddressFamily == AddressFamily.InterNetwork)
        {
            return ipAddress.ToString();
        }
    }

    return "";
}
