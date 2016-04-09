angular.module('app').directive("directiveHexbin", [function () {
    return {
        restrict: "E",
        scope: {
            colorLow: '@',
            colorHigh: '@',
            minX: '@',
            maxX: '@',
            minY: '@',
            maxY: '@', 
            city: '@',
            locations: '@'
        },
        template: '<div class="hexbinContainer"></div>',
        link: function (scope, element, attributes) {
            var minX = Number(scope.minX);
            var maxX = Number(scope.maxX);
            var minY = Number(scope.minY);
            var maxY = Number(scope.maxY); 
 
            var radius = 10;
            var width = 280;
            var height = 230;   
            
            var color = d3.scale.linear() 
                .range([scope.colorLow, scope.colorHigh])
                .interpolate(d3.interpolateLab);
            
            var x = d3.scale.linear()
                   .domain([minX * 100, maxX * 100])
                   .range([0, width]);
            var y = d3.scale.linear()
                    .domain([minY * 100, maxY * 100])
                    .range([height, 0]);

            var hexbin = d3.hexbin()
                .size([width, height])
                .x(function (d) { return x(d[0]); })
                .y(function (d) { return y(d[1]); })
                .radius(radius);
             
            var container = $(element).find('.hexbinContainer')[0];
            var svg = d3.select(container).append("svg")
                .attr("width", width)
                .attr("height", height)
              .append("g");

            svg.append("clipPath")
                .attr("id", "clip")
              .append("rect")
                .attr("class", "mesh")
                .attr("width", width)
                .attr("height", height);

            svg.append("svg:path")
            .attr("clip-path", "url(#clip)")
            .attr("d", hexbin.mesh())
            .style("stroke-width", .5)
            .style("stroke", "black")
            .style("fill", "none");              
             
            $(container).append('<div class="hexbin-bg"><div></div><div class="hexbin-label">' + scope.city + '</div></div>');
            //var xMax = -9999;
            //var xMin = 0;
            //var yMax = 0;
            //var yMin = 100;
            scope.$watchCollection('locations', function (newVal, oldVal) {
                if (newVal.length == 0 || newVal === undefined) return;
                var data = JSON.parse(newVal);   
                var points = [];

                data.forEach(function (point, i) {
                    var latitude = Number(point.latitude);
                    var longitude = Number(point.longitude);
                    if (latitude < maxY && latitude > minY && longitude < maxX && longitude > minX) {
                        points.push([longitude * 100, latitude * 100]);
                        //if (latitude > yMax) {
                        //    yMax = latitude;
                        //}
                        //if (latitude < yMin) {
                        //    yMin = latitude;
                        //}
                        //if (longitude > xMax) {
                        //    xMax = longitude;
                        //}
                        //if (longitude < xMin) {
                        //    xMin = longitude;
                        //}
                    }
                });
                //console.log(scope.city + " " + xMin + " " + xMax + " " + yMin + " " + yMax);
                var projectedPoints = hexbin(points); 

                svg.append("g")
                 .attr("clip-path", "url(#clip)")
               .selectAll(".hexagon")
                 .data(projectedPoints)
               .enter().append("path")
                 .attr("class", "hexagon")
                 .attr("d", hexbin.hexagon())
                 .attr("transform", function (d) {
                     return "translate(" + d.x + "," + d.y + ")";
                 })
                 .style("fill", function (d) { return color(d.length); });
            });
             
        }
    };
}]);