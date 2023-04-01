using System;
using System.Collections.Generic;

namespace MDB.Models
{
    public class ResetPasswordCommand
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int VerificationCode { get; set; }
    }
}
