module app.controllers.webJob {
    "use strict";

    export interface IWebJobScope extends app.controllers.IBaseScope {
        errorMessage: string;
        isLoading: boolean;
        loadingMessage: string;
        populate: () => void;
        subscriptions: app.models.client.ISubscriptionInfo[];
        webJobs: app.models.client.IWebJobInfo[];
        viewType: app.models.client.ViewType;
    }

    class WebJobCtrl extends BaseCtrl {

        public static $inject = ["$scope", "$q", "localApiSvc"];
        public constructor(private $scope: IWebJobScope, private $q: ng.IQService, localApiSvc: app.services.LocalApiSvc) {
            super($scope, false);

            $scope.viewType = app.models.client.ViewType.Table;

            $scope.populate = function () {
                $scope.isLoading = true;

                $scope.loadingMessage = "Loading Subscriptions...";
                $scope.webJobs = [];
                localApiSvc.getSubscriptions()
                    .then(function (response) {
                        $scope.subscriptions = response.data.map(subscription => {
                            return { subscription: subscription, highestStatusLevel: app.models.api.StatusLevel.None, webApps: [] };
                        });

                        $scope.loadingMessage = "Loading Web Apps...";
                        var subscriptionPromises = $scope.subscriptions.map(subscription => {
                            return localApiSvc.getWebApps(subscription.subscription.id)
                                .then(function (response) {
                                    var subscriptionWebApps = response.data;

                                    var webAppPromises = subscriptionWebApps.map(webApp => {
                                        var webAppInfo = { subscription: subscription, webJobs: <app.models.client.IWebJobInfo[]>[], webApp: webApp, highestStatusLevel: app.models.api.StatusLevel.None }
                                        subscription.webApps.push(webAppInfo);

                                        $scope.loadingMessage = "Loading WebJobs...";
                                        return localApiSvc.getWebJobs(webApp.id, webApp.scmUrl)
                                            .then(function (response) {
                                                var webAppWebJobs = response.data;

                                                webAppWebJobs.forEach(webJob => {
                                                    var webJobInfo = { subscription: subscription, webApp: webAppInfo, webJob: webJob };
                                                    webAppInfo.webJobs.push(webJobInfo);
                                                    if (subscription.highestStatusLevel < webJob.statusLevel) {
                                                        subscription.highestStatusLevel = webJob.statusLevel;
                                                    }
                                                    if (webAppInfo.highestStatusLevel < webJob.statusLevel) {
                                                        webAppInfo.highestStatusLevel = webJob.statusLevel;
                                                    }
                                                    $scope.webJobs.push(webJobInfo);
                                                });
                                            });
                                    });

                                    return $q.all(webAppPromises);
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

    angular.module("app").controller("webJobCtrl", WebJobCtrl);
}