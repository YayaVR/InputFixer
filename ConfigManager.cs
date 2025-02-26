using System;
using System.IO;

namespace InputFixer
{
    public static class ConfigManager
    {
        private static string configFilePath = "InputFixer_config.txt"; // Path to the config file
        public static double threshold = 0.75; // Default threshold value

        public static void InitializeConfig()
        {
            if (!File.Exists(configFilePath))
            {
                CreateConfigFile();
            }
            else
            {
                threshold = ReadThresholdFromConfig();
            }
        }

        // Creates the config file with the default threshold value
        private static void CreateConfigFile()
        {
            Console.WriteLine("Config file not found. Creating default config...\nYou can edit the Activation-threshold within this file.");
            using (StreamWriter writer = new StreamWriter(configFilePath))
            {
                writer.WriteLine("Threshold=0.75"); // Default value
            }
        }

        // Reads the threshold value from the config file
        private static double ReadThresholdFromConfig()
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
