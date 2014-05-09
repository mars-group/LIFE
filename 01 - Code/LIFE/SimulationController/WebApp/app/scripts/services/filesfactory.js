'use strict';

angular.module('marsmissionControlApp')
    .factory('FilesFactory', function($http){
       return {
                getFile:    function (fileId, callback) {
                                $http.post('/api/marscomfiles/getfile', {fileId: fileId}).success(callback);
                            },

                fileExists: function (filename, callback) {
                                $http.post('/api/marscomfiles/fileexists', {filename: filename}).success(callback);
                            },

                getFiles:   function (callback) {
                                $http.post('/api/marscomfiles/getfiles', {}).success(callback);
                            },

                createFile: function (filename, content, callback) {
                                $http.post('/api/marscomfiles/createfile', {filename: filename, content: content}).success(callback);
                            },

                updateFile: function (fileId, content, callback) {
                                $http.post('/api/marscomfiles/updatefile', {fileId: fileId, content: content}).success(callback);
                            },

                deleteFile: function (fileId, callback) {
                                $http.post('/api/marscomfiles/deletefile', {fileId: fileId}).success(callback);
                            }
        }
    });
