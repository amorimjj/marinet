﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Nancy;
using Nancy.Authentication.Forms;
using Nancy.ModelBinding;
using Nancy.Security;

namespace Marinete.Web.modules
{
    public class LoginModule : NancyModule
    {
        public LoginModule()
        {
            Get["/login"] = parameters => View[new LoginModel()];

            Get["/logout"] = parameters => this.Logout("/");

            Post["/login"] = parameters =>
                {
                    var viewModel = this.Bind<LoginModel>();
                    
                    if ("admin" == viewModel.Username &&
                        "password" == viewModel.Password)
                    {
                        return this.Login(new Guid("757538E3-A798-4DCF-8620-D349B4BEECFA"));
                    }

                    return HttpStatusCode.Unauthorized;
                };
        }
    }

    public class LoginModel
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }
}