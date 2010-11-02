﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using Composite.Core.Collections.Generic;
using Composite.Core.IO;
using Composite.Core.Types;
using Composite.Data.Streams;

namespace Composite.Data.Plugins.DataProvider.Streams
{
	internal class FileChangeNotificator
	{
        private static readonly object _syncRoot = new object();
        private static FileSystemWatcher _fileWatcher;

        private static int _counter;

        // We're holding only weak reference to subscriber objects, in order to avoid memory leaks
        private static readonly Hashtable<string, ReadOnlyList<Pair<MethodInfo, WeakReference>>> _subscribers = new Hashtable<string, ReadOnlyList<Pair<MethodInfo, WeakReference>>>();


        private static void EnsureInitialization()
        {
            if (_fileWatcher != null)
            {
                return;
            }

            lock (_syncRoot)
            {
                if (_fileWatcher != null)
                {
                    return;
                }

                _fileWatcher = new FileSystemWatcher(AppDomain.CurrentDomain.BaseDirectory);
                _fileWatcher.Created += FileWatcher_Created;
                _fileWatcher.Changed += FileWatcher_Changed;
                _fileWatcher.Deleted += FileWatcher_Deleted;

                _fileWatcher.IncludeSubdirectories = true;
                _fileWatcher.NotifyFilter = System.IO.NotifyFilters.FileName | System.IO.NotifyFilters.LastWrite;

                _fileWatcher.EnableRaisingEvents = true;
            }
        }


        static void FileWatcher_Deleted(object sender, System.IO.FileSystemEventArgs e)
        {
            FireFileChangedEvent(e.FullPath, FileChangeType.Deleted);
        }

        static void FileWatcher_Changed(object sender, System.IO.FileSystemEventArgs e)
        {
            FileChangeType changeType = e.ChangeType == System.IO.WatcherChangeTypes.Renamed
                                            ? FileChangeType.Renamed
                                            : FileChangeType.Modified;
            FireFileChangedEvent(e.FullPath, changeType);
        }

        static void FileWatcher_Created(object sender, System.IO.FileSystemEventArgs e)
        {
            // Do nothing...
        }

        private static void FireFileChangedEvent(string filePath, FileChangeType changeType)
        {
            filePath = filePath.ToLower();

            ReadOnlyList<Pair<MethodInfo, WeakReference>> weakInvocationList;

            if (!_subscribers.TryGetValue(filePath.ToLower(), out weakInvocationList))
            {
                return;
            }

            var parameters = new object[] { filePath, changeType };

            foreach (var callInfo in weakInvocationList)
            {
                if (callInfo.Second == null) // Call to a static method
                {
                    callInfo.First.Invoke(null, parameters);
                }
                else
                {
                    object target = callInfo.Second.Target;
                    if (target != null) // Checking if object is alive
                    {
                        callInfo.First.Invoke(target, parameters);
                    }
                }
            }
        }


        public static void Subscribe(FileSystemFileBase file, OnFileChangedDelegate handler)
        {
            Verify.ArgumentNotNull(file, "file");

            Subscribe(file.SystemPath, handler);
        }


	    public static void Subscribe(string filePath, OnFileChangedDelegate handler)
        {
            Verify.ArgumentNotNullOrEmpty(filePath, "filePath");
            Verify.ArgumentNotNull(handler, "handler");

            EnsureInitialization();

            int counterValue = Interlocked.Increment(ref _counter);
            if (counterValue % 100 == 0)
            {
                ClearDeadReferences();
            }

            var weakInvocationList = new List<Pair<MethodInfo, WeakReference>>();
            foreach (Delegate func in handler.GetInvocationList())
            {
                var targetObject = func.Target;
                if (targetObject == null)
                {
                    weakInvocationList.Add(new Pair<MethodInfo, WeakReference>(func.Method, null));
                }
                else
                {
                    weakInvocationList.Add(new Pair<MethodInfo, WeakReference>(func.Method, new WeakReference(handler.Target)));
                }
            }

            string key = filePath.ToLower();
            lock (_syncRoot)
            {
                if (_subscribers.ContainsKey(key))
                {
                    ReadOnlyList<Pair<MethodInfo, WeakReference>> oldList = _subscribers[key];

                    var newList = new ReadOnlyList<Pair<MethodInfo, WeakReference>>(
                        new List<Pair<MethodInfo, WeakReference>>(oldList.Concat(weakInvocationList)));

                    _subscribers[key] = newList;
                }
                else
                {
                    _subscribers.Add(key, new ReadOnlyList<Pair<MethodInfo, WeakReference>>(weakInvocationList));
                }
            }
        }

        private static void ClearDeadReferences()
        {
            lock(_syncRoot)
            {
                ICollection<string> keys = _subscribers.GetKeys();

                foreach(string key in keys)
                {
                    ReadOnlyList<Pair<MethodInfo, WeakReference>> currentList = _subscribers[key];

                    int countOfAlive = currentList.Count(pair => pair.Second == null || pair.Second.IsAlive);
                    if(countOfAlive == 0)
                    {
                        _subscribers.Remove(key);
                        continue;
                    }

                    if (countOfAlive == currentList.Count)
                    {
                        continue;
                    }

                    var newList = new List<Pair<MethodInfo, WeakReference>>(
                        currentList.Where(pair => pair.Second == null || pair.Second.IsAlive));
                    
                    _subscribers[key] = new ReadOnlyList<Pair<MethodInfo, WeakReference>>(newList);
                }
            }
        }
	}
}
