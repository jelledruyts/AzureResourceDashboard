module app.models.api {
    "use strict";

    export interface IWebApp {
        subscriptionId: string;
        id: string;
        name: string;
        location: string;
        state: WebAppState;
        enabled: boolean;
        scmUrl: string;
        resourceGroupName: string;
        statusLevel: StatusLevel;
        statusTime: string;
    }
}