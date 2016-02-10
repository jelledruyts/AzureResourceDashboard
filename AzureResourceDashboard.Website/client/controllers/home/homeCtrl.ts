module app.controllers.home {
    "use strict";

    class HomeCtrl extends BaseCtrl {
        public static $inject = ["$scope", "$location"];
        public constructor(private $scope: IBaseScope, $location: ng.ILocationService) {
            super($scope, true);
        }
    }

    angular.module("app").controller("homeCtrl", HomeCtrl);
}