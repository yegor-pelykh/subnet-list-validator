using SubnetListValidator;
using System.Net;
using Whois.NET;

Console.ForegroundColor = ConsoleColor.Gray;

var filePath = Helpers.GetPathFromFileName(args.FirstOrDefault());
if (string.IsNullOrEmpty(filePath))
    filePath = Helpers.LetEnterFilePath(ConsoleColor.Yellow);

Helpers.PrintMessage($"File: {filePath}\n", ConsoleColor.Green);

List<IPNetwork>? networks = null;
try
{
    networks = Helpers.GetNetworksFromFile(filePath);
}
catch (Exception ex)
{
    Helpers.PrintMessage($"Error loading subnet list from file: {ex.Message}");
    Helpers.PrintWaitForKey();
    return;
}

if (networks == null)
{
    Helpers.PrintMessage("There is no correct list of subnets for the application to work.\n", ConsoleColor.Red);
    Helpers.PrintWaitForKey();
    return;
}

var networksToDelete = new List<IPNetwork>();
for (int i = 0; i < networks.Count; i++)
{
    for (int j = 0; j < networks.Count; j++)
    {
        if (j != i && networks[i].Contains(networks[j]))
        {
            var infoi = WhoisClient.Query(networks[i].FirstUsable.ToString());
            var orgi = infoi != null && !string.IsNullOrEmpty(infoi.OrganizationName)
                ? infoi.OrganizationName
                : "[No organization info]";
            var infoj = WhoisClient.Query(networks[j].FirstUsable.ToString());
            var orgj = infoj != null && !string.IsNullOrEmpty(infoj.OrganizationName)
                ? infoj.OrganizationName
                : "[No organization info]";

            Helpers.PrintMessage($"Network {networks[i]} ({orgi}) contains {networks[j]} ({orgj}).\n", ConsoleColor.DarkYellow);
            var chosenNetworkIndex = Helpers.LetChooseBetween(new[] { networks[i], networks[j] });
            networksToDelete.Add(chosenNetworkIndex == 0
                ? networks[j]
                : networks[i]);
        }
    }
}

networks.RemoveAll(n => networksToDelete.Contains(n));

try
{
    Helpers.WriteNetworksToFile(filePath, networks);
}
catch (Exception ex)
{
    Helpers.PrintMessage($"Error saving subnet list to file: {ex.Message}");
    Helpers.PrintWaitForKey();
    return;
}

Helpers.PrintMessage("Done\n", ConsoleColor.Green);
Helpers.PrintWaitForKey();