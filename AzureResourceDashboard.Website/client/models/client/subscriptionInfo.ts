module app.models.client {
    "use strict";

    export interface ISubscriptionInfo {
        subscription: app.models.api.ISubscription;
        webApps: app.models.client.IWebAppInfo[];
        highestStatusLevel: app.models.api.StatusLevel;
    }
}