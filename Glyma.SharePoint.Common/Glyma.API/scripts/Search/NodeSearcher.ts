/// <reference path="../Model/NodeType.ts" />
/// <reference path="../Helpers/NodeTypeResolver.ts" />
/// <reference path="../Helpers/Utils.ts" />
/// <reference path="FeedFilter.ts" />
/// <reference path="SearchMapSOAPUtil.ts" />
module Glyma.Search {
    import Model = Glyma.Model;
    import Helpers = Glyma.Helpers;

    export interface NodeSearcherConfig {
        ConfigId: string;
        BaseUrl: string;
    }

    export class NodeSearcher {
        private _configId: string = null;
        private _baseUrl: string = null;

        constructor(options: NodeSearcherConfig) {
            this._configId = options.ConfigId;
            this._baseUrl = options.BaseUrl;
        }

        public setConfigId(configId: string): void {
            this._configId = configId;
        }

        public getConfigId(): string {
            return this._configId;
        }

        public setBaseUrl(baseUrl: string): void {
            this._baseUrl = baseUrl;
        }

        public getBaseUrl(): string {
            return this._baseUrl;
        }

        public searchMap(parameters: SearchMapParameters): void {
            var params: SOAPInvokeParams = {
                url: this.getBaseUrl() + '/_vti_bin/SevenSigma/TransactionalMappingToolService.svc',
                method: 'SearchMap',
                namespace: 'http://sevensigma.com.au/TransactionalNodeService',
                SOAPAction: 'http://sevensigma.com.au/TransactionalNodeService/ITransactionalMappingToolService/SearchMap',
                parameters: {
                    callingUrl: this.getConfigId(),
                    domainId: parameters.DomainUid,
                    rootMapUid: parameters.RootMapUid,
                    pageNumber: parameters.PageNumber,
                    pageSize: parameters.PageSize,
                    sortOrder: parameters.SortOrder,
                    searchOperation: parameters.SearchOperation,
                    conditions: { metadataFilters: [] }
                },
                success: function (data, status, responseObject) {
                    try {
                        var results: any = [];

                        var items = $(responseObject.responseXML).find("a\\:SearchedNodes, SearchedNodes").children();
                        var itemCountStr: string = $(responseObject.responseXML).find("a\\:Count, Count").text();
                        var itemCount: number = parseInt(itemCountStr);

                        $.each(items, function (index) {
                            $.each($(this), function (i2) {
                                var mapNodeName = $(this).find("a\\:MapNodeName, MapNodeName").text();
                                var mapNodeUid = $(this).find("a\\:MapNodeUid, MapNodeUid").text();
                                var modifiedBy = $(this).find("a\\:ModifiedBy, ModifiedBy").text();

                                var modifiedDateTimeRaw = $(this).find("a\\:Modified, Modified").text();
                                var modifiedDateTime: Date = Helpers.Utils.GetDateTime(modifiedDateTimeRaw);
                                var modifiedDateTimeStr = Helpers.Utils.FormatDateString(modifiedDateTime);

                                var createdBy = $(this).find("a\\:CreatedBy, CreatedBy").text();

                                var createdDateTimeRaw = $(this).find("a\\:Created, Created").text();
                                var createdDateTime: Date = Helpers.Utils.GetDateTime(createdDateTimeRaw);
                                var createdDateTimeStr = Helpers.Utils.FormatDateString(createdDateTime);

                                var nodeTypeUid = $(this).find("a\\:NodeTypeUid, NodeTypeUid").text();
                                var nodeType = Helpers.NodeTypeResolver.GetNodeType(nodeTypeUid);
                                var nodeName = NodeSearcher.GetMetadataValue($(this).children("b\\:Value, Value"), "Name");
                                var nodeUid = $(this).find("a\\:NodeUid, NodeUid").text();

                                var domainId = $(this).find("a\\:DomainUid, DomainUid").text();

                                var metaDataTable = NodeSearcher.GetMetadataArray($(this).children("b\\:Value, Value"));

                                var rootMapUid = $(this).find("a\\:RootMapUid, RootMapUid").text();

                                var node: Glyma.Model.Node = {
                                    NodeId: nodeUid,
                                    Name: nodeName,
                                    NodeType: nodeType,
                                    Map: mapNodeName,
                                    MapNodeId: mapNodeUid,
                                    Modified: modifiedDateTimeStr,
                                    ModifiedDateObj: modifiedDateTime,
                                    ModifiedBy: modifiedBy,
                                    Created: createdDateTimeStr,
                                    CreatedDateObj: createdDateTime,
                                    CreatedBy: createdBy,
                                    DomainId: domainId,
                                    RootMapId: rootMapUid,
                                    Metadata: metaDataTable
                                }

                            results.push(node);
                            });
                        });

                        switch (parameters.SortOrder) {
                            case SearchMapSortOrder.ModifiedDescending:
                                //sort the list by the last modified dates in descending order
                                results.sort(NodeSearcher.SortModifiedDescending);
                                break;
                            case SearchMapSortOrder.ModifiedAscending:
                                //sort the list by the last modified dates in ascending order
                                results.sort(NodeSearcher.SortModifiedAscending);
                                break;
                            case SearchMapSortOrder.CreatedDescending:
                                //sort the list by the created dates in descending order
                                results.sort(NodeSearcher.SortCreatedDescending);
                                break;
                            case SearchMapSortOrder.CreatedAscending:
                                //sort the list by the created dates in ascending order
                                results.sort(NodeSearcher.SortCreatedAscending);
                                break;
                        }

                        if (parameters.CompletedCallback != null && parameters.CompletedCallback != undefined) {
                            parameters.CompletedCallback(results, parameters.PageNumber, parameters.PageSize, itemCount);
                        }
                    }
                    catch (err) {
                        if (parameters.ErrorProcessingCompletedCallback != null && parameters.ErrorProcessingCompletedCallback != undefined) {
                            parameters.ErrorProcessingCompletedCallback("Failed to process the Glyma search response.");
                        }
                    }
                },
                fail: function (request, data, status) {
                    if (parameters.FailCallback != null && parameters.FailCallback != undefined) {
                        parameters.FailCallback(request, data, status);
                    }
                }
            };

            NodeSearcher.BuildFilters(params, parameters.Filters);

            SearchMapSOAPUtil.Request(params);
        }

        private static BuildFilters(params: SOAPInvokeParams, filters: FeedFilter[]) {
            $.each(filters, function (index, filter) {
                var conditionValue = filter.Value;
                var searchType: SearchMapSearchType = SearchMapSearchType.Exact;
                switch (filter.Type) {
                    case FeedFilterType.Boolean:
                        /* True/False values */
                        if (filter.Value == true) {
                            conditionValue = "true";
                        }
                        else {
                            conditionValue = "false";
                        }
                        break;
                    case FeedFilterType.ContainsString:
                        searchType = SearchMapSearchType.Contains;
                        break;
                    case FeedFilterType.FreeTextString:
                        searchType = SearchMapSearchType.FreeText;
                        break;
                    case FeedFilterType.WildcardString:
                        searchType = SearchMapSearchType.Wildcard;
                        break;
                    case FeedFilterType.ExactString:
                        searchType = SearchMapSearchType.Exact;
                        break;
                    case FeedFilterType.LessThanNumerical:
                        searchType = SearchMapSearchType.NumericallyLessThan;
                        break;
                    case FeedFilterType.LessThanOrEqualNumerical:
                        searchType = SearchMapSearchType.NumericallyLessThanOrEqual;
                        break;
                    case FeedFilterType.MoreThanNumerical:
                        searchType = SearchMapSearchType.NumericallyMoreThan;
                        break;
                    case FeedFilterType.MoreThanOrEqualNumerical:
                        searchType = SearchMapSearchType.NumericallyMoreThanOrEqual;
                        break;
                    case FeedFilterType.EqualsNumerical:
                        /* This will convert to the same SQL as FeedFilterType.ExactString */
                        searchType = SearchMapSearchType.NumericallyEqual;
                        break;
                }
                params.parameters.conditions.metadataFilters.push(
                    {
                        conditionValue: conditionValue,
                        metadataName: filter.Name,
                        searchType: searchType
                    });
            });
        }

        private static GetMetadataValue(xmldom, keyName): string {
            var metadataPairs = $(xmldom).find("b\\:KeyValueOfstringstring, KeyValueOfstringstring");

            var result = "";
            $.each(metadataPairs, function (index) {
                var key = $(this).find("b\\:Key, Key").text(); //different browsers need require/don't require the namespace prefix, test both
                if (key == keyName) {
                    result = $(this).find("b\\:Value, Value").text(); 
                    return false;
                }
            });
            return result;
        }

        private static GetMetadataArray(xmldom): Array<Model.MetadataItem> {
            var metadataPairs = $(xmldom).find("b\\:KeyValueOfstringstring, KeyValueOfstringstring");

            var result: Array<Model.MetadataItem> = new Array<Model.MetadataItem>();
            $.each(metadataPairs, function (index, valueOfElement) {
                var key = $(this).find("b\\:Key, Key").text();
                var value = $(this).find("b\\:Value, Value").text();
                var metadataObj: Model.MetadataItem = { Key: key, Value: value };
                result.push(metadataObj);
            });
            return result;
        }

        private static SortModifiedDescending(a: Model.Node, b: Model.Node): number {
            if (a.ModifiedDateObj > b.ModifiedDateObj) {
                return -1;
            }
            else if (a.ModifiedDateObj < b.ModifiedDateObj) {
                return 1;
            }
            return 0;
        }

        private static SortModifiedAscending(a: Model.Node, b: Model.Node): number {
            if (a.ModifiedDateObj < b.ModifiedDateObj) {
                return -1;
            }
            else if (a.ModifiedDateObj > b.ModifiedDateObj) {
                return 1;
            }
            return 0;
        }

        private static SortCreatedDescending(a: Model.Node, b: Model.Node): number {
            if (a.CreatedDateObj > b.CreatedDateObj) {
                return -1;
            }
            else if (a.CreatedDateObj < b.CreatedDateObj) {
                return 1;
            }
            return 0;
        }

        private static SortCreatedAscending(a: Model.Node, b: Model.Node): number {
            if (a.CreatedDateObj < b.CreatedDateObj) {
                return -1;
            }
            else if (a.CreatedDateObj > b.CreatedDateObj) {
                return 1;
            }
            return 0;
        }
    }
} 