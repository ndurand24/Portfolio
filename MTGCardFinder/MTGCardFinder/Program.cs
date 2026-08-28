/************************************************************************************************************************  
 *  Program         : MTG Card Finder + Deck Builder
 *  Description     : Simple program to help MTG players search for cards and create temporary decks before purchasing
 *  Author          : Noah Durand
 *  Creation Date   : 2026-06-05
 *  Last Rev. Date  : 2026-06-10
 **********************************************************************************************************************/

using MTGCardFinder.MTGDB;
using System.Text.RegularExpressions;
using BCrypt.Net;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using MtgApiManager.Lib;
using MtgApiManager.Lib.Service;
using MtgApiManager.Lib.Core;
using MtgApiManager.Lib.Model;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

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
                options.ExpireTimeSpan = TimeSpan.FromMinutes(20);

                options.Cookie.HttpOnly = true;
                options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
                options.Cookie.SameSite = SameSiteMode.Strict;

                options.Events.OnRedirectToLogin = context =>
                {
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    return Task.CompletedTask;
                };
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
            app.MapGet("/search", async (HttpRequest request) =>
            {
                // Create MTGServideProvider()
                IMtgServiceProvider serviceProvider = new MtgServiceProvider();
                ICardService service = serviceProvider.GetCardService();
                var query = service;
                // Get json data from get request
                string rawJSON = request.Query.Keys.FirstOrDefault();

                // Ignore if null
                if (!string.IsNullOrEmpty(rawJSON))
                {
                    try
                    {
                        // Deserialize object into Dictionary
                        var searchData = JsonConvert.DeserializeObject<Dictionary<string, string>>(rawJSON);

                        // Check for Keys (query parameters from user) and add to query if present
                        if (searchData != null)
                        {
                            if (searchData.TryGetValue("cardName", out var name) && !string.IsNullOrEmpty(name))
                                query = query.Where(x => x.Name, name);
                            if (searchData.TryGetValue("cardText", out var text) && !string.IsNullOrEmpty(text))
                                query = query.Where(x => x.Text, text);
                            if (searchData.TryGetValue("cardArtist", out var artist) && !string.IsNullOrEmpty(artist))
                                query = query.Where(x => x.Artist, artist);
                            if (searchData.TryGetValue("cardId", out var id) && !string.IsNullOrEmpty(id))
                                query = query.Where(x => x.Id, id);
                            if (searchData.TryGetValue("cardType", out var type) && !string.IsNullOrEmpty(type))
                                query = query.Where(x => x.Type, type);
                            if (searchData.TryGetValue("cardSupertype", out var superType) && !string.IsNullOrEmpty(superType))
                                query = query.Where(x => x.SuperTypes, superType);
                            if (searchData.TryGetValue("cardTypes", out var types) && !string.IsNullOrEmpty(types))
                                query = query.Where(x => x.Types, types);
                            if (searchData.TryGetValue("cardSubtype", out var subtype) && !string.IsNullOrEmpty(subtype))
                                query = query.Where(x => x.SubTypes, subtype);
                            if (searchData.TryGetValue("cardCMC", out var cmc) && !string.IsNullOrEmpty(cmc))
                                query = query.Where(x => x.Cmc, cmc);
                            if (searchData.TryGetValue("cardColors", out var colors) && !string.IsNullOrEmpty(colors))
                                query = query.Where(x => x.Colors, colors);
                            if (searchData.TryGetValue("cardColorsIdentities", out var identity) && !string.IsNullOrEmpty(identity))
                                query = query.Where(x => x.ColorIdentity, identity);
                            if (searchData.TryGetValue("cardPower", out var power) && !string.IsNullOrEmpty(power))
                                query = query.Where(x => x.Power, power);
                            if (searchData.TryGetValue("cardToughness", out var toughness) && !string.IsNullOrEmpty(toughness))
                                query = query.Where(x => x.Toughness, toughness);
                            if (searchData.TryGetValue("cardLoyalty", out var loyalty) && !string.IsNullOrEmpty(loyalty))
                                query = query.Where(x => x.Loyalty, loyalty);
                            if (searchData.TryGetValue("cardNumber", out var number) && !string.IsNullOrEmpty(number))
                                query = query.Where(x => x.Number, number);
                            if (searchData.TryGetValue("cardSet", out var setCode) && !string.IsNullOrEmpty(setCode))
                                query = query.Where(x => x.Set, setCode);
                            if (searchData.TryGetValue("cardSetName", out var setName) && !string.IsNullOrEmpty(setName))
                                query = query.Where(x => x.SetName, setName);
                            if (searchData.TryGetValue("cardRarity", out var rarity) && !string.IsNullOrEmpty(rarity))
                                query = query.Where(x => x.Rarity, rarity);

                            // Note: Border, Reserved, and Release Date are not filterable directly in the standard MTG SDK query 
                            // options. If you need to filter on them, you can perform an in-memory filter later on the list.

                            if (searchData.TryGetValue("cardFormat", out var gameFormat) && !string.IsNullOrEmpty(gameFormat))
                                query = query.Where(x => x.GameFormat, gameFormat);
                            if (searchData.TryGetValue("cardLegality", out var legality) && !string.IsNullOrEmpty(legality))
                                query = query.Where(x => x.Legality, legality);
                        }
                    }
                    catch (JsonException)
                    {
                        return Results.BadRequest(new { status = "Invalid search format" });
                    }
                }

                // Send final query to MTG service
                var results = await query.AllAsync();
                if (results.IsSuccess)
                {
                    return Results.Ok(results.Value);
                }

                return Results.BadRequest(new { status = "Error in card search" });
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
