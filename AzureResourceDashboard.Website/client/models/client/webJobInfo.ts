module app.models.client {
    "use strict";

    export interface IWebJobInfo {
        subscription: app.models.client.ISubscriptionInfo;
        webApp: app.models.client.IWebAppInfo;
        webJob: app.models.api.IWebJob;
    }
}