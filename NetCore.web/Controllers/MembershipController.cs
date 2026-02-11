using Microsoft.AspNetCore.Mvc;
using NetCore.Data.ViewModels;
using NetCore.Services.Interfaces;
using NetCore.Services.Svcs;
using NetCore.web.Models;

namespace NetCore.web.Controllers
{
    public class MembershipController : Controller
    {
        // private IUser _user = new UserService(); // 의존성 주입을 할 때는 직접 서비스를 받아서 사용하지 않음

        // 의존성 주입 - 생성자
        // 생성자의 파라미터를 통해 인터페이스를 지정하여 서비스 클래스 인스턴스를 받아옴
        private IUser _user;

        public MembershipController(IUser user)
        {
            _user = user;
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

        [HttpPost]
        // Post 방식엔 지정해줘야 함
        [ValidateAntiForgeryToken] // 위조 방지 토큰을 통해 view로 부터 받은 Post Data가 유효한지 검증(Forgery: 위조)
        public IActionResult Login(LoginInfo login)
        {
            string message = string.Empty;

            if (ModelState.IsValid)
            {

                // DB에서 값을 가져와야 함 -> 컨트롤러가 담당하는게 아님(서비스, 리포지토리 등)
                // 서비스 계층을 두고 의존성 주입을 통해 처리하는게 좋음
                if (_user.MatchTheUserInfo(login))
                {
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
            return View();
        }
    }
}
