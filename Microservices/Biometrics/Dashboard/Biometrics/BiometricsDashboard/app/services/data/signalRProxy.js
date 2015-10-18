

angular.module('app').service('signalRProxy', ['$rootScope',
    function ($rootScope) {
        var hubName = 'healthHub';
        function signalRHubProxyFactory() {
            var connection = $.hubConnection(); 
            var proxy = connection.createHubProxy(hubName);
            console.log('signalr proxy constructor'); 

            function start() {
                connection.start().done(function () {
                    console.log('signalr connected to server hub');
                });
            }

            return {
                on: function (eventName, callback) {
                    proxy.on(eventName, function (result) {
                        console.log('signalr subscription callback');
                        $rootScope.$apply(function () {
                            if (callback) {
                                callback(result);
                            }
                        });
                    });
                    //dont start till after subscription is set, or else it wont work when u change route
                    start();
                },
                off: function (){  
                    //override default behavior to force disconnection
                    connection.stop(); 
                },
                invoke: function (methodName, callback) {
                    proxy.invoke(methodName)
                        .done(function (result) {
                            $rootScope.$apply(function () {
                                if (callback) {
                                    callback(result);
                                }
                            });
                        });
                },
                connection: connection
            };
        };

        return signalRHubProxyFactory;
    }]);