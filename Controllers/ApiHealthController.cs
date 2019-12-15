﻿using API.Mowizz2.EHH.Configs;
using API.Mowizz2.EHH.Facades;
using API.Mowizz2.EHH.Services;
using Microsoft.AspNetCore.Mvc;
using System;

namespace API.Mowizz2.EHH.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ApiHealthController : ControllerBase
    {
        private readonly IDataBaseSettings _dbSettings;
        private readonly BankAccountsService _bankAccountsService;
        private readonly UsersService _usersService;        

        public ApiHealthController(
            IDataBaseSettings settings, 
            BankAccountsService bankAccountsService,
            UsersService usersService
            )
        {
            _dbSettings = settings;
            _bankAccountsService = bankAccountsService;
            _usersService = usersService;
        }

        #region Public methods

        [HttpGet]
        public HealthData Get()
        {            
            return new HealthData
            {
                CheckDate = DateTime.Now,
                DataBaseName = _dbSettings.DatabaseName,
                ServerName = GetServerName(),
                DataBaseUserName = GetUserName(),
                BankAccountsHealthStatus = _bankAccountsService.Check(),
                UsersHealthStatus = _usersService.Check()
            };
        }

        #endregion

        #region Private methods

        private string GetServerName()
        {
            int index = _dbSettings.ConnectionString.IndexOf("@") + 1;
            string longServerName = _dbSettings.ConnectionString.Substring(index);
            index = longServerName.IndexOf("/");
            return longServerName.Substring(0, index);
        }

        private string GetUserName()
        {
            int index = _dbSettings.ConnectionString.IndexOf(":");
            string longServerName = _dbSettings.ConnectionString.Substring(index);
            index = _dbSettings.ConnectionString.IndexOf(":");
            return longServerName.Substring(3, index + 3);
        }

        #endregion
    }
}