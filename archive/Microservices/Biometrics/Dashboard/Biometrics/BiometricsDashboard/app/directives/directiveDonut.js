angular.module('app').directive("directiveDonut", [function () {
    return {
        restrict: "E",
        scope: {
            distribution: '@',
            total: '@'
        },
        template: '<div class="donut-container"><div class="donut-bg"></div><div class="svg-container"></div><div id="tooltip" class="tooltip-hidden"><span id="arc-label">test</span>: <span id="arc-value">0</span></div></div>',
        link: function (scope, element, attributes) {
            scope.$watchCollection('distribution', function (newVal, oldVal) {
                if (newVal.length <= 2 || newVal === undefined) {
                    //$('.donut-bg').show();
                    return;
                }

                $('.donutLabel').remove();
                $('.donut-container svg').remove();
                var strData = newVal;
                //if (oldVal.length > 20) { return; }  //value is reset when hovering on scales
               // $('.donut-bg').hide();
                var data = JSON.parse(strData);
                var width = 150,
                height = 150,
                radius = height / 2;

                var color = d3.scale.ordinal()
                    .range(['#fd3189', '#c3d407', '#2d90eb']);

                var arc = d3.svg.arc()
                    .outerRadius(radius)
                    .innerRadius(radius - 40);

                var pie = d3.layout.pie()
                    .sort(null)
                    .value(function (d) { return d.Count; });
                var container = $(element).find('.donut-container .svg-container')[0];
                var svg = d3.select(container).append("svg")
                    .attr("width", width)
                    .attr("height", height)
                    .append("g")
                    .attr("transform", "translate(" + width / 2 + "," + height / 2 + ")");

                svg.append("text").attr("class", "donutLabel")
                    .attr("transform", function (d) { return "translate(" + 0 + ',' + 0 + ")"; })
                    .attr("dy", ".5em")
                    .attr("fill", "white")
                    .style("text-anchor", "middle")
                    .text(function (d) { return scope.total; }); 

                var g = svg.selectAll(".arc")
                .data(pie(data))
                .enter().append("g")
                .attr("class", "arc")
                .on("mouseover", function (d) {
                    //console.log(d.data.Count); 
                    d3.select("#tooltip")
                        //.style("left", d3.event.offsetX + "px")
                        //.style("top", d3.event.offsetY + "px")
                        .style("opacity", 1)
                        .select("#arc-label").text(d.data.Label);

                    d3.select("#arc-value").text(d.data.Count);
                })
                .on("mouseout", function () {
                    // Hide the tooltip
                    d3.select("#tooltip")
                        .style("opacity", 0);;
                });

                g.append("path")
                    .attr("d", arc)
                    .style("fill", function (d) {
                        return color(d.data.Label);
                    });

            });
        }
    };
}]);