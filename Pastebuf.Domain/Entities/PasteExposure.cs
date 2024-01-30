namespace Pastebug.Domain.Entities;

public class PasteExposure
{
    public Guid Id { get; set; }
    public int ExposureId { get; set; }
    public string PasteHash { get; set; }
    public Paste Paste { get; set; }
}
