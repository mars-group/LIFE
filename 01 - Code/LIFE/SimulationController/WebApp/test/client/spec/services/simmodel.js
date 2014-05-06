'use strict';

describe('Service: simmodel', function () {

  // load the service's module
  beforeEach(module('marsmissionControlApp'));

  // instantiate service
  var simmodel;
  beforeEach(inject(function (_simmodel_) {
    simmodel = _simmodel_;
  }));

  it('should do something', function () {
    expect(!!simmodel).toBe(true);
  });

});
