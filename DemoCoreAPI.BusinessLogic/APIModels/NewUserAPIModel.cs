﻿using System;
using System.Collections.Generic;
using System.Text;

namespace DemoCoreAPI.BusinessLogic.APIModels
{
    public class NewUserAPIModel
    {
        public string Message { get; set; } = "User has been created";
        public bool Success { get; set; } = true;
    }
}
