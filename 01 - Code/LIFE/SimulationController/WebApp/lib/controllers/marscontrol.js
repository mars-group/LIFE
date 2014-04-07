'use strict';

var edge = require('edge');



var getAllModels = edge.func({
    assemblyFile: __dirname + '../../../../SimulationController/bin/Debug/SimulationController.dll',
    typeName: 'SimulationController.SimulationControllerNodeJsInterface',
    methodName: 'GetAllModels'
});

exports.allModels = function(req, res) {
    getAllModels('',function (error, result) {
        if(error) return res.json(500,error);
        res.json(result);
    });
};
