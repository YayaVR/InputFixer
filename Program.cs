using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using FastOSC;
using System.Threading;
using Valve.VR;

namespace InputFixer // By Yayabites
{
    class Program
    {
        private static OSCSender sender = new OSCSender();
        private static IPEndPoint vrChatEndpoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 9000);  // VRChat OSC Port
        private static bool movingForward = false;
        private static bool movingBackward = false;
        private static bool loggedForward = false; // Flag to track logging of forward movement
        private static bool loggedBackward = false; // Flag to track logging of backward movement
        private static double threshold = 0.75; // Default threshold value

        private static bool isMenuOpen = false; // Track if SteamVR menu is open
        private static string configFilePath = "InputFixer_config.txt"; // Path to the config file

        static async Task Main(string[] args)
        {
            Console.WriteLine("//InputFixer by Yayabites//");
            // Check if the config file exists, if not create it with default threshold value
            if (!File.Exists(configFilePath))
            {
                CreateConfigFile();
            }
            else
            {
                // Read the threshold value from the config file
                threshold = ReadThresholdFromConfig();
            }

            // Initialize OSC and connect to VRChat
            await sender.ConnectAsync(vrChatEndpoint);
            Console.WriteLine("Connected to VRChat OSC at 127.0.0.1:9000");

            EVRInitError initError = EVRInitError.None;
            OpenVR.Init(ref initError, EVRApplicationType.VRApplication_Background);

            if (initError != EVRInitError.None)
            {
                Console.WriteLine("Failed to initialize OpenVR: " + initError);
                return;
            }
            Console.WriteLine("OpenVR initialized successfully.");

            CVRSystem vrSystem = OpenVR.System;
            if (vrSystem == null)
            {
                Console.WriteLine("Failed to get VR system.");
                return;
            }

            uint leftControllerIndex = OpenVR.System.GetTrackedDeviceIndexForControllerRole(ETrackedControllerRole.LeftHand);

            while (leftControllerIndex == OpenVR.k_unTrackedDeviceIndexInvalid)
            {
                Console.WriteLine("Waiting for left controller...\n(Trying again in 10sec)");
                await Task.Delay(10000); // Async sleep before looking for controller again
                leftControllerIndex = OpenVR.System.GetTrackedDeviceIndexForControllerRole(ETrackedControllerRole.LeftHand);
            }

            Console.WriteLine($"Left controller found at index {leftControllerIndex}");

            while (true)
            {
                if (vrSystem.IsTrackedDeviceConnected(leftControllerIndex))
                {
                    VRControllerState_t controllerState = new VRControllerState_t();

                    try
                    {
                        // Try to get the controller state
                        if (vrSystem.GetControllerState(leftControllerIndex, ref controllerState, (uint)System.Runtime.InteropServices.Marshal.SizeOf(typeof(VRControllerState_t))))
                        {
                            if (isMenuOpen == true)
                            {
                                Console.WriteLine("SteamVR Menu closed, Controller State retrieved.");
                                isMenuOpen = false;
                            }
                            float verticalMovement = controllerState.rAxis0.y;

                            // Positive Tilt Detection
                            if (verticalMovement > threshold && !movingForward)
                            {
                                movingForward = true;
                                if (!loggedForward)
                                {
                                    Console.WriteLine("Positive Tilt detected, working as intended.");
                                    loggedForward = true;
                                }
                                SendMovementInput("/input/MoveForward", 1); 
                                await Task.Delay(10); // Delay after sending OSC message
                            }
                            else if (verticalMovement <= threshold && movingForward)
                            {
                                movingForward = false;
                                SendMovementInput("/input/MoveForward", 0);  
                            }

                            // Negative Tilt Detection
                            if (verticalMovement < -threshold && !movingBackward)
                            {
                                movingBackward = true;
                                if (!loggedBackward)
                                {
                                    Console.WriteLine("Negative Tilt detected, working as intended.");
                                    loggedBackward = true;
                                }
                                SendMovementInput("/input/MoveBackward", 1);  
                                await Task.Delay(10); // Delay after sending OSC message
                            }
                            else if (verticalMovement >= -threshold && movingBackward)
                            {
                                movingBackward = false;
                                SendMovementInput("/input/MoveBackward", 0);  
                            }
                        }
                        else
                        {
                            if (isMenuOpen == false)
                            {
                                Console.WriteLine("Failed to get controller state, likely because SteamVR Menu is open");
                                isMenuOpen = true;
                            }
                            await Task.Delay(500);  // Retry after a delay
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error getting controller state: {ex.Message}");
                    }
                }
                else
                {
                    Console.WriteLine("Left controller is not connected.");
                    break;
                }

                await Task.Delay(10); // Delay after each loop iteration
            }
        }

        static void SendMovementInput(string address, int value)
        {
            var message = new OSCMessage(address, value);
            sender.Send(message); // Synchronous send, no need for async
        }

        // Creates the config file with the default threshold value
        static void CreateConfigFile()
        {
            Console.WriteLine("Config file not found. Creating default config...\nYou can edit the Activation-threshold within this file.");
            using (StreamWriter writer = new StreamWriter(configFilePath))
            {
                writer.WriteLine("Threshold=0.75"); // Default value
            }
        }

        // Reads the threshold value from the config file
        static double ReadThresholdFromConfig()
        {
            try
            {
                string[] configLines = File.ReadAllLines(configFilePath);
                foreach (string line in configLines)
                {
                    if (line.StartsWith("Threshold="))
                    {
                        string thresholdValue = line.Substring("Threshold=".Length);
                        if (double.TryParse(thresholdValue, out double parsedThreshold))
                        {
                            Console.WriteLine($"Threshold value loaded: {parsedThreshold}");
                            return parsedThreshold;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error reading config file: {ex.Message}");
            }
            return 0.75; // Return default if reading fails
        }
    }
}
