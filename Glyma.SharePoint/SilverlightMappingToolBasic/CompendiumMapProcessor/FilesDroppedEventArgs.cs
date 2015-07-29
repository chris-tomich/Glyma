using System;
using System.IO;

namespace SilverlightMappingToolBasic.CompendiumMapProcessor
{
    public class FilesDroppedEventArgs : EventArgs
    {
        public FilesDroppedEventArgs()
            : base()
        {
        }

        public FileInfo[] DroppedFiles
        {
            get;
            set;
        }
    }
}
