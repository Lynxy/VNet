using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Win32;


namespace Lynxy.Registry
{
    public class RegistryHelper
    {
        public static RegistryKey Key = Microsoft.Win32.Registry.CurrentUser;

        public static string ReadKey(string keyPath, string keyName)
        {
            try
            {
                using (RegistryKey k = Key.OpenSubKey(keyPath))
                {
                    string ret = (string)k.GetValue(keyName);
                    k.Close();
                    if (ret == null)
                        ret = "";
                    return ret;
                }
            }
            catch (Exception)
            { }
            return "";
        }

        public static void WriteKey(string keyPath, string keyName, object keyValue)
        {
            try
            {
                using (RegistryKey k = Key.CreateSubKey(keyPath))
                {
                    k.SetValue(keyName, keyValue);
                    k.Close();
                }
            }
            catch (Exception)
            { }
        }

        public static void DeleteKey(string keyPath, string keyName)
        {
            try
            {
                Key.OpenSubKey(keyPath, true).DeleteValue(keyName);
            }
            catch (Exception)
            { }
        }
    }
}
