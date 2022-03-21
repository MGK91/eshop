using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
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
    public class MicrosoftAccountController : Controller
    {
        private UserManager<ApplicationUser> userManager;
        private SignInManager<ApplicationUser> signInManager;
        private readonly IBasketService _basketService;

        public MicrosoftAccountController(UserManager<ApplicationUser> userMgr, SignInManager<ApplicationUser> signinMgr, IBasketService basketService)
        {
            userManager = userMgr;
            signInManager = signinMgr;
            _basketService = basketService;
        }

        [AllowAnonymous]
        [Route("sign-in")]
        public IActionResult SignIn()
        {
            string redirectUrl = Url.Action("AuthResponse", "MicrosoftAccount");
            //return Challenge(
            //    new AuthenticationProperties { RedirectUri = redirectUrl },
            //    OpenIdConnectDefaults.AuthenticationScheme);

            var properties = signInManager.ConfigureExternalAuthenticationProperties(OpenIdConnectDefaults.AuthenticationScheme, redirectUrl);
            return new ChallengeResult(OpenIdConnectDefaults.AuthenticationScheme, properties);

        }

        public IActionResult SignOut()
        {
            var callbackUrl = Url.Content("~/");
            return SignOut(
                new AuthenticationProperties { RedirectUri = callbackUrl },
                CookieAuthenticationDefaults.AuthenticationScheme,
                OpenIdConnectDefaults.AuthenticationScheme);
        }

        public IActionResult SignedOut()
        {
            string returnUrl = Url.Content("~/");
            if (User.Identity.IsAuthenticated)
            {
                // Redirect to home page if the user is authenticated.
                return LocalRedirect(returnUrl);
            }

            return LocalRedirect(returnUrl);
        }

        public IActionResult AccessDenied()
        {
            return View();
        }

        public async Task<IActionResult> AuthResponse(string returnUrl = null)
        {
            returnUrl = returnUrl ?? Url.Content("~/");
            ExternalLoginInfo info = await signInManager.GetExternalLoginInfoAsync();
            //Task.Delay(2000).Wait();
            if (info == null)
            {
                return RedirectToPage("./Lockout");
            }
            else
            {
                string[] userInfo = { info.Principal.Identity.Name, info.Principal.Identity.Name };
                var existinguser = await userManager.FindByNameAsync(info.Principal.Identity.Name);
                if (existinguser != null)
                {
                    await signInManager.SignInAsync(existinguser, false);
                }
                else
                {
                    ApplicationUser user = new ApplicationUser
                    {
                        Email = userInfo[0],
                        UserName = userInfo[1]
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
