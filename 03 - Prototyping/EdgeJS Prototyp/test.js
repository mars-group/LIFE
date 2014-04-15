var edge = require('edge');



var test = edge.func(function () {/*
  async (input) => {
        var waitEvent = new ManualResetEventSlim(false);
        waitEvent.Wait(2500);
        return 42;
    }
*/});


test(0, function (error, result) {
    if(error) throw error;
    console.log(result);
});
