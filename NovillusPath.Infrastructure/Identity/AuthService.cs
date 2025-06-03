using System;
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

    // We'll inject IMapper later if needed for mapping DTO to ApplicationUser if not done before calling service
    // Or RoleManager if dealing with roles here
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
        };

        var result = await _userManager.CreateAsync(newUser, registerUserDto.Password);

        if (!result.Succeeded)
        {
            return AuthResult.Failure(result.Errors.Select(e => e.Description));
        }
        // TODO (Week 2, Day 4): Assign a default role (e.g., "Student") to the new user
        // await _userManager.AddToRoleAsync(newUser, "Student");
        return AuthResult.Success();
    }
}
