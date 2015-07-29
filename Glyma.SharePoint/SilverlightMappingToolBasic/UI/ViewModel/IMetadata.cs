using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.ComponentModel;
using Proxy = TransactionalNodeService.Proxy;
using TransactionFramework = TransactionalNodeService.Soap.TransactionFramework;

namespace SilverlightMappingToolBasic.UI.ViewModel
{
    public interface IMetadata : INotifyPropertyChanged
    {
        string Name { get; }
        string Value { get; }
        Proxy.IMetadataSet MetadataSet { get; }

        bool SetName(string name, ref TransactionFramework.TransactionChain chain);
        bool SetValue(string value, ref TransactionFramework.TransactionChain chain);

        void Delete();
    }
}
