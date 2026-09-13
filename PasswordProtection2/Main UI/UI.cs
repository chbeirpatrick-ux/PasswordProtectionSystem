using System;
using PasswordProtectionSystem.Services;

namespace PasswordProtectionSystem
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("..........................................");
            Console.WriteLine("   PASSWORD PROTECTION DEMO SYSTEM");
            Console.WriteLine("..........................................");

            bool running = true;
            while (running)
            {
                Console.WriteLine("\nChoose an option:");
                Console.WriteLine("1) Register new user");
                Console.WriteLine("2) Login (Bonus)");
                Console.WriteLine("3) Exit");
                Console.Write("> ");

                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        Console.Write("Enter username: ");
                        string regUser = Console.ReadLine();

                        Console.Write("Enter password: ");
                        string regPass = ReadPasswordMasked();

                        UserService.RegisterUser(regUser, regPass);
                        break;

                    case "2":
                        Console.Write("Enter username: ");
                        string logUser = Console.ReadLine();

                        Console.Write("Enter password: ");
                        string logPass = ReadPasswordMasked();

                        UserService.LoginUser(logUser, logPass);
                        break;

                    case "3":
                        running = false;
                        Console.WriteLine("Goodbye!");
                        break;

                    default:
                        Console.WriteLine("Invalid option. Try again.");
                        break;
                }
            }
        }

      
        static string ReadPasswordMasked()
        {
            string password = "";
            ConsoleKeyInfo key;

            do
            {
                key = Console.ReadKey(intercept: true);

                if (key.Key == ConsoleKey.Backspace && password.Length > 0)
                {
                    password = password.Substring(0, password.Length - 1);
                    Console.Write("\b \b");
                }
                else if (!char.IsControl(key.KeyChar))
                {
                    password += key.KeyChar;
                    Console.Write("*");
                }
            } while (key.Key != ConsoleKey.Enter);

            Console.WriteLine();
            return password;
        }
    }
}