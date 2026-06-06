namespace MTGCardFinder
{
    /// <summary>
    /// Password hashing/verifying class
    /// </summary>
    public class PasswordService
    {
        /// <summary>
        /// Hash password
        /// </summary>
        /// <param name="password"></param>
        /// <returns>password hash</returns>
        public static string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.EnhancedHashPassword(password, workFactor: 12);
        }

        /// <summary>
        /// Verify password
        /// </summary>
        /// <param name="enteredPassword"></param>
        /// <param name="storedHash"></param>
        /// <returns>true if hashes match. False if not</returns>
        public static bool VerifyPassword(string enteredPassword, string storedHash)
        {
            return BCrypt.Net.BCrypt.EnhancedVerify(enteredPassword, storedHash);
        }
    }
}
