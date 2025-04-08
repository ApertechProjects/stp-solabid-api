using Microsoft.AspNetCore.StaticFiles;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace SolaBid.Extensions
{
    public static class FileExtensions
    {
        public async static Task<string> SaveFileBase64(this string file, string fileName, string subfolder, string root)
        {

            string filename = Guid.NewGuid().ToString() + "-" + fileName;
            string parentDirectoryPath = Path.Combine(root, "appfiles");
            string documentFilePath = Path.Combine(parentDirectoryPath, subfolder);
            string document = Path.Combine(parentDirectoryPath, subfolder, filename).Replace("%20", " ");

            if (!Directory.Exists(parentDirectoryPath))
                Directory.CreateDirectory(parentDirectoryPath);

            if (!Directory.Exists(documentFilePath))
                Directory.CreateDirectory(documentFilePath);

            var fileBytes = ConvertBase64ToByte(file);
            await File.WriteAllBytesAsync(document, fileBytes);

            return Path.Combine(subfolder, filename);
        }
        public static byte[] ConvertBase64ToByte(this string file)
        {
            byte[] srcByte = Convert.FromBase64String(file.Substring(file.IndexOf(";base64,") + 8));
            return srcByte;
        }
        public static string GetFileContentType(this string filePath)
        {
            var provider = new FileExtensionContentTypeProvider();
            if (!provider.TryGetContentType(filePath, out var contentType))
                contentType = "application/octet-stream";
            return contentType;
        }
        public static string ConvertFileToBase64(string filePathName, string subFolder, string fileBaseType = "data:image/jpeg;base64,") //Subfolder only wwwroot after folder name.Default file base type is image
        {
            try
            {
                if (filePathName == null)
                {
                    return string.Empty;
                }
                else
                {
                    string fullFilePath = Path.Combine("wwwroot", subFolder);
                    var fileWithBytes = File.ReadAllBytes(Path.Combine(fullFilePath, filePathName));
                    string base64String = Convert.ToBase64String(fileWithBytes, 0, fileWithBytes.Length);
                    base64String = fileBaseType + base64String;
                    return base64String;
                }

            }
            catch (Exception)
            {
                return string.Empty;
            }
        }

        public static void DeleteFile(string root, string subFolder, string fileName)
        {
            try
            {
                var fullPath = Path.Combine(root, subFolder, fileName);
                if (File.Exists(fullPath))
                {
                    File.Delete(fullPath);
                }
            }
            catch (Exception)
            {

            }

        }

        public async static Task<string> SaveBIDAttachment(this string file, string fileName, string root, int bidId)
        {
            string filename = Guid.NewGuid().ToString().Substring(0, 5) + "-" + fileName;
            string parentDirectoryPath = Path.Combine(root, "appfiles");
            string parentSubDirectoryPath = Path.Combine(parentDirectoryPath, "BIDDocs");
            string documentFilePath = Path.Combine(parentSubDirectoryPath, bidId.ToString());
            string document = Path.Combine(root, "appfiles", "BIDDocs", bidId.ToString(), filename).Replace("%20", " ");

            if (!Directory.Exists(parentDirectoryPath))
                Directory.CreateDirectory(parentDirectoryPath);

            if (!Directory.Exists(parentSubDirectoryPath))
                Directory.CreateDirectory(parentSubDirectoryPath);

            if (!Directory.Exists(documentFilePath))
                Directory.CreateDirectory(documentFilePath);

            var fileBytes = ConvertBase64ToByte(file);
            await File.WriteAllBytesAsync(document, fileBytes);
            return Path.Combine("BIDDocs", bidId.ToString(), filename);
        }

        public async static Task<string> SaveVendorAttachment(this string file, string fileName, string root, int vendorId)
        {
            string filename = Guid.NewGuid().ToString().Substring(0, 5) + "-" + fileName;
            string parentDirectoryPath = Path.Combine(root, "appfiles");
            string parentSubDirectoryPath = Path.Combine(parentDirectoryPath, "VendorDocs");
            string documentFilePath = Path.Combine(parentSubDirectoryPath, vendorId.ToString());
            string document = Path.Combine(root, "appfiles", "VendorDocs", vendorId.ToString(), filename).Replace("%20", " ");

            if (!Directory.Exists(parentDirectoryPath))
                Directory.CreateDirectory(parentDirectoryPath);

            if (!Directory.Exists(parentSubDirectoryPath))
                Directory.CreateDirectory(parentSubDirectoryPath);

            if (!Directory.Exists(documentFilePath))
                Directory.CreateDirectory(documentFilePath);

            var fileBytes = ConvertBase64ToByte(file);
            await File.WriteAllBytesAsync(document, fileBytes);
            return Path.Combine("VendorDocs", vendorId.ToString(), filename);
        }
        public static string GetFileBaseType(this string fileBase64)
        {
            return fileBase64.Substring(0, fileBase64.IndexOf(",") + 1);
        }
        public static void RemoveFile(string root, string filePath)
        {
            string path = Path.Combine(root, "appfiles", filePath);

            if (File.Exists(path))
            {
                File.Delete(path);
            }
        }
        public static void RemoveFolder(string root, string subFolder)
        {
            string path = Path.Combine(root, "appfiles", subFolder);
            if (Directory.Exists(path))
            {
                Directory.Delete(path, true);
            }
        }

        public static bool CopyFolderToCopy(string baseFolderPath, string copiedFolderName, string subFolder, string root)
        {
            try
            {
                string targetFolder = Path.Combine(root, "appfiles", copiedFolderName);
                if (!Directory.Exists(targetFolder))
                    Directory.CreateDirectory(targetFolder);

                string copiedFolder = Path.Combine(root, "appfiles", copiedFolderName, subFolder);

                DirectoryInfo sourceDircetory = new DirectoryInfo(baseFolderPath);
                DirectoryInfo targetDircetory = new DirectoryInfo(copiedFolder);
                CopyAll(sourceDircetory, targetDircetory);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static void CopyAll(DirectoryInfo source, DirectoryInfo target)
        {
            Directory.CreateDirectory(target.FullName);
            foreach (FileInfo fi in source.GetFiles())
            {
                fi.CopyTo(Path.Combine(target.FullName, fi.Name), true);
            }
            //foreach (DirectoryInfo diSourceSubDir in source.GetDirectories())
            //{
            //    DirectoryInfo nextTargetSubDir =
            //    target.CreateSubdirectory(diSourceSubDir.Name);
            //    CopyAll(diSourceSubDir, nextTargetSubDir);
            //}
        }
    }

}
