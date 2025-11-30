using System.ComponentModel.DataAnnotations;
using LicenseApplication.Models;

namespace LicenseApplication.DTOs;

public class CreateApplicationDto
{
    [Required(ErrorMessage = "Account type is required")]
    public AccountType AccountType { get; set; }

    [Required(ErrorMessage = "Account name is required")]
    [StringLength(200, ErrorMessage = "Account name cannot exceed 200 characters")]
    public string AccountName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid email format")]
    [StringLength(100, ErrorMessage = "Email cannot exceed 100 characters")]
    public string Email { get; set; } = string.Empty;

    [Phone(ErrorMessage = "Invalid phone number format")]
    [StringLength(20, ErrorMessage = "Phone cannot exceed 20 characters")]
    public string? Phone { get; set; }

    [StringLength(100, ErrorMessage = "Address line 1 cannot exceed 100 characters")]
    public string? AddressLine1 { get; set; }

    [StringLength(100, ErrorMessage = "Address line 2 cannot exceed 100 characters")]
    public string? AddressLine2 { get; set; }

    [StringLength(100, ErrorMessage = "City cannot exceed 100 characters")]
    public string? City { get; set; }

    [StringLength(100, ErrorMessage = "State cannot exceed 100 characters")]
    public string? State { get; set; }

    [RegularExpression(@"^\d{6}$", ErrorMessage = "Invalid PIN code format (use 6 digits, e.g., 110001)")]
    public string? ZipCode { get; set; }

    [StringLength(100, ErrorMessage = "Country cannot exceed 100 characters")]
    public string? Country { get; set; }

    // Individual-specific fields
    [StringLength(100, ErrorMessage = "First name cannot exceed 100 characters")]
    public string? FirstName { get; set; }

    [StringLength(100, ErrorMessage = "Middle name cannot exceed 100 characters")]
    public string? MiddleName { get; set; }

    [StringLength(100, ErrorMessage = "Last name cannot exceed 100 characters")]
    public string? LastName { get; set; }

    public DateTime? DateOfBirth { get; set; }

    [StringLength(20, ErrorMessage = "Social Security Number cannot exceed 20 characters")]
    public string? SocialSecurityNumber { get; set; }

    // Business-specific fields
    [StringLength(200, ErrorMessage = "Business name cannot exceed 200 characters")]
    public string? BusinessName { get; set; }

    [StringLength(50, ErrorMessage = "Business registration number cannot exceed 50 characters")]
    public string? BusinessRegistrationNumber { get; set; }

    [StringLength(50, ErrorMessage = "Tax identification number cannot exceed 50 characters")]
    public string? TaxIdentificationNumber { get; set; }

    public DateTime? BusinessEstablishedDate { get; set; }

    [StringLength(100, ErrorMessage = "Business type cannot exceed 100 characters")]
    public string? BusinessType { get; set; }

    // Government-specific fields
    [StringLength(200, ErrorMessage = "Agency name cannot exceed 200 characters")]
    public string? AgencyName { get; set; }

    [StringLength(100, ErrorMessage = "Department name cannot exceed 100 characters")]
    public string? DepartmentName { get; set; }

    [StringLength(200, ErrorMessage = "Authorized officer name cannot exceed 200 characters")]
    public string? AuthorizedOfficer { get; set; }

    [StringLength(100, ErrorMessage = "Officer designation cannot exceed 100 characters")]
    public string? OfficerDesignation { get; set; }

    [StringLength(50, ErrorMessage = "Government ID number cannot exceed 50 characters")]
    public string? GovernmentIdNumber { get; set; }

    // License information
    public bool HasPreviousLicense { get; set; }

    [StringLength(50, ErrorMessage = "Previous license number cannot exceed 50 characters")]
    public string? PreviousLicenseNumber { get; set; }

    public DateTime? PreviousLicenseExpiry { get; set; }

    [StringLength(1000, ErrorMessage = "Notes cannot exceed 1000 characters")]
    public string? Notes { get; set; }
}
