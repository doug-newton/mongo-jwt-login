using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Dynamic;
using System.Net;

namespace jwt_user
{
    public class User
    {
        public string username { get; set; } = "";
        public string password { get; set; } = "";
    }

    public class Program
    {

        public static bool HandleSignupRequest(User user)
        {
            try
            {
                string responseText = ApiRequester.Post("/login", JsonConvert.SerializeObject(user));
                return true;
            }
            catch (WebException ex)
            {
                return false;
            }
        }

        public static bool HandleLoginRequest(User user)
        {
            try
            {
                string responseText = ApiRequester.Post("/login", JsonConvert.SerializeObject(user));
                dynamic response = JsonConvert.DeserializeObject<object>(responseText);
                ApiRequester.SetToken((string)response.token);
                return true;
            }
            catch (WebException ex)
            {
                return false;
            }
        }

        private static User PromptUsernameAndPassword()
        {
            User user = new User();
            Console.WriteLine("enter username: ");
            user.username = Console.ReadLine();
            Console.WriteLine("enter password: ");
            user.password = Console.ReadLine();
            return user;
        }

        public static void Signup()
        {
            Console.WriteLine("Signup");
            User user = PromptUsernameAndPassword();

            if (HandleSignupRequest(user) == true)
            {
                Console.WriteLine("signup successful");
            }
            else
            {
                Console.WriteLine("signup failed");
            }
        }

        public static void Login()
        {
            Console.WriteLine("Login");
            User user = PromptUsernameAndPassword();

            if (HandleLoginRequest(user) == true)
            {
                Console.WriteLine("login successful");
            }
            else
            {
                Console.WriteLine("login failed");
            }
        }

        public static void AccessProtectedRoute()
        {
            string responseText = ApiRequester.Get("/data");
            dynamic response = JsonConvert.DeserializeObject<object>(responseText);
            Console.WriteLine(response.message);
        }

        public static void Main(string[] args)
        {
            Config.ReadConfig();
            Signup();
            Login();
            AccessProtectedRoute();
        }
    }
}
