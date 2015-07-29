using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Glyma.Powershell.Model
{
    public class Map
    {
        public Model.Domain Domain
        {
            get;
            set;
        }

        public Guid NodeId
        {
            get;
            set;
        }

        public string Name
        {
            get;
            set;
        }

        /// <summary>
        /// Returns true if valid, false otherwise.
        /// </summary>
        /// <returns></returns>
        public bool CheckIsValid()
        {
            /// Check that the NodeId is provided.
            if (NodeId == Guid.Empty)
            {
                return false;
            }

            /// Check the Domain validity.
            if (!Domain.CheckIsValid())
            {
                return false;
            }

            return true;
        }
    }
}
