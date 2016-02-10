module app {
    "use strict";

    angular.module("app", ["ngRoute"])
        // Filters
        .filter("bootstrap", ["$filter", function ($filter: ng.IFilterService) {
            return function (input: any, format: string) {
                if (format === "StatusLevel") {
                    switch (<app.models.StatusLevel>input) {
                        case app.models.StatusLevel.Info: return "label-info";
                        case app.models.StatusLevel.Success: return "label-success";
                        case app.models.StatusLevel.Warning: return "label-warning";
                        case app.models.StatusLevel.Error: return "label-danger";
                        case app.models.StatusLevel.Active: return "label-info";
                        case app.models.StatusLevel.Inactive: return "label-default";
                        default: return "label-default";
                    }
                } else if (format === "WebAppState") {
                    switch (<app.models.WebAppState>input) {
                        case app.models.WebAppState.Running: return "label-success";
                        case app.models.WebAppState.Stopped: return "label-danger";
                        default: return "label-default";
                    }
                } else if (format === "boolean") {
                    switch (<boolean>input) {
                        case true: return "label-success";
                        case false: return "label-danger";
                        default: return "label-default";
                    }
                } else {
                    return input;
                }
            };
        }])
        .filter("datetime", ["$filter", function ($filter: ng.IFilterService) {
            return function (input: string, format: string) {
                if (input === null) {
                    return "";
                }
                if (angular.isUndefined(format) || format === null || format === "") {
                    format = "yyyy-MM-dd HH:mm:ss";
                }
                return $filter("date")(new Date(input), format);
            };
        }])

        // Configuration
        .config(["$routeProvider", "$httpProvider", function ($routeProvider: ng.route.IRouteProvider, $httpProvider: ng.IHttpProvider) {
            // Configure the routes.
            $routeProvider
                .when("/", {
                    templateUrl: "client/controllers/home/index.html",
                    controller: "homeCtrl"
                })
                .when("/webapp", {
                    templateUrl: "client/controllers/webApp/index.html",
                    controller: "webAppCtrl"
                })
                .when("/webjob", {
                    templateUrl: "client/controllers/webJob/index.html",
                    controller: "webJobCtrl"
                })
                .otherwise({ redirectTo: "/" });
        }])

        // Initialization
        .run(["$rootScope", function ($rootScope: any) {
            // Make enums available on the root scope and therefore any child scope.
            $rootScope.StatusLevel = app.models.StatusLevel;
            $rootScope.WebAppState = app.models.WebAppState;
            $rootScope.WebJobType = app.models.WebJobType;
        }]);
}