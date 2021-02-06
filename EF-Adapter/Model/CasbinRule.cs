using System;

namespace Casbin.NET.Adapter.EF.Model
{
    public class CasbinRule : ICasbinRule
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string PType { get; set; }
        public string V0 { get; set; }
        public string V1 { get; set; }
        public string V2 { get; set; }
        public string V3 { get; set; }
        public string V4 { get; set; }
        public string V5 { get; set; }

    }
}
