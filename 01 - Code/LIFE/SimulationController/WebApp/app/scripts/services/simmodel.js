'use strict';

angular.module('marsmissionControlApp')
  .factory('Simmodel', function ($resource) {
        return $resource('/api/marscontrol/models');
  });
