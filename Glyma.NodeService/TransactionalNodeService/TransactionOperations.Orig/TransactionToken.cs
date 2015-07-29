using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TransactionalNodeService
{
    public class TransactionToken
    {
        public TransactionToken()
        {
            TokenId = Guid.NewGuid();
            MapObjectId = Guid.Empty;
            DelayedExecutionMapObjectId = Guid.Empty;
        }

        public TransactionToken(Guid tokenId)
        {
            TokenId = tokenId;
            MapObjectId = Guid.Empty;
            DelayedExecutionMapObjectId = Guid.Empty;
        }

        public Guid TokenId
        {
            get;
            set;
        }

        public Guid MapObjectId
        {
            get;
            set;
        }

        public Guid DelayedExecutionMapObjectId
        {
            get;
            set;
        }

        public Guid GetValue()
        {
            if (DelayedExecutionMapObjectId != Guid.Empty)
            {
                return DelayedExecutionMapObjectId;
            }
            else
            {
                return MapObjectId;
            }
        }

        internal void Refresh(TransactionToken token)
        {
            MapObjectId = token.MapObjectId;

            if (token.DelayedExecutionMapObjectId != null)
            {
                DelayedExecutionMapObjectId = token.DelayedExecutionMapObjectId;
            }
        }
    }
}