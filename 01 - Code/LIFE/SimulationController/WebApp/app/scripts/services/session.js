'use strict';

angular.module('marsmissionControlApp')
  .factory('Session', function ($resource) {
    return $resource('/api/session/');
  });
