using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using ICSharpCode.SharpZipLib.Zip;
using ICSharpCode.SharpZipLib.Zip.Compression;
using ICSharpCode.SharpZipLib.Zip.Compression.Streams;
namespace ItSoftware.Core.Zip
{
    public static class ItsZip
    {
        #region Public Static Methods
        /// <summary>
        /// Unpack a zip file.
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="destinationDirectory"></param>
        public static void Unpack(string filename, string destinationDirectory)
        {
            if (filename == null)
            {
                throw new ArgumentNullException("filename");
            }
            if (destinationDirectory == null)
            {
                throw new ArgumentNullException("destinationDirectory");
            }
            if (!File.Exists(filename))
            {
                throw new ArgumentException("File does not exist", "filename");
            }
            if (!Directory.Exists(destinationDirectory))
            {
                throw new ArgumentException("Directory does not exist", "destinationDirectory");
            }

            using (ZipInputStream zis = new ZipInputStream(File.OpenRead(filename)))
            {
                ZipEntry ze;
                while ((ze = zis.GetNextEntry()) != null)
                {
                    string directory = Path.Combine(destinationDirectory, Path.GetDirectoryName(ze.Name)!);
                    string file = Path.GetFileName(ze.Name);

                    if (!Directory.Exists(directory))
                    {
                        Directory.CreateDirectory(directory);
                    }

                    if (!string.IsNullOrEmpty(file))
                    {
                        using (FileStream fs = File.Create(Path.Combine(directory, file)))
                        {
                            if (ze.Size > 0)
                            {
                                int size = 2048;
                                byte[] data = new byte[size];
                                while (true)
                                {
                                    size = zis.Read(data, 0, data.Length);
                                    if (size > 0)
                                    {
                                        fs.Write(data, 0, size);
                                    }
                                    else
                                    {
                                        break;
                                    }
                                }
                            }
                        }// using ( FileStream
                    }
                }// while
            }// using ( ZipInputStream
        }// Unpack
        /// <summary>
        /// Zip a file.
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="appendZipExtension"></param>
        public static void PackFile(string filename, bool appendZipExtension)
        {
            if (filename == null)
            {
                throw new ArgumentNullException("filename");
            }
            if (!File.Exists(filename))
            {
                throw new ArgumentException(string.Format("File does not exist: {0}.", filename), "filename");
            }

            string newFilename = filename + ".zip";
            if (!appendZipExtension)
            {
                newFilename = Path.GetDirectoryName(filename) + Path.DirectorySeparatorChar + Path.GetFileNameWithoutExtension(filename) + ".zip";
            }
            if (File.Exists(newFilename))
            {
                File.Delete(newFilename);
            }

            FileStream fileStreamOut = File.Create(newFilename);

            ICSharpCode.SharpZipLib.Zip.ZipOutputStream zipOutputStream = new ICSharpCode.SharpZipLib.Zip.ZipOutputStream(fileStreamOut);
            zipOutputStream.SetLevel(9);

            ICSharpCode.SharpZipLib.Zip.ZipEntry zipEntry = new ICSharpCode.SharpZipLib.Zip.ZipEntry(Path.GetFileName(filename));
            zipOutputStream.PutNextEntry(zipEntry);

            FileStream fileStreamIn = File.OpenRead(filename);
            const long BUFFER_SIZE = 8192;
            long currentIndex = 0;
            byte[] buffer = new byte[BUFFER_SIZE];
            if (fileStreamIn.Length <= BUFFER_SIZE)
            {
                fileStreamIn.Read(buffer, 0, Convert.ToInt32(fileStreamIn.Length));
                zipOutputStream.Write(buffer, 0, Convert.ToInt32(fileStreamIn.Length));
            }
            else
            {
                do
                {
                    long remaining = BUFFER_SIZE;
                    if (currentIndex + BUFFER_SIZE >= fileStreamIn.Length)
                    {
                        remaining = fileStreamIn.Length - currentIndex;
                    }
                    fileStreamIn.Read(buffer, 0, Convert.ToInt32(remaining));
                    currentIndex += remaining;

                    zipOutputStream.Write(buffer, 0, Convert.ToInt32(remaining));
                } while (currentIndex < fileStreamIn.Length);
            }
            fileStreamIn.Close();

            zipOutputStream.Flush();
            zipOutputStream.Finish();
            zipOutputStream.Close();

            fileStreamOut.Close();
        }// ZipFile
        /// <summary>
        /// Zip multiple files(fileNames) in directory(directory) to (reportName).zip.
        /// </summary>
        /// <param name="directory"></param>
        /// <param name="fileNames"></param>
        /// <param name="reportName"></param>
        public static void PackFilesIntoOne(string directory, string[] fileNames, string outputFilename)
        {
            if (directory == null)
            {
                throw new ArgumentNullException("directory");
            }
            if (fileNames == null)
            {
                throw new ArgumentNullException("fileNames");
            }
            if (outputFilename == null)
            {
                throw new ArgumentNullException("outputFilename");
            }
            if (fileNames.Length == 0)
            {
                throw new ArgumentException("Length cannot be 0.", "fileNames");
            }

            string newFilename = Path.Combine(directory, outputFilename);
            if (File.Exists(newFilename))
            {
                File.Delete(newFilename);
            }

            using (FileStream fileStreamOut = File.Create(newFilename))
            {

                using (ICSharpCode.SharpZipLib.Zip.ZipOutputStream zipOutputStream = new ICSharpCode.SharpZipLib.Zip.ZipOutputStream(fileStreamOut))
                {
                    zipOutputStream.SetLevel(9);

                    foreach (string filename in fileNames)
                    {
                        ICSharpCode.SharpZipLib.Zip.ZipEntry zipEntry = new ICSharpCode.SharpZipLib.Zip.ZipEntry(filename);
                        zipOutputStream.PutNextEntry(zipEntry);

                        using (FileStream fileStreamIn = File.OpenRead(Path.Combine(directory, filename)))
                        {
                            const long BUFFER_SIZE = 8192;
                            long currentIndex = 0;
                            byte[] buffer = new byte[BUFFER_SIZE];
                            if (fileStreamIn.Length <= BUFFER_SIZE)
                            {
                                fileStreamIn.Read(buffer, 0, Convert.ToInt32(fileStreamIn.Length));
                                zipOutputStream.Write(buffer, 0, Convert.ToInt32(fileStreamIn.Length));
                            }
                            else
                            {
                                do
                                {
                                    long remaining = BUFFER_SIZE;
                                    if (currentIndex + BUFFER_SIZE >= fileStreamIn.Length)
                                    {
                                        remaining = fileStreamIn.Length - currentIndex;
                                    }
                                    fileStreamIn.Read(buffer, 0, Convert.ToInt32(remaining));
                                    currentIndex += remaining;

                                    zipOutputStream.Write(buffer, 0, Convert.ToInt32(remaining));
                                } while (currentIndex < fileStreamIn.Length);
                            }
                        }// using ( FileStream fileStreamIn = File.OpenRead( Path.Combine( directory, filename ) ...
                    }// foreach

                    zipOutputStream.Flush();
                    zipOutputStream.Finish();
                }//using ( ICSharpCode.SharpZipLib.Zip.ZipOutputStream zipOutputStream = new ICSharpCode.SharpZipLib.Zip.ZipOutputStream( fileStreamOut ) ) ...

            }// using ( FileStream fileStreamOut = File.Create( newFilename ) ...
        }
        public static string PackToBase64(string text)
        {
            byte[] buffer;
            MemoryStream ms = new MemoryStream();
            using (ICSharpCode.SharpZipLib.Zip.ZipOutputStream zipOutputStream = new ICSharpCode.SharpZipLib.Zip.ZipOutputStream(ms))
            {
                zipOutputStream.SetLevel(9);


                ICSharpCode.SharpZipLib.Zip.ZipEntry zipEntry = new ICSharpCode.SharpZipLib.Zip.ZipEntry("Boliglag.Admin");
                zipOutputStream.PutNextEntry(zipEntry);
                
                byte[] bytes = System.Text.Encoding.Unicode.GetBytes(text.ToCharArray());
               
                zipOutputStream.Write( bytes, 0, bytes.Length);

                zipOutputStream.Flush();
                zipOutputStream.Finish();

                buffer = new byte[ms.Length];
                ms.Position = 0;
                ms.Read(buffer, 0, Convert.ToInt32(ms.Length));
                
            }//using ( ICSharpCode.SharpZipLib.Zip.ZipOutputStream zipOutputStream = new ICSharpCode.SharpZipLib.Zip.ZipOutputStream( fileStreamOut ) ) ...
            
            return ToBase64(buffer);
        }
        public static string PackToBase64(byte[] data)
        {
            byte[] buffer;
            MemoryStream ms = new MemoryStream();
            using (ICSharpCode.SharpZipLib.Zip.ZipOutputStream zipOutputStream = new ICSharpCode.SharpZipLib.Zip.ZipOutputStream(ms))
            {
                zipOutputStream.SetLevel(9);


                ICSharpCode.SharpZipLib.Zip.ZipEntry zipEntry = new ICSharpCode.SharpZipLib.Zip.ZipEntry("Boliglag.Admin");
                zipOutputStream.PutNextEntry(zipEntry);

                zipOutputStream.Write(data, 0, data.Length);

                zipOutputStream.Flush();
                zipOutputStream.Finish();

                buffer = new byte[ms.Length];
                ms.Position = 0;
                ms.Read(buffer, 0, Convert.ToInt32(ms.Length));

            }//using ( ICSharpCode.SharpZipLib.Zip.ZipOutputStream zipOutputStream = new ICSharpCode.SharpZipLib.Zip.ZipOutputStream( fileStreamOut ) ) ...
            
            return ToBase64(buffer);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static string UnpackFromBase64(string text) 
        {
            byte[] data = ItsZip.FromBase64(text);
            MemoryStream ms = new MemoryStream(data);
            ms.Position = 0;
            using (ZipInputStream zis = new ZipInputStream(ms))
            {
                ZipEntry ze = zis.GetNextEntry();

                if (ze.Size > 0)
                {
                    MemoryStream msResult = new MemoryStream();
                    int size = 2048;
                    byte[] data2 = new byte[size];
                    while (true)
                    {
                        size = zis.Read(data2, 0, size);
                        if (size > 0)
                        {
                            msResult.Write(data2, 0, size);
                        }
                        else
                        {
                            break;
                        }
                    }
                    msResult.Position = 0;
                    byte[] textBytes = new byte[msResult.Length];
                    msResult.Read(textBytes, 0, Convert.ToInt32(msResult.Length));
                    StringBuilder sb = new StringBuilder();
                    sb.Append(Encoding.Unicode.GetChars(textBytes));
                    return sb.ToString();
                }
            }// using ( ZipInputStream
            return null!;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static byte[] UnpackFromBase64ToByteArray(string text)
        {
            byte[] data = ItsZip.FromBase64(text);
            MemoryStream ms = new MemoryStream(data);
            ms.Position = 0;
            using (ZipInputStream zis = new ZipInputStream(ms))
            {
                ZipEntry ze = zis.GetNextEntry();

                if (ze.Size > 0)
                {
                    MemoryStream msResult = new MemoryStream();                    
                    int size = 2048;
                    byte[] data2 = new byte[size];
                    while (true)
                    {
                        size = zis.Read(data2, 0, size);
                        if (size > 0)
                        {
                            msResult.Write(data2, 0, size);
                        }
                        else
                        {
                            break;
                        }
                    }
                    msResult.Position = 0;
                    byte[] textBytes = new byte[msResult.Length];
                    msResult.Read(textBytes, 0, Convert.ToInt32(msResult.Length));
                    return textBytes;
                    //StringBuilder sb = new StringBuilder();
                    //sb.Append(Encoding.Unicode.GetChars(textBytes));
                    //return sb.ToString();
                }
            }// using ( ZipInputStream
            return null!;
        }
        public static string ToBase64(byte[] data)   
        {
            return Convert.ToBase64String(data);
        }
        public static byte[] FromBase64(string data) 
        {
            return Convert.FromBase64String(data);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static void UnpackFromBase64ToStream(string text, Stream output)
        {
            byte[] data = ItsZip.FromBase64(text);
            MemoryStream ms = new MemoryStream(data);
            ms.Position = 0;
            using (ZipInputStream zis = new ZipInputStream(ms))
            {
                ZipEntry ze = zis.GetNextEntry();

                if (ze.Size > 0)
                {
                    int size = 2048;
                    byte[] data2 = new byte[size];
                    while (true)
                    {
                        size = zis.Read(data2, 0, size);
                        if (size > 0)
                        {
                            output.Write(data2, 0, size);
                        }
                        else
                        {
                            break;
                        }
                    }
                }
            }// using ( ZipInputStream        
        }
        #endregion
    }
}
