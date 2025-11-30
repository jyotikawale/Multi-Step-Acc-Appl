using System.ComponentModel.DataAnnotations;

namespace LicenseApplication.Models;

public class FileMetadata
{
    [Key]
    public Guid Id { get; set; }

    [Required]
    public Guid ApplicationId { get; set; }

    [Required]
    [StringLength(255)]
    public string FileName { get; set; } = string.Empty;

    [Required]
    [StringLength(500)]
    public string FilePath { get; set; } = string.Empty;

    [Required]
    [StringLength(100)]
    public string ContentType { get; set; } = string.Empty;

    [Required]
    public long FileSize { get; set; }

    public DateTime UploadedAt { get; set; }

    // Navigation property
    public virtual Application? Application { get; set; }
}
