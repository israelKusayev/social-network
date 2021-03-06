﻿using Authorization_Common.Interfaces;
using Authorization_Common.Interfaces.Repositories;
using Authorization_Common.Models;
using Jose;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Text;

namespace Authorization_Bl
{
    public class TokenBuilder : ITokenBuilder
    {
        private IDynamoDbRepository<TokenHistory> _tokenReposirory;
        public TokenBuilder(IDynamoDbRepository<TokenHistory> tokenReposirory)
        {
            _tokenReposirory = tokenReposirory;
        }
        public string GenerateKey(string userId, string username, bool isAdmin = false, string facebookToken = null)
        {
            int ttl = int.Parse(ConfigurationManager.AppSettings["TokenTTL"]);
            long exp = DateTimeOffset.UtcNow.ToUnixTimeSeconds() + 60 * ttl;
            long iat = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

            var payload = new Dictionary<string, object>()
                {
                    { "sub", userId },
                    { "exp",exp  },
                    { "iat",iat  },
                    { "aud","social network"},
                    { "username",username},
                    { "isAdmin", isAdmin }
                };


            string key = ConfigurationManager.AppSettings["tokenSignKey"];
            byte[] secretKey = Encoding.ASCII.GetBytes(key);
            string token = JWT.Encode(payload, secretKey, JwsAlgorithm.HS256);

            TokenHistory history = new TokenHistory() { Token = token, FacebookToken = facebookToken, UserId = userId, TimeStamp = iat };
            _tokenReposirory.Add(history);

            return token;
        }

    }
}
