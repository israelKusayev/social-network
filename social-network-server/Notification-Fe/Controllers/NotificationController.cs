﻿using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using Notification_Bl.Managers;
using Notification_Common.Interfaces.Managers;
using Notification_Common.Models.Dtos;
using NotificationFe.Hubs;

namespace Notification_Fe.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationController : ControllerBase
    {
        IHubContext<NotificationsHub> _hub;
        NotificationsManager _notificationsManager;
        ITokenManager _tokenManager;
        public NotificationController(IHubContext<NotificationsHub> hub, IConfiguration configuration)
        {
            _hub = hub;
            _notificationsManager = new NotificationsManager(configuration);
            //_notificationsManager.AddToHistory("")
            var res = configuration.GetValue<string>("key");
            _tokenManager = new TokenManager();
        }

        [Authorize]
        [HttpGet]
        [Route("GetNotifications")]
        public IActionResult GetNotifications()
        {
            try
            {
                var token = Request.Headers["x-auth-token"][0];

              var userId=  _tokenManager.GetUserId(token);
                return Ok(_notificationsManager.GetNotifications(userId));
            }
            catch (Exception e)
            {
                //TODO: add logger
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpPost]
        [Authorize(Policy = "ServerOnly")]
        [Route("UserLikePost")]
        public IActionResult UserLikePost(PostActionDto action)
        {
            try
            {
                action.ActionId = 0;
                _notificationsManager.SendNotification(_hub, action.ReciverId, "getNotification", action);
                return Ok();
            }
            catch (Exception e)
            {
                //TODO: add logger
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpPost]
        [Authorize(Policy = "ServerOnly")]
        [Route("UserLikeComment")]
        public ActionResult UserLikeComment(CommentActionDto action)
        {
            try
            {
                action.ActionId = 1;
                _notificationsManager.SendNotification(_hub, action.ReciverId, "getNotification", action);
                return Ok();
            }
            catch (Exception e)
            {
                //TODO: add logger
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpPost]
        [Authorize(Policy = "ServerOnly")]
        [Route("CommentOnPost")]
        public ActionResult CommentOnPost(CommentActionDto action)
        {
            try
            {
                action.ActionId = 2;
                _notificationsManager.SendNotification(_hub, action.ReciverId, "getNotification", action);
                return Ok();
            }
            catch (Exception e)
            {
                //TODO: add logger
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpPost]
        [Authorize(Policy = "ServerOnly")]
        [Route("ReferenceInPost")]
        public ActionResult ReferenceInPost(PostActionDto action)
        {
            try
            {
                action.ActionId = 3;
                _notificationsManager.SendNotification(_hub, action.ReciverId, "getNotification", action);
                return Ok();
            }
            catch (Exception e)
            {
                //TODO: add logger
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpPost]
        [Authorize(Policy = "ServerOnly")]
        [Route("ReferenceInComment")]
        public ActionResult ReferenceInComment(CommentActionDto action)
        {
            try
            {
                action.ActionId = 4;
                _notificationsManager.SendNotification(_hub, action.ReciverId, "getNotification", action);
                return Ok();
            }
            catch (Exception e)
            {
                //TODO: add logger
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpPost]
        [Authorize(Policy = "ServerOnly")]
        [Route("Follow")]
        public ActionResult Follow(UsersActionDto action)
        {
            try
            {
                action.ActionId = 5;
                _notificationsManager.SendNotification(_hub, action.ReciverId, "getNotification", action);
                return Ok();
            }
            catch (Exception e)
            {
                //TODO: add logger
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpPost]
        [Authorize(Policy = "ServerOnly")]
        [Route("RecommendFollwers")]
        public ActionResult RecommendFollwers([FromBody]List<FollowRecommendationDto> followRecommendations)
        {
            try
            {
                foreach (FollowRecommendationDto rec in followRecommendations)
                {
                    object action = new
                    {
                        actionId = 6,
                        recomendedId = rec.RecommededUserId
                    };
                    _notificationsManager.SendNotification(_hub, rec.UserId, "getNotification", action);
                }
                    return Ok();
            }
            catch (Exception e)
            {
                //TODO: add logger
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
    }
}