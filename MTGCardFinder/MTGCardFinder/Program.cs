/************************************************************************************************************************  
 *  Program         : MTG Card Finder + Deck Builder
 *  Description     : Simple program to help MTG players search for cards and create temporary decks before purchasing
 *  Author          : Noah Durand
 *  Creation Date   : 2026-06-05
 *  Last Rev. Date  : 2026-06-05
 **********************************************************************************************************************/

using MTGCardFinder.MTGDB;
using System.Text.RegularExpressions;
using BCrypt.Net;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using MtgApiManager.Lib;
using MtgApiManager.Lib.Service;

namespace MTGCardFinder
{
    public class Program
    {
        /// <summary>
        /// Record class for user login + registration
        /// </summary>
        /// <param name="email"></param>
        /// <param name="username"></param>
        /// <param name="password"></param>
        record class UserData(string email, string username, string password); 

        public static void Main(string[] args)
        {
            // CORS policies and page build
            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddControllers();

            // Create named CORS policy
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowLiveServer", policy =>
                {
                    policy.WithOrigins("http://127.0.0.1:5500", "http://localhost:5500")
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials();
                });
            });

            // Direct authentication to login endpoint, set time limit to 20min
            builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(options =>
            {
                options.LoginPath = "/login";
                options.ExpireTimeSpan = TimeSpan.FromMinutes(20);
            });

            var app = builder.Build();
            app.UseRouting();

            // Use named CORS policy
            app.UseCors("AllowLiveServer");

            app.UseAuthentication();
            app.UseAuthorization();

            // Temp page init message
            app.MapGet("/", () => "Server inititialization complete.");

            // GET endpoint to check if username is available
            app.MapGet("/checkuser", (string username) =>
            {
                var db = new MtgDbContext();
                try
                {
                    // Clean user input
                    string uName = CleanInput(username);

                    // Verify username is not null or blank
                    if (string.IsNullOrWhiteSpace(uName))
                        return Results.BadRequest(new { status = "Username cannot be empty" });

                    // Check if username exists
                    var user = db.Users.FirstOrDefault(u => u.Username == uName);
                    if (user != null)
                        return Results.Conflict(new { status = "Username taken" });

                    // If previous checks pass, return OK
                    return Results.Ok("Username available");
                }
                catch (Exception ex)
                {
                    // Server error message
                    Console.WriteLine($"GET Error: {ex.Message}");

                    // Return status error message to client
                    return Results.Json(
                        new { status = $"Error communicating with database. Please contact administrator." },
                            statusCode: 500);
                }

            });

            // Card search GET request
            app.MapGet("/search", async () =>
            {
                IMtgServiceProvider serviceProvider = new MtgServiceProvider();
            });

            // User login endpoint
            app.MapPost("/login", async (UserData ud, HttpContext httpContext) =>
            {
                var db = new MtgDbContext();
                try
                {
                    // Clean Inputs before using
                    string username = CleanInput(ud.username);
                    string password = CleanInput(ud.password);

                    // Verify user exists
                    var user = db.Users.FirstOrDefault(user => user.Username == username);
                    if (user == null)
                        return Results.Unauthorized();

                    // Verify password
                    bool correctPass = PasswordService.VerifyPassword(password, user.Password);
                    if (!correctPass)
                        return Results.Unauthorized();

                    // Setup up Identity session claims
                    var claims = new List<Claim> { new Claim(ClaimTypes.Name, user.Username) };
                    var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                    // Issue cookie to browser storage
                    await httpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identity));

                    // Return 200 OK for success
                    return Results.Ok(new { status = "Success" });
                }
                catch (Exception ex)
                {
                    // Server Error Message
                    Console.WriteLine($"Login Exception: {ex.Message}");

                    // Return error message as JSON object to client
                    return Results.Json(
                        new { status = "Login error. Please contact administrator" },
                        statusCode: 500);
                }
            });

            // User registration endpoint
            app.MapPost("/register", (UserData ud) =>
            {
                var db = new MtgDbContext();
                try
                {
                    string email = CleanInput(ud.email); // client email
                    string username = CleanInput(ud.username); // client username
                    string password = CleanInput(ud.password); // client password

                    // Ensure all required data is present
                    if (string.IsNullOrWhiteSpace(email))
                        return Results.BadRequest(new { status = "Must enter an Email." });
                    if (string.IsNullOrWhiteSpace(username))
                        return Results.BadRequest(new { status = "Must enter a username" });
                    if (string.IsNullOrEmpty(password))
                        return Results.BadRequest(new { status = "Must enter a password" });

                    if (!(db.Users.Any(user => user.Username == username))) // Verify user doesn't already exist
                    {
                        // Create new user and save db changes
                        User newUser = new User { Username = username, Password = PasswordService.HashPassword(password), Email = email};
                        db.Users.Add(newUser);
                        db.SaveChanges();

                        return Results.Ok(new { status = "Registration Successful!" });
                    }
                    else
                        return Results.Conflict(new { status = "User already exists" });
                }
                catch (Exception ex) // If db error, let user know to contact admin
                {
                    // Server error message
                    Console.WriteLine($"Registration Exception: {ex.Message}");

                    // Client error message returned as json
                    return Results.Json(
                        new { status = "Registration error. Please contact administrator" },
                        statusCode: 500);
                }
            });

            // Logout and clear cookies
            app.MapPost("/logout", async (HttpContext httpContext) =>
            {
                var db = new MtgDbContext();

                // Clear auth claims principle & wipe cookie
                await httpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

                // Return 200 OK for success
                return Results.Ok(new { status = "Logged out successfully" });
            });

            app.Run();
        }

        /// <summary>
        /// Used to clean user inputs to security
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string CleanInput(string input)
        {
            return Regex.Replace(input.Trim(), "<.*?|&.*?;", string.Empty);
        }
    }
}
