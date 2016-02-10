module app.controllers.webApp {
    "use strict";

    export interface IWebAppScope extends app.controllers.IBaseScope {
        errorMessage: string;
        isLoading: boolean;
        loadingMessage: string;
        populate: () => void;
        webAppInfos: IWebAppInfo[];
        subscriptions: app.models.ISubscription[];
        webApps: app.models.IWebApp[];
    }

    class WebAppCtrl extends BaseCtrl {

        public static $inject = ["$scope", "$q", "localApiSvc"];
        public constructor(private $scope: IWebAppScope, private $q: ng.IQService, localApiSvc: app.services.LocalApiSvc) {
            super($scope, false);
            $scope.populate = function () {
                $scope.isLoading = true;

                $scope.loadingMessage = "Loading Subscriptions...";
                $scope.webApps = [];
                $scope.webAppInfos = [];
                localApiSvc.getSubscriptions()
                    .then(function (response) {
                        $scope.subscriptions = response.data;

                        $scope.loadingMessage = "Loading Web Apps...";
                        var subscriptionPromises = $scope.subscriptions.map(subscription => {
                            return localApiSvc.getWebApps(subscription.id)
                                .then(function (response) {
                                    var subscriptionWebApps = response.data;
                                    $scope.webApps = $scope.webApps.concat(subscriptionWebApps);
                                    subscriptionWebApps.forEach(webApp => {
                                        $scope.webAppInfos.push({ subscription: subscription, webApp: webApp });
                                    });
                                });
                        });

                        return $q.all(subscriptionPromises);

                    }, function (response) {
                        if (response.status === -1) {
                            document.location.assign("account/signin");
                        } else {
                            $scope.errorMessage = response.statusText;
                            $scope.isLoading = false;
                        }
                    })
                    .finally(function () {
                        $scope.isLoading = false;
                    });
            };
        }
    }

    angular.module("app").controller("webAppCtrl", WebAppCtrl);
}