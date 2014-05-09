var mongoose = require('mongoose'),
    Schema = mongoose.Schema;

/**
 * MARSComFile Schema
 */
var MARSComFileSchema = new Schema({
    userId: String,
    filename: String,
    content: String,
    createdAt: Date,
    updatedAt: Date
});

MARSComFileSchema.pre('save', function(next, done){
    if (this.isNew) {
        this.createdAt = Date.now();
    }
    this.updatedAt = Date.now();
    next();
});

mongoose.model('MARSComFile', MARSComFileSchema);