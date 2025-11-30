using Microsoft.EntityFrameworkCore;
using LicenseApplication.Data;
using LicenseApplication.DTOs;
using LicenseApplication.Models;

namespace LicenseApplication.Services;

public class ApplicationService
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<ApplicationService> _logger;

    public ApplicationService(ApplicationDbContext context, ILogger<ApplicationService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<ApplicationDto> CreateApplicationAsync(CreateApplicationDto dto)
    {
        var application = new Application
        {
            Id = Guid.NewGuid(),
            ReferenceNumber = GenerateReferenceNumber(),
            Status = ApplicationStatus.Submitted,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            SubmittedAt = DateTime.UtcNow,

            // Map DTO fields
            AccountType = dto.AccountType,
            AccountName = dto.AccountName,
            Email = dto.Email,
            Phone = dto.Phone,
            AddressLine1 = dto.AddressLine1,
            AddressLine2 = dto.AddressLine2,
            City = dto.City,
            State = dto.State,
            ZipCode = dto.ZipCode,
            Country = dto.Country,

            // Individual fields
            FirstName = dto.FirstName,
            MiddleName = dto.MiddleName,
            LastName = dto.LastName,
            DateOfBirth = dto.DateOfBirth,
            SocialSecurityNumber = dto.SocialSecurityNumber,

            // Business fields
            BusinessName = dto.BusinessName,
            BusinessRegistrationNumber = dto.BusinessRegistrationNumber,
            TaxIdentificationNumber = dto.TaxIdentificationNumber,
            BusinessEstablishedDate = dto.BusinessEstablishedDate,
            BusinessType = dto.BusinessType,

            // Government fields
            AgencyName = dto.AgencyName,
            DepartmentName = dto.DepartmentName,
            AuthorizedOfficer = dto.AuthorizedOfficer,
            OfficerDesignation = dto.OfficerDesignation,
            GovernmentIdNumber = dto.GovernmentIdNumber,

            // License information
            HasPreviousLicense = dto.HasPreviousLicense,
            PreviousLicenseNumber = dto.PreviousLicenseNumber,
            PreviousLicenseExpiry = dto.PreviousLicenseExpiry,
            Notes = dto.Notes
        };

        _context.Applications.Add(application);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Application created with ID: {Id}, Reference: {Reference}",
            application.Id, application.ReferenceNumber);

        return MapToDto(application);
    }

    public async Task<ApplicationDto> CreateDraftAsync(CreateApplicationDto dto)
    {
        var application = new Application
        {
            Id = Guid.NewGuid(),
            ReferenceNumber = GenerateReferenceNumber(),
            Status = ApplicationStatus.Draft,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,

            // Map all fields
            AccountType = dto.AccountType,
            AccountName = dto.AccountName,
            Email = dto.Email,
            Phone = dto.Phone,
            AddressLine1 = dto.AddressLine1,
            AddressLine2 = dto.AddressLine2,
            City = dto.City,
            State = dto.State,
            ZipCode = dto.ZipCode,
            Country = dto.Country,
            FirstName = dto.FirstName,
            MiddleName = dto.MiddleName,
            LastName = dto.LastName,
            DateOfBirth = dto.DateOfBirth,
            SocialSecurityNumber = dto.SocialSecurityNumber,
            BusinessName = dto.BusinessName,
            BusinessRegistrationNumber = dto.BusinessRegistrationNumber,
            TaxIdentificationNumber = dto.TaxIdentificationNumber,
            BusinessEstablishedDate = dto.BusinessEstablishedDate,
            BusinessType = dto.BusinessType,
            AgencyName = dto.AgencyName,
            DepartmentName = dto.DepartmentName,
            AuthorizedOfficer = dto.AuthorizedOfficer,
            OfficerDesignation = dto.OfficerDesignation,
            GovernmentIdNumber = dto.GovernmentIdNumber,
            HasPreviousLicense = dto.HasPreviousLicense,
            PreviousLicenseNumber = dto.PreviousLicenseNumber,
            PreviousLicenseExpiry = dto.PreviousLicenseExpiry,
            Notes = dto.Notes
        };

        _context.Applications.Add(application);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Draft created with ID: {Id}", application.Id);

        return MapToDto(application);
    }

    public async Task<ApplicationDto> UpdateDraftAsync(Guid id, UpdateDraftDto dto)
    {
        var application = await _context.Applications
            .Include(a => a.Files)
            .FirstOrDefaultAsync(a => a.Id == id);

        if (application == null)
            throw new KeyNotFoundException($"Application with ID {id} not found");

        if (application.Status != ApplicationStatus.Draft)
            throw new InvalidOperationException("Only draft applications can be updated with this endpoint");

        // Update fields if provided
        if (dto.AccountType.HasValue)
            application.AccountType = dto.AccountType.Value;

        if (dto.AccountName != null)
            application.AccountName = dto.AccountName;

        if (dto.Email != null)
            application.Email = dto.Email;

        application.Phone = dto.Phone;
        application.AddressLine1 = dto.AddressLine1;
        application.AddressLine2 = dto.AddressLine2;
        application.City = dto.City;
        application.State = dto.State;
        application.ZipCode = dto.ZipCode;
        application.Country = dto.Country;
        application.FirstName = dto.FirstName;
        application.MiddleName = dto.MiddleName;
        application.LastName = dto.LastName;
        application.DateOfBirth = dto.DateOfBirth;
        application.SocialSecurityNumber = dto.SocialSecurityNumber;
        application.BusinessName = dto.BusinessName;
        application.BusinessRegistrationNumber = dto.BusinessRegistrationNumber;
        application.TaxIdentificationNumber = dto.TaxIdentificationNumber;
        application.BusinessEstablishedDate = dto.BusinessEstablishedDate;
        application.BusinessType = dto.BusinessType;
        application.AgencyName = dto.AgencyName;
        application.DepartmentName = dto.DepartmentName;
        application.AuthorizedOfficer = dto.AuthorizedOfficer;
        application.OfficerDesignation = dto.OfficerDesignation;
        application.GovernmentIdNumber = dto.GovernmentIdNumber;

        if (dto.HasPreviousLicense.HasValue)
            application.HasPreviousLicense = dto.HasPreviousLicense.Value;

        application.PreviousLicenseNumber = dto.PreviousLicenseNumber;
        application.PreviousLicenseExpiry = dto.PreviousLicenseExpiry;
        application.Notes = dto.Notes;

        application.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        _logger.LogInformation("Draft updated: {Id}", application.Id);

        return MapToDto(application);
    }

    public async Task<ApplicationDto?> GetByIdAsync(Guid id)
    {
        var application = await _context.Applications
            .Include(a => a.Files)
            .FirstOrDefaultAsync(a => a.Id == id);

        return application == null ? null : MapToDto(application);
    }

    public async Task<ApplicationDto?> GetByReferenceNumberAsync(string referenceNumber)
    {
        var application = await _context.Applications
            .Include(a => a.Files)
            .FirstOrDefaultAsync(a => a.ReferenceNumber == referenceNumber);

        return application == null ? null : MapToDto(application);
    }

    private string GenerateReferenceNumber()
    {
        var timestamp = DateTime.UtcNow.ToString("yyyyMMddHHmmss");
        var random = new Random().Next(1000, 9999);
        return $"LIC-{timestamp}-{random}";
    }

    private ApplicationDto MapToDto(Application app)
    {
        return new ApplicationDto
        {
            Id = app.Id,
            ReferenceNumber = app.ReferenceNumber,
            AccountType = app.AccountType,
            Status = app.Status,
            AccountName = app.AccountName,
            Email = app.Email,
            Phone = app.Phone,
            AddressLine1 = app.AddressLine1,
            AddressLine2 = app.AddressLine2,
            City = app.City,
            State = app.State,
            ZipCode = app.ZipCode,
            Country = app.Country,
            FirstName = app.FirstName,
            MiddleName = app.MiddleName,
            LastName = app.LastName,
            DateOfBirth = app.DateOfBirth,
            SocialSecurityNumber = app.SocialSecurityNumber,
            BusinessName = app.BusinessName,
            BusinessRegistrationNumber = app.BusinessRegistrationNumber,
            TaxIdentificationNumber = app.TaxIdentificationNumber,
            BusinessEstablishedDate = app.BusinessEstablishedDate,
            BusinessType = app.BusinessType,
            AgencyName = app.AgencyName,
            DepartmentName = app.DepartmentName,
            AuthorizedOfficer = app.AuthorizedOfficer,
            OfficerDesignation = app.OfficerDesignation,
            GovernmentIdNumber = app.GovernmentIdNumber,
            HasPreviousLicense = app.HasPreviousLicense,
            PreviousLicenseNumber = app.PreviousLicenseNumber,
            PreviousLicenseExpiry = app.PreviousLicenseExpiry,
            Notes = app.Notes,
            CreatedAt = app.CreatedAt,
            UpdatedAt = app.UpdatedAt,
            SubmittedAt = app.SubmittedAt,
            Files = app.Files.Select(f => new FileMetadataDto
            {
                Id = f.Id,
                FileName = f.FileName,
                ContentType = f.ContentType,
                FileSize = f.FileSize,
                UploadedAt = f.UploadedAt
            }).ToList()
        };
    }
}
