using ContactAppWeb.Models;
using Microsoft.AspNetCore.Identity;

namespace ContactAppWeb.Data
{
    public static class DataSeeder
    {
        public static void SeedInitialContacts(DataContext context, IServiceProvider serviceProvider)
        {
            // Clear existing sandbox contacts
            var sandboxContacts = context.ContactModels.Where(c => c.UserId == null);
            context.ContactModels.RemoveRange(sandboxContacts);
            context.SaveChanges();

            // Seed new sandbox contacts (no UserId)
            var random = new Random();
            var newSandboxContacts = Enumerable.Range(1, 10).Select(i => new ContactModel
            {
                FirstName = "Sandbox" + i,
                LastName = "User" + i,
                PhoneNumber = GenerateRandomPhoneNumber(random),
                Email = $"sandbox.user{i}@example.com",
                UserId = null // Ensure UserId is null for sandbox contacts
            }).ToList();

            context.ContactModels.AddRange(newSandboxContacts);
            context.SaveChanges();

            // Seed user-specific contacts if users do not exist
            if (!context.Users.Any())
            {
                var userManager = serviceProvider.GetRequiredService<UserManager<IdentityUser>>();

                var user = new IdentityUser
                {
                    UserName = "testuser@example.com",
                    Email = "testuser@example.com"
                };

                var result = userManager.CreateAsync(user, "Password123!").Result;

                if (result.Succeeded)
                {
                    var userContacts = new List<ContactModel>
                    {
                        new ContactModel
                        {
                            FirstName = "John",
                            LastName = "Doe",
                            PhoneNumber = "1234567890",
                            Email = "john.doe@example.com",
                            UserId = user.Id
                        },
                        new ContactModel
                        {
                            FirstName = "Jane",
                            LastName = "Doe",
                            PhoneNumber = "0987654321",
                            Email = "jane.doe@example.com",
                            UserId = user.Id
                        }
                    };

                    context.ContactModels.AddRange(userContacts);
                    context.SaveChanges();
                }
            }
        }

        private static string GenerateRandomPhoneNumber(Random random)
        {
            return $"{random.Next(100, 1000)}{random.Next(100, 1000)}{random.Next(1000, 10000)}";
        }
    }
}
