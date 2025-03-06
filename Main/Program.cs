
Utility utility = new Utility();

string sourceFolder = "";
string destinationFolder = "";
string workOnSubfolders = "";

while(!Directory.Exists(sourceFolder))
{
    Console.WriteLine("Inserisci il percorso della cartella da comprimere:");
    sourceFolder = Console.ReadLine();
}

while (!Directory.Exists(destinationFolder))
{
    Console.WriteLine("Inserisci il percorso della cartella di destinazione:");
    destinationFolder = Console.ReadLine();
}

while (workOnSubfolders?.Trim() != "1" && workOnSubfolders?.Trim() != "2")
{
    Console.WriteLine("Comprimo l'intera cartella (1) o le singole cartelle al suo interno(2)? (1,2):");
    workOnSubfolders = Console.ReadLine();
}

utility.OnNewMessage += (sender, message) =>
{
    Console.WriteLine(message);
};

if(workOnSubfolders == "1")
{
    try
    {
        utility.CompressAndConcatenate(sourceFolder, destinationFolder);
    }
    catch (Exception ex)
    {
        Console.WriteLine(ex.Message);
    }
    finally
    {
        Console.WriteLine();
        Console.WriteLine("Premi un tasto per uscire...");
        Console.ReadKey();
    }
}
else
{
    try
    {
        utility.CompressAndConcatenateSubfolders(sourceFolder, destinationFolder);
    }
    catch (Exception ex)
    {
        Console.WriteLine(ex.Message);
    }
    finally
    {
        Console.WriteLine();
        Console.WriteLine("Premi un tasto per uscire...");
        Console.ReadKey();
    }
}