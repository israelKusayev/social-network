﻿using Social_Common.Interfaces.Managers;
using Social_Common.Models.Dtos;
using Social_Fe.Attributes;
using SocialBl.Managers;
using System.Linq;
using System.Web.Http;

namespace Social_Fe.Controllers
{
    public class CommentsController : ApiController
    {
        private ICommentsManager _commentsManager;
        private ITokenManager _tokenManager;

        public CommentsController(ICommentsManager commentsManager,
            ITokenManager tokenManager)
        {
            _commentsManager = commentsManager;
            _tokenManager = tokenManager;
        }

        [HttpPost]
        [JWTAuth]
        public IHttpActionResult Create([FromBody] CreateCommentDto commentDto)
        {
            var token = Request.Headers.GetValues("x-auth-token").First();
            var res = _commentsManager.Create(commentDto, _tokenManager.GetUser(token));
            if (res)
            {
                return Ok();
            }
            else
            {
                return InternalServerError();
            }
        }


        [HttpGet]
        [JWTAuth]
        public IHttpActionResult Get(string id)
        {
            var token = Request.Headers.GetValues("x-auth-token").First();
            string userId = _tokenManager.GetUserId(token);
            var res = _commentsManager.GetByPost(id, userId);
            if (res != null)
            {
                return Ok(res);
            }
            else
            {
                return InternalServerError();
            }
        }
    }
}
