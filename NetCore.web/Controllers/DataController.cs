using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc;
using NetCore.Data.ViewModels;

namespace NetCore.web.Controllers
{
    public class DataController : Controller
    {
        private IDataProtector _protector;

        public DataController(IDataProtectionProvider provider)
        {
            _protector = provider.CreateProtector("NetCore.Data.v1"); // 보호자 생성, 버전 관리 위해 문자열 인자로 버전 정보 전달
        }

        [HttpGet]
        [Authorize(Roles = "GeneralUser, SuperUser, SystemUser")]
        public IActionResult AES()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "GeneralUser, SuperUser, SystemUser")]
        public IActionResult AES(AESInfo aes) // AESInfo 뷰모델을 매개변수로 받음
        {
            string message = string.Empty;
            if (ModelState.IsValid)
            {
                string userInfo = aes.UserId + aes.Password;
                aes.EncUserInfo = _protector.Protect(userInfo); // 암호화 정보
                aes.DecUserInfo = _protector.Unprotect(aes.EncUserInfo); // 복호화 정보
                ViewData["Message"] = "암복호화가 성공적으로 이루어졌습니다."; // 성공 메시지 설정
                return View(aes);
            }
            else
            {
                message = "암복호화를 위한 정보를 올바르게 입력하세요.";
            }

            // 키: string.Empty, 값: message로 모델 상태에 오류 메시지 추가
            ModelState.AddModelError(string.Empty, message);
            return View(aes);
        }
    }
}
