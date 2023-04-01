using System;
using System.Collections.Generic;

namespace MDB.Models
{
    public class UnverifiedEmail
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public int VerificationCode { get; set; }
        public int UserId { get; set; }
    }
}
