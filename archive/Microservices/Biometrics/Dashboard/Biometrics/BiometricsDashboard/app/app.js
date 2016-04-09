(function () { 

    var app = angular.module('app', ['ngRoute', 'apiMock']);

    //routing
    app.config(['$routeProvider', function ($routeProvider) {
      $routeProvider.
        when('/', { 
            templateUrl: 'app/templates/home.html',
            controller: 'HomeController'
        }).
        when('/temp', {
            templateUrl: 'app/templates/detail/temp-detail.html',
            controller: 'TempController'
        }).
        when('/glucose', {
            templateUrl: 'app/templates/detail/glucose-detail.html',
            controller: 'GlucoseController'
        }).
        when('/heartrate', {
            templateUrl: 'app/templates/detail/heart-rate-detail.html',
            controller: 'HeartRateController'
        }).
        when('/oxygen', {
            templateUrl: 'app/templates/detail/oxygen-detail.html',
            controller: 'OxygenController',
        }).
        otherwise({
            redirectTo: '/'
        });
  }]); 
     
    app.constant('HUB_NAME', 'healthHub');
    //for some reason default route doesnt get loaded on init
    app.run(['$route', function ($route) {
        $route.reload();

    }]);  
})();