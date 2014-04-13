'use strict';



angular.module('marsmissionControlApp')
  .controller('MarscontrolCtrl', function ($scope, $http, $timeout) {


        $http.get('/api/marscontrol/').success(function(models) {
            $scope.models = models;
        });

        (function refreshNodes() {
            $http.get('/api/marscontrol/nodes').success(function(nodes) {
                $scope.nodes = nodes;
                $scope.promise = $timeout(refreshNodes, 5000);
            });
        })();


        $scope.startModel = function(model){
            $http.post('/api/marscontrol/startsim', model)
                .success(function(result) {
                if(result==0){
                    model.Running = true;
                    model.Status.StatusMessage = "Running";
                    $scope.modelResult = 'Model: ' + model.Name + ', successfully started!';
                }
                })
                .error(function(error) {
                    $scope.modelResult = 'Model: ' + model.Name + ', not started with error:' + error;
                });
        };

        $scope.resumeSim = function(model){
            $http.post('/api/marscontrol/resumesim', model)
                .success(function(result) {
                    if(result==0){
                        model.Running = true;
                        model.Status.StatusMessage = "Running";
                        $scope.modelResult = 'Model: ' + model.Name + ', successfully resumed!';
                    }
                })
                .error(function(error) {
                    $scope.modelResult = 'Model: ' + model.Name + ', not resumed with error:' + error;
                });
        };

        $scope.abortSim = function(model){
            $http.post('/api/marscontrol/abortsim', model)
                .success(function(result) {
                    if(result==0){
                        model.Running = false;
                        model.Status.StatusMessage = "Aborted";
                        $scope.modelResult = 'Model: ' + model.Name + ', successfully aborted!';
                    }
                })
                .error(function(error) {
                    $scope.modelResult = 'Model: ' + model.Name + ', not aborted with error:' + error;
                });
        };

        $scope.pauseSim = function(model){
            $http.post('/api/marscontrol/pausesim', model)
                .success(function(result) {
                    if(result==0){
                        // leave to true, since this shows the simulation is in an overall running state
                        model.Running = true;
                        model.Status.StatusMessage = "Paused";
                        $scope.modelResult = 'Model: ' + model.Name + ', successfully paused!';
                    }
                })
                .error(function(error) {
                    $scope.modelResult = 'Model: ' + model.Name + ', not paused with error:' + error;
                });
        };



  });
