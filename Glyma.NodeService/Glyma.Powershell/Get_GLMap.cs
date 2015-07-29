using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Management.Automation;
using Glyma.Powershell.Base;

namespace Glyma.Powershell
{
    [Cmdlet(VerbsCommon.Get, "GLMap")]
    public class Get_GLMap : PSCmdlet
    {
        [Parameter(Position = 0, Mandatory = true)]
        public List<Model.Domain> Domains
        {
            get;
            set;
        }

        [Parameter(Position = 2, Mandatory = false)]
        public Guid MapId
        {
            get;
            set;
        }

        [Parameter(Position = 1, Mandatory = false)]
        public string MapName
        {
            get;
            set;
        }

        protected override void ProcessRecord()
        {
            base.ProcessRecord();

            if (Domains == null || Domains.Count == 0)
            {
                WriteWarning("No domain has been provided.");

                return;
            }

            foreach (Model.Domain domain in Domains)
            {
                Get_GLMapBase getGLMapBase = new Get_GLMapBase();
                getGLMapBase.Domain = domain;

                if (MapId != Guid.Empty)
                {
                    getGLMapBase.MapId = MapId;
                }
                else if (!string.IsNullOrEmpty(MapName))
                {
                    getGLMapBase.MapName = MapName;
                }

                getGLMapBase.ExecuteCmdletBase(this);
            }
        }
    }
}
