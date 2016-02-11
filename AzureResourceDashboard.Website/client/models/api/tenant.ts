module app.models.api {
    "use strict";

    export interface ITenant {
        id: string;
        displayName: string;
        isCurrent: boolean;
    }
}