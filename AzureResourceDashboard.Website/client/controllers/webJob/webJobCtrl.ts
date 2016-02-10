module app.controllers.webJob {
    "use strict";

    export interface IWebJobScope extends app.controllers.IBaseScope {
        errorMessage: string;
        isLoading: boolean;
        loadingMessage: string;
        populate: () => void;
        webJobInfos: IWebJobInfo[];
        subscriptions: app.models.ISubscription[];
        webApps: app.models.IWebApp[];
        webJobs: app.models.IWebJob[];
    }

    class WebJobCtrl extends BaseCtrl {

        public static $inject = ["$scope", "$q", "localApiSvc"];
        public constructor(private $scope: IWebJobScope, private $q: ng.IQService, localApiSvc: app.services.LocalApiSvc) {
            super($scope, false);
            $scope.populate = function () {
                $scope.isLoading = true;

                $scope.loadingMessage = "Loading Subscriptions...";
                $scope.webApps = [];
                $scope.webJobs = [];
                $scope.webJobInfos = [];
                localApiSvc.getSubscriptions()
                    .then(function (response) {
                        $scope.subscriptions = response.data;

                        $scope.loadingMessage = "Loading Web Apps...";
                        var subscriptionPromises = $scope.subscriptions.map(subscription => {
                            return localApiSvc.getWebApps(subscription.id)
                                .then(function (response) {
                                    var subscriptionWebApps = response.data;
                                    $scope.webApps = $scope.webApps.concat(subscriptionWebApps);

                                    var webAppPromises = subscriptionWebApps.map(webApp => {
                                        $scope.loadingMessage = "Loading WebJobs...";
                                        return localApiSvc.getWebJobs(webApp.id, webApp.scmUrl)
                                            .then(function (response) {
                                                var webAppWebJobs = response.data;
                                                $scope.webJobs = $scope.webJobs.concat(webAppWebJobs);

                                                webAppWebJobs.forEach(webJob => {
                                                    $scope.webJobInfos.push({ subscription: subscription, webApp: webApp, webJob: webJob });
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