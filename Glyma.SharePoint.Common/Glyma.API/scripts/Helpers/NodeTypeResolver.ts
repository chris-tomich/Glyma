/// <reference path="../Model/NodeType.ts" />
module Glyma.Helpers {
    import NodeType = Glyma.Model.NodeType;

    export class NodeTypeResolver {
        public static GetNodeType(nodeTypeUid: string): NodeType {
            var result: NodeType = NodeType.UnknownType;
            nodeTypeUid = nodeTypeUid.toUpperCase();
            switch (nodeTypeUid) {
                case "3B53600F-39EC-42FB-B08A-325062037130":
                    result = NodeType.Idea;
                    break;
                case "042E7E3B-8A5F-4A52-B1DD-3361A3ACD72A":
                    result = NodeType.Argument;
                    break;
                case "B8C354CB-C7D0-4982-9A0F-6C4368FAB749":
                    result = NodeType.Map;
                    break;
                case "47B75628-7FDF-4440-BF35-8506D3FE6F2A":
                    result = NodeType.Generic;
                    break;
                case "53EC78E3-F189-4340-B251-AAF9D78CF56D":
                    result = NodeType.Decision;
                    break;
                case "DA66B603-F6B3-4ECF-8ED0-AB34A288CF08":
                    result = NodeType.Con;
                    break;
                case "7D3C9B87-F31D-400F-A375-ABC0D1888625":
                    result = NodeType.List;
                    break;
                case "8F3DA942-06C4-4075-AD8B-B51361ABC900":
                    result = NodeType.Reference;
                    break;
                case "99FD1475-8099-45D3-BEDF-BEC396CCB4DD":
                    result = NodeType.Question;
                    break;
                case "84B7634B-DB8D-449B-B8CE-D3F3F80E95DD":
                    result = NodeType.Note;
                    break;
                case "084F38B7-115F-4AF6-9E30-D9D91226F86B":
                    result = NodeType.Pro;
                    break;
            }
            return result;
        }

        public static NodeTypeToString(nodeType: NodeType): string {
            return NodeType[nodeType];
        }
    }
} 