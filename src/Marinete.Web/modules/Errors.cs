﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Marinete.Common.Domain;
using Marinete.Common.Indexes;
using Nancy;
using Raven.Client;

namespace Marinete.Web.modules
{
    public class Errors : NancyModule
    {
        private readonly IDocumentSession _documentSession;

        public Errors(IDocumentSession documentSession)
        {
            _documentSession = documentSession;

            Get["/errors/{appName}"] = _ =>
                {
                    var appName = (string)_.appName;
                    int page;
                    int size = 10;

                    if (!int.TryParse(Request.Query["page"], out page))
                        page = 1;

                    RavenQueryStatistics stats;

                    var errors =
                        _documentSession.Query<UniqueVisitorsIndex.UniqueError, UniqueVisitorsIndex>()
                        .Statistics(out stats)
                                       .Where(c => c.AppName == appName)
                                       .OrderByDescending(c => c.CreatedAt)
                                       .Skip((page - 1 > 0 ? page - 1 : 0) * size)
                                       .Take(size)
                                       .ToList();

                    return  Response.AsJson(new PagedResult<UniqueVisitorsIndex.UniqueError>(errors, 
                        stats.TotalResults, 
                        page, 
                        size));
                };

            Get["/error/{id}"] = _ =>
                {
                    var id = (string) _.id;

                    var error = _documentSession.Load<Error>("errors/" + id);

                    return Response.AsJson(new
                        {
                            error.Exception,
                            error.Message,
                            error.OsVersion,
                            error.Platform,
                            error.Processors,
                            error.ServicePack,
                            error.AppName,
                            error.CreatedAt,
                            error.CurrentUser
                        });
                };
        }
    }

    public class PagedResult<TObj>
    {
        private readonly int _totalSize;
        private readonly int _currentPage;
        private readonly int _pageSize;
        private readonly int _totalPages;

        public int TotalPages
        {
            get { return _totalPages; }
        }

        public int PageSize
        {
            get { return _pageSize; }
        }

        public int CurrentPage
        {
            get { return _currentPage; }
        }

        public int TotalSize
        {
            get { return _totalSize; }
        }

        public IEnumerable<TObj> Data { get; protected set; }

        public PagedResult(IEnumerable<TObj> errors,int totalSize, int currentPage, int pageSize)
        {
            _totalSize = totalSize;
            _currentPage = currentPage;
            _pageSize = pageSize;
            _totalPages = (int)Math.Ceiling((decimal)TotalSize/PageSize);
            Data = errors;
        }
    }
}