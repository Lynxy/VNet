using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.ComponentModel;

namespace Lynxy.IO
{
    //eg "file.exe:mystream"
    public class PInvokeWin32Api
    {
        #region constants
        //
        // these are constants used by the Win32 api functions.  They can be found in the documentation and header files.
        //
        public const UInt32 GENERIC_READ = 0x80000000;
        public const UInt32 GENERIC_WRITE = 0x40000000;
        public const UInt32 FILE_SHARE_READ = 0x00000001;
        public const UInt32 FILE_SHARE_WRITE = 0x00000002;
        public const UInt32 FILE_ATTRIBUTE_DIRECTORY = 0x00000010;

        public const UInt32 CREATE_NEW = 1;
        public const UInt32 CREATE_ALWAYS = 2;
        public const UInt32 OPEN_EXISTING = 3;
        public const UInt32 OPEN_ALWAYS = 4;
        public const UInt32 TRUNCATE_EXISTING = 5;
        #endregion

        #region dll imports
        //
        // DllImport statements identify specific functions and declare their C# function signature
        // 
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtr CreateFile(
                string lpFileName,
                uint dwDesiredAccess,
                uint dwShareMode,
                IntPtr lpSecurityAttributes,
                uint dwCreationDisposition,
                uint dwFlagsAndAttributes,
                IntPtr hTemplateFile);

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool CloseHandle(
                IntPtr hObject);

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GetFileInformationByHandle(
                IntPtr hFile,
                out BY_HANDLE_FILE_INFORMATION lpFileInformation);

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool DeleteFile(
                string fileName);

        [DllImport("kernel32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool ReadFile(
                IntPtr hFile,
                IntPtr lpBuffer,
                uint nNumberOfBytesToRead,
                out uint lpNumberOfBytesRead,
                IntPtr lpOverlapped);

        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool WriteFile(
                IntPtr hFile,
                IntPtr bytes,
                uint nNumberOfBytesToWrite,
                out uint lpNumberOfBytesWritten,
                int overlapped);

        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool WriteFile(
                IntPtr hFile,
                byte[] lpBuffer,
                uint nNumberOfBytesToWrite,
                out uint lpNumberOfBytesWritten,
                int overlapped);

        [DllImport("kernel32.dll")]
        public static extern void ZeroMemory(IntPtr ptr, int size);
        #endregion

        #region structures
        //
        // This section declares the structures used by the Win32 functions so that the information can be accessed by C# code
        //
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct BY_HANDLE_FILE_INFORMATION
        {
            public uint FileAttributes;
            public FILETIME CreationTime;
            public FILETIME LastAccessTime;
            public FILETIME LastWriteTime;
            public uint VolumeSerialNumber;
            public uint FileSizeHigh;
            public uint FileSizeLow;
            public uint NumberOfLinks;
            public uint FileIndexHigh;
            public uint FileIndexLow;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct FILETIME
        {
            public uint DateTimeLow;
            public uint DateTimeHigh;
        }
        #endregion

        #region functions
        //
        // These are the functions in C# that wrap the Win32 functions
        //

        //
        // this functions writes the string text to the alternate stream named altStreamName of the file whose path is currentfile
        //
        public static void WriteAlternateStream(string currentfile, string altStreamName, string text)
        {
            string altStream = currentfile + ":" + altStreamName;
            IntPtr txtBuffer = IntPtr.Zero;
            IntPtr hFile = IntPtr.Zero;
            DeleteFile(altStream);
            try
            {
                //
                // call CreateFile
                // 
                hFile = CreateFile(altStream, GENERIC_WRITE, 0, IntPtr.Zero,
                                                           CREATE_ALWAYS, 0, IntPtr.Zero);
                if (-1 != hFile.ToInt32())  // check the return code for success
                {
                    txtBuffer = Marshal.StringToHGlobalUni(text);
                    uint nBytes, count;
                    nBytes = (uint)text.Length;
                    bool bRtn = WriteFile(hFile, txtBuffer, sizeof(char) * nBytes, out count, 0);
                    if (!bRtn)
                    {
                        if ((sizeof(char) * nBytes) != count)
                        {
                            throw new Exception(string.Format("Bytes written {0} should be {1} for file {2}.",
                                    count, sizeof(char) * nBytes, altStream));
                        }
                        else
                        {
                            throw new Exception("WriteFile() returned false");
                        }
                    }
                }
                else
                {
                    throw new Win32Exception(Marshal.GetLastWin32Error());
                }
            }
            catch (Exception exception)
            {
                string msg = string.Format("Exception caught in WriteAlternateStream()\n  '{0}'\n  for file '{1}'.",
                        exception.Message, altStream);
            }
            finally
            {
                CloseHandle(hFile);
                hFile = IntPtr.Zero;
                Marshal.FreeHGlobal(txtBuffer);
                GC.Collect();
            }
        }

        //
        // this function reads the alternate stream named altStreamName of the file whose path is currentfile and returns the contents as a string
        //
        public static string ReadAlternateStream(string currentfile, string altStreamName)
        {
            IntPtr hFile = IntPtr.Zero;
            string returnstring = string.Empty;
            string altStream = currentfile + ":" + altStreamName;

            IntPtr buffer = IntPtr.Zero;
            try
            {
                hFile = CreateFile(altStream, GENERIC_READ, 0, IntPtr.Zero, OPEN_EXISTING, 0, IntPtr.Zero);
                if (-1 != hFile.ToInt32())
                {
                    buffer = Marshal.AllocHGlobal(1000 * sizeof(char));
                    ZeroMemory(buffer, 1000 * sizeof(char));
                    uint nBytes;
                    bool bRtn = ReadFile(hFile, buffer, 1000 * sizeof(char), out nBytes, IntPtr.Zero);
                    if (bRtn)
                    {
                        if (nBytes > 0)
                        {
                            returnstring = Marshal.PtrToStringAuto(buffer);
                        }
                        else
                        {
                            throw new Exception("ReadFile() returned true but read zero bytes");
                        }
                    }
                    else
                    {
                        if (nBytes <= 0)
                        {
                            throw new Exception("ReadFile() read zero bytes.");
                        }
                        else
                        {
                            throw new Exception("ReadFile() returned false");
                        }
                    }
                }
                else
                {
                    Exception excptn = new Win32Exception(Marshal.GetLastWin32Error());
                    if (!excptn.Message.Contains("cannot find the file"))
                    {
                        throw excptn;
                    }
                }
            }
            catch (Exception exception)
            {
                string msg = string.Format("Exception caught in ReadAlternateStream(), '{0}'\n  for file '{1}'.",
                        exception.Message, currentfile);
            }
            finally
            {
                CloseHandle(hFile);
                hFile = IntPtr.Zero;
                if (buffer != IntPtr.Zero)
                {
                    Marshal.FreeHGlobal(buffer);
                }
                GC.Collect();
            }
            return returnstring;
        }
        #endregion
    }

    public static class AlternateStream
    {
        public static string ReadStream(string fileName, string streamName)
        {
            string altStreamText = PInvokeWin32Api.ReadAlternateStream(fileName, streamName);
            return altStreamText;
        }

        public static void WriteStream(string fileName, string streamName, string data)
        {
            PInvokeWin32Api.WriteAlternateStream(fileName, streamName, data);
        }
    }
}
