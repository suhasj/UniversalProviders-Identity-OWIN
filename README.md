UniversalProviders-Identity-OWIN
================================
This project is intended as a sample for migrations from Universal Providers to the new ASP.NET Identity system. The initial state of the application is based on the ASP.NET Web Forms templates in VS 2012, which demonstrated logging in using Local and Social Providers. For more information on this template please see http://www.asp.net/aspnet/overview/aspnet-45/oauth-in-the-default-aspnet-45-templates
This sample demonstrates how to migrate an application written using Universal Providers

Note: This is a way of migrating to the new system. Please proceed with caution and use your test database first for migrating before you deploy this in production. The steps in this application are very generic and can be followed for any type of application written using Universal Provider with some customization 

Steps followed for migration
1. Create  a new C# 4.5 Web application Visual Studio 2012 template. Enable Facebook OAuth.
2. Create a local user and a new user using the Facebook OAuth login
3. Create a new role 'Admin' and add the local user to the 'Admin' role
4. Create a 'AdminManagement' section in the application and have rules so that only the user in 'Admin' role can access it.
5. Create a local Nuget feed pointing to http://www.myget.org/f/aspnetwebstacknightly. Open Manage Nuget packages for the project and install the following packages. Note, you would not need to use this feed once the RTM versions of these packages are released. You can then download these file directly from the Nuget feed.

- Microsoft.AspNet.Identity.Owin
- Microsoft.AspNet.Identity.EntityFramework
- Microsoft.Owin.Host.SystemWeb
- Microsoft.Owin.Security.Facebook
- Microsoft.Owin.Security.Google
- Microsoft.Owin.Security.MicrosoftAccount
- Microsoft.Owin.Security.Twitter

The dependent packages will also be pulled in. Note, the packages on the feed http://www.myget.org/f/aspnetwebstacknightly are nightly builds so they are updated each night.
6. Add the Startup class needed by OWIN middleware for configuring authentication.
7. Create Model POCO classes that map to the existing database. In the case of this template the model classes are under the IdentityModels folder. It also has the DBContext file which includes the mapping between classes and tables along with relations.
8. Create new pages for Account management which use the new Identity system. In our current example these pages are created under the IdentityAccount folder. The pages added are similar to the ones in the template except that they use the new Identity system.
9.Set the authentication mode to 'None' in web.config for the OWIN authentication to be used.
10.Login using old user credentials and facebook credentials to see that the user data before migration is preserved. Also verify that that the roles behavior is as expected.

Updates pending
1. Mapping existing user profile data
2. Create an OAuth user without password
