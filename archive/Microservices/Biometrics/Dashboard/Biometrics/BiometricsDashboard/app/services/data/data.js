angular.module('app').service('dataService', ['$http', '$q', function ($http, $q) {
    this.getTempData = function () {
        return $http.get('/api/temperature/data').then(function (response) {
            return response.data;
        }, function (response) {
            return $q.reject(response.statusText);
        })
    }

    this.getGlucoseData = function () {
        return $http.get('/api/glucose/data').then(function (response) {
            return response.data;
        }, function (response) {
            return $q.reject(response.statusText);
        })
    }

    this.getOxygenData = function () {
        return $http.get('/api/oxygen/data').then(function (response) {
            return response.data;
        }, function (response) {
            return $q.reject(response.statusText);
        })
    }

    this.getHeartRateData = function () {
        return $http.get('/api/heartrate/data').then(function (response) {
            return response.data;
        }, function (response) {
            return $q.reject(response.statusText);
        })
    }
}])