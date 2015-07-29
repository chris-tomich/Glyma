/// <reference path="FeedFilter.ts" />
/// <reference path="SearchMapSOAPUtil.ts" />
module Glyma.Search {
    export interface SearchMapParameters {
        DomainUid: string;
        RootMapUid: string;
        Filters: FeedFilter[];
        SortOrder: SearchMapSortOrder;
        SearchOperation: SearchMapOperation;
        PageNumber: number;
        PageSize: number;
        CompletedCallback?: (results: Glyma.Model.Node[], pageNumber: number, pageSize: number, totalItems: number) => void;
        ErrorProcessingCompletedCallback?: (message: string) => void;
        FailCallback?: (request: string, data: string, status: string) => void;
    }
} 