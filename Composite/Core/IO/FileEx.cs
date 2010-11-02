﻿

namespace Composite.Core.IO
{
    /// <summary>    
    /// </summary>
    /// <exclude />
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)] 
    public static class FileEx
    {
        public static bool RemoveReadOnly(string filePath)
        {
            if (File.Exists(filePath) == false) return false;

            System.IO.FileAttributes fileAttributes = File.GetAttributes(filePath);

            if ((fileAttributes & System.IO.FileAttributes.ReadOnly) == System.IO.FileAttributes.ReadOnly)
            {
                fileAttributes ^= System.IO.FileAttributes.ReadOnly;
                File.SetAttributes(filePath, fileAttributes);
            }

            return true;
        }



        public static void Delete(string filePath)
        {
            if (File.Exists(filePath) == true)
            {
                RemoveReadOnly(filePath);
                File.Delete(filePath);
            }
        }
    }
}
