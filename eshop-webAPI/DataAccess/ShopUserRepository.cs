﻿using eshopAPI.Models;
using eshopAPI.Requests.User;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace eshopAPI.DataAccess
{
    public interface IShopUserRepository : IBaseRepository
    {
        Task<ShopUserProfile> GetUserProfile(string email);
        void UpdateUserProfile(ShopUser user, UpdateUserRequest request);
        Task<ShopUser> GetUserWithEmail(string email);
        IQueryable<UserVM> GetAllUsersAsQueryable();
    }

    public class ShopUserRepository : BaseRepository, IShopUserRepository
    {
        public ShopUserRepository(ShopContext context) : base(context)
        {
        }

        public async Task<ShopUserProfile> GetUserProfile(string email)
        {
            IQueryable<ShopUser> query = Context.Users.Where(u => u.NormalizedEmail.Equals(email.Normalize())).Include(user => user.Address);
            if (await query.CountAsync() != 1)
                return null;

            return query.First().GetUserProfile();
        }

        public void UpdateUserProfile(ShopUser user, UpdateUserRequest request)
        {
            if (user == null)
                return;

            if (user.Address == null)
            {
                user.UpdateUserFromRequestCreateAddress(request);
            }
            else
            {

                user.UpdateUserFromRequestUpdateAddress(request);
            }
        }

        public async Task<ShopUser> GetUserWithEmail(string email)
        {
            var query = Context.Users.Where(u =>
                u.NormalizedEmail.Equals(email.Normalize())).Include(user => user.Address);

            if (await query.CountAsync() != 1)
                return null;
            return query.First();
        }



        public IQueryable<UserVM> GetAllUsersAsQueryable()
        {
            var query =
                from user in Context.Users
                join uRole in Context.UserRoles on user.Id equals uRole.UserId
                join role in Context.Roles on uRole.RoleId equals role.Id
                select new UserVM
                {
                    Id = user.Id,
                    Email = user.Email,
                    FullName = $"{user.Name} {user.Surname}",
                    Role = role.Name
                };

            return query;
        }
    }
}

