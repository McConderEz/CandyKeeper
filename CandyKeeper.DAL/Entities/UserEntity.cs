namespace CandyKeeper.DAL.Entities;

public class UserEntity
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string PasswordHashed { get; set; } = string.Empty;
    public int PrincipalId { get; set; }
    public int? StoreId { get; set; }
    public virtual StoreEntity? Store { get; set; }
}