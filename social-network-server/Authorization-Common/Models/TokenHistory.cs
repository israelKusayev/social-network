﻿using Amazon.DynamoDBv2.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Authorization_Common.Models
{
    [DynamoDBTable("Tokens")]
    public class TokenHistory
    {
        [DynamoDBHashKey]
        public string UserId { get; set; }

        [DynamoDBRangeKey]
        public long TimeStamp { get; set; }

        public string Token { get; set; }

        public string FacebookToken { get; set; }
    }
}
