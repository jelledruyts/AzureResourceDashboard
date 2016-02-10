module app.models {
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
        statusTime: string;
    }
}