using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace SurveyBasket.Authentication;

public class JwtOptions
{
    // Option Pattern To Read AppSettings Section 
    public static string SectionName = "jwt";
    [Required]
    public string Key { get; init; } = string.Empty;
    [Required]
    public string Issuer { get; init; } = string.Empty;
    [Required]
    public string Audience { get; init; } = string.Empty;
    [Required,Range(1,int.MaxValue)]
    public int ExpiredIn { get; init; }
}
