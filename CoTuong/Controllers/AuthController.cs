using Libs.DTOs;
using Libs.Entity;
using Libs.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace CoTuong.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IConfiguration _configuration;
        private TokenService _tokenService;
        private readonly TokenValidationParameters _tokenValidationParameters;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public AuthController(UserManager<IdentityUser> userManager, IConfiguration configuration, TokenService tokenService,
            TokenValidationParameters tokenValidationParameters, IHttpContextAccessor httpContextAccessor)
        {
            this._userManager = userManager;
            this._configuration = configuration;
            this._tokenService = tokenService;
            this._tokenValidationParameters = tokenValidationParameters;
            _httpContextAccessor = httpContextAccessor;
        }

        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login([FromBody] UserDTO loginRequest)
        {
            if (ModelState.IsValid)
            {
                var checkUser = await _userManager.FindByNameAsync(loginRequest.Username);
                if (checkUser == null)
                {
                    return BadRequest(new AuthResult()
                    {
                        Error = new List<string>()
                        {
                            "Username does not exist"
                        },
                        Result = false
                    });
                }
                var isCorrect = await _userManager.CheckPasswordAsync(checkUser, loginRequest.Password);
                if (!isCorrect)
                {
                    return BadRequest(new AuthResult()
                    {
                        Error = new List<string>()
                         {
                             "Password Invalid"
                         },
                        Result = false
                    });
                }
                else
                {
                    var jwtToken = await GenerateJwtToken(checkUser);
                    return Ok(jwtToken);
                }
            }
            return BadRequest(new AuthResult()
            {
                Error = new List<string>()
                {
                     "Invalid Payload"
                },
                Result = false
            });
        }

        [HttpPost]
        [Route("register")]
        public async Task<IActionResult> Register([FromBody] UserDTO registerRequest)
        {
            if (ModelState.IsValid)
            {
                var checkUserName = await _userManager.FindByNameAsync(registerRequest.Username);
                if (checkUserName != null)
                {
                    return BadRequest(new AuthResult()
                    {
                        Result = false,
                        Error = new List<string>()
                        {
                            "Username already exist"
                        }
                    });
                }
                var newUser = new IdentityUser()
                {
                    UserName = registerRequest.Username
                };
                var isCreate = await _userManager.CreateAsync(newUser, registerRequest.Password);
                if (isCreate.Succeeded)
                {
                    // Generate the token
                    var token = await GenerateJwtToken(newUser);
                    return Ok(token);
                }
                return BadRequest(new AuthResult()
                {
                    Result = false,
                    Error = new List<string>()
                    {
                        "Server Error"
                    }
                });
            }
            return BadRequest(new AuthResult()
            {
                Error = new List<string>()
                {
                     "Invalid Payload"
                },
                Result = false
            });
        }

        [HttpPost]
        [Route("refreshToken")]
        public async Task<IActionResult> RefreshToken([FromBody] TokenRequest tokenRequest)
        {
            if (ModelState.IsValid)
            {
                var result = await VerifyAndGenerateToken(tokenRequest);
                if (result == null)
                {
                    return BadRequest(new AuthResult()
                    {
                        Error = new List<string>()
                        {
                             "Invalid tokens"
                        },
                        Result = false
                    });
                }
                return Ok(result);
            }
            return BadRequest(new AuthResult()
            {
                Error = new List<string>()
                {
                     "Invalid Parameters"
                },
                Result = false
            });
        }
        
        private DateTime UnixTimeStampToDateTime(long unixTimeStamp)
        {
            var dateTimeVal = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            dateTimeVal = dateTimeVal.AddSeconds(unixTimeStamp).ToUniversalTime();
            return dateTimeVal;
        }

        [NonAction]
        public async Task<AuthResult> VerifyAndGenerateToken(TokenRequest tokenRequest)
        {
            var jwtTokenHandler = new JwtSecurityTokenHandler();
            try
            {
                _tokenValidationParameters.ValidateLifetime = false;
                var tokenInverification = jwtTokenHandler.ValidateToken(tokenRequest.Token,
                    _tokenValidationParameters, out var validatedToken);
                if (validatedToken is JwtSecurityToken jwtSecurityToken)
                {
                    var result = jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256,
                        StringComparison.InvariantCultureIgnoreCase);
                    if (result == false)
                    {
                        return null;
                    }
                }
                var utcExpiryDate = long.Parse(tokenInverification.Claims
                    .FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Exp).Value);

                var expiryDate = UnixTimeStampToDateTime(utcExpiryDate);
                var storedToken = await _tokenService.StoredToken(tokenRequest);

                var jti = tokenInverification.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Jti).Value;

                if (storedToken == null || storedToken.IsUsed || storedToken.IsRevoked || storedToken.JwtId != jti)
                {
                    return new AuthResult()
                    {
                        Error = new List<string>()
                        {
                             "Invalid token"
                        },
                        Result = false
                    };
                }
                if (expiryDate > DateTime.Now || storedToken.ExpiryDate < DateTime.Now)
                {
                    return new AuthResult()
                    {
                        Error = new List<string>()
                        {
                             "Expired token"
                        },
                        Result = false
                    };
                }
                storedToken.IsUsed = true;
                _tokenService.UpdateToken(storedToken);
                await _tokenService.SaveTokenAsync();
                var dbUser = await _userManager.FindByIdAsync(storedToken.UserId);
                return await GenerateJwtToken(dbUser);
            }
            catch (Exception ex)
            {
                return new AuthResult()
                {
                    Error = new List<string>()
                        {
                             "Server Error"
                        },
                    Result = false
                };
            }
        }

        private async Task<AuthResult> GenerateJwtToken(IdentityUser user)
        {
            var jwtTokenHanlder = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_configuration.GetSection("JwtConfig:Secret").Value);
            //Token Description
            var tokenDescription = new SecurityTokenDescriptor()
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim("Id", user.Id),
                    new Claim("Username", user.UserName),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new Claim(JwtRegisteredClaimNames.Iat, DateTime.Now.ToUniversalTime().ToString()),
                }),
                //Setting time JWT
                Expires = DateTime.Now.Add(TimeSpan.Parse(_configuration.GetSection("JwtConfig:ExpiryTimeFrame").Value)),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256)
            };
            var token = jwtTokenHanlder.CreateToken(tokenDescription);
            var jwtToken = jwtTokenHanlder.WriteToken(token);
            var refreshToken = new RefreshToken()
            {
                JwtId = token.Id,
                Token = RandomStringGeneration(23),
                AddedDate = DateTime.Now,
                ExpiryDate = DateTime.Now.AddHours(1),
                IsRevoked = false,
                IsUsed = false,
                UserId = user.Id
            };
            await _tokenService.AddTokenAsync(refreshToken);
            await _tokenService.SaveTokenAsync();
            return new AuthResult()
            {
                Token = jwtToken,
                RefreshToken = refreshToken.Token,
                Result = true
            };
        }

        private string RandomStringGeneration(int length)
        {
            var random = new Random();
            var chars = "asdxzcxzcxzczxcxzcxzcxzcxvcxvbcxvcxvcx32432432_";
            return new string(Enumerable.Repeat(chars, length).Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
}
