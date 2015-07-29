module Glyma.RelatedContentPanels {
    export class FeedUtils {

        public static SetFeedListIcon(item, iconElement): void {
            switch (item.NodeType) {
                case NodeType.Map:
                    $(iconElement).addClass("map");
                    break;
                case NodeType.Idea:
                    $(iconElement).addClass("idea");
                    break;
                case NodeType.Con:
                    $(iconElement).addClass("con");
                    break;
                case NodeType.Pro:
                    $(iconElement).addClass("pro");
                    break;
                case NodeType.Decision:
                    $(iconElement).addClass("decision");
                    break;
                case NodeType.Question:
                    $(iconElement).addClass("question");
                    break;
                case NodeType.Note:
                    $(iconElement).addClass("note");
                    break;
                case NodeType.Argument:
                    $(iconElement).addClass("argument");
                    break;
                case NodeType.Reference:
                    $(iconElement).addClass("reference");
                    break;
            }
        }

        public static GetActionText(wasCreated: boolean, nodeType: NodeType): { [key: string]: string } {
            var actionText: { [key: string]: string } = {};
            switch (nodeType) {
                case NodeType.Decision:
                case NodeType.Question:
                case NodeType.Note:
                case NodeType.Reference:
                case NodeType.Map:
                    if (wasCreated) {
                        actionText["start"] = " added a " + Utils.NodeTypeToString(nodeType).toLowerCase() + ", ";
                        actionText["end"] = ", to";
                    }
                    else {
                        actionText["start"] = " changed a " + Utils.NodeTypeToString(nodeType).toLowerCase() + ", ";
                        actionText["end"] = ", in";
                    }
                    break;
                case NodeType.Idea:
                    if (wasCreated) {
                        actionText["start"] = " added an idea, ";
                        actionText["end"] = ", to";
                    }
                    else {
                        actionText["start"] = " changed an idea, ";
                        actionText["end"] = ", in";
                    }
                    break;
                case NodeType.Con:
                    if (wasCreated) {
                        actionText["start"] = " provided an opposing argmument, ";
                        actionText["end"] = ", in";
                    }
                    else {
                        actionText["start"] = " changed an opposing argument, ";
                        actionText["end"] = ", in";
                    }
                    break;
                case NodeType.Pro:
                    if (wasCreated) {
                        actionText["start"] = " provided a supporting argument, ";
                        actionText["end"] = ", in";
                    }
                    else {
                        actionText["start"] = " changed a supporting argument, ";
                        actionText["end"] = ", in";
                    }
                    break;
                case NodeType.Argument:
                    if (wasCreated) {
                        actionText["start"] = " provided an argument, ";
                        actionText["end"] = ", in";
                    }
                    else {
                        actionText["start"] = " changed an argument, ";
                        actionText["end"] = ", in";
                    }
                    break;
            }
            return actionText;
        }

        public static ProcessUserName(userName: string): string {
            var normalisedName: string = userName;
            var indexOfSemiColon: number = userName.indexOf(";");
            if (indexOfSemiColon > 0) {
                //AD based names like: S-1-5-21-3851129750-3191885917-1653347546-2606;GLYMADEMO\sp_web
                //or 24;#John User
                normalisedName = normalisedName.substring(indexOfSemiColon + 1);
            }
            var indexOfLastPipe: number = userName.lastIndexOf("|");
            if (indexOfLastPipe > 0) {
                //Claims based names like: 05.t|social network|paul.culmsee@sevensigma.com.au
                normalisedName = normalisedName.substring(indexOfLastPipe + 1);
            }
            normalisedName = normalisedName.trimEnds("#");
            return normalisedName;
        }
    }
} 