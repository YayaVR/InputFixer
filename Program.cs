using System;
using System.Threading.Tasks;
using Valve.VR;

namespace InputFixer
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("//InputFixer by Yayabites//");

            // Initialize configuration
            ConfigManager.InitializeConfig();

            // Initialize OSC and connect to VRChat
            OSCManager.Initialize();

            // Initialize VR system
            VRControllerManager.Initialize();

            // Main loop to process controller input
            await VRControllerManager.ProcessControllerInput();
        }
    }
}
