using System;
using System.IO;
using ICSharpCode.SharpZipLib.Zip;

namespace Memos.Framework
{
    /// <summary>
    /// ZIP manager.
    /// </summary>
    public class ZipManager
    {
        #region GenerateZipFile
        /// <summary>
        /// Generate zip file with single file.
        /// </summary>
        /// <param name="fileInfo"></param>
        /// <param name="fileName"></param>
        /// <returns>Path to the generated file.</returns>
        public static string GenerateZipFile(FileInfo fileInfo, string fileName)
        {
            string zipFile = fileName;

            using (ZipOutputStream zipOutputStream = new ZipOutputStream(File.Create(zipFile)))
            {
                // If fileName is null or empty - use name of the file
                if (String.IsNullOrEmpty(fileName))
                {
                    fileName = fileInfo.Name;
                }

                AddFileToZipStream(fileName, fileInfo.FullName, zipOutputStream);
                zipOutputStream.Finish();
            }

            return zipFile;
        }
        #endregion

        #region GenerateZipFile
        /// <summary>
        /// Generate zip file with selected files.
        /// </summary>
        /// <param name="selectedFiles"></param>
        /// <returns>Path to the generated file.</returns>
        public static string GenerateZipFile(FileInfo[] selectedFiles, string filename)
        {
            string zipFile = filename;

            using (ZipOutputStream zipOutputStream = new ZipOutputStream(File.Create(zipFile)))
            {
                foreach (FileInfo fileInfo in selectedFiles)
                {
                    AddFileToZipStream(fileInfo.Name, fileInfo.FullName, zipOutputStream);
                }

                zipOutputStream.Finish();
            }

            return zipFile;
        }
        #endregion

        #region CreateZipFile
        /// <summary>
        /// Create unique ZIP file.
        /// </summary>
        /// <returns></returns>
        public static string CreateZipFile()
        {
            string fileName = String.Concat(Guid.NewGuid(), ".zip");
            string zipFile = Path.Combine(Utilities.TempDirectory, fileName);

            return zipFile;
        }
        #endregion

        #region AddFileToZipStream
        /// <summary>
        /// Add file to ZIP stream.
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="filePath"></param>
        /// <param name="zipOutputStream"></param>
        /// <returns></returns>
        public static void AddFileToZipStream(string fileName, string filePath, ZipOutputStream zipOutputStream)
        {
            ZipEntry zipEntry = new ZipEntry(fileName);

            using (FileStream fileStream = File.OpenRead(filePath))
            {
                byte[] buff = new byte[Convert.ToInt32(fileStream.Length)];
                fileStream.Read(buff, 0, (int)fileStream.Length);
                zipEntry.Size = fileStream.Length;

                zipOutputStream.PutNextEntry(zipEntry);
                zipOutputStream.Write(buff, 0, buff.Length);
            }
        }
        #endregion
    }
}