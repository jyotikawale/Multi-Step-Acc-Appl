using System.ComponentModel.DataAnnotations;
using LicenseApplication.Models;

namespace LicenseApplication.DTOs;

// Lenient validation for drafts - fields are optional
public class UpdateDraftDto
{
    public AccountType? AccountType { get; set; }

    [StringLength(200, ErrorMessage = "Account name cannot exceed 200 characters")]
    public string? AccountName { get; set; }

    [EmailAddress(ErrorMessage = "Invalid email format")]
    [StringLength(100, ErrorMessage = "Email cannot exceed 100 characters")]
    public string? Email { get; set; }

    [Phone(ErrorMessage = "Invalid phone number format")]
    [StringLength(20, ErrorMessage = "Phone cannot exceed 20 characters")]
    public string? Phone { get; set; }

    [StringLength(100)]
    public string? AddressLine1 { get; set; }

    [StringLength(100)]
    public string? AddressLine2 { get; set; }

    [StringLength(100)]
    public string? City { get; set; }

    [StringLength(100)]
    public string? State { get; set; }

    [RegularExpression(@"^\d{6}$", ErrorMessage = "Invalid PIN code format (use 6 digits)")]
    public string? ZipCode { get; set; }

    [StringLength(100)]
    public string? Country { get; set; }

    // Individual fields
    [StringLength(100)]
    public string? FirstName { get; set; }

    [StringLength(100)]
    public string? MiddleName { get; set; }

    [StringLength(100)]
    public string? LastName { get; set; }

    public DateTime? DateOfBirth { get; set; }

    [StringLength(20)]
    public string? SocialSecurityNumber { get; set; }

    // Business fields
    [StringLength(200)]
    public string? BusinessName { get; set; }

    [StringLength(50)]
    public string? BusinessRegistrationNumber { get; set; }

    [StringLength(50)]
    public string? TaxIdentificationNumber { get; set; }

    public DateTime? BusinessEstablishedDate { get; set; }

    [StringLength(100)]
    public string? BusinessType { get; set; }

    // Government fields
    [StringLength(200)]
    public string? AgencyName { get; set; }

    [StringLength(100)]
    public string? DepartmentName { get; set; }

    [StringLength(200)]
    public string? AuthorizedOfficer { get; set; }

    [StringLength(100)]
    public string? OfficerDesignation { get; set; }

    [StringLength(50)]
    public string? GovernmentIdNumber { get; set; }

    // License information
    public bool? HasPreviousLicense { get; set; }

    [StringLength(50)]
    public string? PreviousLicenseNumber { get; set; }

    public DateTime? PreviousLicenseExpiry { get; set; }

    [StringLength(1000)]
    public string? Notes { get; set; }
}
