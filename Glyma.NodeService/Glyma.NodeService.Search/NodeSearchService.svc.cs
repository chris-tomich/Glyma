using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.ServiceModel.Activation;
using System.Xml.Linq;
using System.ServiceModel.Syndication;
using Sql = Glyma.NodeService.Search.Model.Sql;
using Memory = Glyma.NodeService.Search.Model.Memory;
using Microsoft.SharePoint;
using System.Web;
using System.Data.SqlClient;

namespace Glyma.NodeService.Search
{
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class NodeSearchService : INodeSearchService
    {
        private const string NodeSearchServiceFilename = "NodeSearchService.svc";

        private string BuildRequestedServiceUrl()
        {
            Uri currentServerUri = OperationContext.Current.RequestContext.RequestMessage.Headers.To;

            string requestedUrl = currentServerUri.ToString();

            int indexOfNodeSearchServiceFile = requestedUrl.IndexOf(NodeSearchServiceFilename, StringComparison.CurrentCultureIgnoreCase);

            int sizeOfUrl = indexOfNodeSearchServiceFile + NodeSearchServiceFilename.Length;

            return requestedUrl.Substring(0, sizeOfUrl);
        }

        public XElement GetOpenSearchDefinition()
        {
            string requestedUrl = BuildRequestedServiceUrl();

            OpenSearchDescriptionDocument openSearchDefinition = new OpenSearchDescriptionDocument();
            openSearchDefinition.ShortName = "Glyma Search Service";
            openSearchDefinition.Description = "The Glyma search service provides search results for maps contained in a Glyma hypergraph database.";
            openSearchDefinition.Tags = "glyma hypergraph graph map";
            openSearchDefinition.Contact = "chris.tomich@sevensigma.com.au";
            openSearchDefinition.FeedType = FeedFormat.Rss20;
            openSearchDefinition.Template = requestedUrl + "/search?q={searchTerms}&amp;pw={startPage}&amp;format={format}";

            return openSearchDefinition.ToXDocument().Root;
        }

        public SyndicationFeedFormatter SearchGlymaDatabase(string searchTerms, int page, string format, Guid domainId, Guid siteId)
        {
            SyndicationFeed syndicationFeed = new SyndicationFeed();

            SPSecurity.RunWithElevatedPrivileges(delegate()
            {
                SPSite site;

                if (siteId != Guid.Empty)
                {
                    site = new SPSite(siteId);
                }
                else
                {
                    string fullUrl = HttpContext.Current.Request.Url.AbsoluteUri;

                    int indexOfEndOfWebUrl = fullUrl.IndexOf("/_vti_bin");
                    string webUrl = fullUrl.Substring(0, indexOfEndOfWebUrl);

                    site = new SPSite(webUrl);
                }

                try
                {
                    using (SPWeb web = site.OpenWeb())
                    {
                        string databaseServer = web.Properties["Glyma.DatabaseServer"];
                        string databaseName = web.Properties["Glyma.DatabaseName"];
                        string defaultPage = web.Properties["Glyma.DefaultPage"];

                        SqlConnection connection = new SqlConnection("Data Source=" + databaseServer + ";Initial Catalog=" + databaseName + ";Integrated Security=True;");

                        using (Sql.GlymaDatabaseDataContext dataContext = new Sql.GlymaDatabaseDataContext(connection))
                        {
                            var basicSearchResults = dataContext.BasicSearchFullResults(domainId, searchTerms);

                            var matchingNodes = basicSearchResults.GetResult<Sql.Node>();
                            var matchingMetadata = basicSearchResults.GetResult<Sql.Metadata>();

                            List<Sql.Metadata> allMetadata = new List<Sql.Metadata>(matchingMetadata);

                            List<SyndicationItem> nodeResults = new List<SyndicationItem>();

                            foreach (var matchingNode in matchingNodes)
                            {
                                Memory.Node memoryNode = new Memory.Node(matchingNode);

                                /// TODO: The performance can really be improved on the following.
                                memoryNode.FillMetadata(allMetadata);

                                Uri syndicationUri = new Uri(defaultPage + "?DomainUid=" + domainId.ToString() + "&NodeUid=" + memoryNode.Id.ToString(), UriKind.Relative);

                                SyndicationItem nodeResult = new SyndicationItem(memoryNode.Metadata["Name"], memoryNode.ToString(), syndicationUri);
                                nodeResults.Add(nodeResult);
                            }

                            syndicationFeed.Title = new TextSyndicationContent("Glyma Database Results");
                            syndicationFeed.Description = new TextSyndicationContent("Results for the searchTerms - '" + searchTerms + "'");
                            syndicationFeed.Items = nodeResults;
                        }
                    }
                }
                finally
                {
                    if (site != null)
                    {
                        site.Dispose();
                    }
                }
            });

            if (format == "atom")
            {
                return new Atom10FeedFormatter(syndicationFeed);
            }
            else if (format == "rss")
            {
                return new Rss20FeedFormatter(syndicationFeed);
            }
            else
            {
                return new Rss20FeedFormatter(syndicationFeed);
            }
        }
    }
}
