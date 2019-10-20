﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using Minotaur.Core.Anonymous;

namespace Minotaur.Core
{
    public static class FileExtensions
    {
        private static readonly string lockFileContent = $"{Environment.MachineName}:{Process.GetCurrentProcess().Id}";

        public static string CreateFolderIfNotExist(this string path)
        {
            if (string.IsNullOrEmpty(path)) return path;

            try
            {
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);
            }
            catch (Exception)
            {
                // Todo: log error here
            }

            return path;
        }

        public static string GetFolderPath(this string filePath)
        {
            if (string.IsNullOrEmpty(filePath)) return filePath;

            return new FileInfo(filePath).Directory?.FullName;
        }

        public static bool FileSpinWait(this string filePath, int timeout = -1)
        {
            while (filePath.IsFileLocked() || timeout-- > 0)
                Thread.Sleep(1);

            return !filePath.IsFileLocked();
        }

        private static bool IsFileLocked(this string filePath)
        {
            if (string.IsNullOrEmpty(filePath)) return false;

            var lockFile = filePath.GetFileLock();
            return File.Exists(lockFile) && File.ReadAllText(lockFile) != lockFileContent;
        }

        private static string GetFileLock(this string filePath)
        {
            if (string.IsNullOrEmpty(filePath)) return null;
            return filePath + ".lock";
        }

        // Todo: Lock the file when it's write as when it's read => 2 modes
        // Todo: the goal is to supports many reads but only 1 write
        public static IDisposable FileLock(this string filePath)
        {
            if (!filePath.FileSpinWait()) return AnonymousDisposable.Empty;

            var lockFilePath = filePath.GetFileLock();
            try
            {
                File.WriteAllText(lockFilePath, lockFileContent);
            }
            catch (Exception)
            {
                // Todo: log error here
            }

            return new AnonymousDisposable(() =>
            {
                try
                {
                    File.Delete(lockFilePath);
                }
                catch (Exception)
                {
                    // Todo: log error here
                }
            });
        }

        public static bool FileExists(this string filePath)
            => !string.IsNullOrEmpty(filePath) && File.Exists(filePath);

        public static IEnumerable<string> MoveToTmpFiles(this IEnumerable<string> files)
            => files.Select(MoveToTmpFile);

        public static string MoveToTmpFile(this string filePath)
        {
            if (!filePath.FileExists()) return filePath;

            try
            {
                var tmpFile = filePath + ".tmp";
                File.Move(filePath, tmpFile);
                return tmpFile;
            }
            catch (Exception)
            {
                // Todo: Log here
            }

            return filePath;
        }

        public static bool DeleteFile(this string filePath)
        {
            if (!filePath.FileExists()) return true;

            try
            {
                File.Delete(filePath);
                return true;
            }
            catch (Exception)
            {
                // Todo: Log here
                return false;
            }
        }

        public static bool FolderExists(this string path)
            => !string.IsNullOrEmpty(path) && Directory.Exists(path);

        public static bool DeleteFolder(this string path)
        {
            if (!path.FolderExists()) return true;

            try
            {
                Directory.Delete(path, true);
                return true;
            }
            catch (Exception)
            {
                // Todo: Log here
                return false;
            }
        }
    }
}
