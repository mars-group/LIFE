var edge = require('edge');



var test = edge.func(function () {/*
    using System.Threading.Tasks;
    using System.Threading;

    public class Startup
    {
        public async Task<object> Invoke(object input)
        {
            int v = (int)input;
            var waitEvent = new ManualResetEventSlim(false);
            waitEvent.Wait(2500);
            return v + 2;
        }
    }
*/});


test(0, function (error, result) {
    if(error) throw error;
    console.log(result);
});
