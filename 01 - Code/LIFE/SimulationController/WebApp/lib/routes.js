'use strict';

var index = require('./controllers'),
    users = require('./controllers/users'),
    session = require('./controllers/session'),
    marscontrol = require('./controllers/marscontrol'),
    marscomfiles = require('./controllers/marscomfiles');

var middleware = require('./middleware');

/**
 * Application routes
 */
module.exports = function(app) {

  // Server API Routes
  app.get('/api/marscontrol/', marscontrol.allModels);
  app.get('/api/marscontrol/nodes', marscontrol.getConnectedNodes);

  app.post('/api/marscontrol/startsim', marscontrol.startSimWithModel);
  app.post('/api/marscontrol/pausesim', marscontrol.pauseSimulation);
  app.post('/api/marscontrol/abortsim', marscontrol.abortSimulation);
  app.post('/api/marscontrol/resumesim', marscontrol.resumeSimulation);

  //MARSComFile routes
  app.post('/api/marscomfiles/getfile', marscomfiles.getFileById);
  app.post('/api/marscomfiles/getfiles', marscomfiles.getFilesFromUser);
  app.post('/api/marscomfiles/fileexists', marscomfiles.fileExists);
  app.post('/api/marscomfiles/createfile', marscomfiles.createFile);
  app.post('/api/marscomfiles/updatefile', marscomfiles.updateFile);
  app.post('/api/marscomfiles/deletefile', marscomfiles.removeFile);

  // User specific API
  app.post('/api/users', users.create);
  app.put('/api/users', users.changePassword);
  app.get('/api/users/me', users.me);
  app.get('/api/users/:id', users.show);

  app.post('/api/session', session.login);
  app.del('/api/session', session.logout);

  // All undefined api routes should return a 404
  app.get('/api/*', function(req, res) {
    res.send(404);
  });
  
  // All other routes to use Angular routing in app/scripts/app.js
  app.get('/partials/*', index.partials);
  app.get('/*', middleware.setUserCookie, index.index);
};