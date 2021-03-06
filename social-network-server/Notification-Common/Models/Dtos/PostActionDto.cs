﻿using Newtonsoft.Json;
using Notification_Common.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Notification_Common.Models.Dtos
{
    public class PostActionDto
    {
        [JsonProperty("user")]
        public User User { get; set; }

        [JsonProperty("postId")]
        public string PostId { get; set; }

        [JsonProperty("reciverId")]
        public string ReciverId { get; set; }

        [JsonProperty("actionId")]
        public NotificationAction Action { get; set; }
    }
}
