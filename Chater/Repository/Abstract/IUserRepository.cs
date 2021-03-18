﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Chater.Models;
using MongoDB.Bson;

namespace Chater.Repository.Abstract
{
    public interface IUserRepository
    { 
        Task<IEnumerable<User>> GetUsersAsync();

        Task<User> GetUserAsync(Guid id);

        Task CreateUserAsync(User user);

        Task UpdateUserAsync(User user);

        Task DeleteUserAsync(Guid id);
    }
}