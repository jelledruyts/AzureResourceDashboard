module app.services {
    "use strict";

    export class LocalApiSvc {
        static $inject = ['$http'];
        constructor(private $http: ng.IHttpService) {
        }

        public getAccount(): ng.IHttpPromise<app.models.api.IAccount> {
            return this.$http.get("api/account");
        }

        public getTenants(): ng.IHttpPromise<app.models.api.ITenant[]> {
            return this.$http.get("api/tenant");
        }

        public getSubscriptions(): ng.IHttpPromise<app.models.api.ISubscription[]> {
            return this.$http.get("api/subscription");
        }

        public getWebApps(subscriptionId: string): ng.IHttpPromise<app.models.api.IWebApp[]> {
            return this.$http.get("api/webapp?subscriptionId=" + encodeURIComponent(subscriptionId));
        }

        public getWebJobs(webAppId: string, webAppScmUrl: string): ng.IHttpPromise<app.models.api.IWebJob[]> {
            return this.$http.get("api/webjob?webAppId=" + encodeURIComponent(webAppId) + "&webAppScmUrl=" + encodeURIComponent(webAppScmUrl));
        }
    }

    angular.module("app").service("localApiSvc", LocalApiSvc);
}