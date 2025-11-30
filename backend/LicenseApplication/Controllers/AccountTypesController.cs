using Microsoft.AspNetCore.Mvc;

namespace LicenseApplication.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AccountTypesController : ControllerBase
{
    [HttpGet]
    public IActionResult GetAccountTypes()
    {
        var accountTypes = new[]
        {
            new
            {
                id = 1,
                name = "Individual",
                description = "Personal account for individuals",
                fields = new[]
                {
                    new { name = "firstName", label = "First Name", type = "text", required = true, step = 2 },
                    new { name = "middleName", label = "Middle Name", type = "text", required = false, step = 2 },
                    new { name = "lastName", label = "Last Name", type = "text", required = true, step = 2 },
                    new { name = "dateOfBirth", label = "Date of Birth", type = "date", required = true, step = 2 },
                    new { name = "socialSecurityNumber", label = "SSN/Aadhaar", type = "text", required = false, step = 2 }
                }
            },
            new
            {
                id = 2,
                name = "Business",
                description = "Account for business entities",
                fields = new[]
                {
                    new { name = "businessName", label = "Business Name", type = "text", required = true, step = 2 },
                    new { name = "businessRegistrationNumber", label = "Registration Number", type = "text", required = true, step = 2 },
                    new { name = "taxIdentificationNumber", label = "Tax ID (GST/PAN)", type = "text", required = true, step = 2 },
                    new { name = "businessEstablishedDate", label = "Established Date", type = "date", required = true, step = 2 },
                    new { name = "businessType", label = "Business Type", type = "text", required = true, step = 2 }
                }
            },
            new
            {
                id = 3,
                name = "Government",
                description = "Account for government agencies",
                fields = new[]
                {
                    new { name = "agencyName", label = "Agency Name", type = "text", required = true, step = 2 },
                    new { name = "departmentName", label = "Department Name", type = "text", required = true, step = 2 },
                    new { name = "authorizedOfficer", label = "Authorized Officer", type = "text", required = true, step = 2 },
                    new { name = "officerDesignation", label = "Officer Designation", type = "text", required = true, step = 2 },
                    new { name = "governmentIdNumber", label = "Government ID", type = "text", required = true, step = 2 }
                }
            }
        };

        return Ok(accountTypes);
    }
}
