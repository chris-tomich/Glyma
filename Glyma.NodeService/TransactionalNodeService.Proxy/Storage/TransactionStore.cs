using System;
using System.Collections.Generic;
using System.IO;
using System.IO.IsolatedStorage;
using System.Net;
using System.Runtime.Serialization;
using TransactionFramework = TransactionalNodeService.Soap.TransactionFramework;

namespace TransactionalNodeService.Storage
{
    public class TransactionStore : IDisposable
    {
        private bool _hasChanged = false;
        //private object _lock = new object();
        private string _context;
        private Queue<TransactionFramework.TransactionChain> _transactionChains = null;
        //private IsolatedStorageFile _store = null;

        public TransactionStore(string context)
        {
            _context = context;

            //string fileName = _context.ToString() + ".xml";
            //string fileLocation = Path.Combine("UnfinishedTransactionChains", fileName);

            //if (Store.FileExists(fileLocation))
            //{
            //    IsolatedStorageFileStream fileStream = null;

            //    try
            //    {
            //        fileStream = Store.OpenFile(fileLocation, FileMode.Open, FileAccess.Read);
            //        DataContractSerializer serializer = new DataContractSerializer(typeof(QueueSerializationWrapper<TransactionFramework.TransactionChain>));
            //        QueueSerializationWrapper<TransactionFramework.TransactionChain> incompleteTransactionChains = serializer.ReadObject(fileStream) as QueueSerializationWrapper<TransactionFramework.TransactionChain>;
            //        _transactionChains = incompleteTransactionChains.DeserializeQueue();
            //    }
            //    catch
            //    {
            //        if (fileStream != null)
            //        {
            //            fileStream.Dispose();
            //            fileStream = null;
            //        }

            //        Store.DeleteFile(fileLocation);
            //    }
            //    finally
            //    {
            //        if (fileStream != null)
            //        {
            //            fileStream.Dispose();
            //            fileStream = null;
            //        }
            //    }
            //}
        }

        private Queue<TransactionFramework.TransactionChain> TransactionChains
        {
            get
            {
                if (_transactionChains == null)
                {
                    _transactionChains = new Queue<TransactionFramework.TransactionChain>();
                }

                return _transactionChains;
            }
        }

        //private IsolatedStorageFile Store
        //{
        //    get
        //    {
        //        lock (_lock)
        //        {
        //            if (_store == null)
        //            {
        //                _store = IsolatedStorageFile.GetUserStoreForApplication();
        //            }
        //        }

        //        return _store;
        //    }
        //}

        public int Count
        {
            get
            {
                return TransactionChains.Count;
            }
        }

        public TransactionFramework.TransactionChain CurrentChain
        {
            get;
            private set;
        }

        public void AddTransactionChain(TransactionFramework.TransactionChain transactionChain)
        {
            TransactionChains.Enqueue(transactionChain);
            _hasChanged = true;

            //Save();
        }

        public TransactionFramework.TransactionChain MoveToNextChain()
        {
            CurrentChain = TransactionChains.Dequeue();
            _hasChanged = true;

            //Save();

            return CurrentChain;
        }

        //public bool Save()
        //{
        //    IsolatedStorageFileStream fileStream = null;

        //    if (!_hasChanged)
        //    {
        //        return true;
        //    }

        //    try
        //    {
        //        string directoryName = "UnfinishedTransactionChains";
        //        string fileName = _context.ToString() + ".xml";
        //        string fileLocation = Path.Combine(directoryName, fileName);

        //        if (Store.FileExists(fileLocation))
        //        {
        //            Store.DeleteFile(fileLocation);
        //        }

        //        if (!Store.DirectoryExists(directoryName))
        //        {
        //            Store.CreateDirectory(directoryName);
        //        }

        //        fileStream = Store.CreateFile(fileLocation);

        //        QueueSerializationWrapper<TransactionFramework.TransactionChain> queueSerializationWrapper = new QueueSerializationWrapper<TransactionFramework.TransactionChain>(TransactionChains, CurrentChain);

        //        DataContractSerializer serializer = new DataContractSerializer(typeof(QueueSerializationWrapper<TransactionFramework.TransactionChain>));
        //        serializer.WriteObject(fileStream, queueSerializationWrapper);

        //        _hasChanged = false;

        //        return true;
        //    }
        //    catch
        //    {
        //        return false;
        //    }
        //    finally
        //    {
        //        if (fileStream != null)
        //        {
        //            fileStream.Dispose();
        //            fileStream = null;
        //        }
        //    }
        //}

        public void Dispose()
        {
            //if (_store != null)
            //{
            //    _store.Dispose();
            //    _store = null;
            //}
        }
    }
}
