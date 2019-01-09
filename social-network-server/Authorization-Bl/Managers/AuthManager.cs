﻿using System;
using System.Configuration;
using System.Net.Http;
using Authorization_Common.Exceptions;
using Authorization_Common.Interfaces;
using Authorization_Common.Interfaces.Helppers;
using Authorization_Common.Interfaces.Managers;
using Authorization_Common.Interfaces.Repositories;
using Authorization_Common.Models;
using Authorization_Common.Models.DTO;

namespace Authorization_Bl.Managers
{
    public class AuthManager : IAuthManager
    {
        IDynamoDbRepository<UserAuth> _authRepository;
        IDynamoDbRepository<UserFacebook> _oAuthRepository;
        IDynamoDbRepository<BlockedUser> _blockedRepository;
        ITokenValidator _tokenValidator;
        ITokenBuilder _tokenBuilder;

        public AuthManager(IDynamoDbRepository<UserAuth> authRepository,
            IDynamoDbRepository<UserFacebook> oAuthRepository,
            IDynamoDbRepository<BlockedUser> blockedRepository,
            ITokenValidator tokenValidator,
            ITokenBuilder tokenBuilder)
        {
            _authRepository = authRepository;
            _oAuthRepository = oAuthRepository;
            _blockedRepository = blockedRepository;
            _tokenValidator = tokenValidator;
            _tokenBuilder = tokenBuilder;
        }

        public UserAuth Register(RegisterDTO model)
        {
            var guid = Guid.NewGuid().ToString();
            return _authRepository.Add(new UserAuth() { Username = model.Username, Password = model.Password, UserId = guid });
        }

        public UserAuth Login(LoginDTO model)
        {
            var user = _authRepository.Get(model.Username);
            if (user == null || user.Password != model.Password)
            {
                return null;
            }
            if (IsBlocked(user.UserId))
            {
                throw new UserBlockedException();
            }
            return user;
        }

        public UserFacebook LoginFacebook(FacebookLoginDTO model)
        {
            var existingUser = _oAuthRepository.Get(model.FacebookId);
            if (existingUser != null)
            {
                if (IsBlocked(existingUser.UserId))
                {
                    throw new UserBlockedException();
                }
                return existingUser;
            }
            else
            {
                var guid = Guid.NewGuid().ToString();
                return _oAuthRepository.Add(new UserFacebook() { FacebookId = model.FacebookId, Username = model.Username, UserId = guid });
            }
        }

        public bool ResetPassword(ResetPasswordDTO model)
        {
            var user = _authRepository.Get(model.Username);
            if (user == null) return false;
            user.Password = model.NewPassword;
            _authRepository.Update(user);
            return true;
        }

        /// <summary>
        /// create new user in user table on dynamoDB
        /// </summary>
        public string AddUserToIdentity(string userId, string username, string email, string token)
        {
            string url = ConfigurationManager.AppSettings["IdentityServiceUrl"];
            url += "/UsersIdentity";
            User user = new User() { UserId = userId, Email = email, Username = username };
            using (var http = new HttpClient())
            {
                http.DefaultRequestHeaders.Add("x-auth-token", token);
                var response = http.PostAsJsonAsync(url, user).Result;
                return response.Content.ReadAsStringAsync().Result;
            }
        }

        /// <summary>
        /// create new node in neo4j
        /// </summary>
        public bool AddUserToSocial(string userId, string username, string token)
        {
            string url = ConfigurationManager.AppSettings["SocialServiceUrl"];
            url += "/users";
            User user = new User() { UserId = userId, Username = username };
            using (var http = new HttpClient())
            {
                http.DefaultRequestHeaders.Add("x-auth-token", token);
                var response = http.PostAsJsonAsync(url, user).Result;
                return response.IsSuccessStatusCode;
            }
        }

        public string RefreshToken(string token)
        {
            var data = _tokenValidator.ValidaleRefreshToken(token);
            if (data == null)
                return data;
            return _tokenBuilder.GenerateKey((string)data.sub, (string)data.username, (bool)data.IsAdmin);
        }

        private bool IsBlocked(string userId)
        {
            if (_blockedRepository.Get(userId) == null)
                return false;
            return true;
        }
    }
}
