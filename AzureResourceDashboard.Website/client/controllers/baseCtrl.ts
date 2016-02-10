module app.controllers {
    "use strict";

    export interface IBaseScope extends ng.IScope {
        accountInfo: app.models.IAccount;
    }

    export class BaseCtrl {
        public constructor($scope: IBaseScope, allowAnonymous: boolean) {
            if (!allowAnonymous) {
                $scope.$watch("accountInfo", function (newValue, oldValue) {
                    if (!angular.isUndefined($scope.accountInfo) && !$scope.accountInfo.isAuthenticated) {
                        document.location.assign("account/signin");
                    }
                });
            }
        }
    }
}