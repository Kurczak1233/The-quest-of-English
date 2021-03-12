﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TheEnglishQuestCore.Interfaces;
using TheEnglishQuestDatabase;
using TheEnglishQuestDatabase.Entities;

namespace TheEnglishQuestCore.Managers
{
    public class ApplicationUserManager : DTOManager<ApplicationUser ,ApplicationUserDto>, IApplicationUserDto
    {
        protected readonly IApplicationUserRepository _ApplicationUserRepository;
        public ApplicationUserManager(IApplicationUserRepository _enc, DTOMapper<ApplicationUser, ApplicationUserDto> mapper) : base(mapper)
        {
            _ApplicationUserRepository = _enc;
        }

        public Task<bool> AddNewUser(IApplicationUserDto user)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteUser(IApplicationUserDto user)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<IApplicationUserDto>> GetUser(IApplicationUserDto user)
        {
            throw new NotImplementedException();
        }
    }
}
