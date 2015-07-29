using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Linq;

namespace Glyma.NodeService.Search
{
    public partial class OpenSearchDescriptionDocument
    {
        private const string OpenSearch11NamespaceUri = "http://a9.com/-/spec/opensearch/1.1/";

        private XNamespace _namespace;
        private XDocument _document;
        private XElement _shortNameElement;
        private XElement _descriptionElement;
        private XElement _tagsElement;
        private XElement _contactElement;
        private XElement _urlElement;
        private XElement _rootElement;
        private XAttribute _feedTypeAttribute;
        private XAttribute _templateAttribute;

        protected XNamespace OpenSearch11Namespace
        {
            get
            {
                _namespace = OpenSearch11NamespaceUri;

                return _namespace;
            }
        }

        protected XElement ShortNameElement
        {
            get
            {
                _shortNameElement = new XElement(OpenSearch11Namespace + "ShortName", ShortName);

                return _shortNameElement;
            }
        }

        protected XElement DescriptionElement
        {
            get
            {
                _descriptionElement = new XElement(OpenSearch11Namespace + "Description", Description);

                return _descriptionElement;
            }
        }

        protected XElement TagsElement
        {
            get
            {
                _tagsElement = new XElement(OpenSearch11Namespace + "Tags", Tags);

                return _tagsElement;
            }
        }

        protected XElement ContactElement
        {
            get
            {
                _contactElement = new XElement(OpenSearch11Namespace + "Contact", Contact);

                return _contactElement;
            }
        }

        protected XAttribute FeedTypeAttribute
        {
            get
            {
                string feedTypeValue;

                switch (FeedType)
                {
                    case FeedFormat.Atom10:
                        feedTypeValue = "application/atom+xml";
                        break;

                    case FeedFormat.Rss20:
                        feedTypeValue = "application/rss+xml";
                        break;

                    case FeedFormat.Html:
                        goto default;

                    default:
                        feedTypeValue = "text/html";
                        break;
                }

                _feedTypeAttribute = new XAttribute("type", feedTypeValue);

                return _feedTypeAttribute;
            }
        }

        protected XAttribute TemplateAttribute
        {
            get
            {
                _templateAttribute = new XAttribute("template", Template);

                return _templateAttribute;
            }
        }

        protected XElement UrlElement
        {
            get
            {
                _urlElement = new XElement(OpenSearch11Namespace + "Url", FeedTypeAttribute, TemplateAttribute);

                return _urlElement;
            }
        }

        protected XElement RootElement
        {
            get
            {
                _rootElement = new XElement(OpenSearch11Namespace + "OpenSearchDescription",
                                            ShortNameElement,
                                            DescriptionElement,
                                            TagsElement,
                                            ContactElement,
                                            UrlElement);

                return _rootElement;
            }
        }

        protected XDocument Document
        {
            get
            {
                _document = new XDocument(RootElement);

                return _document;
            }
        }

        public XDocument ToXDocument()
        {
            return Document;
        }
    }
}