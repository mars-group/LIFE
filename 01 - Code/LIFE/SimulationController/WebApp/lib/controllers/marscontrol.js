'use strict';

var edge = require('edge');



var getAllModels = edge.func({
    assemblyFile: __dirname + '../../../../SimulationController/bin/Debug/SimulationController.dll',
    typeName: 'SimulationController.SimulationControllerNodeJsInterface',
    methodName: 'GetAllModels'
});

var startSimulationWithModel = edge.func({
    assemblyFile: __dirname + '../../../../SimulationController/bin/Debug/SimulationController.dll',
    typeName: 'SimulationController.SimulationControllerNodeJsInterface',
    methodName: 'StartSimulationWithModel'
});

var getConnectedNodes = edge.func({
    assemblyFile: __dirname + '../../../../SimulationController/bin/Debug/SimulationController.dll',
    typeName: 'SimulationController.SimulationControllerNodeJsInterface',
    methodName: 'GetConnectedNodes'
});
/*
var subscribeForStatusUpdate = edge.func({
    assemblyFile: __dirname + '../../../../SimulationController/bin/Debug/SimulationController.dll',
    typeName: 'SimulationController.SimulationControllerNodeJsInterface',
    methodName: 'SubscribeForStatusUpdate'
});
*/

exports.allModels = function(req, res) {
    getAllModels('',function (error, result) {
        if(error) return res.json(500, error);
        res.json(result);
    });
};

exports.startSimWithModel = function(req, res) {
    startSimulationWithModel(req.body, function (error, result) {
        if(error) return res.json(500,error);
        res.json(result);
    });
};

exports.getConnectedNodes = function(req, res) {
    getConnectedNodes('',function (error, result) {
        if(error) return res.json(500, error);
        res.json(result);
    });
};
/*
exports.subscribeForStatusUpdate = function(req, res) {
    subscribeForStatusUpdate('', function (error, result) {
        if(error) return res.json(500,error);
        res.json(result);
    });
};*/
