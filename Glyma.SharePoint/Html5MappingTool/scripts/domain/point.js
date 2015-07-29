var Glyma;
(function (Glyma) {
    var Point = (function () {
        function Point(x, y) {
            this.x = x;
            this.y = y;
            this.initialEvents();
        }
        Point.prototype.initialEvents = function () {
        };
        return Point;
    })();
    Glyma.Point = Point;
})(Glyma || (Glyma = {}));
//# sourceMappingURL=Point.js.map
