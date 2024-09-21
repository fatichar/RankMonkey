﻿namespace RankMonkey.Shared.Models;

public class CreateUserDto
{
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string AuthType { get; set; } = string.Empty;
    public string? ExternalId { get; set; }
}