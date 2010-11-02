﻿using System;
using System.Collections.Generic;


namespace Composite.Core.Types.Foundation
{
    internal sealed class AssemblyFilenameCollection
    {
        private static Dictionary<string, string> _assemblyFilenames = new Dictionary<string, string>();



        public bool ContainsAssemblyFilename(string assemblyFilename)
        {
            if (string.IsNullOrEmpty(assemblyFilename) == true) throw new ArgumentNullException("assemblyFilename");

            string assemblyName = AssemblyFilenameCollection.GetAssemblyName(assemblyFilename);

            return _assemblyFilenames.ContainsKey(assemblyName);
        }



        public bool ContainsAssemblyName(string assemblyName)
        {
            if (string.IsNullOrEmpty(assemblyName) == true) throw new ArgumentNullException("assemblyName");

            return _assemblyFilenames.ContainsKey(assemblyName);
        }



        public void Add(string assemblyFilename)
        {
            if (string.IsNullOrEmpty(assemblyFilename) == true) throw new ArgumentNullException("assemblyFilename");

            string assemblyName = AssemblyFilenameCollection.GetAssemblyName(assemblyFilename);

            _assemblyFilenames.Add(assemblyName, assemblyFilename);
        }



        public string GetFilenameByAssemblyName(string assemblyName)
        {
            if (string.IsNullOrEmpty(assemblyName) == true) throw new ArgumentNullException("assemblyName");

            string assemblyFilename;
            if (_assemblyFilenames.TryGetValue(assemblyName, out assemblyFilename) == false) throw new ArgumentException(string.Format("Does not contain the assembly name '{0}'", assemblyName));

            return assemblyFilename;
        }



        public static string GetAssemblyName(string assemblyFilename)
        {
            string filename = System.IO.Path.GetFileName(assemblyFilename);

            string extension = System.IO.Path.GetExtension(filename);

            filename = filename.Remove(filename.Length - extension.Length);

            return filename;
        }
    }
}
