using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using NetCore.Data.ViewModels;
using NetCore.Services.Interfaces;
using NetCore.Services.Svcs;
using NetCore.web.Models;
using System.Security.Claims;

namespace NetCore.web.Controllers
{
    public class MembershipController : Controller
    {
        // private IUser _user = new UserService(); // 의존성 주입을 할 때는 직접 서비스를 받아서 사용하지 않음

        // 의존성 주입 - 생성자
        // 생성자의 파라미터를 통해 인터페이스를 지정하여 서비스 클래스 인스턴스를 받아옴
        // builder.Services.AddScoped<IUser, UserService>(); -> Program.cs, IUser 인터페이스에 UserService 클래스 인스턴스 주입

        private IUser _user; // IUser 인터페이스 타입의 필드 선언
        private HttpContext _context;

        public MembershipController(IHttpContextAccessor accessor, IUser user) // 생성자에서 IUser 인터페이스 타입의 매개변수를 받아서 필드에 할당
        {
            _context = accessor.HttpContext; // HttpContextAccessor를 통해 현재 HTTP 컨텍스트에 접근하여 _context 필드에 할당
            _user = user; // 의존성 주입을 통해 IUser 인터페이스 타입의 인스턴스를 받아와서 _user 필드에 할당
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        // IActionResult: 이 메서드는 View / Redirect / Json 뭐든 돌려줄 수 있음
        public IActionResult Login()
        {
            return View();
        }


        //[HttpPost("/Login")]
        [HttpPost]
        // Post 방식엔 지정해줘야 함
        [ValidateAntiForgeryToken] // 위조 방지 토큰을 통해 view로 부터 받은 Post Data가 유효한지 검증(Forgery: 위조)
        // public IActionResult Login(LoginInfo login) // LoginInfo 모델을 매개변수로 받아서 로그인 정보를 전달받음
        public async Task<IActionResult> Login(LoginInfo login) // 비동기 방식으로 로그인 처리, Task<IActionResult> 반환 타입
        {
            string message = string.Empty;

            if (ModelState.IsValid)
            {

                // DB에서 값을 가져와야 함 -> 컨트롤러가 담당하는게 아님(서비스, 리포지토리 등)
                // 서비스 계층을 두고 의존성 주입을 통해 처리하는게 좋음
                if (_user.MatchTheUserInfo(login))
                {
                    // 신원보증과 승인권한
                    var userInfo = _user.GetUserInfo(login.UserId);
                    var roles = _user.GetRolesOwnedByUser(login.UserId);
                    var userTopRole = roles.FirstOrDefault();

                    var identity = new ClaimsIdentity(
                        claims: new[]
                            {
                                new Claim(type: ClaimTypes.Name, value: userInfo.UserName),
                                new Claim(type: ClaimTypes.Role, value: userTopRole.RoleId + "|" + userTopRole.Role.RoleName + "|" + userTopRole.Role.RolePriority.ToString())
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

                    return RedirectToAction("Index", "Membership");
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
            return View("Login", login);
        }

        [HttpGet]
        public async Task<IActionResult> LogOut()
        {
            await _context.SignOutAsync(scheme: CookieAuthenticationDefaults.AuthenticationScheme);

            TempData["Message"] = "로그아웃이 성공적으로 이루어졌습니다. <br /> 웹사이트를 원활히 이용하시려면 로그인하세요.";

            return RedirectToAction("Index", "Membership");
        }
    }
}
