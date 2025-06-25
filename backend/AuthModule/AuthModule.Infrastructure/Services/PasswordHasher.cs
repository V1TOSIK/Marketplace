using AuthModule.Application.Interfaces;

namespace AuthModule.Infrastructure.Services
{
    public class PasswordHasher : IPasswordHasher
    {
        public string HashPassword(string password)
        {
            if (string.IsNullOrWhiteSpace(password))
                throw new ArgumentException("Password cannot be null or empty.", nameof(password));

            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(password);

            return hashedPassword;
        }

        public bool VerifyHashedPassword(string hashedPassword, string password)
        {
            if (string.IsNullOrWhiteSpace(hashedPassword))
                throw new ArgumentException("Hashed password cannot be null or empty.", nameof(hashedPassword));

            if (string.IsNullOrWhiteSpace(password))
                throw new ArgumentException("Password cannot be null or empty.", nameof(password));

            var isVerified = BCrypt.Net.BCrypt.Verify(password, hashedPassword);

            return isVerified;
        }
    }
}
