using System;
namespace Microsoft.AspNetCore.Components.Authentication;

public interface IAuthService
{
    Task Login();

    Task Logout();
}

