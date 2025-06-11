using System;
using System.IO;
using System.Collections.Generic;

class Program
{
    static void Main(string[] args)
    {
        Utility utility = new Utility();
        utility.OnNewMessage += (sender, message) => Console.WriteLine(message);

        string sourceFolder = "";
        string destinationFolder = "";
        string mode = "";

        while (!Directory.Exists(sourceFolder))
        {
            Console.Write("\nInserisci il percorso della cartella da processare: ");
            sourceFolder = Console.ReadLine();
        }

        while (!Directory.Exists(destinationFolder))
        {
            Console.Write("\nInserisci il percorso della cartella di destinazione: ");
            destinationFolder = Console.ReadLine();
        }

        while (mode != "1" && mode != "2" && mode != "3")
        {
            Console.WriteLine("\nScegli un'opzione:");
            Console.WriteLine("1. Comprimo l’intera cartella");
            Console.WriteLine("2. Comprimo le singole cartelle al suo interno");
            Console.WriteLine("3. Comprimo solo le cartelle modificate dopo una certa data");
            mode = Console.ReadLine();
        }

        try
        {
            if (mode == "1")
            {
                utility.CompressAndConcatenate(sourceFolder, destinationFolder);
            }
            else if (mode == "2")
            {
                utility.CompressAndConcatenateSubfolders(sourceFolder, destinationFolder);
            }
            else if (mode == "3")
            {
                Console.Write("Inserisci la data di riferimento (es. aaaa-mm-dd): ");
                if (!DateTime.TryParse(Console.ReadLine(), out DateTime cutoffDate))
                {
                    Console.WriteLine("[ERRORE] Data non valida.");
                    return;
                }

                var modifiedFolders = GetModifiedFolders(sourceFolder, cutoffDate);

                if (modifiedFolders.Count == 0)
                {
                    Console.WriteLine("[INFO] Nessuna cartella modificata trovata.");
                }
                else
                {
                    Console.WriteLine($"\n[INFO] Trovate {modifiedFolders.Count} cartelle modificate.");
                    foreach (var folder in modifiedFolders)
                    {
                        try
                        {
                            Console.WriteLine($"\n[INFO] Comprimendo: {folder}");
                            utility.CompressAndConcatenate(folder, destinationFolder);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"[ERRORE] Compressione fallita per {folder}: {ex.Message}");
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[ERRORE] {ex.Message}");
        }
        finally
        {
            Console.WriteLine("\nPremi un tasto per uscire...");
            Console.ReadKey();
        }
    }

    static List<string> GetModifiedFolders(string rootFolder, DateTime cutoffDate)
    {
        List<string> modifiedFolders = new List<string>();
        string[] topLevelFolders = Directory.GetDirectories(rootFolder);

        foreach (string topLevelFolder in topLevelFolders)
        {
            try
            {
                Console.WriteLine($"[INFO] Analizzo: {topLevelFolder}");
                if (FolderHasRecentChanges(topLevelFolder, cutoffDate))
                {
                    modifiedFolders.Add(topLevelFolder);
                    Console.WriteLine($"[MODIFICATA] {topLevelFolder}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERRORE] Impossibile analizzare {topLevelFolder}: {ex.Message}");
            }
        }

        return modifiedFolders;
    }

    static bool FolderHasRecentChanges(string folderPath, DateTime cutoffDate)
    {
        // Controlla cartella stessa
        DateTime created = Directory.GetCreationTime(folderPath);
        DateTime modified = Directory.GetLastWriteTime(folderPath);
        if (created > cutoffDate || modified > cutoffDate) 
            return true;

        // Controlla file
        foreach (string file in Directory.EnumerateFiles(folderPath, "*.*", SearchOption.AllDirectories))
        {
            try
            {
                DateTime cDate = File.GetCreationTime(file);
                DateTime mDate = File.GetLastWriteTime(file);
                if (cDate > cutoffDate || mDate > cutoffDate) 
                    return true;
            }
            catch { }
        }

        // Controlla sottocartelle
        foreach (string dir in Directory.EnumerateDirectories(folderPath, "*", SearchOption.AllDirectories))
        {
            try
            {
                DateTime cDate = Directory.GetCreationTime(dir);
                DateTime mDate = Directory.GetLastWriteTime(dir);
                if (cDate > cutoffDate || mDate > cutoffDate) 
                    return true;
            }
            catch { }
        }

        return false;
    }
}
