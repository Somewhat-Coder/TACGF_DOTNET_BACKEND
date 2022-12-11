using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjectBackend.Models
{
     public class teacher
     {
          public String id { get; set; }
          public String firstName { get; set; }
          public String lastName { get; set; }

          public String phoneNumber { get; set; }

          public String email { get; set; }
          public String password { get; set; }

          public String subjects { get; set; }
     }
}