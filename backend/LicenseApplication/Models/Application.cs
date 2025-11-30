using System.ComponentModel.DataAnnotations;

namespace LicenseApplication.Models;

public class Application
{
    [Key]
    public Guid Id { get; set; }

    [Required]
    [StringLength(50)]
    public string ReferenceNumber { get; set; } = string.Empty;

    [Required]
    public AccountType AccountType { get; set; }

    [Required]
    public ApplicationStatus Status { get; set; }

    // Common fields for all account types
    [Required]
    [StringLength(200)]
    public string AccountName { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    [StringLength(100)]
    public string Email { get; set; } = string.Empty;

    [Phone]
    [StringLength(20)]
    public string? Phone { get; set; }

    [StringLength(100)]
    public string? AddressLine1 { get; set; }

    [StringLength(100)]
    public string? AddressLine2 { get; set; }

    [StringLength(100)]
    public string? City { get; set; }

    [StringLength(100)]
    public string? State { get; set; }

    [StringLength(10)]
    public string? ZipCode { get; set; }

    [StringLength(100)]
    public string? Country { get; set; }

    // Individual-specific fields
    [StringLength(100)]
    public string? FirstName { get; set; }

    [StringLength(100)]
    public string? MiddleName { get; set; }

    [StringLength(100)]
    public string? LastName { get; set; }

    public DateTime? DateOfBirth { get; set; }

    [StringLength(20)]
    public string? SocialSecurityNumber { get; set; }

    // Business-specific fields
    [StringLength(200)]
    public string? BusinessName { get; set; }

    [StringLength(50)]
    public string? BusinessRegistrationNumber { get; set; }

    [StringLength(50)]
    public string? TaxIdentificationNumber { get; set; }

    public DateTime? BusinessEstablishedDate { get; set; }

    [StringLength(100)]
    public string? BusinessType { get; set; }

    // Government-specific fields
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
    public bool HasPreviousLicense { get; set; }

    [StringLength(50)]
    public string? PreviousLicenseNumber { get; set; }

    public DateTime? PreviousLicenseExpiry { get; set; }

    [StringLength(1000)]
    public string? Notes { get; set; }

    // Audit fields
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public DateTime? SubmittedAt { get; set; }

    // Navigation property
    public virtual ICollection<FileMetadata> Files { get; set; } = new List<FileMetadata>();
}
