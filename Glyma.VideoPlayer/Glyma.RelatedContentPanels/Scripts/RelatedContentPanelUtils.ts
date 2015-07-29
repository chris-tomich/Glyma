module Glyma.RelatedContentPanels {
    export enum VideoPlayerType {
        SILVERLIGHT,
        YOUTUBE,
        HTML5
    }

    export enum NodeType {
        UnknownType,
        Idea,
        Argument,
        Map,
        Generic,
        Decision,
        Con,
        List,
        Reference,
        Question,
        Note,
        Pro
    }

    export class Utils {
        public static EncodeCallingUrl(callingUrl: string): string {
            callingUrl = callingUrl.replace(/&/g, '&amp;'); //remove ampersands out of the calling URL to make it XML safe
            return callingUrl;
        }

        static GetPlayerType(videoUrl: string): VideoPlayerType {
            var videoId = Utils.GetYouTubeVideoId(videoUrl);
            if (videoId != false) {
                return VideoPlayerType.YOUTUBE;
            }
            else {
                //TODO: Handling of HTML5 player or SilverlightPlayer
                return VideoPlayerType.SILVERLIGHT;
            }
        }

        static GetYouTubeVideoId(videoUrl: string): any {
            var p = /^(?:https?:\/\/)?(?:www\.|m\.)?(?:youtu\.be\/|youtube\.com\/(?:embed\/|v\/|watch\?v=|watch\?.+&v=))((\w|-){11})(?:\S+)?$/;
            return (videoUrl.match(p)) ? RegExp.$1 : false;
        }

        static ConvertToYouTubeEmbedUrl(videoSourceUrl): string {
            var videoId = Utils.GetYouTubeVideoId(videoSourceUrl);
            if (videoId != false) {
                return "https://www.youtube.com/embed/" + videoId;
            }
            else {
                return null;
            }
        }

        static ConvertSecondsToTimeSpanString(secondsStr:string):string {
            var seconds:number = parseFloat(secondsStr);
            if (!isNaN(seconds)) {
                var msPerSecond = 1000;
                var msPerMinute = 60000;
                var msPerHour = 3600000;
                var totalMilliseconds = seconds * msPerSecond;

                var milliseconds = Math.floor(totalMilliseconds) % 1000;
                var seconds = Math.floor(totalMilliseconds / msPerSecond) % 60;
                var minutes = Math.floor(totalMilliseconds / msPerMinute) % 60;
                var hours = Math.floor(totalMilliseconds / msPerHour);

                var hoursStr = hours + "";
                if (hoursStr.length < 2) { hoursStr = "0" + hoursStr; }
                var minutesStr = minutes + "";
                if (minutesStr.length < 2) { minutesStr = "0" + minutesStr; }
                var secondsStr = seconds + "";
                if (secondsStr.length < 2) { secondsStr = "0" + secondsStr; }

                return hoursStr + ":" + minutesStr + ":" + secondsStr + "." + milliseconds;
            }
            else {
                return "00:00:00.000"; //invalid seconds provided
            }
        }

        static ConvertTimeSpanToSeconds(timeSpanString:string):number {
            if (timeSpanString != undefined && timeSpanString != null && timeSpanString.length > 10) {
                var hours = parseInt(timeSpanString.substr(0, 2));
                if (isNaN(hours)) return 0;//invalid TimeSpan string provided
                var minutes = parseInt(timeSpanString.substr(3, 2));
                if (isNaN(minutes)) return 0;//invalid TimeSpan string provided
                var seconds = parseInt(timeSpanString.substr(6, 2));
                if (isNaN(seconds)) return 0;//invalid TimeSpan string provided
                var fractionOfSeconds = parseInt(timeSpanString.substr(9));
                if (isNaN(fractionOfSeconds)) return 0;//invalid TimeSpan string provided

                var msPerSecond = 1000;
                var msPerMinute = 60000;
                var msPerHour = 3600000;

                var totalMilliseconds = (hours * msPerHour) + (minutes * msPerMinute) + (seconds * msPerSecond);;
                var totalSeconds = Math.floor(totalMilliseconds / msPerSecond);
                return parseFloat(totalSeconds + "." + fractionOfSeconds);
            }
            else {
                return 0; //invalid TimeSpan string provided
            }
        }

        static QueryString(key) {
            var re = new RegExp('(?:\\?|&)' + key + '=(.*?)(?=&|$)', 'gi');
            var r = [], m;
            while ((m = re.exec(document.location.search)) != null) r.push(m[1]);
            return r;
        }

        static GetDateTime(dateString): Date {
            //The datetime string looks like this "2012-11-01T00:00:00"
            if (dateString != undefined && dateString.length > 17) {
                var year = dateString.substring(0, 4);
                var month = dateString.substring(5, 7);
                var day = dateString.substring(8, 10);
                var hour = dateString.substring(11, 13);
                var minute = dateString.substring(14, 16);

                var date = new Date(parseInt(year, 10), parseInt(month, 10) - 1, parseInt(day, 10), parseInt(hour, 10), parseInt(minute, 10), 0);
                return date;
            }
            return null;
        }

        private static PadDigits(num: number, digits: number):string {
            return Array(Math.max(digits - String(num).length + 1, 0)).join("0") + num;
        }

        static FormatDateString(date: Date): string {
            if (date != null) {
                var localYear: number = date.getFullYear();
                var localMonth: number = date.getMonth();
                var localDate: number = date.getDate();
                var localHour: number = date.getHours();
                var localMinute: number = date.getMinutes();

                var monthName;
                switch (localMonth) {
                    case 0:
                        monthName = "Jan";
                        break;
                    case 1:
                        monthName = "Feb";
                        break;
                    case 2:
                        monthName = "Mar";
                        break;
                    case 3:
                        monthName = "Apr";
                        break;
                    case 4:
                        monthName = "May";
                        break;
                    case 5:
                        monthName = "Jun";
                        break;
                    case 6:
                        monthName = "Jul";
                        break;
                    case 7:
                        monthName = "Aug";
                        break;
                    case 8:
                        monthName = "Sept";
                        break;
                    case 9:
                        monthName = "Oct";
                        break;
                    case 10:
                        monthName = "Nov";
                        break;
                    case 11:
                        monthName = "Dec";
                        break;
                }
                return Utils.PadDigits(localHour,2) + ":" + Utils.PadDigits(localMinute,2) + " " + localDate + " " + monthName + ", " + localYear;
            }
            else {
                return "";
            }
        }
       
        static CalculateScrollbarWidth(): number {
            var innerElement = document.createElement('p');
            innerElement.style.width = "100%";
            innerElement.style.height = "100px";

            var outerElement = document.createElement('div');
            outerElement.style.position = "absolute";
            outerElement.style.top = "0px";
            outerElement.style.left = "0px";
            outerElement.style.visibility = "hidden";
            outerElement.style.width = "100px";
            outerElement.style.height = "50px";
            outerElement.style.overflow = "hidden";
            outerElement.appendChild(innerElement);

            document.body.appendChild(outerElement);
            var w1 = innerElement.offsetWidth;
            outerElement.style.overflow = 'scroll';
            var w2 = innerElement.offsetWidth;
            if (w1 == w2) w2 = outerElement.clientWidth;

            document.body.removeChild(outerElement);

            return (w1 - w2); 
        }

        static CalculateActualHeight(element: JQuery): number {
            var height: number = 0;
            $.each(element.children(), function (index, value) {
                height += $(value).height();
            });
            return height;
        }

        static GetNodeType(nodeTypeUid:string): NodeType {
            var result:NodeType = NodeType.UnknownType;
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

interface String {
    insertAt(index: number, value: string): string;
    trimEnds(chars: string): string;
}

String.prototype.insertAt = function (index:number, value:string):string {
    return this.substr(0, index) + value + this.substr(index);
}

String.prototype.trimEnds = function (chars: string): string {
    var result = this;
    var position = this.indexOf(chars);
    if (position == 0) {
        result = result.substr(chars.length);
    }
    position = result.indexOf(chars);
    if (position > 0 && (position == (result.length - chars.length))) {
        result = result.substr(0, (result.length - chars.length));
    }
    return result;
}