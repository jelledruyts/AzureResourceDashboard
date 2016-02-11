module app.models.client {
    "use strict";

    export interface IWebAppInfo {
        subscription: app.models.client.ISubscriptionInfo;
        webJobs: app.models.client.IWebJobInfo[];
        webApp: app.models.api.IWebApp;
        highestStatusLevel: app.models.api.StatusLevel;
    }
}