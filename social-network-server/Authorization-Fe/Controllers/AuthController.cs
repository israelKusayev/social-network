﻿using Authorization_Bl;
using Authorization_Common.Interfaces;
using Authorization_Common.Interfaces.Managers;
using Authorization_Common.Interfaces.Helppers;
using Authorization_Common.Models;
using Authorization_Common.Models.DTO;
using System;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Authorization_Common.Exceptions;
using System.Linq;
using log4net;
using System.Reflection;

namespace Authorization_Fe.Controllers
{
    public class AuthController : ApiController
    {
        private readonly ITokenBuilder _token;
        private readonly IAuthManager _authManager;
        private readonly IFaceBookTokenValidator _facebookValidator;

        private readonly ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public AuthController(ITokenBuilder token, IAuthManager authManager,
            IFaceBookTokenValidator facebookValidator)
        {
            _authManager = authManager;
            _token = token;
            _facebookValidator = facebookValidator;
        }

        [Route("api/register")]
        [HttpPost]
        public IHttpActionResult Register([FromBody] RegisterDTO model)
        {
            string error = Validations.ValidateRegister(model);
            if (error != null)
            {
                return BadRequest(error);
            }
            UserAuth auth;
            string token;
            try
            {
                auth = _authManager.Register(model);
                if (auth == null)
                {
                    return BadRequest("Username already exists");
                }
                token = _token.GenerateKey(auth.UserId, model.Username);
                _authManager.AddUserToIdentity(auth.UserId, model.Username, model.Email, token);
                _authManager.AddUserToSocial(auth.UserId, model.Username, token);
            }
            catch (Exception e)
            {
                _log.Error(e);
                return InternalServerError();
            }

            HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.OK);
            response.Headers.Add("x-auth-token", token);
            return ResponseMessage(response);
        }

        [Route("api/login")]
        [HttpPost]
        public IHttpActionResult Login([FromBody] LoginDTO model)
        {
            string error = Validations.ValidateLogin(model);
            if (error != null)
            {
                return BadRequest(error);
            }
            try
            {
                var auth = _authManager.Login(model);
                if (auth == null)
                {
                    return BadRequest("incorrect details");
                }

                var token = _token.GenerateKey(auth.UserId, model.Username, auth.IsAdmin);

                HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.OK);
                response.Headers.Add("x-auth-token", token);
                return ResponseMessage(response);
            }
            catch (UserBlockedException ube)
            {
                return BadRequest(ube.Message);
            }
            catch (Exception e)
            {
                _log.Error(e);
                return InternalServerError();
            }
        }

        [Route("api/loginFacebook")]
        [HttpPost]
        public IHttpActionResult LoginFacebook([FromBody] string facebookToken)
        {
            if (facebookToken == null)
            {
                return BadRequest("Token is missing");
            }
            try
            {
                FacebookLoginDTO model = _facebookValidator.ValidateAndGet(facebookToken);
                if (model == null)
                {
                    return BadRequest("invalid token");
                }
                UserFacebook facebookUser = _authManager.LoginFacebook(model);

                var token = _token.GenerateKey(facebookUser.UserId, model.Username, facebookUser.IsAdmin, facebookToken);

                if (_authManager.IsNewFacebookUser(model.FacebookId))
                {
                    _authManager.AddUserToIdentity(facebookUser.UserId, model.Username, model.Email, token);
                    _authManager.AddUserToSocial(facebookUser.UserId, model.Username, token);
                }

                HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.OK);
                response.Headers.Add("x-auth-token", token);
                return ResponseMessage(response);
            }
            catch (UserBlockedException ube)
            {
                return BadRequest(ube.Message);
            }
            catch (Exception e)
            {
                _log.Error(e);
                return InternalServerError();
            }

        }

        [HttpPost]
        [Route("api/resetPassword")]
        public IHttpActionResult ResetPassword(ResetPasswordDTO model)
        {
            string error = Validations.ValidateResetPassword(model);
            if (error != null)
            {
                return BadRequest(error);
            }

            try
            {
                if (!_authManager.ResetPassword(model))
                {
                    return BadRequest("incorrect details");
                }
                return Ok();
            }
            catch (Exception e)
            {
                _log.Error(e);
                return InternalServerError();
            }
        }

        [HttpGet]
        [Route("api/refreshToken")]
        public IHttpActionResult RefreshToken()
        {
            try
            {
                if (Request.Headers.Contains("x-auth-token"))
                {
                    string token = Request.Headers.GetValues("x-auth-token").First();
                    token = _authManager.RefreshToken(token);
                    if (token != null)
                    {
                        HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.OK);
                        response.Headers.Add("x-auth-token", token);
                        return ResponseMessage(response);
                    }
                    return BadRequest("invalid token");
                }
                return BadRequest("No token was given");
            }
            catch (Exception e)
            {
                _log.Error(e);
                return InternalServerError();
            }
        }
    }
}
