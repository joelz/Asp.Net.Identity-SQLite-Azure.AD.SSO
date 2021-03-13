# Asp.Net.Identity-SQLite-Azure.AD.SSO



## demo user
user name: a@abc.com

password: Abc@123456

## 方法一：直接用ASP.NET Identity中的app.UseMicrosoftAccountAuthentication
1. 升级Owin相关package到最新的4.1.1 （20210313）：这点最关键！
2. Azure AD 中的callback url要写成固定的：https://{domain}/signin-microsoft
3. app.UseMicrosoftAccountAuthentication时只用写ClientId和ClientSecret，其他什么都不用写
4. 登录成功的请求流程看ExternalLoginCallback-Microsoft-Success.har


问题：signin-microsoft请求的处理的时间特别长，有40多秒，为什么？

因为Microsoft.ApplicationInsights。看这里：https://stackoverflow.com/questions/62628937/net-core-3-1-identity-takes-too-long-to-login-on-azure 

如何移除？运行`UnInstall-Package Microsoft.ApplicationInsights.Web -RemoveDependencies`。看这里。https://stackoverflow.com/a/43096519


## 方法二：用通用型的Microsoft.Owin.Security.OpenIdConnect 
1. 安装Microsoft.Owin.Security.OpenIdConnect（会引入一些依赖）
2. 测试发现Azure AD 中的callback url写成https://{domain}/signin-oidc 没什么用，去到Account/ExternalLoginCallback 里面loginInfo都是空的
3. 用例子AppModelv2-WebApp-OpenIDConnect-DotNet-master中的取user信息的方法可以取到值
4. 尽管重复`app.UseCookieAuthentication(new CookieAuthenticationOptions());`，但似乎没什么影响

## links
* [Quickstart: Add sign-in with Microsoft to an ASP.NET web app - Microsoft identity platform | Microsoft Docs](https://docs.microsoft.com/en-us/azure/active-directory/develop/quickstart-v2-aspnet-webapp#how-the-sample-works)
* [Azure-Samples/ms-identity-aspnet-webapp-openidconnect: A sample showcasing how to develop a web application that handles sign on via the unified Azure AD and MSA endpoint](https://github.com/Azure-Samples/ms-identity-aspnet-webapp-openidconnect)

