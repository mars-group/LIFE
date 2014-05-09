'use strict';

var edge = require('edge');



var createSimController = edge.func({
    assemblyFile: __dirname + '../../../net_code/SimulationController.dll',
    typeName: 'SimulationController.Interface.SimulationControllerNodeJsInterface',
    methodName: 'GetSimController'
});

var simController = createSimController('', true);


exports.allModels = function(req, res) {

    simController.getAllModels('',function (error, result) {
        if(error) return res.json(500, error);
        res.json(result);
    });

};

exports.startSimWithModel = function(req, res) {
    simController.startSimulationWithModel(req.body, function (error, result) {
        if(error) return res.json(500,error);
        res.json(result);
    });
};


exports.resumeSimulation = function(req, res) {
    simController.resumeSimulation(req.body, function (error, result) {
        if(error) return res.json(500,error);
        res.json(result);
    });
};


exports.abortSimulation = function(req, res) {
    simController.abortSimulation(req.body, function (error, result) {
        if(error) return res.json(500,error);
        res.json(result);
    });
};


exports.pauseSimulation = function(req, res) {
    simController.pauseSimulation(req.body, function (error, result) {
        if(error) return res.json(500,error);
        res.json(result);
    });
};

exports.getConnectedNodes = function(req, res) {
    simController.getConnectedNodes('',function (error, result) {
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
