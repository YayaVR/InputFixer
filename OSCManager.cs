using FastOSC;
using System.Net;

namespace InputFixer
{
    public static class OSCManager
    {
        private static OSCSender sender = new OSCSender();
        private static IPEndPoint vrChatEndpoint = new IPEndPoint(System.Net.IPAddress.Parse("127.0.0.1"), 9000);  // VRChat OSC Port

        public static void Initialize()
        {
            sender.ConnectAsync(vrChatEndpoint).Wait();
            Console.WriteLine("Connected to VRChat OSC at 127.0.0.1:9000");
        }

        public static void SendMovementInput(string address, float value)
        {
            var message = new OSCMessage(address, value);
            sender.Send(message);
        }
    }
}
