'use strict';

module.exports = function (Q, Error) {
    return {
        'execute': function (filter, page) {
            let defered = Q.defer(),
                /*    url = config.db + '_fti/local/' + config.dbName + '/_design/errors/by_message?sort=' + encodeURIComponent(filter.sort) + 'createdAt%3Cdate%3E&include_docs=true&limit=25&skip=' + ((page - 1) * 25) + '&q=appName:' + filter.appName;


            if (filter.query) url += ' AND message:' + filter.query + '~';
            url += ' AND solved:' + filter.solved.toString();

            console.log(url);
            request.get(url, function (res, body) {
                let result = JSON.parse(body.body);

                if (!result.rows) defered.reject(body);
                else {
                    let errors = [];
                    result.rows.forEach(function (item) {
                        errors.push(item.doc);
                    });
                    defered.resolve({
                        currentPage: page,
                        sugestions: [],
                        totalPages: Math.ceil(result.total_rows / 25),
                        totalSize: result.total_rows,
                        data: errors
                    });
                }
            });*/

                Error.aggregate({
                        $group: {
                            _id: {
                                hash: "$hash",
                                appName: "$appName"
                            },
                            message: {
                                $last: "$message"
                            },
                            createdAt: {
                                $last: "$createdAt"
                            },
                            count: {
                                $sum: 1
                            }
                        }
                    },
                    function (err, errors) {
                        if (err) defered.reject(err);
                        if (errors) defered.resolve(errors);
                    });


            return defered.promise;
        }
    }
}
