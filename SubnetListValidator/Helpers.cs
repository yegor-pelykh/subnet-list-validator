using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace SubnetListValidator
{
    internal static class Helpers
    {
        internal static ConsoleKeyInfo ReadKey(ConsoleColor? color = null)
        {
            if (color == null)
                return Console.ReadKey();
            else
            {
                var oldColor = Console.ForegroundColor;
                Console.ForegroundColor = color.Value;
                var key = Console.ReadKey();
                Console.ForegroundColor = oldColor;
                return key;
            }
        }

        internal static void PrintMessage(string message, ConsoleColor? color = null)
        {
            if (color == null)
                Console.Write(message);
            else
            {
                var oldColor = Console.ForegroundColor;
                Console.ForegroundColor = color.Value;
                Console.Write(message);
                Console.ForegroundColor = oldColor;
            }
        }

        internal static void PrintWaitForKey()
        {
            PrintMessage("Press any key to exit...\n");
            ReadKey();
        }

        internal static string? GetPathFromFileName(string? fileName)
        {
            if (fileName != null)
            {
                try
                {
                    var filePath = Path.GetFullPath(fileName);
                    if (File.Exists(filePath))
                        return filePath;
                }
                catch (Exception) { }
            }
            return null;
        }

        internal static string LetEnterFilePath(ConsoleColor? color = null)
        {
            while (true)
            {
                PrintMessage("Please enter the file path: ");
                var filePath = GetPathFromFileName(Console.ReadLine());
                if (filePath != null)
                    return filePath;
                PrintMessage("Wrong file path. Please enter the correct one.\n", ConsoleColor.Red);
            }
        }

        internal static int LetChooseBetween(IPNetwork[] array)
        {
            PrintMessage("Please select the subnet you want to keep:\n", ConsoleColor.Yellow);
            for (int i = 0; i < array.Length; i++)
            {
                IPNetwork item = array[i];
                PrintMessage($"{i + 1}: ", ConsoleColor.Blue);
                PrintMessage($"{item}\n", ConsoleColor.Yellow);
            }
            while (true)
            {
                PrintMessage("Enter a sequence number: ", ConsoleColor.Yellow);
                var key = ReadKey(ConsoleColor.Blue);
                if (int.TryParse(key.KeyChar.ToString(), out var index))
                {
                    index--;
                    if (0 <= index && index < array.Length)
                    {
                        PrintMessage($"\nNetwork {array[index]} will remain in the list.\n", ConsoleColor.DarkGreen);
                        return index;
                    }
                }
                PrintMessage("\nWrong index. Please choose the correct one.\n", ConsoleColor.Red);
            }
        }

        internal static List<IPNetwork>? GetNetworksFromFile(string fileName)
        {
            var lines = File.ReadAllLines(fileName);
            var networks = new List<IPNetwork>();
            for (var i = 0; i < lines.Length; i++)
            {
                var line = lines[i];
                if (!IPNetwork.TryParse(line, out var network))
                {
                    PrintMessage($"Error parsing subnet data from line {i}.\n", ConsoleColor.Red);
                    return null;
                }
                networks.Add(network);
            }
            return networks;
        }

        internal static string NetworkToString(IPNetwork network)
        {
            return network.AddressFamily == System.Net.Sockets.AddressFamily.InterNetworkV6
                ? network.ToString()
                : $"{network.Network} {network.Netmask}";
        }

        internal static void WriteNetworksToFile(string fileName, List<IPNetwork> networks)
        {
            var lines = networks.Select(n => NetworkToString(n));
            File.WriteAllLines(fileName, lines);
        }
    }
}
