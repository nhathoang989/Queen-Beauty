﻿'use strict';
app.controller('ConfigurationController', ['$scope', '$rootScope', '$routeParams', '$timeout', '$location', 'authService', 'ConfigurationServices',
    function ($scope, $rootScope, $routeParams, $timeout, $location, authService, configurationServices) {
        $scope.request = {
            pageSize: '10',
            pageIndex: 0,
            status: $rootScope.swStatus[1],
            orderBy: 'CreatedDateTime',
            direction: '1',
            fromDate: null,
            toDate: null,
            keyword: ''
        };
        $scope.configurationFile = {
            file: null,
            fullPath: '',
            folder: 'Configuration',
            title: '',
            description: ''
        };
        $scope.dataTypes = $rootScope.configurations.dataTypes;
        $scope.activedConfiguration = null;
        $scope.relatedConfigurations = [];
        $rootScope.isBusy = false;
        $scope.data = {
            pageIndex: 0,
            pageSize: 1,
            totalItems: 0,
        };
        $scope.errors = [];

        $scope.range = function (max) {
            var input = [];
            for (var i = 1; i <= max; i += 1) input.push(i);
            return input;
        };
        $scope.initEditors = function () {
            $rootScope.initEditor();
        }
        $scope.getConfiguration = async function (id) {
            $rootScope.isBusy = true;
            var resp = await configurationServices.getConfiguration(id, 'be');
            if (resp && resp.isSucceed) {
                $scope.activedConfiguration = resp.data;
                $rootScope.initEditor();
                $scope.$apply();
            }
            else {
                if (resp) { $rootScope.showErrors(resp.errors); }
                $scope.$apply();
            }
        };
        $scope.loadConfiguration = async function () {
            $rootScope.isBusy = true;
            var id = $routeParams.id;
            var response = await configurationServices.getConfiguration(id, 'be');
            if (response.isSucceed) {
                $scope.activedConfiguration = response.data;
                $rootScope.initEditor();
                $scope.$apply();
            }
            else {
                $rootScope.showErrors(response.errors);
                $scope.$apply();
            }
        };

        $scope.loadConfigurations = async function (pageIndex) {
            if (pageIndex != undefined) {
                $scope.request.pageIndex = pageIndex;
            }

            var resp = await configurationServices.getConfigurations($scope.request);
            if (resp && resp.isSucceed) {

                $scope.data = resp.data;
                $.each($scope.data.items, function (i, configuration) {

                    $.each($scope.activedConfigurations, function (i, e) {
                        if (e.configurationId == configuration.id) {
                            configuration.isHidden = true;
                        }
                    })
                });
                $scope.$apply();
            }
            else {
                if (resp) { $rootScope.showErrors(resp.errors); }
                $scope.$apply();
            }
        };

        $scope.removeConfiguration = function (id) {
            $rootScope.showConfirm($scope, 'removeConfigurationConfirmed', [id], null, 'Remove Configuration', 'Are you sure');
        }

        $scope.removeConfigurationConfirmed = async function (id) {
            var result = await configurationServices.removeConfiguration(id);
            if (result.isSucceed) {
                $scope.loadConfigurations();
            }
            else {
                $rootScope.showMessage('failed');
                $scope.$apply();
            }
        }

        $scope.saveConfiguration = async function (configuration) {
            configuration.content = $('.editor-content').val();
            configuration.dataType = configuration.property.dataType;
            var resp = await configurationServices.saveConfiguration(configuration);
            if (resp && resp.isSucceed) {
                $scope.activedConfiguration = resp.data;
                $rootScope.showMessage('Thành công', 'success');
                $rootScope.isBusy = false;
                $scope.$apply();
            }
            else {
                if (resp) { $rootScope.showErrors(resp.errors); }
                $scope.$apply();
            }
        };

    }]);
