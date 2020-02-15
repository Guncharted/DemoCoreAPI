﻿using DemoCoreAPI.BusinessLogic.Interfaces;
using DemoCoreAPI.BusinessLogic.ViewModels;
using DemoCoreAPI.Data;
using DemoCoreAPI.DomainModels.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace DemoCoreAPI.BusinessLogic.Implementation
{
    public class AuthService : IAuthService
    {
        private readonly IRepository<UserDb> _repo;
        public AuthService(IRepository<UserDb> repo)
        {
            _repo = repo;
            if (repo == null)
                throw new ArgumentNullException(nameof(repo), "Repo is null.");
        }
        public LoginResultViewModel Login(LoginViewModel model)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model), "LoginViewModel can not be null.");
            if (string.IsNullOrWhiteSpace(model.Email))
                throw new ArgumentException("Email can not be empty.");
            if (string.IsNullOrWhiteSpace(model.Password))
                throw new ArgumentException("Password can not be empty.");            

            try
            {
                var hashedPassword = HashPassword(model.Password);
                var user = _repo.Where(x => x.Email == model.Email && x.Password == hashedPassword).FirstOrDefault();
                if (user == null)
                    throw new ArgumentException("User with such credentials doesn't exist.");
                return new LoginResultViewModel
                {
                    Id = user.Id,
                    Age = user.Age,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Email = user.Email,
                    IsAdmin = user.IsAdmin
                };
            }
            catch (Exception)
            {
                throw;
            }
        }

        public RegisterResultViewModel Register(RegisterViewModel model)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));
            var isValid = ComparePasswords(model);
            if (!isValid)
                throw new ArgumentException("Passwords don't match!");
            var exists = _repo.Where(x=>x.Email == model.Email).Any();
            if(exists)
                throw new ArgumentException("User with such email already exists!");
            try
            {
                var hashedPassword = HashPassword(model.Password);
                var user = new UserDb();
                user.FirstName = model.FirstName;
                user.LastName = model.LastName;
                user.Age = model.Age;
                user.Email = model.Email;
                user.Password = hashedPassword;
                _repo.Add(user);
                _repo.SaveChanges();

                return new RegisterResultViewModel()
                {
                    Message = "User has been created",
                    Success = true
                };
            }
            catch (Exception)
            {
                throw;
            }
        }

        private string HashPassword(string password)
        {
            return UseHash256(Encoding.UTF8.GetBytes(password));
        }

        private string UseHash256(byte[] input)
        {
            using (var algo = HashAlgorithm.Create("sha256"))
            {
                return Convert.ToBase64String(algo.ComputeHash(input));
            }
        }
        private bool ComparePasswords(RegisterViewModel model)
        {
            if (model.Password == model.PasswordConfirmation)
                return true;
            else
                return false;
        }
    }
}
