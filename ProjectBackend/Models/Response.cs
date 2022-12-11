using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjectBackend.Models
{
     public class Response
     {
          public List<string> Courses { get; set; }

          public string firstName { get; set; }

          public string email { get; set; }

          public string task { get; set; }

          public string role { get; set; }

          public string phoneNumber { get; set; }

          public List<int> ids { get; set; }
     }
}