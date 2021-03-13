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


## 方法二：用通用型的Microsoft.Owin.Security.OpenIdConnect 
1. TODO

## links
* [Quickstart: Add sign-in with Microsoft to an ASP.NET web app - Microsoft identity platform | Microsoft Docs](https://docs.microsoft.com/en-us/azure/active-directory/develop/quickstart-v2-aspnet-webapp#how-the-sample-works)
* [Azure-Samples/ms-identity-aspnet-webapp-openidconnect: A sample showcasing how to develop a web application that handles sign on via the unified Azure AD and MSA endpoint](https://github.com/Azure-Samples/ms-identity-aspnet-webapp-openidconnect)

