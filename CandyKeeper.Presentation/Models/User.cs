using System.Windows;

namespace CandyKeeper.Presentation.Models;

public class User
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;

    public string PasswordHashed { get; set; } = string.Empty;
    
    public int PrincipalId { get; set; }
    public int? StoreId { get; set; }
    public virtual Store? Store { get; set; }
    public bool IsBlocked { get; set; } = false;

}