using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.eShopWeb.ApplicationCore.Interfaces;
using Microsoft.eShopWeb.Infrastructure.Identity;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Microsoft.eShopWeb.Web.Controllers
{
    [AllowAnonymous, Route("account")]
    public class AccountController : Controller
    {
        private UserManager<ApplicationUser> userManager;
        private SignInManager<ApplicationUser> signInManager;
        private readonly IBasketService _basketService;

        public AccountController(UserManager<ApplicationUser> userMgr, SignInManager<ApplicationUser> signinMgr, IBasketService basketService)
        {
            userManager = userMgr;
            signInManager = signinMgr;
            _basketService = basketService;
        }

        // other methods
        [Route("google-accessdenied")]
        public IActionResult AccessDenied()
        {
            return View();
        }

        [AllowAnonymous]
        [Route("google-login")]
        public IActionResult GoogleLogin()
        {
            string redirectUrl = Url.Action("GoogleResponse", "Account");
            var properties = signInManager.ConfigureExternalAuthenticationProperties("Google", redirectUrl);
            return new ChallengeResult("Google", properties);
        }

        [AllowAnonymous]
        [Route("google-response")]
        public async Task<IActionResult> GoogleResponse(string returnUrl = null)
        {
            returnUrl = returnUrl ?? Url.Content("~/");

            ExternalLoginInfo info = await signInManager.GetExternalLoginInfoAsync();

            //Task.Delay(2000).Wait();

            if (info == null)
            {
                return RedirectToPage("./Lockout");
            }

            var result = await signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, true, true);

           // Task.Delay(2000).Wait();

            string[] userInfo = { info.Principal.FindFirst(ClaimTypes.Name).Value, info.Principal.FindFirst(ClaimTypes.Email).Value };

            if (result.Succeeded)
            {
                await TransferAnonymousBasketToUserAsync(userInfo[1]);
                return LocalRedirect(returnUrl);
            }
            else
            {
                ApplicationUser user = new ApplicationUser
                {
                    Email = info.Principal.FindFirst(ClaimTypes.Email).Value,
                    UserName = info.Principal.FindFirst(ClaimTypes.Email).Value
                };

                IdentityResult identResult = await userManager.CreateAsync(user);
                if (identResult.Succeeded)
                {
                    identResult = await userManager.AddLoginAsync(user, info);
                    if (identResult.Succeeded)
                    {
                        await signInManager.SignInAsync(user, false);
                    }
                }
                return LocalRedirect(returnUrl);
            }
        }
        private async Task TransferAnonymousBasketToUserAsync(string userName)
        {
            if (Request.Cookies.ContainsKey(Constants.BASKET_COOKIENAME))
            {
                var anonymousId = Request.Cookies[Constants.BASKET_COOKIENAME];
                await _basketService.TransferBasketAsync(anonymousId, userName);
                Response.Cookies.Delete(Constants.BASKET_COOKIENAME);
            }
        }
    }


}
