﻿'use strict';
app.factory('DashboardServices', ['$http', 'commonServices', function ($http, commonServices) {

    //var serviceBase = 'http://ngauthenticationapi.azurewebsites.net/';

    var usersServiceFactory = {};
    var apiUrl = '/api/portal/';
    var _getDashboardInfo = async function () {
        var req = {
            method: 'GET',
            url: apiUrl + 'dashboard'
        };

        return await commonServices.getApiResult(req);
    };

    usersServiceFactory.getDashboardInfo = _getDashboardInfo;
    return usersServiceFactory;

}]);
