angular.module('app').directive("directiveScale", [function () {
    return {
        restrict: "E", 
        templateUrl: 'app/templates/charting/scale.html',
        scope: { 
            average: '@',
            high: '@',
            low: '@',
            label: '@'
        },
        link: function (scope, element, attributes) {
            scope.$watchGroup(['average', 'high', 'low'], function (newValue) {
                var average = Number(newValue[0]);
                var high = Number(newValue[1]);
                var low = Number(newValue[2]);  
                //console.log(average + " " + high + " " + low);
                if (average > 0 && high > 0 && low > 0) {

                    // var total = Number(scope.total); 
                    // var height = Math.round((active / total) * 100) + '%';
                    var height = (average - low) / (high - low) * 100 + '%';
                    //var bg = $(element).find('.scaleBG')[0];
                    //$(bg).animate({ 'height': height });

                    var items = $(element).find('.scaleBar, .count');
                    $(items).animate({ 'bottom': height });
                }
            });
        }
    };
}]);