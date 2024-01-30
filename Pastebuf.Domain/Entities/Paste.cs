namespace Pastebug.Domain.Entities;

public class Paste
{
    public string Hash { get; set; }
    public Guid? UserId { get; set; }
    public string Title { get; set; }
    public string Text { get; set; }
    public int Likes { get; set; }
    public string Syntax { get; set; }
    public int Dislikes { get; set; }
    public int ViewdTimes { get; set; }
    public int Visibility { get; set; }
    public DateTime CreationDate { get; set; }
    public DateTime? EditDate { get; set; }
    public DateTime ExpireDate { get; set; }
    public PasteExposure PasteExposure {  get; set; }
}