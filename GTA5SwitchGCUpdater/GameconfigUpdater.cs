using System;
using System.IO;
using System.Linq;
using CodeWalker.Core.Utils;
using CodeWalker.GameFiles;

namespace GTA5SwitchGCUpdater
{
    public class GameconfigUpdater
    {
        private const string GAMECONFIG_PATH = "common/data/gameconfig.xml";

        public void UpdateGameconfig(string updateRpfPath, string newGameconfigPath, string outputRpfPath)
        {
            if (!File.Exists(updateRpfPath))
                throw new FileNotFoundException($"update.rpf not found: {updateRpfPath}");

            if (!File.Exists(newGameconfigPath))
                throw new FileNotFoundException($"gameconfig.xml not found: {newGameconfigPath}");

            if (Path.GetFullPath(updateRpfPath) == Path.GetFullPath(outputRpfPath))
                throw new InvalidOperationException("Output RPF cannot be the same as the source RPF.");

            try
            {
                RpfFile sourceRpfFile = new RpfFile(updateRpfPath, "");
                sourceRpfFile.ScanStructure((s) => { }, (s) => { });

                var sourceEntry = FindGameconfigEntry(sourceRpfFile.Root);
                if (sourceEntry == null)
                    throw new FileNotFoundException($"gameconfig.xml not found in RPF at path: {GAMECONFIG_PATH}");

                File.Copy(updateRpfPath, outputRpfPath, overwrite: true);

                RpfFile rpfFile = new RpfFile(outputRpfPath, "");
                rpfFile.ScanStructure((s) => { }, (s) => { });

                var gameconfigEntry = FindGameconfigEntry(rpfFile.Root);
                if (gameconfigEntry == null)
                {
                    CleanupOutput(outputRpfPath);
                    throw new FileNotFoundException($"gameconfig.xml not found in RPF at path: {GAMECONFIG_PATH}");
                }

                byte[] newGameconfigData = File.ReadAllBytes(newGameconfigPath);

                var parent = gameconfigEntry.Parent;
                RpfFile.CreateFile(parent, gameconfigEntry.Name, newGameconfigData, overwrite: true);
            }
            catch (Exception ex)
            {
                CleanupOutput(outputRpfPath);
                throw new InvalidOperationException($"Failed to update RPF: {ex.Message}", ex);
            }
        }

        private static void CleanupOutput(string outputRpfPath)
        {
            try
            {
                if (File.Exists(outputRpfPath))
                    File.Delete(outputRpfPath);
            }
            catch
            {
                //
            }
        }

        private RpfFileEntry? FindGameconfigEntry(RpfDirectoryEntry? directory, int depth = 0)
        {
            if (directory == null)
                return null;

            if (depth > 50)
                return null;

            if (directory.Files != null)
            {
                var gameconfigFile = directory.Files.FirstOrDefault(f => 
                    f.Name.Equals("gameconfig.xml", StringComparison.OrdinalIgnoreCase));
                
                if (gameconfigFile != null)
                {
                    return gameconfigFile;
                }
            }

            if (directory.Directories != null)
            {
                var commonDir = directory.Directories.FirstOrDefault(d => 
                    d.Name.Equals("common", StringComparison.OrdinalIgnoreCase));
                
                if (commonDir != null)
                {
                    var dataDir = commonDir.Directories?.FirstOrDefault(d => 
                        d.Name.Equals("data", StringComparison.OrdinalIgnoreCase));
                    
                    if (dataDir != null)
                    {
                        var result = FindGameconfigEntry(dataDir, depth + 1);
                        if (result != null)
                            return result;
                    }
                }

                foreach (var subdir in directory.Directories)
                {
                    var result = FindGameconfigEntry(subdir, depth + 1);
                    if (result != null)
                        return result;
                }
            }

            return null;
        }
    }
}