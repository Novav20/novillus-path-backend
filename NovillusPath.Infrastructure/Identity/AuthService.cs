using Microsoft.AspNetCore.Identity;
using NovillusPath.Application.DTOs.User;
using NovillusPath.Application.Interfaces.Identity;
using NovillusPath.Domain.Entities;

namespace NovillusPath.Infrastructure.Identity;

public class AuthService(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, ITokenGeneratorService tokenGenerator) : IAuthService
{
    private readonly UserManager<ApplicationUser> _userManager = userManager;
    private readonly SignInManager<ApplicationUser> _signInManager = signInManager;
    private readonly ITokenGeneratorService _tokenGenerator = tokenGenerator;

    public async Task<LoginResult> LoginUserAsync(LoginUserDto loginUserDto, CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByEmailAsync(loginUserDto.Email);

        if (user is null)
        {
            return LoginResult.Failure(["Invalid email or password."]);
        }

        var result = await _signInManager.CheckPasswordSignInAsync(user, loginUserDto.Password, lockoutOnFailure: false);
        if (!result.Succeeded)
        {
            return LoginResult.Failure(["Invalid email or password."]);
        }
        string token = await _tokenGenerator.GenerateTokenAsync(user);
        return LoginResult.Success(token);
    }

    public async Task<AuthResult> RegisterUserAsync(RegisterUserDto registerUserDto, CancellationToken cancellationToken = default)
    {
        var existingUser = await _userManager.FindByEmailAsync(registerUserDto.Email);

        if (existingUser is not null)
        {
            return AuthResult.Failure(["User with this email already exists."]);
        }

        var newUser = new ApplicationUser
        {
            Email = registerUserDto.Email,
            UserName = registerUserDto.Email,
            FullName = registerUserDto.FullName,
            EmailConfirmed = true
        };

        var createUserResult = await _userManager.CreateAsync(newUser, registerUserDto.Password);

        if (!createUserResult.Succeeded)
        {
            return AuthResult.Failure(createUserResult.Errors.Select(e => e.Description));
        }
        const string defaultRole = "Student";
        var addToRoleResult = await _userManager.AddToRoleAsync(newUser, defaultRole);
        if (!addToRoleResult.Succeeded)
        {
            await _userManager.DeleteAsync(newUser); // Attempt to roll back user creation
            return AuthResult.Failure(addToRoleResult.Errors.Select(e => e.Description).Concat(["User registration failed due to role assignment issue."]));
        }
        return AuthResult.Success();
    }
}
