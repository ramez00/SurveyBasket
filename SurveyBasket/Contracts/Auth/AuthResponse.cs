﻿namespace SurveyBasket.Contracts.Auth;

public record AuthResponse(
    string Id,
    string? Email,
    string FirstName,
    string LastName,
    string token,
    int ExpiryValid,
    string RefreshToken,
    DateTime RefreshTokenExpiration
);
