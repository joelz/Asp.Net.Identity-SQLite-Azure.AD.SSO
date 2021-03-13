using System;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.Google;
using Microsoft.Owin.Security.Notifications;
using Microsoft.Owin.Security.OpenIdConnect;
using Owin;
using Sample.Models;

namespace Sample
{
    public partial class Startup
    {
        // For more information on configuring authentication, please visit https://go.microsoft.com/fwlink/?LinkId=301864
        public void ConfigureAuth(IAppBuilder app)
        {
            // Configure the db context, user manager and signin manager to use a single instance per request
            app.CreatePerOwinContext(ApplicationDbContext.Create);
            app.CreatePerOwinContext<ApplicationUserManager>(ApplicationUserManager.Create);
			app.CreatePerOwinContext<ApplicationRoleManager>(ApplicationRoleManager.Create);
			app.CreatePerOwinContext<ApplicationSignInManager>(ApplicationSignInManager.Create);

            // Enable the application to use a cookie to store information for the signed in user
            // and to use a cookie to temporarily store information about a user logging in with a third party login provider
            // Configure the sign in cookie
            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
                LoginPath = new PathString("/Account/Login"),
                Provider = new CookieAuthenticationProvider
                {
                    // Enables the application to validate the security stamp when the user logs in.
                    // This is a security feature which is used when you change a password or add an external login to your account.  
                    OnValidateIdentity = SecurityStampValidator.OnValidateIdentity<ApplicationUserManager, ApplicationUser>(
                        validateInterval: TimeSpan.FromMinutes(30),
                        regenerateIdentity: (manager, user) => user.GenerateUserIdentityAsync(manager))
                }
            });            
            app.UseExternalSignInCookie(DefaultAuthenticationTypes.ExternalCookie);

            // Enables the application to temporarily store user information when they are verifying the second factor in the two-factor authentication process.
            app.UseTwoFactorSignInCookie(DefaultAuthenticationTypes.TwoFactorCookie, TimeSpan.FromMinutes(5));

            // Enables the application to remember the second login verification factor such as phone or email.
            // Once you check this option, your second step of verification during the login process will be remembered on the device where you logged in from.
            // This is similar to the RememberMe option when you log in.
            app.UseTwoFactorRememberBrowserCookie(DefaultAuthenticationTypes.TwoFactorRememberBrowserCookie);

            // Uncomment the following lines to enable logging in with third party login providers
            //app.UseMicrosoftAccountAuthentication(new Microsoft.Owin.Security.MicrosoftAccount.MicrosoftAccountAuthenticationOptions()
            //{
            //    ClientId = "76d4df1a-da09-482e-a9fb-c531c3ee79d8",
            //    ClientSecret = "",
            //    // Scope = { "openid", "email" }, // scope 不能加！！
            //});

            //app.UseTwitterAuthentication(
            //   consumerKey: "",
            //   consumerSecret: "");

            //app.UseFacebookAuthentication(
            //   appId: "",
            //   appSecret: "");

            //app.UseGoogleAuthentication(new GoogleOAuth2AuthenticationOptions()
            //{
            //    ClientId = "",
            //    ClientSecret = ""
            //});

            ConfigurationOpenIdConnect(app);
        }


        /// <summary>
        /// Configure OWIN to use OpenIdConnect 
        /// </summary>
        /// <param name="app"></param>
        public void ConfigurationOpenIdConnect(IAppBuilder app)
        {
            // The Client ID is used by the application to uniquely identify itself to Azure AD.
            string clientId = System.Configuration.ConfigurationManager.AppSettings["ClientId"];

            // RedirectUri is the URL where the user will be redirected to after they sign in.
            string redirectUri = System.Configuration.ConfigurationManager.AppSettings["RedirectUri"];

            // Tenant is the tenant ID (e.g. contoso.onmicrosoft.com, or 'common' for multi-tenant)
            string tenant = System.Configuration.ConfigurationManager.AppSettings["Tenant"];

            // Authority is the URL for authority, composed by Microsoft identity platform endpoint and the tenant name (e.g. https://login.microsoftonline.com/contoso.onmicrosoft.com/v2.0)
            string authority = String.Format(System.Globalization.CultureInfo.InvariantCulture, System.Configuration.ConfigurationManager.AppSettings["Authority"], tenant);

            // 这两句不能没有！
            app.SetDefaultSignInAsAuthenticationType(CookieAuthenticationDefaults.AuthenticationType);
            app.UseCookieAuthentication(new CookieAuthenticationOptions());// 这句会不会覆盖上面的那个app.UseCookieAuthentication？

            app.UseOpenIdConnectAuthentication(
            new OpenIdConnectAuthenticationOptions
            {
                // Sets the ClientId, authority, RedirectUri as obtained from web.config
                ClientId = clientId,
                Authority = authority,
                RedirectUri = redirectUri,
                // PostLogoutRedirectUri is the page that users will be redirected to after sign-out. In this case, it is using the home page
                PostLogoutRedirectUri = redirectUri,
                Scope = OpenIdConnectScope.OpenIdProfile,
                // ResponseType is set to request the code id_token - which contains basic information about the signed-in user
                ResponseType = OpenIdConnectResponseType.CodeIdToken,
                // OpenIdConnectAuthenticationNotifications configures OWIN to send notification of failed authentications to OnAuthenticationFailed method
                Notifications = new OpenIdConnectAuthenticationNotifications
                {
                    AuthenticationFailed = OnAuthenticationFailed
                },
            }
        );
        }

        /// <summary>
        /// Handle failed authentication requests by redirecting the user to the home page with an error in the query string
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        private Task OnAuthenticationFailed(AuthenticationFailedNotification<OpenIdConnectMessage, OpenIdConnectAuthenticationOptions> context)
        {
            context.HandleResponse();
            context.Response.Redirect("/?errormessage=" + context.Exception.Message);
            return Task.FromResult(0);
        }
    }
}