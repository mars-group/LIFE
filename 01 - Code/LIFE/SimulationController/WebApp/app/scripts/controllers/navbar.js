'use strict';

angular.module('marsmissionControlApp')
  .controller('NavbarCtrl', function ($scope, $location, Auth) {
    $scope.menu = [{
      'title': 'Dashboard',
      'link': '/'
    }, {
      'title': 'MARS Control',
      'link': '/marscontrol'
    }, {
      'title': 'MARS VIEW',
      'link': '/marsview'
    }, {
      'title': 'Settings',
      'link': '/settings'
    }];
    
    $scope.logout = function() {
      Auth.logout()
      .then(function() {
        $location.path('/login');
      });
    };
    
    $scope.isActive = function(route) {
      return route === $location.path();
    };
  });
