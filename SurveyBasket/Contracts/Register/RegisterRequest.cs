﻿namespace SurveyBasket.Contracts.Register;

public record RegisterRequest(
    string FirstName,
    string LastName,
    string Email,
    string Password
);
