var app = angular.module('Myapp', ['ngMaterial', 'ngRoute']).config(function ($routeProvider) {
    $routeProvider
        .when("/", {
            controller: "AppCtrl",            
            templateUrl: "../home/index"
        })
        .when("/about", {
            controller: "AppCtrl",           
            templateUrl: "../home/about"
        })
        .when("/contact", {
            controller: "AppCtrl",           
            templateUrl: "../home/contact"
        });
});

    app.controller('AppCtrl', function ($scope, $timeout, $mdSidenav) {
        //$scope.toggleLeft = buildToggler('left');
        //$scope.toggleRight = buildToggler('right');

        //function buildToggler(componentId) {
        //    return function () {
        //        $mdSidenav(componentId).toggle();
        //    };
        //}
    });
