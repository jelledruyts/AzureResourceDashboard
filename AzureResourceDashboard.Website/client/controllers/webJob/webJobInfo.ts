module app.controllers.webJob {
    "use strict";

    export interface IWebJobInfo {
        subscription: app.models.ISubscription;
        webApp: app.models.IWebApp;
        webJob: app.models.IWebJob;
    }
}