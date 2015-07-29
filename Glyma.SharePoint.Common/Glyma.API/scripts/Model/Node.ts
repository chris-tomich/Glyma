module Glyma.Model {
    export interface Node {
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