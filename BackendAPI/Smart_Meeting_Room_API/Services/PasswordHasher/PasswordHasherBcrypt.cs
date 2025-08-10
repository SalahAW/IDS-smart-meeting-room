using BCrypt.Net;

namespace Smart_Meeting_Room_API.Services.PasswordHasher
{
    public interface IPasswordHasher
    {
        string HashPassword(string plainTextPassword);
        bool ComparePassword(string plainTextPassword , string HashedPassword);
    }

    public class PasswordHasherBcrypt : IPasswordHasher
    {
        private readonly int _workFactor;

        public PasswordHasherBcrypt(int workFactor = 10) // workfactor of BCrypt Hashing (about ~100ms delay)
        {
            _workFactor = workFactor;
        }
        public string HashPassword(string plainTextPassword)
        {

            if(string.IsNullOrWhiteSpace(plainTextPassword))
                throw new ArgumentException("Password cant be empty" 
                    , nameof(plainTextPassword));


            return BCrypt.Net.BCrypt.HashPassword(plainTextPassword, _workFactor); // hashes plain text password using BCrypt algorithm at 10 workfactor

        }

        public bool ComparePassword(string plainTextpassword , string hashedPassword)
        {
            if (string.IsNullOrWhiteSpace(plainTextpassword) || string.IsNullOrEmpty(hashedPassword))
                return false;

            return BCrypt.Net.BCrypt.Verify(plainTextpassword , hashedPassword);
        }
    }
}
