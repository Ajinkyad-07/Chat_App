﻿using FirebaseAdmin.Auth;
using WebSocketServer.Domain.Entities;
using WebSocketServer.Domain.Interfaces;
using System.Net.Http.Json;
using Microsoft.Extensions.Configuration;
using WebSocketServer.Application.ViewModels;
using WebSocketServer.Domain.RequestModels;

namespace WebSocketServer.Infrastructure.Services
{
    public class FirebaseUserService : IFirebaseUserService
    {
        private readonly FirebaseAuth _adminAuth;
        private readonly string _firebaseApiKey;
        private readonly HttpClient _httpClient;

        public FirebaseUserService(IConfiguration configuration)
        {
            _adminAuth = FirebaseAuth.DefaultInstance;
            _firebaseApiKey = configuration.GetSection("Firebase")["ApiKey"];
            _httpClient = new HttpClient();
        }

        public async Task<User> RegisterUserAsync(User user)
        {
            // Create user with Firebase Admin SDK
            var createdUser = await _adminAuth.CreateUserAsync(new UserRecordArgs
            {
                Email = user.Email,
                Password = user.Password,
                DisplayName = user.DisplayName,
            });
            user.Uid = createdUser.Uid;
            return user;
        }

        public async Task<User> LoginUserAsync(LoginRequest request)
        {
            try
            {
                //// Login user with Firebase REST API
                //var payload = new
                //{
                //    email = email,
                //    password = password,
                //    returnSecureToken = true
                //};

                var response = await _httpClient.PostAsJsonAsync(
                    $"https://identitytoolkit.googleapis.com/v1/accounts:signInWithPassword?key={_firebaseApiKey}",
                    request);

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    throw new Exception($"Login failed: {errorContent}");
                }

                var result = await response.Content.ReadFromJsonAsync<FirebaseLoginResponse>();
                var user = await _adminAuth.GetUserAsync(result.LocalId);
                return new User
                {
                    Uid = user.Uid,
                    Email = user.Email,
                    DisplayName = user.DisplayName,
                };
            }
            catch (Exception ex)
            {
                throw new Exception("Login failed: " + ex.Message);
            }
        }

        public async Task<IEnumerable<User>> GetAllUsersAsync()
        {
            var users = new List<User>();
            var pagedEnumerable = _adminAuth.ListUsersAsync(null);
            await foreach (var user in pagedEnumerable)
            {
                users.Add(new User
                {
                    Uid = user.Uid,
                    Email = user.Email,
                    DisplayName = user.DisplayName,
                });
            }

            return users;
        }

        public async Task<User> EditUserAsync(string uid, User updatedUser)
        {
            var userRecord = await _adminAuth.UpdateUserAsync(new UserRecordArgs
            {
                Uid = uid,
                Email = updatedUser.Email,
                DisplayName = updatedUser.DisplayName,
            });

            return new User
            {
                Uid = userRecord.Uid,
                Email = userRecord.Email,
                DisplayName = userRecord.DisplayName,
            };
        }

        public async Task DeleteUserAsync(string uid)
        {
            await _adminAuth.DeleteUserAsync(uid);
        }

        public async Task<string> VerifyIdTokenAsync(string idToken)
        {
            try
            {
                var decodedToken = await _adminAuth.VerifyIdTokenAsync(idToken);
                return decodedToken.Uid;
            }
            catch (Exception ex)
            {
                throw new Exception("Token verification failed: " + ex.Message);
            }
        }
    }
}
