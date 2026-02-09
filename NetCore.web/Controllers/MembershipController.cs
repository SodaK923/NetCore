using Microsoft.AspNetCore.Mvc;
using NetCore.web.Models;

namespace NetCore.web.Controllers
{
    public class MembershipController : Controller
    {
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
                string userId = "jadejs";
                string password = "123456";

                if(login.UserId.Equals(userId) && login.Password.Equals(password))
                {
                    // TempData[Message]: 일회성 메시지 전달
                    TempData["Message"] = "로그인이 성공적으로 이루어졌습니다.";
                    // RedirectToAction(): 다른 Action으로 이동
                    // Index: Action 이름, Membership: Controller 이름
                    // Action: 사용자가 요청한 기능 단위
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
