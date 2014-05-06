var edge = require('edge');



var test = edge.func(function () {/*
    async (i) => {
        return 42;
    }
*/});


test(0, function (error, result) {
    if(error) throw error;
    console.log(result);
});
