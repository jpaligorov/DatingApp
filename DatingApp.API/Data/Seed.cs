using System;
using System.Collections.Generic;
using System.Linq;
using DatingApp.API.Models;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace DatingApp.API.Data
{
    public class Seed
    {
        public static void SeedUsers(DataContext context, ILogger logger)
        {
            logger.LogInformation("CHECK IF NO USERS");
            if (!context.Users.Any())
            {
                try
                {
                    logger.LogInformation("START READING JSON FILE");
                    var userData = System.IO.File.ReadAllText("Data/UserSeedData.json");
                    logger.LogInformation("START DESERIALISE DATA");
                    var users = JsonConvert.DeserializeObject<List<User>>(userData);

                    foreach (var user in users)
                    {
                        byte[] passwordHash, passwordSalt;
                        CreatePasswordHash("password", out passwordHash, out passwordSalt);

                        user.PasswordHash = passwordHash;
                        user.PasswordSalt = passwordSalt;
                        user.Username = user.Username.ToLower();
                        context.Users.Add(user);
                    }
                }
                catch (Exception ex)
                {
                    logger.LogError("ERROR SEEDING USERS: {0}", ex);
                }

                context.SaveChanges();
            }
        }

        private static void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using(var hmac = new System.Security.Cryptography.HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }
    }
}