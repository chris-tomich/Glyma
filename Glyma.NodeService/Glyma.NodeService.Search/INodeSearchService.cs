using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.Xml.Linq;
using System.ServiceModel.Syndication;

namespace Glyma.NodeService.Search
{
    [ServiceContract]
    [ServiceKnownType(typeof(Atom10FeedFormatter))]
    [ServiceKnownType(typeof(Rss20FeedFormatter))]
    public interface INodeSearchService
    {
        [OperationContract]
        [WebGet(UriTemplate = "*")]
        XElement GetOpenSearchDefinition();

        [OperationContract]
        [WebInvoke(UriTemplate = "search?q={searchTerms}&pw={page}&format={format}&d={domainId}&s={siteId}", Method = "GET", BodyStyle = WebMessageBodyStyle.WrappedRequest, RequestFormat = WebMessageFormat.Xml, ResponseFormat = WebMessageFormat.Xml)]
        SyndicationFeedFormatter SearchGlymaDatabase(string searchTerms, int page, string format, Guid domainId, Guid siteId);
    }
}
