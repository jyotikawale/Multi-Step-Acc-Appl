using LicenseApplication.Models;

namespace LicenseApplication.DTOs;

public class ApplicationDto
{
    public Guid Id { get; set; }
    public string ReferenceNumber { get; set; } = string.Empty;
    public AccountType AccountType { get; set; }
    public ApplicationStatus Status { get; set; }
    public string AccountName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string? AddressLine1 { get; set; }
    public string? AddressLine2 { get; set; }
    public string? City { get; set; }
    public string? State { get; set; }
    public string? ZipCode { get; set; }
    public string? Country { get; set; }

    // Individual fields
    public string? FirstName { get; set; }
    public string? MiddleName { get; set; }
    public string? LastName { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public string? SocialSecurityNumber { get; set; }

    // Business fields
    public string? BusinessName { get; set; }
    public string? BusinessRegistrationNumber { get; set; }
    public string? TaxIdentificationNumber { get; set; }
    public DateTime? BusinessEstablishedDate { get; set; }
    public string? BusinessType { get; set; }

    // Government fields
    public string? AgencyName { get; set; }
    public string? DepartmentName { get; set; }
    public string? AuthorizedOfficer { get; set; }
    public string? OfficerDesignation { get; set; }
    public string? GovernmentIdNumber { get; set; }

    // License information
    public bool HasPreviousLicense { get; set; }
    public string? PreviousLicenseNumber { get; set; }
    public DateTime? PreviousLicenseExpiry { get; set; }
    public string? Notes { get; set; }

    // Audit fields
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public DateTime? SubmittedAt { get; set; }

    // Files
    public List<FileMetadataDto> Files { get; set; } = new();
}
