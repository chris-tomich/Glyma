using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TransactionalNodeService
{
    public class TransactionTokenFactory
    {
        private Dictionary<Guid, TransactionToken> _transactionTokens;

        public TransactionTokenFactory()
        {
            _transactionTokens = new Dictionary<Guid, TransactionToken>();
        }

        public TransactionToken CreateToken()
        {
            return CreateToken(Guid.NewGuid());
        }

        public TransactionToken CreateToken(Guid tokenId)
        {
            TransactionToken newToken = new TransactionToken(tokenId);

            _transactionTokens[newToken.TokenId] = newToken;

            return newToken;
        }

        public TransactionToken GetToken(Guid tokenId)
        {
            if (_transactionTokens.ContainsKey(tokenId))
            {
                return _transactionTokens[tokenId];
            }
            else
            {
                return CreateToken(tokenId);
            }
        }

        public TransactionToken ProcessToken(TransactionToken token)
        {
            if (_transactionTokens.ContainsKey(token.TokenId))
            {
                if (_transactionTokens[token.TokenId] == token)
                {
                    return token;
                }
                else
                {
                    _transactionTokens[token.TokenId].Refresh(token);

                    return _transactionTokens[token.TokenId];
                }
            }
            else
            {
                return _transactionTokens[token.TokenId] = token;
            }
        }
    }
}