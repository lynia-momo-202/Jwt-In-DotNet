namespace MonAPI.Models;

public record LoginRequest(string UserName, string Password, string Audience);
public record RefreshRequest(string UserName, string Audience, string RefreshToken);
