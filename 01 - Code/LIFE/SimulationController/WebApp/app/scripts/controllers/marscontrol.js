'use strict';

angular.module('marsmissionControlApp')
  .controller('MarscontrolCtrl', function ($scope, $http) {
        $http.get('/api/marscontrol/').success(function(models) {
            $scope.models = models;
        });

        $http.get('/api/marscontrol/nodes').success(function(nodes) {
            $scope.nodes = nodes;
        });

        $scope.startModel = function(model){
            $http.post('/api/marscontrol/', model)
                .success(function(result) {
                if(result==0){
                    $scope.modelResult = 'Model: ' + model.Name + ", successfully started!";
                }
                })
                .error(function(error) {
                    $scope.modelResult = 'Model: ' + model.Name + ', not started with error:' + error;
                });
        };
  });
