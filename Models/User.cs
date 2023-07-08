namespace Musich_API.Models
{
    public class User
    {
        public string Id { get; set; }
        public String UserName { get; set; }
        public String Password { get; set; }
        public String rol { get; set; }

        public static List<User> GetUsers()
        {
            List<User> users = new List<User>()
                {
                    new User(){Id="1", UserName = "Juan", Password = "sjjsjsj", rol="administrator"},
                    new User(){Id="2", UserName = "Jose", Password = "1234", rol="user"},
                    new User(){Id="3", UserName = "Fer", Password = "4444", rol="user"},
                    new User(){Id="4", UserName = "Andres", Password = "222", rol="user"},
                    new User(){Id="5", UserName = "Pedro", Password = "2112", rol="user"}
                };
            return users;
        }


    }
}
