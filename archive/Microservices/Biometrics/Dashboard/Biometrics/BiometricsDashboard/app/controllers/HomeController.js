(function () {
    'use strict';
    var controllerId = 'HomeController';
    angular.module('app').controller(controllerId, ['$scope', '$rootScope', 'dataService', 'HUB_NAME', HomeController]);

    function HomeController($scope, $rootScope, dataService, HUB_NAME) {
        $rootScope.isFrozen = false;
        var titles = {
            'all': '',
            'boston': 'Boston',
            'chicago': 'Chicago',
            'newyork': 'New York',
            'hr': 'Heart Rate',
            'temp': 'Temperature',
            'oxygen': 'Oxygen',
            'glucose': 'Glucose'
        };
        $rootScope.title = titles['all'];
        $rootScope.selectedMeasure = 'summary';
        function applyScopeData(data) { 
            $rootScope.$apply(function () {
                setScopeData(data); 
            });
        }

        function setScopeData(data) {
            if (data) {
                $scope.Average = data.AverageReading;
                
                $scope.Total = data.TotalCount;
                $scope.High = data.HighPercent;
                $scope.Normal = data.NormalPercent;
                $scope.Low = data.LowPercent;
                $scope.Inactive = data.InactivePercent;
                $scope.Distribution = data.Distribution;
                if ($rootScope.isFrozen == false) {
                    $scope.GlucoseActive = data.GlucoseActive;
                    $scope.OxygenActive = data.OxygenActive;
                    $scope.TemperatureActive = data.TemperatureActive;
                    $scope.HeartRateActive = data.HeartRateActive;
                }
            } else {
                $scope.Average = 0;
                $scope.Total = 0;
                $scope.High = 0;
                $scope.Normal = 0;
                $scope.Low = 0;
                $scope.Inactive = 0;
                $scope.Distribution = 0;
                if ($rootScope.isFrozen == false) {
                    $scope.GlucoseActive = 0;
                    $scope.OxygenActive = 0;
                    $scope.TemperatureActive = 0;
                    $scope.HeartRateActive = 0;
                }
            }
        }
        //signalr
        var connection = $.hubConnection();
        var proxy = connection.createHubProxy(HUB_NAME);
        proxy.on('updateSummary', function (data) {
            $scope.summaryData = data;
            if ($rootScope.selectedMeasure == 'summary') {
                applyScopeData(data);
            }
        });
        proxy.on('updateHeartRate', function (data) {
            $scope.heartData = data;
            $scope.$apply(function () {
                $scope.HeartMin = data.MinReading;
                $scope.HeartMax = data.MaxReading;
                $scope.HeartAverage = data.AverageReading;
            });
            if ($rootScope.selectedMeasure == 'hr') {
                applyScopeData(data);
            }
        });
        proxy.on('updateTemperature', function (data) {
            $scope.tempData = data;
            $scope.$apply(function () {
                $scope.TempMin = data.MinReading;
                $scope.TempMax = data.MaxReading;
                $scope.TempAverage = data.AverageReading;
            });
            if ($rootScope.selectedMeasure == 'temp') {
                applyScopeData(data);
            }
        });
        proxy.on('updateOxygen', function (data) {
            $scope.oxygenData = data;
            $scope.$apply(function () {
                $scope.OxygenMin = data.MinReading;
                $scope.OxygenMax = data.MaxReading;
                $scope.OxygenAverage = data.AverageReading;
            });
            if ($rootScope.selectedMeasure == 'oxygen') {
                applyScopeData(data);
        }
        });
        proxy.on('updateGlucose', function (data) {
            $scope.glucoseData = data;
            $scope.$apply(function () {
                $scope.GlucoseMin = data.MinReading;
                $scope.GlucoseMax = data.MaxReading;
                $scope.GlucoseAverage = data.AverageReading;
            });

            if ($rootScope.selectedMeasure == 'glucose') {
                applyScopeData(data);
        }
        });
        proxy.on('updateBoston', function (data) {
            $scope.$apply(function () {
                $scope.bostonData = data;
            });
            if ($rootScope.selectedMeasure == 'boston') {
                applyScopeData(data);
            }
        });
        proxy.on('updateChicago', function (data) {
            $scope.$apply(function () {
                $scope.chicagoData = data;
            });
            if ($rootScope.selectedMeasure == 'chicago') {
                applyScopeData(data);
            }
        });
        proxy.on('updateNewYork', function (data) {
            $scope.$apply(function () {
                $scope.newyorkData = data;
            });
            if ($rootScope.selectedMeasure == 'newyork') {
                applyScopeData(data);
            }
            });
        connection.start().done(function () {
            proxy.invoke('getLatestSummaryMessage').then(function (data) {
                applyScopeData(data);
                //console.log('invocation finished');
            });
        })

        $rootScope.$on("$locationChangeStart", function (event, toState) {
            var index = toState.indexOf('#/');
            if (index < toState.length - 2) {
                connection.stop();
            }
        });

        //$scope.geos = [
        //    { id: 'All', name: 'All' },
        //    { id: 'Boston', name: 'Boston' },
        //    { id: 'Chicago', name: 'Chicago' },
        //    { id: 'NewYork', name: 'New York' }
        //];

        //$scope.selectedGeo = $scope.geos[0];
        //$scope.selectionChanged = function (selection) {
        //    proxy.invoke('setGeoFilter', selection.id).then(function () {

        //    });
        //};

        $scope.onScaleMouseenter = function (item) { 
            $rootScope.title = titles[item];
            $rootScope.isFrozen = true;
            $rootScope.selectedMeasure = item;
            //console.log(item);

            if (item === 'boston') {
                setScopeData($scope.bostonData);
            } else if (item === 'chicago') {
                setScopeData($scope.chicagoData);
            } else if (item === 'newyork') {
                setScopeData($scope.newyorkData);
            } else if (item === 'glucose') {
                setScopeData($scope.glucoseData);
            } else if (item === 'oxygen') {
                setScopeData($scope.oxygenData);
            } else if (item === 'temp') {
                setScopeData($scope.tempData);
            } else if (item === 'heart') {
                setScopeData($scope.heartData);
            }
        }

        $scope.onScaleMouseleave = function () {
            $rootScope.isFrozen = false; 
            $rootScope.title = titles['all']; 
            $rootScope.selectedMeasure = 'summary';
            setScopeData($scope.summaryData);
        }
    };
})();