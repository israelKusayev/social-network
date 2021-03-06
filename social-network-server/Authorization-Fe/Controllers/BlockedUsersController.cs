﻿using Authorization_Common.Interfaces.Managers;
using Authorization_Fe.Attributes;
using log4net;
using System;
using System.Reflection;
using System.Web.Http;

namespace Authorization_Fe.Controllers
{
    public class BlockedUsersController : ApiController
    {
        private IBlockedUsersManager _blockedUsersManager;
        private readonly ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public BlockedUsersController(IBlockedUsersManager blockedUsersManager)
        {
            _blockedUsersManager = blockedUsersManager;
        }

        [HttpPost]
        [JWTAdminAuth]
        [Route("/api/Blocked/Block/{userId}")]
        IHttpActionResult Block(string userId)
        {
            try
            {
                bool blocked = _blockedUsersManager.BlockUser(userId);
                if (blocked)
                    return Ok("user has been blocked");
                else
                    return BadRequest("user was not found or already blocked");
            }
            catch (Exception e)
            {
                _log.Error(e);
                return InternalServerError();
            }
        }

        [HttpPost]
        [JWTAdminAuth]
        [Route("/api/Blocked/UnBlock/{userId}")]
        IHttpActionResult UnBlock(string userId)
        {
            try
            {
                bool blocked = _blockedUsersManager.UnBlockUser(userId);
                if (blocked)
                    return Ok("user has been unblocked");
                else
                    return BadRequest("user was not found or already unblocked");
            }
            catch (Exception e)
            {
                _log.Error(e);
                return InternalServerError();
            }
        }
    }
}
