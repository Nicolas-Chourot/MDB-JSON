using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace MDB.Models
{
    public class Login
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public System.DateTime LoginDate { get; set; }
        public System.DateTime LogoutDate { get; set; }
        public string IpAddress { get; set; }
        [JsonIgnore]
        public User User
        {
            get
            {
                return DB.Users.Get(UserId);
            }
        }
    }
}
