var edge = require('edge');

var meaningOfLife = edge.func({
    assemblyFile: __dirname + '/NETCode/EdgeJSPrototype/SimulationController/bin/Debug/SimulationController.dll',
    typeName: 'SimulationController.SimulationManagerComponent',
    methodName: 'GenerateMeaningOfLife'
});

meaningOfLife(0, function (error, result) {
    if(error) throw error;
    console.log(result);
})