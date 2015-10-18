(function () {
    'use strict';
    var controllerId = 'OxygenController';
    angular.module('app').controller(controllerId, ['$scope', 'dataService', OxygenController]);

    function OxygenController($scope, dataService) {

        $scope.test = 'this is Oxygen';
        dataService.getOxygenData().then(function (data) {
            $scope.test = data;
        }, function (error) {
            $scope.test = 'data failed to load';
        })
    };
})();