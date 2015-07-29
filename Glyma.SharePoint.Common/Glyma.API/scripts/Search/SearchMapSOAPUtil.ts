module Glyma.Search {
    export interface SOAPInvokeParams {
        url: string;
        namespace: string;
        method: string;
        SOAPAction: string;
        parameters: SearchMapParams;
        success: (data, status, responseObject) => void;
        fail?: (request, status, error) => void;
    }

    export interface SearchMapParams {
        callingUrl: string;
        domainId: string;
        rootMapUid: string;
        conditions?: SearchMapConditions;
        sortOrder: SearchMapSortOrder;
        searchOperation: SearchMapOperation;
        pageNumber: number;
        pageSize: number;
    }

    /*
     * MetadataFilters property of TransactionalNodeService.Common.SearchConditions class equivalent
     */
    export interface SearchMapConditions {
        metadataFilters: Array<MetadataFilters>;
    }

    /*
     * TransactionalNodeService.Common.SearchCondition class equivalent
     */
    export interface MetadataFilters {
        metadataName: string;
        conditionValue: string;
        searchType: SearchMapSearchType;
    }

    /*
     * TransactionalNodeService.Common.SearchType enum mapping
     */
    export enum SearchMapSearchType {
        Exact,
        Contains,
        FreeText,
        Wildcard,
        NumericallyLessThan,
        NumericallyLessThanOrEqual,
        NumericallyMoreThan,
        NumericallyMoreThanOrEqual,
        NumericallyEqual
    }

    /*
     * TransactionalNodeService.Common.SortOrderOptions enum mapping
     */
    export enum SearchMapSortOrder {
        ModifiedDescending,
        ModifiedAscending,
        CreatedDescending,
        CreatedAscending
    }

    /*
     * TransactionalNodeService.Common.SearchOperation enum mapping
     */
    export enum SearchMapOperation {
        AND,
        OR
    }

    /* Builds the SOAP Envelope for the SearchMap function of the WCF SOAP endpoint */
    export class SOAPEnvelope {
        private _soapEnvelopeXml: string = null;
        
        constructor(params: SOAPInvokeParams) {

            var metadataFilters = "<tns:MetadataFilters/>";
            if (params.parameters.conditions.metadataFilters.length > 0) {
                metadataFilters = "<tns:MetadataFilters>";
                $.each(params.parameters.conditions.metadataFilters, function (index, value) {
                    metadataFilters += "<tns:SearchCondition>";
                    metadataFilters += "<tns:ConditionValue>" + value.conditionValue + "</tns:ConditionValue>";
                    metadataFilters += "<tns:MetadataName>" + value.metadataName + "</tns:MetadataName>";
                    var searchType: string = SearchMapSearchType[value.searchType];
                    metadataFilters += "<tns:SearchType>" + searchType + "</tns:SearchType>";
                    metadataFilters += "</tns:SearchCondition>";
                });
                metadataFilters += "</tns:MetadataFilters>";
            }

            var sortOrder: string = SearchMapSortOrder[params.parameters.sortOrder]; //Converts the enum to the string value
            var searchOperation: string = SearchMapOperation[params.parameters.searchOperation]; //Converts the enum to the string value
            this._soapEnvelopeXml = "<soap:Envelope xmlns:soap='http://schemas.xmlsoap.org/soap/envelope/'>" +
                "<soap:Body>" +
                "<" + params.method + " xmlns='" + params.namespace + "'>" +
                "<callingUrl>"+params.parameters.callingUrl+"</callingUrl>" +
                "<domainId>"+params.parameters.domainId+"</domainId>" +
                "<rootMapUid>"+params.parameters.rootMapUid+"</rootMapUid>" +
                "<conditions xmlns:i='http://www.w3.org/2001/XMLSchema-instance' xmlns:tns='http://schemas.datacontract.org/2004/07/TransactionalNodeService.Common'>" +
                metadataFilters +
                "<tns:SearchOperation>" + searchOperation + "</tns:SearchOperation>" +
                "<tns:SortOrder>" + sortOrder + "</tns:SortOrder>" +
                "</conditions>" +
                "<pageNumber>"+params.parameters.pageNumber.toString()+"</pageNumber>" +
                "<pageSize>"+params.parameters.pageSize.toString()+"</pageSize>" +
                "</SearchMap>" +
                "</soap:Body>" + 
                "</soap:Envelope>";
        }

        public toString(): string {
            return this._soapEnvelopeXml;
        }
    }

    /* Does the SOAP Request to SearchMap function and passes the responses to the SOAP parameters */
    export class SearchMapSOAPUtil {
        public static Request(params: SOAPInvokeParams): void {
            var message: SOAPEnvelope = SearchMapSOAPUtil.BuildSoapEnvelope(params);
            $.ajax({
                url: params.url,
                accepts: "application/xml, text/xml, */*; q=0.01",
                type: "POST",
                headers: { "SOAPAction": params.SOAPAction},
                dataType: "xml",
                data: message.toString(),
                processData: false,
                contentType: "text/xml; charset=\"utf-8\"",
                success: params.success,
                error: params.fail
            });
        }

        /* Build the SOAP Envelope */
        private static BuildSoapEnvelope(params: SOAPInvokeParams): SOAPEnvelope {
            return new SOAPEnvelope(params);
        }
    }
} 