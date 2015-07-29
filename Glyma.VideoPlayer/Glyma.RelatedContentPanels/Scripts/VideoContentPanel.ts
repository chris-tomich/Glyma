/// <reference path="RelatedContentPanel.ts"/>
module Glyma.RelatedContentPanels {
    export class VideoContentPanel extends RelatedContentPanel {

        constructor(config: RelatedContentPanelConfig, serverRelativeVersionedLayoutsFolder:string) {
            super(config, serverRelativeVersionedLayoutsFolder);
        }
    }
} 