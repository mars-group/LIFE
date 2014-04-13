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
            $http.post('/api/marscontrol/', model)
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


  });
