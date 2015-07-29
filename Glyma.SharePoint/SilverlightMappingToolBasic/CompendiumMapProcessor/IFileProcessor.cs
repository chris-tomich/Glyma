using System;
using System.ComponentModel;

namespace SilverlightMappingToolBasic.CompendiumMapProcessor
{
    public interface IFileProcessor : IDisposable
    {
        void ProcessFile();
        event EventHandler<ProgressChangedEventArgs> ProgressChanged;
        event EventHandler<FileProcessorCompletedEventArgs> ProgressCompleted;
    }
}
