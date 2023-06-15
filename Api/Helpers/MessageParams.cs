using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Api.Helpers
{
    public class MessageParams : PaginationParams
    {
        public string UserName { get; set; } = "keller";
        public string Container { get; set; } = "Unread";
    }
}