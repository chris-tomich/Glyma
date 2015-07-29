using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TransactionalNodeService
{
    public class DelayedNodeService
    {
        public DelayedNodeService()
        {
        }

        public Guid BeginTransaction()
        {
            throw new NotImplementedException();
        }

        public MapParameter DomainCreate(Guid token)
        {
            throw new NotImplementedException();
        }

        public MapParameter NodeCreate(Guid token, MapParameter delayedDomain, Guid CompendiumQuestionNodeTypeId)
        {
            throw new NotImplementedException();
        }

        public MapParameter RelationshipCreate(Guid token, MapParameter delayedDomain, Guid FromToRelationshipTypeId, IDictionary<Guid, MapParameter> nodes)
        {
            throw new NotImplementedException();
        }

        public MapResponse EndTransaction(Guid token)
        {
            throw new NotImplementedException();
        }
    }
}