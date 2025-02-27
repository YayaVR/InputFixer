using Valve.VR;
using System;
using System.Threading.Tasks;

namespace InputFixer
{
    public static class VRControllerManager
    {
        private static double threshold = ConfigManager.threshold;
        private static double minActivation = 0.05; // Min activation to avoid load while stick is idle
        private static bool isMenuOpen = false; // Track if SteamVR menu is open
        private static CVRSystem vrSystem;

        public static void Initialize()
        {
            EVRInitError initError = EVRInitError.None;
            OpenVR.Init(ref initError, EVRApplicationType.VRApplication_Background);

            if (initError != EVRInitError.None)
            {
                Console.WriteLine("Failed to initialize OpenVR: " + initError);
                return;
            }
            Console.WriteLine("OpenVR initialized successfully.");

            vrSystem = OpenVR.System;
            if (vrSystem == null)
            {
                Console.WriteLine("Failed to get VR system.");
            }
        }

        public static async Task ProcessControllerInput()
        {
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
                        if (vrSystem.GetControllerState(leftControllerIndex, ref controllerState, (uint)System.Runtime.InteropServices.Marshal.SizeOf(typeof(VRControllerState_t))))
                        {
                            if (isMenuOpen == true)
                            {
                                Console.WriteLine("SteamVR Menu closed, Controller State retrieved.");
                                isMenuOpen = false;
                            }
                            ProcessMovement(controllerState, controllerState.rAxis0.y, controllerState.rAxis0.x);
                        }
                        else
                        {
                            OSCManager.SendMovementInput("/input/Vertical", 0);     // Stop movement if menu is opem
                            OSCManager.SendMovementInput("/input/Horizontal", 0);
                            if (isMenuOpen == false)
                            {
                                Console.WriteLine("Failed to get controller state, likely because SteamVR Menu is open");
                            }
                            isMenuOpen = true;
                            await Task.Delay(1000);  // Retry after a delay
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

        private static void ProcessMovement(VRControllerState_t controllerState, float originalY, float originalX)
        {

            // Tilt Detection Y-Axis
            if (Math.Abs(originalY) > minActivation)
            {
                // Linearly scale the Y-axis input based on threshold, preserve sign
                double verticalValue = Math.Sign(originalY) * (Math.Abs(originalY) / threshold);

                // Ensure the value is capped between -1 and 1
                verticalValue = Math.Clamp(verticalValue, -1, 1);

                OSCManager.SendMovementInput("/input/Vertical", (float)verticalValue);
            }
            else if (Math.Abs(originalY) <= minActivation)
            {
                OSCManager.SendMovementInput("/input/Vertical", 0);
            }

            // Tilt Detection X-Axis
            if (Math.Abs(originalX) > minActivation)
            {
                // Linearly scale the X-axis input based on threshold, preserve sign
                double horizontalValue = Math.Sign(originalX) * (Math.Abs(originalX) / threshold);

                // Ensure the value is capped between -1 and 1
                horizontalValue = Math.Clamp(horizontalValue, -1, 1);

                OSCManager.SendMovementInput("/input/Horizontal", (float)horizontalValue);
            }
            else if (Math.Abs(originalX) <= minActivation)
            {
                OSCManager.SendMovementInput("/input/Horizontal", 0);
            }
        }




    }
}
