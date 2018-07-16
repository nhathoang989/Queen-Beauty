﻿
app.component('pageMain', {
    templateUrl: '/app-portal/pages/page/components/main/main.html',
    controller: ['$rootScope', function ($rootScope) {
        this.settings = $rootScope.settings;
        this.setPageType = function (type) {
            this.page.type = $index;
        }
    }],
    bindings: {
        page: '=',
        onDelete: '&',
        onUpdate: '&'
    }
});