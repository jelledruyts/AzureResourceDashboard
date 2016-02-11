module app.controllers.root {
    "use strict";

    export interface IRootScope extends IBaseScope {
        isActiveLocation: (viewLocation: string) => boolean;
        isLoadingAccountInfo: boolean;
        errorMessage: string;
        tenants: app.models.api.ITenant[];
        selectedTenant: app.models.api.ITenant;
        selectTenant: () => void;
    }

    class RootCtrl extends BaseCtrl {
        public static $inject = ["$scope", "$location", "localApiSvc"];
        public constructor(private $scope: IRootScope, private $location: ng.ILocationService, private localApiSvc: app.services.LocalApiSvc) {
            super($scope, true);
            $scope.isLoadingAccountInfo = true;

            $scope.isActiveLocation = function (viewLocation: string) {
                return viewLocation === $location.path();
            };

            $scope.selectTenant = function () {
                document.location.assign("account/signin?tenant=" + $scope.selectedTenant.id);
            }

            localApiSvc.getAccount()
                .then(function (response) {
                    $scope.errorMessage = "";
                    $scope.accountInfo = response.data;
                }, function (response) {
                    $scope.errorMessage = response.statusText;
                })
                .finally(function () {
                    $scope.isLoadingAccountInfo = false;
                });

            localApiSvc.getTenants()
                .then(function (response) {
                    $scope.tenants = response.data;
                    for (var tenant of $scope.tenants) {
                        if (tenant.isCurrent) {
                            $scope.selectedTenant = tenant;
                        }
                    }
                }, function (response) {
                });
        }
    }

    angular.module("app").controller("rootCtrl", RootCtrl);
}