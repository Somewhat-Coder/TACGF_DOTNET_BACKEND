using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjectBackend.Models
{
     public class registerStudent
     {
          public string fullName { get; set; }
          public string phoneNumber { get; set; }

          public string gender { get; set; }

          public string mode { get; set; }

          public List<string> courses { get; set; }

          public int hours { get; set; }

          public int expFees { get; set; }
     }
}