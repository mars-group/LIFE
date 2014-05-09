var mongoose = require('mongoose'),
    MARSComFile = mongoose.model('MARSComFile');

/**
 * List of files from user
 */
exports.getFilesFromUser = function(req, res) {
    MARSComFile.find({userId: req.user._id}).sort('-updatedAt').exec(function(err, files) {
        if (err) return res.json(500, err);
        res.json(files);
    });
};

/**
 * Show a file
 */
exports.getFileById = function(req, res) {
    console.log("Get a file");
    MARSComFile.findById(req.body.fileId, function(err, file) {
        if (err){
            console.log(err);
            return res.json(500, err);
        }
        res.json(file);
    });
};

/**
 * Does file exist
 */
exports.fileExists = function(req, res) {
    MARSComFile.find({userId: req.user._id, filename: req.body.filename}).exec(function(err, files) {
        if (err) return res.json(500, err);

        if(files.length > 0)
            res.json({fileExists: true, fileId: files[0]._id});
        else
            res.json({fileExists: false});
    });
};

/**
 * Create a file
 */
exports.createFile = function(req, res) {
    var file = new MARSComFile({
        userId: req.user._id,
        filename: req.body.filename,
        content: req.body.content
    });

    console.log("Created this File: ", file);

    file.save(function(err) {
        if (err) return res.json(500, err);
        res.json(file);
    });
};

/**
 * Update a file
 */
exports.updateFile = function(req, res) {

    MARSComFile.update(
            { _id: req.body.fileId },
            {content: req.body.content, updatedAt: Date.now()},
            { },
            function(err, updatedFile) {
                if (err){
                    console.log(err);
                    return res.json(500, err);
                }
                res.json(updatedFile);
    });
};

/**
 * Remove a file
 */
exports.removeFile = function(req, res) {
    MARSComFile.remove({ _id: req.body.fileId }, function(err) {
        if (err) return res.json(500, err);
        res.json({"success" : 1});
    });
};