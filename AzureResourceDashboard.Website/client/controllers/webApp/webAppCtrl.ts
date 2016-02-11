module app.controllers.webApp {
    "use strict";

    export interface IWebAppScope extends app.controllers.IBaseScope {
        errorMessage: string;
        isLoading: boolean;
        loadingMessage: string;
        populate: () => void;
        subscriptions: app.models.client.ISubscriptionInfo[];
        webApps: app.models.client.IWebAppInfo[];
        viewType: app.models.client.ViewType;
    }

    class WebAppCtrl extends BaseCtrl {

        public static $inject = ["$scope", "$q", "localApiSvc"];
        public constructor(private $scope: IWebAppScope, private $q: ng.IQService, localApiSvc: app.services.LocalApiSvc) {
            super($scope, false);

            $scope.viewType = app.models.client.ViewType.Table;

            $scope.populate = function () {
                $scope.isLoading = true;

                $scope.loadingMessage = "Loading Subscriptions...";
                $scope.webApps = [];
                localApiSvc.getSubscriptions()
                    .then(function (response) {
                        $scope.subscriptions = response.data.map(subscription => {
                            return { subscription: subscription, highestStatusLevel: app.models.api.StatusLevel.None, webApps: <app.models.client.IWebAppInfo[]>[] };
                        });

                        $scope.loadingMessage = "Loading Web Apps...";
                        var subscriptionPromises = $scope.subscriptions.map(subscription => {
                            return localApiSvc.getWebApps(subscription.subscription.id)
                                .then(function (response) {
                                    var subscriptionWebApps = response.data;
                                    subscriptionWebApps.forEach(webApp => {
                                        var webAppInfo = { subscription: subscription, webJobs: <app.models.client.IWebJobInfo[]>[], webApp: webApp, highestStatusLevel: app.models.api.StatusLevel.None };
                                        subscription.webApps.push(webAppInfo);
                                        if (subscription.highestStatusLevel < webApp.statusLevel) {
                                            subscription.highestStatusLevel = webApp.statusLevel;
                                        }
                                        $scope.webApps.push(webAppInfo);
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