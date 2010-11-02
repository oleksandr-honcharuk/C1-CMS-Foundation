using System.Globalization;


namespace Composite.Core.Extensions
{
    /// <summary>    
    /// </summary>
    /// <exclude />
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)] 
    public static class StringExtensionMethods
    {
        public static string FormatWith(this string format, params object[] args)
        {
            Verify.ArgumentNotNull(format, "format");

            return string.Format(format, args);
        }

        public static bool IsNullOrEmpty(this string str)
        {
            return string.IsNullOrEmpty(str);
        }

        public static bool StartsWith(this string str, string value, bool ignoreCase)
        {
            return str.StartsWith(value, ignoreCase, CultureInfo.InvariantCulture);
        }

        public static bool EndsWith(this string str, string value, bool ignoreCase)
        {
            return str.EndsWith(value, ignoreCase, CultureInfo.InvariantCulture);
        }


        public static bool IsCorrectNamespace(this string s, char separator)
        {
            if (s == null) return false;
            if (s == "") return true;

            string[] splits = s.Split(separator);

            foreach (string split in splits)
            {
                if (split == "") return false;
            }

            return true;
        }



        public static string CreateNamespace(string namespaceName, string name, char separator)
        {
            if (string.IsNullOrEmpty(namespaceName) == true)
            {
                return name;
            }
            else
            {
                return string.Format("{0}{1}{2}", namespaceName, separator, name);
            }
        }


        /// <summary>
        /// Default separator is '.'
        /// </summary>
        /// <param name="namespaceName"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static string CreateNamespace(string namespaceName, string name)
        {
            return CreateNamespace(namespaceName, name, '.');
        }



        public static string GetNameFromNamespace(this string s)
        {
            int index = s.LastIndexOf(".");
            if (index < 0)
            {
                return s;
            }
            return s.Substring(index + 1);
        }

        public static string GetNamespace(this string s)
        {
            int index = s.LastIndexOf(".");
            if (index <= 0)
            {
                return string.Empty;
            }

            string result = s.Substring(0, index);
            if (result.EndsWith("."))
            {
                result = result.Substring(0, result.Length);
            }

            return result;
        }



        public static bool IsCorrectFolderName(this string s, char separator)
        {
            if (s == null) return false;
            if (s == "/") return true;
            if (!s.StartsWith("/")) return false;
            if (s.Length > 1 && s.EndsWith("/")) return false;

            string[] splits = s.Split(separator);

            for (int i = 1; i < splits.Length; i++)
            {
                if (splits[i] == "") return false;
            }

            return true;
        }



        public static bool IsDirectChildOf(this string s, string possibleParentPath, char separator)
        {
            if (possibleParentPath.Length > s.Length)
            {
                return false;
            }
            if (s == possibleParentPath)
            {
                return false;
            }

            if (!s.StartsWith(possibleParentPath))
            {
                return false;
            }

            if (possibleParentPath == separator.ToString())
            {
                string remaining = s.Remove(0, possibleParentPath.Length);
                if (!remaining.Contains(separator.ToString()))
                {
                    return true;
                }
            }

            if (s[possibleParentPath.Length] == '/')
            {
                string remaining = s.Remove(0, possibleParentPath.Length + 1);
                if (!remaining.Contains(separator.ToString()))
                {
                    return true;
                }
            }

            return false;
        }



        public static bool IsParentOf(this string s, string possibleChild, char separator)
        {
            return possibleChild.IsDirectChildOf(s, separator);
        }



        public static string GetFolderName(this string s, char separator)
        {
            if (s == separator.ToString())
            {
                return null;
            }

            if (!s.Contains(separator.ToString()))
            {
                return "/";
            }

            string[] foldernames = s.Split(separator);

            if (foldernames[foldernames.Length - 1] == "")
            {
                if (foldernames.Length >= 2)
                {
                    return foldernames[foldernames.Length - 2];
                }
                else
                {
                    throw new System.NotImplementedException();
                }
            }
            else
            {
                return foldernames[foldernames.Length - 1];
            }
        }



        public static string GetNameWithoutExtension(this string s)
        {
            string name = System.IO.Path.GetFileName(s);
            if (System.IO.Path.HasExtension(name))
            {
                int lastIndex = name.LastIndexOf('.');
                return name.Remove(lastIndex);
            }
            return name;
        }



        public static string GetDirectory(this string s, char separator)
        {
            int lastIndex = s.LastIndexOf(separator);
            if (lastIndex > 0)
            {
                return s.Remove(lastIndex);
            }

            if (lastIndex == 0 && s.Length > 1)
            {
                return "/";
            }
            if (lastIndex == 0 && s.Length == 1)
            {
                return "";
            }

            if (!s.Contains(separator.ToString()))
            {
                return "";
            }

            return s;
        }



        public static string Combine(this string path, string otherPath, char separator)
        {
            string childPath = otherPath;
            if (otherPath.StartsWith("/"))
            {
                childPath = otherPath.Substring(1, otherPath.Length - 1);
            }
            if (childPath.EndsWith("/"))
            {
                childPath = otherPath.Substring(0, otherPath.Length - 1);
            }

            if (path == separator.ToString())
            {
                return path + childPath;
            }

            if (otherPath == "/" || otherPath == string.Empty)
            {
                return path;
            }

            return path + "/" + childPath;
        }
    }
}
