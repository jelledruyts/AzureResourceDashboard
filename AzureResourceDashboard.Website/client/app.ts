module app {
    "use strict";

    angular.module("app", ["ngRoute"])
        // Filters
        .filter("bootstrap", ["$filter", function ($filter: ng.IFilterService) {
            return function (input: any, format: string) {
                if (format === "StatusLevel") {
                    switch (<app.models.api.StatusLevel>input) {
                        case app.models.api.StatusLevel.Info: return "info";
                        case app.models.api.StatusLevel.Success: return "success";
                        case app.models.api.StatusLevel.Warning: return "warning";
                        case app.models.api.StatusLevel.Error: return "danger";
                        default: return "default";
                    }
                } else if (format === "WebAppState") {
                    switch (<app.models.api.WebAppState>input) {
                        case app.models.api.WebAppState.Running: return "success";
                        case app.models.api.WebAppState.Stopped: return "danger";
                        default: return "default";
                    }
                } else if (format === "boolean") {
                    switch (<boolean>input) {
                        case true: return "success";
                        case false: return "danger";
                        default: return "default";
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
            $rootScope.StatusLevel = app.models.api.StatusLevel;
            $rootScope.WebAppState = app.models.api.WebAppState;
            $rootScope.WebJobType = app.models.api.WebJobType;
            $rootScope.ViewType = app.models.client.ViewType;
        }]);
}