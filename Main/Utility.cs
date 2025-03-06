using System.Diagnostics;

public class Utility
{
    public event EventHandler<string> OnNewMessage;

    public void CompressAndConcatenate(string sourceFolder, string destinationFolder)
    {
        if (!Directory.Exists(sourceFolder))
        {
            throw new Exception("La cartella specificata non esiste.");
        }

        if (!Directory.Exists(destinationFolder))
        {
            throw new Exception("La cartella di destinazione non esiste.");
        }

        string folderName = new DirectoryInfo(sourceFolder).Name;
        string compressedFile = Path.Combine(destinationFolder, folderName + ".7z");
        string imageFile = "Resources/image.png"; // Immagine base
        string outputFile = Path.Combine(destinationFolder, folderName + ".png");
        string sevenZipPath = "C:\\Program Files\\7-Zip\\7z.exe";

        if (!File.Exists(sevenZipPath))
        {
            throw new Exception("7-Zip non trovato nel percorso predefinito.");
        }

        if (!File.Exists(imageFile))
        {
            throw new Exception($"File immagine '{imageFile}' non trovato.");
        }

        OnNewMessage?.Invoke(this, "Step 1 di 4: Compressione in corso...");
        // Compressione con 7-Zip
        ProcessStartInfo psi = new ProcessStartInfo
        {
            FileName = sevenZipPath,
            Arguments = $"a -t7z \"{compressedFile}\" \"{sourceFolder}\" -mx=0",
            RedirectStandardOutput = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        using (Process process = Process.Start(psi))
        {
            process.WaitForExit();
        }

        if (!File.Exists(compressedFile))
        {
            throw new Exception("Errore nella compressione della cartella.");
        }

        OnNewMessage?.Invoke(this, "Step 2 di 4: Concatenazione con immagine in corso...");
        // Concatenazione con copy /b
        string copyCommand = $"/c copy /b \"{imageFile}\"+\"{compressedFile}\" \"{outputFile}\"";
        ProcessStartInfo copyPsi = new ProcessStartInfo("cmd.exe", copyCommand)
        {
            RedirectStandardOutput = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        using (Process copyProcess = Process.Start(copyPsi))
        {
            copyProcess.WaitForExit();
        }

        OnNewMessage?.Invoke(this, "Step 3 di 4: Verifica del file immagine con 7z...");

        // Verifica del file 7z utilizzando 7-Zip
        ProcessStartInfo verifyPsi = new ProcessStartInfo
        {
            FileName = sevenZipPath,
            Arguments = $"t \"{outputFile}\"",
            RedirectStandardOutput = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        using (Process verifyProcess = Process.Start(verifyPsi))
        {
            verifyProcess.WaitForExit();
            string output = verifyProcess.StandardOutput.ReadToEnd();
            if (!output.Contains("Everything is Ok"))
            {
                throw new Exception("Errore: Il file compresso non è valido.");
            }
        }

        OnNewMessage?.Invoke(this, "Step 4 di 4: Elimino il file 7z di transizione...");
        // Eliminazione del file 7z dopo la concatenazione
        if (File.Exists(compressedFile))
        {
            File.Delete(compressedFile);
        }

        OnNewMessage?.Invoke(this, "Operazione completata.");
    }

    public void CompressAndConcatenateSubfolders(string sourceFolder, string destinationFolder)
    {
        var subFolders = Directory.GetDirectories(sourceFolder);

        foreach (var subFolder in subFolders)
        {
            CompressAndConcatenate(subFolder, destinationFolder);
        }
    }
}
