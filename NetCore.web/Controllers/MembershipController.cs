using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using NetCore.Data.ViewModels;
using NetCore.Services.Interfaces;
using NetCore.Services.Svcs;
using NetCore.web.Models;
using System.Security.Claims;

namespace NetCore.web.Controllers
{
    [Authorize(Roles = "AssociateUser, GeneralUser, SuperUser, SystemUser")]
    public class MembershipController : Controller
    {
        // private IUser _user = new UserService(); // 의존성 주입을 할 때는 직접 서비스를 받아서 사용하지 않음

        // 의존성 주입 - 생성자
        // 생성자의 파라미터를 통해 인터페이스를 지정하여 서비스 클래스 인스턴스를 받아옴
        // builder.Services.AddScoped<IUser, UserService>(); -> Program.cs, IUser 인터페이스에 UserService 클래스 인스턴스 주입

        private IUser _user; // IUser 인터페이스 타입의 필드 선언
        private HttpContext _context;
        private IPasswordHasher _hasher;

        public MembershipController(IHttpContextAccessor accessor, IPasswordHasher hasher, IUser user) // 생성자에서 IUser 인터페이스 타입의 매개변수를 받아서 필드에 할당
        {
            _context = accessor.HttpContext; // HttpContextAccessor를 통해 현재 HTTP 컨텍스트에 접근하여 _context 필드에 할당
            _hasher = hasher;
            _user = user; // 의존성 주입을 통해 IUser 인터페이스 타입의 인스턴스를 받아와서 _user 필드에 할당
        }

        /// <summary>
        /// 로컬 url인지 외부 url인지 체크해서 로컬 url이면 returnUrl로 리다이렉트
        /// </summary>
        /// <param name="returnUrl"></param>
        /// <returns></returns>

        private IActionResult RedirectToLocal(string? returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl)) // 로컬이면
            {
                return Redirect(returnUrl); // 원래 가려던 곳으로
            }
            else
            {
                return RedirectToAction(nameof(MembershipController.Index), "Membership"); // 로컬이 아니면 홈으로
            }
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        [AllowAnonymous] // 로그인하지 않은 사용자도 접근할 수 있도록 허용
        // IActionResult: 이 메서드는 View / Redirect / Json 뭐든 돌려줄 수 있음
        public IActionResult Login(string? returnUrl) // 
        {
            ViewData["ReturnUrl"] = returnUrl; // 로그인 후 리다이렉트할 URL을 ViewData에 저장
            return View();
        }


        //[HttpPost("/Login")]
        [HttpPost]
        // Post 방식엔 지정해줘야 함
        [ValidateAntiForgeryToken] // 위조 방지 토큰을 통해 view로 부터 받은 Post Data가 유효한지 검증(Forgery: 위조)
        [AllowAnonymous] // 로그인하지 않은 사용자도 접근할 수 있도록 허용
        // public IActionResult Login(LoginInfo login) // LoginInfo 모델을 매개변수로 받아서 로그인 정보를 전달받음
        public async Task<IActionResult> Login(LoginInfo login, string? returnUrl) // 비동기 방식으로 로그인 처리, Task<IActionResult> 반환 타입
        {
            string message = string.Empty;

            if (ModelState.IsValid)
            {

                // DB에서 값을 가져와야 함 -> 컨트롤러가 담당하는게 아님(서비스, 리포지토리 등)
                // 서비스 계층을 두고 의존성 주입을 통해 처리하는게 좋음
                //if (_user.MatchTheUserInfo(login))
                //string guidSalt = string.Empty;
                //string rngSalt = string.Empty;
                //string passwordHash = string.Empty;
                if (_hasher.MatchTheUserInfo(login.UserId, login.Password))
                {
                    // 신원보증과 승인권한
                    var userInfo = _user.GetUserInfo(login.UserId);
                    var roles = _user.GetRolesOwnedByUser(login.UserId);
                    var userTopRole = roles.FirstOrDefault();
                    string userDataInfo = userTopRole.Role.RoleName + "|" + 
                                            userTopRole.Role.RolePriority.ToString() + "|" +
                                            userInfo.UserName + "|" +
                                            userInfo.UserEmail;

                    // _context.User.Identity.Name => 사용자 아이디

                    var identity = new ClaimsIdentity(
                        claims: new[]
                            {
                                new Claim(type: ClaimTypes.Name, value: userInfo.UserId),
                                new Claim(type: ClaimTypes.Role, value: userTopRole.RoleId),
                                new Claim(type: ClaimTypes.UserData, value: userDataInfo)
                            },
                         authenticationType: CookieAuthenticationDefaults.AuthenticationScheme);

                    await _context.SignInAsync(scheme: CookieAuthenticationDefaults.AuthenticationScheme,
                                                principal: new ClaimsPrincipal(identity: identity),
                                                properties: new AuthenticationProperties()
                                                {
                                                    IsPersistent = login.RememberMe,
                                                    ExpiresUtc = login.RememberMe ? DateTime.UtcNow.AddDays(7) : DateTime.UtcNow.AddMinutes(30)
                                                });


                    TempData["Message"] = "로그인이 성공적으로 이루어졌습니다.";



                    // return RedirectToAction("Index", "Membership");
                    return RedirectToLocal(returnUrl);
                }
                else
                {
                    message = "로그인되지 않았습니다.";
                }

            }
            else
            {
                message = "로그인 정보를 올바르게 입력하세요.";
            }
            ModelState.AddModelError(string.Empty, message);
            return View("Login", login); // 로그인 실패 시 로그인 페이지로 다시 이동, 입력한 로그인 정보를 유지하기 위해 login 모델을 전달
        }

        [HttpGet]
        public async Task<IActionResult> LogOut()
        {
            await _context.SignOutAsync(scheme: CookieAuthenticationDefaults.AuthenticationScheme);

            TempData["Message"] = "로그아웃이 성공적으로 이루어졌습니다. <br /> 웹사이트를 원활히 이용하시려면 로그인하세요.";

            return RedirectToAction("Index", "Membership");
        }

        [HttpGet]
        public IActionResult Forbidden()
        {
            StringValues paramReturnUrl;
            bool exists = _context.Request.Query.TryGetValue("returnUrl", out paramReturnUrl); // 쿼리 스트링에서 returnUrl 파라미터를 가져옴
            paramReturnUrl = exists ? _context.Request.Host.Value + paramReturnUrl[0] : string.Empty; // returnUrl이 존재하면 첫 번째 값을 사용하고, 존재하지 않으면 빈 문자열로 설정

            ViewData["Message"] = $"귀하는 {paramReturnUrl} 경로로 접근하려고 했습니다만, <br/>" +
                "인증된 사용자도 접근하지 못하는 페이지가 있습니다. <br/>" +
                "담당자에게 해당 페이지의 접근 권한에 대해 문의하세요.";

            return View();
        }
            
    }
}
