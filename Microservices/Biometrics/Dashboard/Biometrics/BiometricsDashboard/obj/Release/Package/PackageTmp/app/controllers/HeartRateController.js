(function () {
    'use strict';
    var app = angular.module('app');
    var controllerId = 'HeartRateController';
    var path = 'temp';

    app.controller(controllerId, ['$scope', '$rootScope', '$location', 'dataService', 'HUB_NAME', HeartRateController]);

    function HeartRateController($scope, $rootScope, $location, dataService, HUB_NAME) {
        function setScopeData(data) {
            $rootScope.$apply(function () {
                if ($rootScope.isFrozen) { return; } 
                $scope.Total = data.TotalCount;
                $scope.High = data.HighPercent;
                $scope.Normal = data.NormalPercent;
                $scope.Low = data.LowPercent;
                $scope.Inactive = data.InactivePercent;
            });
        }

        var connection = $.hubConnection();
        var proxy = connection.createHubProxy(HUB_NAME);
        proxy.on('updateHeartRate', function (data) {
            setScopeData(data);
            //console.log('hr data recd');
        });
        connection.start().done(function () {
            proxy.invoke('getLatestHeartRateMessage').then(function (data) {
                setScopeData(data);
                //console.log('invocation finished');
            });
        })

        $rootScope.$on("$locationChangeStart", function (event, toState) {
            if (toState.indexOf(path) >= 0) {
            } else {
                connection.stop();
                //console.log('disconnecting from ' + path);
            }
        });
    };
})();