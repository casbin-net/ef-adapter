namespace Casbin.NET.Adapter.EF.Model
{
    public interface ICasbinRule
    {
        string PType { get; set; }
        string V0 { get; set; }
        string V1 { get; set; }
        string V2 { get; set; }
        string V3 { get; set; }
        string V4 { get; set; }
        string V5 { get; set; }
    }
    
}
