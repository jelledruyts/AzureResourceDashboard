module app.controllers.webApp {
    "use strict";

    export interface IWebAppInfo {
        subscription: app.models.ISubscription;
        webApp: app.models.IWebApp;
    }
}