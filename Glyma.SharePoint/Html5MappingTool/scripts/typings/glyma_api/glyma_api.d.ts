declare module Glyma.Search {
    interface SOAPInvokeParams {
        url: string;
        namespace: string;
        method: string;
        SOAPAction: string;
        parameters: SearchMapParams;
        success: (data: any, status: any, responseObject: any) => void;
        fail?: (request: any, status: any, error: any) => void;
    }
    interface SearchMapParams {
        callingUrl: string;
        domainId: string;
        rootMapUid: string;
        conditions?: SearchMapConditions;
        sortOrder: SearchMapSortOrder;
        searchOperation: SearchMapOperation;
        pageNumber: number;
        pageSize: number;
    }
    interface SearchMapConditions {
        metadataFilters: MetadataFilters[];
    }
    interface MetadataFilters {
        metadataName: string;
        conditionValue: string;
        searchType: SearchMapSearchType;
    }
    enum SearchMapSearchType {
        Exact = 0,
        Contains = 1,
        FreeText = 2,
        Wildcard = 3,
    }
    enum SearchMapSortOrder {
        ModifiedDescending = 0,
        ModifiedAscending = 1,
    }
    enum SearchMapOperation {
        AND = 0,
        OR = 1,
    }
    class SOAPEnvelope {
        private _soapEnvelopeXml;
        constructor(params: SOAPInvokeParams);
        public toString(): string;
    }
    class SearchMapSOAPUtil {
        static Request(params: SOAPInvokeParams): void;
        private static BuildSoapEnvelope(params);
    }
}
declare module Glyma.Search {
    interface FeedFilter {
        Name: string;
        Value: any;
        Type: FeedFilterType;
    }
    enum FeedFilterType {
        Boolean = 0,
        ContainsString = 1,
        ExactString = 2,
        WildcardString = 3,
        FreeTextString = 4,
    }
}
declare module Glyma.Search {
    interface SearchMapParameters {
        DomainUid: string;
        RootMapUid: string;
        Filters: FeedFilter[];
        SortOrder: SearchMapSortOrder;
        SearchOperation: SearchMapOperation;
        PageNumber: number;
        PageSize: number;
        CompletedCallback?: (results: Model.Node[], pageNumber: number, pageSize: number, totalItems: number) => void;
        ErrorProcessingCompletedCallback?: (message: string) => void;
        FailCallback?: (request: string, data: string, status: string) => void;
    }
}
declare module Glyma.UI {
    class ContextMenuController {
        private static _instance;
        constructor();
        static getInstance(): ContextMenuController;
        public AddAuthorContextMenuItem(contextMenuItemName: string, callback: (rootMap: string, map: string, node: string) => void): void;
        public AddReaderContextMenuItem(contextMenuItemName: string, callback: (rootMap: string, map: string, node: string) => void): void;
    }
}
declare module Glyma.Model {
    enum NodeType {
        UnknownType = 0,
        Idea = 1,
        Argument = 2,
        Map = 3,
        Generic = 4,
        Decision = 5,
        Con = 6,
        List = 7,
        Reference = 8,
        Question = 9,
        Note = 10,
        Pro = 11,
    }
}
declare module Glyma.Helpers {
    class NodeTypeResolver {
        static GetNodeType(nodeTypeUid: string): Model.NodeType;
        static NodeTypeToString(nodeType: Model.NodeType): string;
    }
}
declare module Glyma.Model {
    interface MetadataItem {
        Key: string;
        Value: string;
    }
}
declare module Glyma.Model {
    interface Node {
        NodeId: string;
        Name: string;
        NodeType: NodeType;
        Map: string;
        MapNodeId: string;
        Modified: string;
        ModifiedDateObj: Date;
        ModifiedBy: string;
        Created: string;
        CreatedDateObj: Date;
        CreatedBy: string;
        DomainId: string;
        RootMapId: string;
        Metadata: MetadataItem[];
    }
}
declare module Glyma.Model {
    enum VideoPlayerType {
        SILVERLIGHT = 0,
        YOUTUBE = 1,
        HTML5 = 2,
    }
}
declare module Glyma.Helpers {
    class Utils {
        static GetPlayerType(videoUrl: string): Model.VideoPlayerType;
        static GetYouTubeVideoId(videoUrl: string): any;
        static ConvertToYouTubeEmbedUrl(videoSourceUrl: any): string;
        static ConvertSecondsToTimeSpanString(secondsStr: string): string;
        static ConvertTimeSpanToSeconds(timeSpanString: string): number;
        static QueryString(key: any): any[];
        static GetDateTime(dateString: any): Date;
        private static PadDigits(num, digits);
        static FormatDateString(date: Date): string;
        static CalculateScrollbarWidth(): number;
        static CalculateActualHeight(element: JQuery): number;
    }
}
declare module Glyma.Search {
    interface NodeSearcherConfig {
        ConfigId: string;
        BaseUrl: string;
    }
    class NodeSearcher {
        private _configId;
        private _baseUrl;
        constructor(options: NodeSearcherConfig);
        public setConfigId(configId: string): void;
        public getConfigId(): string;
        public setBaseUrl(baseUrl: string): void;
        public getBaseUrl(): string;
        public searchMap(parameters: SearchMapParameters): void;
        private static BuildFilters(params, filters);
        private static GetMetadataValue(xmldom, keyName);
        private static GetMetadataArray(xmldom);
        private static SortDescending(a, b);
        private static SortAscending(a, b);
    }
}
