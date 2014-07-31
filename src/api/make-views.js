'use strict';
const
    async = require('async'),
    request = require('request'),
    environment = process.env.NODE_ENV || 'development',
    config = require('./config/' + environment + '.js'),
    views = require('./setup/views.js');
async.waterfall([
 // get the existing design doc (if present)
function (next) {
        request.get(config.db + config.dbName + '/_design/marinet', next);
},
 // create a new design doc or use existing
function (res, body, next) {
        if (res.statusCode === 200) {
            next(null, JSON.parse(body));
        } else if (res.statusCode === 404) {
            next(null, {
                language: 'javascript',
                views: {}
            });
        }
},
 // add views to document and submit
function (doc, next) {
        Object.keys(views).forEach(function (name) {
            doc.views[name] = views[name];
        });
        request({
            method: 'PUT',
            url: config.db + config.dbName + '/_design/marinet',
            json: doc
        }, next);
}
], function (err, res, body) {
    if (err) {
        throw err;
    }
    console.log(res.statusCode, body);
});