﻿using Social_Common.Models;
using Social_Fe.Attributes;
using SocialBl.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Social_Fe.Controllers
{
    [RoutePrefix("api/users")]
    public class UsersController : ApiController
    {
        UsersManager _usersManager;
        TokenManager _tokenManager;

        public UsersController(UsersManager usersManager, TokenManager tokenManager)
        {
            _usersManager = usersManager;
            _tokenManager = tokenManager;
        }

        [JWTAuth]
        [HttpPost]
        public IHttpActionResult AddUser([FromBody] User user)
        {
            if (_usersManager.AddUser(user))
            {
                return Ok();
            }
            return InternalServerError();
        }

        [JWTAuth]
        [HttpGet]
        [Route("getUsers/{searchQuery}")]
        public IHttpActionResult GetUsers(string searchQuery)
        {
            try
            {
                var token = Request.Headers.GetValues("x-auth-token").First();
                string userId = _tokenManager.GetUserId(token);
                return Ok(_usersManager.GetUsers(searchQuery, userId));
            }
            catch (Exception ex)
            {
                return InternalServerError();
            }
        }

        [JWTAuth]
        [HttpGet]
        [Route("follow/{followedUserId}")]
        public IHttpActionResult Follow(string followedUserId)
        {
            var token = Request.Headers.GetValues("x-auth-token").First();
            string userId = _tokenManager.GetUserId(token);
            if (_usersManager.Follow(userId, followedUserId))
            {
                return Ok("You followed successfully");
            }
            return InternalServerError();
        }

        [JWTAuth]
        [HttpGet]
        [Route("unfollow/{followedUserId}")]
        public IHttpActionResult Unfollow(string followedUserId)
        {
            var token = Request.Headers.GetValues("x-auth-token").First();
            string userId = _tokenManager.GetUserId(token);
            if (_usersManager.unfollow(userId, followedUserId))
            {
                return Ok("You unfollowed successfully");
            }
            return InternalServerError();
        }

        [JWTAuth]
        [HttpGet]
        [Route("isFollow/{followedUserId}")]
        public IHttpActionResult IsFollow(string followedUserId)
        {
            var token = Request.Headers.GetValues("x-auth-token").First();
            string userId = _tokenManager.GetUserId(token);
            try
            {
                return Ok(_usersManager.IsFollow(userId, followedUserId));
            }
            catch (Exception ex)
            {
                return InternalServerError();
            }
        }

        [JWTAuth]
        [HttpGet]
        [Route("following")]
        public IHttpActionResult GetFollowing()
        {
            var token = Request.Headers.GetValues("x-auth-token").First();
            string userId = _tokenManager.GetUserId(token);
            try
            {
                return Ok(_usersManager.GetFollowing(userId));
            }
            catch (Exception)
            {
                return InternalServerError();
            }
        }



        [JWTAuth]
        [HttpGet]
        [Route("followers")]
        public IHttpActionResult GetFollowers()
        {
            var token = Request.Headers.GetValues("x-auth-token").First();
            string userId = _tokenManager.GetUserId(token);
            try
            {
                return Ok(_usersManager.GetFollowers(userId));
            }
            catch (Exception ex)
            {
                return InternalServerError();
            }
        }

        [JWTAuth]
        [HttpGet]
        [Route("blockUser/{blockedUserId}")]
        public IHttpActionResult BlockUser(string blockedUserId)
        {
            var token = Request.Headers.GetValues("x-auth-token").First();
            string userId = _tokenManager.GetUserId(token);
            if (_usersManager.BlockUser(userId, blockedUserId))
            {
                return Ok("You blocked this user successfully");
            }
            return InternalServerError();
        }

        [JWTAuth]
        [HttpGet]
        [Route("UnblockUser/{blockedUserId}")]
        public IHttpActionResult UnblockUser(string blockedUserId)
        {
            var token = Request.Headers.GetValues("x-auth-token").First();
            string userId = _tokenManager.GetUserId(token);
            if (_usersManager.UnblockUser(userId, blockedUserId))
            {
                return Ok("You unblocked this user successfully");
            }
            return InternalServerError();
        }

        [JWTAuth]
        [HttpGet]
        [Route("blockedUsers")]
        public IHttpActionResult GetBlockedUsers()
        {
            var token = Request.Headers.GetValues("x-auth-token").First();
            string userId = _tokenManager.GetUserId(token);
            try
            {
                return Ok(_usersManager.GetBlockedUsers(userId));
            }
            catch (Exception)
            {
                return InternalServerError();
            }
        }

    }
}
