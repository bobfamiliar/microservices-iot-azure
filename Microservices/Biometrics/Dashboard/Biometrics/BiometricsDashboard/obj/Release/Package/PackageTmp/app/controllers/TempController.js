(function () {
    'use strict';
    var app = angular.module('app');
    var controllerId = 'TempController';
    var path = 'temp';

    app.controller(controllerId, ['$scope', '$rootScope', '$location', 'dataService', 'HUB_NAME', TempController]);

    function TempController($scope, $rootScope, $location, dataService, HUB_NAME) {
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
        proxy.on('updateTemperature', function (data) {
            setScopeData(data); 
        });
        connection.start().done(function () {
            proxy.invoke('getLatestTemperatureMessage').then(function (data) {
                setScopeData(data); 
            });
        })

        $rootScope.$on("$locationChangeStart", function (event, toState) {
            if (toState.indexOf(path) >= 0) {
            } else {
                connection.stop(); 
            }
        });
    };
})();