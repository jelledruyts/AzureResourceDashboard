module app.models {
    "use strict";

    export interface ITenant {
        id: string;
        displayName: string;
        isCurrent: boolean;
    }
}