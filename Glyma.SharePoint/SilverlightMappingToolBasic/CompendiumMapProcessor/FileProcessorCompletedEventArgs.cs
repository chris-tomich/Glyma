using System;

namespace SilverlightMappingToolBasic.CompendiumMapProcessor
{
    public class FileProcessorCompletedEventArgs : EventArgs
    {
        public bool NeedRefresh { get; set; }
    }
}
