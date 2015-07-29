module Glyma.Search {
    export interface FeedFilter {
        Name: string;
        Value: any;
        Type: FeedFilterType;
    }

    export enum FeedFilterType {
        Boolean,
        ContainsString,
        ExactString,
        WildcardString,
        FreeTextString,
        LessThanNumerical,
        LessThanOrEqualNumerical,
        MoreThanNumerical,
        MoreThanOrEqualNumerical,
        EqualsNumerical
    }
} 