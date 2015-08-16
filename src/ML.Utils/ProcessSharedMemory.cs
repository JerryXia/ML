using System;
using System.Collections.Generic;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.AccessControl;
using System.Threading;

namespace ML.Utils
{
    /*
    public class ProcessSharedMemory<T> where T : struct
    {
        // Data
        private string _filePath;
        private string _smName;
        private long _smSize;

        private Mutex _smLock;
        private bool _locked;

        private MemoryMappedFile mmf;
        private MemoryMappedViewAccessor accessor;

        // Constructor
        public ProcessSharedMemory(string fileFullPath, string name, long size)
        {
            _filePath = fileFullPath;
            _smName = name;
            _smSize = size;
        }

        public bool Open()
        {
            bool opened = false;
            try
            {
                var fileStream = new FileStream(_filePath, FileMode.OpenOrCreate, 
                    FileSystemRights.ReadData | FileSystemRights.WriteData, 
                    FileShare.None, 4096, FileOptions.None);

                mmf = MemoryMappedFile.CreateFromFile(fileStream, _smName, _smSize, 
                    MemoryMappedFileAccess.ReadWrite, null, HandleInheritability.None, false);

                accessor = mmf.CreateViewAccessor(0, _smSize, MemoryMappedFileAccess.ReadWrite);

                _smLock = new Mutex(true, "PSM_LOCK_" + _smName, out _locked);

                opened = true;
            }
            catch
            {
                opened = false;
            }
            return opened;
        }

        public void Close()
        {
            accessor.Dispose();
            mmf.Dispose();
            _smLock.Close();
        }

        public T Data
        {
            get
            {
                T dataStruct;
                accessor.Read<T>(0, out dataStruct);
                return dataStruct;
            }
            set
            {
                _smLock.WaitOne();
                accessor.Write<T>(0, ref value);
                _smLock.ReleaseMutex();
            }
        }
    }
    */

    public class ProcessSharedMemory
    {
        // Data
        private string _filePath;
        private string _smName;
        //private long _smSize;

        //private static Mutex _smLock;
        //private static bool _locked;

        //private MemoryMappedFile mmf;
        //private MemoryMappedViewAccessor accessor;

        // Constructor
        public ProcessSharedMemory(string filePath, string mapName)
        {
            _filePath = filePath;
            _smName = Global.IsMonoRuntime ? _filePath : mapName;
        }


        //public bool Open()
        //{
        //    bool opened = false;
        //    try
        //    {
        //        var fileStream = new FileStream(_filePath, FileMode.OpenOrCreate,
        //            FileSystemRights.ReadData | FileSystemRights.WriteData,
        //            FileShare.None, 4096, FileOptions.None);

        //        mmf = MemoryMappedFile.CreateFromFile(fileStream, _smName, _smSize, 
        //            MemoryMappedFileAccess.ReadWrite, null, HandleInheritability.None, false);

        //        accessor = mmf.CreateViewAccessor(0, _smSize, MemoryMappedFileAccess.ReadWrite);

        //        _smLock = new Mutex(true, "PSM_LOCK_" + _smName, out _locked);

        //        opened = true;
        //    }
        //    catch
        //    {
        //        opened = false;
        //    }
        //    return opened;
        //}

        public void Write(object objectData)
        {
            var fs = new FileStream(_filePath, FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite);
            byte[] buffer = ObjectToByteArray(objectData);
            fs.SetLength(buffer.Length);

            using (var mmf = MemoryMappedFile.CreateFromFile(fs, _smName, buffer.Length, MemoryMappedFileAccess.ReadWrite, null, HandleInheritability.None, false))
            {
                using (var accessor = mmf.CreateViewAccessor(0, buffer.Length))
                {
                    accessor.WriteArray<byte>(0, buffer, 0, buffer.Length);
                }
            }
        }

        public object Read()
        {
            object value;

            var fs = new FileStream(_filePath, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite);
            using (var mmf = MemoryMappedFile.CreateFromFile(fs, _smName, fs.Length, MemoryMappedFileAccess.ReadWrite, null, HandleInheritability.None, false))
            {
                using (var accessor = mmf.CreateViewAccessor())
                {
                    byte[] buffer = new byte[accessor.Capacity];
                    accessor.ReadArray<byte>(0, buffer, 0, buffer.Length);

                    value = ByteArrayToObject(buffer);
                }
            }

            return value;
        }

        //public T Data
        //{
        //    get
        //    {
        //        T dataStruct;
        //        accessor.Read<T>(0, out dataStruct);
        //        return dataStruct;
        //    }
        //    set
        //    {
        //        _smLock.WaitOne();
        //        accessor.Write<T>(0, ref value);
        //        _smLock.ReleaseMutex();
        //    }
        //}

        //public void Close()
        //{
        //    accessor.Dispose();
        //    mmf.Dispose();
        //    _smLock.Close();
        //}

        private object ByteArrayToObject(byte[] buffer)
        {
            var binaryFormatter = new BinaryFormatter();
            var memoryStream = new MemoryStream(buffer);
            return binaryFormatter.Deserialize(memoryStream);
        }

        private byte[] ObjectToByteArray(object inputObject)
        {
            var binaryFormatter = new BinaryFormatter();
            var memoryStream = new MemoryStream();
            binaryFormatter.Serialize(memoryStream, inputObject);
            return memoryStream.ToArray();
        }

    }
}
