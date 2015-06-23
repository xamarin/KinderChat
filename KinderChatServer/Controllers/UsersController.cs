using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Migrations;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Helpers;
using System.Web.Hosting;
using System.Web.Http;
using CryptSharp;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OAuth;
using KinderChatServer.Core.EmailNotifications;
using KinderChatServer.DAL;
using KinderChatServer.Models;
using KinderChatServer.Providers;
using KinderChatServer.Results;
using KinderChatServer.Tools;
using Twilio;
using Encoder = System.Text.Encoder;

namespace KinderChatServer.Controllers
{
    public class UsersController : ApiController
    {
        private readonly ApplicationDbContext _db = new ApplicationDbContext();
        private const string AccountSid = "ACbfaf4f4fdd9ed0e2480495491daef678";
        private const string AuthToken = "d1802b9c1efbb080cba8e58dbd75175b";
        private const string FromNumber = "+14155992671";
        private ApplicationUserManager _userManager;
        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? Request.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }
        #region Create User

        [HttpPost]
        public IHttpActionResult CreateUser(string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                return BadRequest("Email not present");
            }

            // If user exists, add them to the database. Else update their codes.
            var user = _db.Users.FirstOrDefault(node => node.Email == email);
            var userExists = user != null;
            if (!userExists)
            {
                user = new User
                {
                    Email = email,
                    NickName = email,
                    Joined = DateTime.UtcNow
                };
            }

            user.ConfirmKey = GetUniqueKeyNumber(6);
            user.ConfirmTimestamp = DateTime.UtcNow.AddMinutes(15);
            user.AvatarId = 1;
            _db.Users.AddOrUpdate(user);
            _db.SaveChanges();
            var message = new UserAccountConfirmation(user, ControllerContext);
            message.Send();

            return Ok();
        }

        public IHttpActionResult CreateUserPhone(string phone)
        {
            if (string.IsNullOrEmpty(phone))
            {
                return BadRequest("Phone not present");
            }

            // If user exists, add them to the database. Else update their codes.
            var user = _db.Users.FirstOrDefault(node => node.Sms == phone);
            var userExists = user != null;
            if (!userExists)
            {
                user = new User
                {
                    Sms = phone,
                    NickName = phone,
                    Joined = DateTime.UtcNow
                };
            }

            user.ConfirmKey = GetUniqueKeyNumber(6);
            user.ConfirmTimestamp = DateTime.UtcNow.AddMinutes(15);
            user.AvatarId = 1;
            _db.Users.AddOrUpdate(user);
            _db.SaveChanges();

            // Create an instance of the Twilio client.
            var client = new TwilioRestClient(AccountSid, AuthToken);

            // Send an SMS message.
            Twilio.Message result = client.SendMessage(
                FromNumber, phone, "Your confirmation number is " + user.ConfirmKey);

            if (result.RestException != null)
            {
                //an exception occurred making the REST call
                string message = result.RestException.Message;
            }
            return Ok();
        }

        [HttpPost]
        [Authorize]
        public IHttpActionResult ChangeNickname(string nickname)
        {
            if (string.IsNullOrEmpty(nickname))
            {
                return BadRequest("Nickname is not present");
            }
            var userdevice = UserManager.FindByName(User.Identity.Name);
            var user = _db.Users.First(node => node.Id == userdevice.UserId);
            user.NickName = nickname;
            _db.Users.AddOrUpdate(user);
            _db.SaveChanges();
            return Ok();
        }

        [HttpPost]
        [Authorize]
        public IHttpActionResult AddAvatar(int id)
        {
            var avatar = _db.Avatars.First(node => node.Id == id);
            var userdevice = UserManager.FindByName(User.Identity.Name);
            var user = _db.Users.First(node => node.Id == userdevice.UserId);
            user.Avatar = avatar;
            user.AvatarId = avatar.Id;
            _db.SaveChanges();
            return Ok(avatar);
        }

        private ImageCodecInfo GetEncoder(ImageFormat format)
        {

            ImageCodecInfo[] codecs = ImageCodecInfo.GetImageDecoders();

            return codecs.FirstOrDefault(codec => codec.FormatID == format.Guid);
        }

        [HttpPost]
        [Authorize]
        public Task<Avatar> AddCustomAvatar()
        {
            var userdevice = UserManager.FindByName(User.Identity.Name);
            var user = _db.Users.First(node => node.Id == userdevice.UserId);

            if (!Request.Content.IsMimeMultipartContent())
                throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotAcceptable,
                    "This request is not properly formatted"));
            /*
            Originally, this method was to set a custom avatar for a user if they didn't have one
            and if they did, to overwrite it. However the client writers wanted to be able to reset their avatars to the last
            one the users had without resending it up. I am to please, so now it just always overwrites it. 
            */
            var fileName = Guid.NewGuid() + ".jpg";
            var test = Request.Content.ReadAsMultipartAsync(new MultipartMemoryStreamProvider()).ContinueWith((task) =>
            {
                var provider = task.Result;
                var content = provider.Contents.First();
                using (var stream = content.ReadAsStreamAsync().Result)
                {
                    using (var image = Image.FromStream(stream))
                    {
                        var qualityParam = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, 60L);
                        var encoderParams = new EncoderParameters(1) { Param = {[0] = qualityParam } };
                        var jgpEncoder = GetEncoder(ImageFormat.Jpeg);
                        var filePath = HostingEnvironment.MapPath("~/Images/Avatars/");
                        if (filePath != null)
                        {
                            var fullPath = Path.Combine(filePath, fileName);
                            image.Save(fullPath, jgpEncoder, encoderParams);
                        }
                        Avatar avatar;
                        if (user.Avatar == null || user.Avatar.Type != AvatarType.User)
                        {
                            avatar = new Avatar()
                            {
                                Location = "/Images/Avatars/" + fileName,
                                Type = AvatarType.User
                            };
                        }
                        else
                        {
                            avatar = user.Avatar;
                            avatar.Location = "/Images/Avatars/" + fileName;
                            avatar.Type = AvatarType.User;
                        }
                        _db.Avatars.AddOrUpdate(avatar);
                        user.AvatarId = avatar.Id;
                        _db.SaveChanges();
                        return avatar;
                    }
                }
            });
            return test;
        }

        [HttpPost]
        [Authorize]
        public IHttpActionResult SendLinkUserPhone(string phone)
        {
            var userdevice = UserManager.FindByName(User.Identity.Name);
            var user = _db.Users.First(node => node.Id == userdevice.UserId);
            if (!string.IsNullOrEmpty(user.Sms))
            {
                return BadRequest("User has phone number linked already.");
            }

            user.ConfirmKey = GetUniqueKeyNumber(6);
            user.ConfirmTimestamp = DateTime.UtcNow.AddMinutes(15);

            // Create an instance of the Twilio client.
            var client = new TwilioRestClient(AccountSid, AuthToken);

            // Send an SMS message.
            Twilio.Message result = client.SendMessage(
               FromNumber, phone, "Your confirmation number is " + user.ConfirmKey);

            if (result.RestException != null)
            {
                //an exception occurred making the REST call
                string message = result.RestException.Message;
            }

            _db.Users.AddOrUpdate(user);
            _db.SaveChanges();
            return Ok();
        }

        [HttpPost]
        [Authorize]
        public IHttpActionResult SendLinkUserEmail(string email)
        {
            var userdevice = UserManager.FindByName(User.Identity.Name);
            var user = _db.Users.First(node => node.Id == userdevice.UserId);
            if (!string.IsNullOrEmpty(user.Email))
            {
                return BadRequest("User has email address linked already.");
            }

            user.ConfirmKey = GetUniqueKey(6);
            user.ConfirmTimestamp = DateTime.UtcNow.AddMinutes(15);

            var message = new LinkUserViaEmailConfirmation(user, email, ControllerContext);
            message.Send();

            _db.Users.AddOrUpdate(user);
            _db.SaveChanges();
            return Ok();
        }

        [Authorize]
        [HttpGet]
        public IHttpActionResult SendUserInvite(string email)
        {
            var userdevice = UserManager.FindByName(User.Identity.Name);
            var user = _db.Users.First(node => node.Id == userdevice.UserId);
            var message = new UserInvite(user, email, ControllerContext);
            message.Send();
            return Ok();
        }

        [HttpPost]
        [Authorize]
        public IHttpActionResult LinkUserPhone(string phone, string confirmKey)
        {
            var userdevice = UserManager.FindByName(User.Identity.Name);
            var user = _db.Users.First(node => node.Id == userdevice.UserId);
            if (user.ConfirmKey == null || !user.ConfirmKey.Equals(confirmKey))
            {
                return BadRequest("Confirm Key not present");
            }
            if (user.ConfirmTimestamp < DateTime.UtcNow)
            {
                return BadRequest("Confirm key timeout");
            }

            user.Sms = phone;
            user.ConfirmKey = null;
            _db.Users.AddOrUpdate(user);
            _db.SaveChanges();
            return Ok();
        }



        [HttpPost]
        [Authorize]
        public IHttpActionResult LinkUserEmail(string email, string confirmKey)
        {
            var userdevice = UserManager.FindByName(User.Identity.Name);
            var user = _db.Users.First(node => node.Id == userdevice.UserId);
            if (user.ConfirmKey == null || !user.ConfirmKey.Equals(confirmKey))
            {
                return BadRequest("Confirm Key not present");
            }
            if (user.ConfirmTimestamp < DateTime.UtcNow)
            {
                return BadRequest("Confirm key timeout");
            }

            user.Email = email;
            user.ConfirmKey = null;
            _db.Users.AddOrUpdate(user);
            _db.SaveChanges();
            return Ok();
        }

        [HttpPost]
        public async Task<IHttpActionResult> CreateUserDevice(string email, string confirmKey, string publicKey, string nickname)
        {
            var user = _db.Users.FirstOrDefault(node => node.Email == email);
            if (user == null)
            {
                return BadRequest("User does not exist");
            }

            if (user.ConfirmKey == null || !user.ConfirmKey.Equals(confirmKey))
            {
                return BadRequest("Confirm Key not present");
            }

            if (string.IsNullOrEmpty(publicKey))
            {
                return BadRequest("Public Key not present");
            }

            if (_db.UserDevices.Any(node => node.PublicKey == publicKey))
            {
                return BadRequest("Public Key already exists on user device");
            }

            if (user.ConfirmTimestamp < DateTime.UtcNow)
            {
                return BadRequest("Confirm key timeout");
            }

            var accessToken = GetUniqueKey(64);
            var username = GetUniqueKey(64);

            var userDevice = new UserDevice()
            {
                UserName = username,
                Email = user.Email,
                PublicKey = publicKey,
                UserId = user.Id,
                EmailConfirmed = true
            };

            // Create the "Users Device" as an Identity Object.
            // Since this is what they log in with, it's the most logical/hacky
            // way I can think of handling it easily with the tools we have.
            // Without totally reinventing the wheel.
            var result = await UserManager.CreateAsync(userDevice, accessToken);

            if (!result.Succeeded)
            {
                return BadRequest("Failed to create user device");
            }
            if (!string.IsNullOrEmpty(nickname))
            {
                user.NickName = nickname;
            }
            user.ConfirmKey = null;
            _db.Users.AddOrUpdate(user);
            _db.SaveChanges();
            return Ok(new { AccessToken = accessToken, Username = username, DeviceId = userDevice.Id});
        }
        #endregion

        #region Login
        [OverrideAuthentication]
        [HostAuthentication(DefaultAuthenticationTypes.ExternalCookie)]
        [AllowAnonymous]
        [Route("ExternalLogin", Name = "ExternalLogin")]
        public async Task<IHttpActionResult> GetExternalLogin(string provider, string error = null)
        {
            if (error != null)
            {
                return Redirect(Url.Content("~/") + "#error=" + Uri.EscapeDataString(error));
            }

            if (!User.Identity.IsAuthenticated)
            {
                return new ChallengeResult(provider, this);
            }

            var externalLogin = ExternalLoginData.FromIdentity(User.Identity as ClaimsIdentity);

            if (externalLogin == null)
            {
                return InternalServerError();
            }

            if (externalLogin.LoginProvider != provider)
            {
                Authentication.SignOut(DefaultAuthenticationTypes.ExternalCookie);
                return new ChallengeResult(provider, this);
            }

            var user = await UserManager.FindAsync(new UserLoginInfo(externalLogin.LoginProvider,
                externalLogin.ProviderKey));

            var hasRegistered = user != null;

            if (hasRegistered)
            {
                Authentication.SignOut(DefaultAuthenticationTypes.ExternalCookie);

                var oAuthIdentity = await user.GenerateUserIdentityAsync(UserManager,
                   OAuthDefaults.AuthenticationType);
                var cookieIdentity = await user.GenerateUserIdentityAsync(UserManager,
                    CookieAuthenticationDefaults.AuthenticationType);

                var properties = ApplicationOAuthProvider.CreateProperties(user.UserName);
                Authentication.SignIn(properties, oAuthIdentity, cookieIdentity);
            }
            else
            {
                IEnumerable<Claim> claims = externalLogin.GetClaims();
                var identity = new ClaimsIdentity(claims, OAuthDefaults.AuthenticationType);
                Authentication.SignIn(identity);
            }

            return Ok();
        }

        private class ExternalLoginData
        {
            public string LoginProvider { get; set; }
            public string ProviderKey { get; set; }
            public string UserName { get; set; }

            public IList<Claim> GetClaims()
            {
                IList<Claim> claims = new List<Claim>();
                claims.Add(new Claim(ClaimTypes.NameIdentifier, ProviderKey, null, LoginProvider));

                if (UserName != null)
                {
                    claims.Add(new Claim(ClaimTypes.Name, UserName, null, LoginProvider));
                }

                return claims;
            }

            public static ExternalLoginData FromIdentity(ClaimsIdentity identity)
            {
                var providerKeyClaim = identity?.FindFirst(ClaimTypes.NameIdentifier);

                if (string.IsNullOrEmpty(providerKeyClaim?.Issuer) || string.IsNullOrEmpty(providerKeyClaim.Value))
                {
                    return null;
                }

                if (providerKeyClaim.Issuer == ClaimsIdentity.DefaultIssuer)
                {
                    return null;
                }

                return new ExternalLoginData
                {
                    LoginProvider = providerKeyClaim.Issuer,
                    ProviderKey = providerKeyClaim.Value,
                    UserName = identity.FindFirstValue(ClaimTypes.Name)
                };
            }
        }
        #endregion

        #region Authorized Methods

        [Authorize]
        [HttpPost]
        public async Task<IHttpActionResult> FlagUser(int userId, int flagId)
        {
            var userdevice = UserManager.FindByName(User.Identity.Name);
            var user = _db.Users.First(node => node.Id == userdevice.UserId);
            var flag = _db.Flags.First(node => node.Id == flagId);
            var accusedUser = _db.Users.First(node => node.Id == userId);

            var userFlag = new UserFlag()
            {
                AccusedUserId = accusedUser.Id,
                AccuserUserId = user.Id,
                FlagId = flag.Id,
                Resolved = false
            };

            _db.UserFlags.AddOrUpdate(userFlag);
            await _db.SaveChangesAsync();
            return Ok(userFlag);
        }

        [Authorize]
        public IHttpActionResult VerifyUserExists(string email, string phone)
        {
            bool exists;
            if (!string.IsNullOrEmpty(email))
            {
                exists = _db.Users.Any(node => node.Email == email);

                if (exists)
                {
                    return Ok();
                }
                return NotFound();
            }

            if (string.IsNullOrEmpty(phone))
                return BadRequest("No parameters given");

            exists = _db.Users.Any(node => node.Sms == phone);

            if (exists)
            {
                return Ok();
            }
            return NotFound();
        }

        [Authorize]
        public IHttpActionResult VerifyUser()
        {
            if (User.Identity.IsAuthenticated)
            {
                return Ok();
            }

            return BadRequest("Not Authenticated");
        }

        [Authorize]
        public IHttpActionResult GetUser(int id)
        {
            var user = _db.Users.FirstOrDefault(node => node.Id == id);

            if (user == null)
            {
                return BadRequest("User does not exist");
            }
            return Ok(CreateReturnUserObject(user));
        }

        private IAuthenticationManager Authentication => Request.GetOwinContext().Authentication;

        [Authorize]
        public IHttpActionResult GetUser(string email, string phone)
        {
            User user;

            if (!string.IsNullOrEmpty(email))
            {
                user = GetUserViaEmail(email);
            }
            else if (!string.IsNullOrEmpty(phone))
            {
                user = GetUserViaPhone(email);
            }
            else
            {
                return BadRequest("No Information Given");
            }

            if (user == null)
            {
                return BadRequest("User does not exist");
            }

            return Ok(CreateReturnUserObject(user));
        }

        private User GetUserViaEmail(string email)
        {
            var user = _db.Users.FirstOrDefault(node => node.Email == email);
            return user;
        }

        private User GetUserViaPhone(string phone)
        {
            var user = _db.Users.FirstOrDefault(node => node.Sms == phone);
            return user;
        }

        private JsonUser CreateReturnUserObject(User user)
        {

            var deviceList = new List<JsonUserDevice>();

            if (user.Devices.Any())
            {
                deviceList = user.Devices.Select(device => new JsonUserDevice
                {
                    DeviceId = device.Id,
                    DeviceLoginId = device.UserName,
                    DeviceEmail = device.Email,
                    DevicePhoneNumber = device.PhoneNumber,
                    PublicKey = device.PublicKey
                }).ToList();
            }

            var achievements = new List<JsonAchievement>();

            if (user.Achievements.Any())
            {
                achievements = user.Achievements.Select(device => new JsonAchievement()
                {
                    Title = device.Achievement.Title,
                    Description = device.Achievement.Description,
                    Timestamp = device.Timestamp,
                    BadgeLocation = device.Achievement.BadgeLocation
                }).ToList();
            }

            return new JsonUser
            {
                Avatar = user.Avatar,
                Achievements = achievements,
                Devices = deviceList,
                Email = user.Email,
                Id = user.Id,
                ConfirmKey = user.ConfirmKey,
                KinderPoints = user.KinderPoints,
                KinderStatus = user.KinderStatus,
                Sms = user.Sms,
                NickName = user.NickName
            };
        }
        #endregion

        #region Helpers
        private static string GetUniqueKey(int maxSize)
        {
            var chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890".ToCharArray();
            var data = new byte[1];
            var crypto = new RNGCryptoServiceProvider();
            crypto.GetNonZeroBytes(data);
            data = new byte[maxSize];
            crypto.GetNonZeroBytes(data);
            var result = new StringBuilder(maxSize);
            foreach (var b in data)
            {
                result.Append(chars[b % (chars.Length)]);
            }
            return result.ToString();
        }

        private static string GetUniqueKeyNumber(int maxSize)
        {
            var chars = "1234567890".ToCharArray();
            var data = new byte[1];
            var crypto = new RNGCryptoServiceProvider();
            crypto.GetNonZeroBytes(data);
            data = new byte[maxSize];
            crypto.GetNonZeroBytes(data);
            var result = new StringBuilder(maxSize);
            foreach (var b in data)
            {
                result.Append(chars[b % (chars.Length)]);
            }
            return result.ToString();
        }

        #endregion
    }
}
