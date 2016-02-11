module app.models.api {
    "use strict";

    export interface IWebJob {
        webAppId: string;
        name: string;
        type: WebJobType;
        detailsUrl: string;
        statusLevel: StatusLevel;
        statusDescription: string;
        statusTime: string;
    }
}