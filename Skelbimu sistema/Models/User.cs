namespace Skelbimu_sistema.Models
{
    public class User
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }

        public string Password { get; set; }
        public bool Blocked { get; set; }

    }

    public class MockUserRepo
    {
        public static List<User> GetUsers()
        {
            // Generate mock user data
            var users = new List<User>
                {
                    new User { Id = 1, FirstName = "Steponas", LastName="Kairys", Email = "pastas@example.com", Password = "password", Blocked = false },
                    new User { Id = 2, FirstName = "Antanas", LastName="Smetona", Email = "pastas@example.com", Password = "password", Blocked = false },
                    new User { Id = 3, FirstName = "Vladas", LastName="Mironas", Email = "pastas@example.com", Password = "password", Blocked = true },
                    new User { Id = 4, FirstName = "Jurgis", LastName="Šaulys", Email = "pastas@example.com", Password = "password", Blocked = true },
                };

            return users;
        }

        public static User GetUserById(int id)
        {
            // Retrieve the user with the specified ID from the list of users
            return GetUsers().FirstOrDefault(user => user.Id == id);
        }
    }
}
