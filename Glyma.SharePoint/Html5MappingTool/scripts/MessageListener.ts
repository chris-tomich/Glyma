module Glyma {
    export function MessageListener(type: string, msg: string, id: string) {
        var visibleNodeIndexes = Glyma.SharedVariables.mapController.visibleNodeIndexes;
        for (var i = 0; i < visibleNodeIndexes.length; i++) {
            if (Glyma.SharedVariables.mapController.nodes[visibleNodeIndexes[i]].nodeId == id) {
                
                return;
            }
        }

    }
} 