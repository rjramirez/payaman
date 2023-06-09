﻿using Common.DataTransferObjects._Base;
using System.Text.Json.Serialization;

namespace Common.DataTransferObjects.AppUserDetails
{
    public class AppUserDetail : SaveDTOExtension
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }

    }
}
