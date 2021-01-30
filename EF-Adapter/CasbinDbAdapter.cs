using NetCasbin.Persist;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System;
using System.Data.Entity;
using System.Threading.Tasks;
using Casbin.NET.Adapter.EF.Model;

namespace Casbin.NET.Adapter.EF
{
    public class CasbinDbAdapter : IAdapter 
    {
        private readonly CasbinDbContext _context;

        public CasbinDbAdapter(CasbinDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        #region sync operations

        public void LoadPolicy(NetCasbin.Model.Model model)
        {
            var rules = _context.CasbinRule.AsNoTracking().ToList();
            LoadPolicyData(model, Helper.LoadPolicyLine, rules);
        }
        
        public void RemovePolicy(string sec, string ptype, IList<string> rule)
        {
            RemoveFilteredPolicy(sec, ptype, 0, rule.ToArray());
        }

        public void RemoveFilteredPolicy(string sec, string ptype, int fieldIndex, params string[] fieldValues)
        {
            if (fieldValues == null || !fieldValues.Any())
                return;
            var line = SavePolicyLine(sec, ptype, fieldIndex, fieldValues);

            var query = _context.CasbinRule.Where(p => p.PType == line.PType);
            query = ApplyQueryFilter(query, line);

            _context.CasbinRule.RemoveRange(query);
            _context.SaveChanges();
        }

        public void SavePolicy(NetCasbin.Model.Model model)
        {
            var lines = SavePolicyLines(model);
            if (lines.Any())
            {
                _context.CasbinRule.AddRange(lines);
                _context.SaveChanges();
            }
        }

        public void AddPolicy(string sec, string ptype, IList<string> rule)
        {
            var line = SavePolicyLine(ptype, rule);
            _context.CasbinRule.Add(line);
            _context.SaveChanges();
        }

        #endregion

        #region async operations

        public async Task LoadPolicyAsync(NetCasbin.Model.Model model)
        {
            var rules = await _context.CasbinRule.AsNoTracking().ToListAsync();
            LoadPolicyData(model, Helper.LoadPolicyLine, rules);
        }

        public async Task SavePolicyAsync(NetCasbin.Model.Model model)
        {
            var lines = SavePolicyLines(model);
            if (lines.Any())
            {
                _context.CasbinRule.AddRange(lines);
                await _context.SaveChangesAsync();
            }
        }

        public async Task AddPolicyAsync(string sec, string ptype, IList<string> rule)
        {
            var line = SavePolicyLine(ptype, rule);
            _context.CasbinRule.Add(line);
            await _context.SaveChangesAsync();
        }

        public async Task RemovePolicyAsync(string sec, string ptype, IList<string> rule)
        {
            await RemoveFilteredPolicyAsync(sec, ptype, 0, rule.ToArray());
        }

        public async Task RemoveFilteredPolicyAsync(string sec, string ptype, int fieldIndex, params string[] fieldValues)
        {
            if (fieldValues == null || !fieldValues.Any())
                return;
            var line = SavePolicyLine(sec, ptype, fieldIndex, fieldValues);

            var query = _context.CasbinRule.Where(p => p.PType == line.PType);
            query = ApplyQueryFilter(query, line);

            _context.CasbinRule.RemoveRange(query);

            await _context.SaveChangesAsync();
        }
        #endregion

        #region helper functions

        private void LoadPolicyData(NetCasbin.Model.Model model, Helper.LoadPolicyLineHandler<string, NetCasbin.Model.Model> handler, IEnumerable<CasbinRule> rules)
        {
            foreach (var rule in rules)
            {
                handler(GetPolicyContent(rule), model);
            }
        }

        private string GetPolicyContent(CasbinRule rule)
        {
            StringBuilder sb = new StringBuilder(rule.PType);
            void Append(string v)
            {
                if (string.IsNullOrEmpty(v))
                {
                    return;
                }
                sb.Append($", {v}");
            }
            Append(rule.V0);
            Append(rule.V1);
            Append(rule.V2);
            Append(rule.V3);
            Append(rule.V4);
            Append(rule.V5);
            return sb.ToString();
        }

        private List<CasbinRule> SavePolicyLines(NetCasbin.Model.Model model)
        {
            List<CasbinRule> lines = new List<CasbinRule>();
            if (model.Model.ContainsKey("p"))
            {
                foreach (var kv in model.Model["p"])
                {
                    var ptype = kv.Key;
                    var ast = kv.Value;
                    foreach (var rule in ast.Policy)
                    {
                        var line = SavePolicyLine(ptype, rule);
                        lines.Add(line);
                    }
                }
            }
            if (model.Model.ContainsKey("g"))
            {
                foreach (var kv in model.Model["g"])
                {
                    var ptype = kv.Key;
                    var ast = kv.Value;
                    foreach (var rule in ast.Policy)
                    {
                        var line = SavePolicyLine(ptype, rule);
                        lines.Add(line);
                    }
                }
            }
            return lines;
        }
        private CasbinRule SavePolicyLine(string ptype, IList<string> rule)
        {
            var line = new CasbinRule();
            line.PType = ptype;
            if (rule.Any())
            {
                line.V0 = rule[0];
            }
            if (rule.Count > 1)
            {
                line.V1 = rule[1];
            }
            if (rule.Count > 2)
            {
                line.V2 = rule[2];
            }
            if (rule.Count > 3)
            {
                line.V3 = rule[3];
            }
            if (rule.Count > 4)
            {
                line.V4 = rule[4];
            }
            if (rule.Count > 5)
            {
                line.V5 = rule[5];
            }

            return line;
        }

        private CasbinRule SavePolicyLine(string sec, string ptype, int fieldIndex, params string[] fieldValues)
        {
            var line = new CasbinRule
            {
                PType = ptype
            };
            var len = fieldValues.Count();
            if (fieldIndex <= 0 && 0 < fieldIndex + len)
            {
                line.V0 = fieldValues[0 - fieldIndex];
            }
            if (fieldIndex <= 1 && 1 < fieldIndex + len)
            {
                line.V1 = fieldValues[1 - fieldIndex];
            }
            if (fieldIndex <= 2 && 2 < fieldIndex + len)
            {
                line.V2 = fieldValues[2 - fieldIndex];
            }
            if (fieldIndex <= 3 && 3 < fieldIndex + len)
            {
                line.V3 = fieldValues[3 - fieldIndex];
            }
            if (fieldIndex <= 4 && 4 < fieldIndex + len)
            {
                line.V4 = fieldValues[4 - fieldIndex];
            }
            if (fieldIndex <= 5 && 5 < fieldIndex + len)
            {
                line.V5 = fieldValues[5 - fieldIndex];
            }
            return line;
        }

        private IQueryable<CasbinRule> ApplyQueryFilter(IQueryable<CasbinRule> query, CasbinRule line)
        {
            if (!string.IsNullOrEmpty(line.V0))
            {
                query = query.Where(p => p.V0 == line.V0);
            }
            if (!string.IsNullOrEmpty(line.V1))
            {
                query = query.Where(p => p.V1 == line.V1);
            }
            if (!string.IsNullOrEmpty(line.V2))
            {
                query = query.Where(p => p.V2 == line.V2);
            }
            if (!string.IsNullOrEmpty(line.V3))
            {
                query = query.Where(p => p.V3 == line.V3);
            }
            if (!string.IsNullOrEmpty(line.V4))
            {
                query = query.Where(p => p.V4 == line.V4);
            }
            if (!string.IsNullOrEmpty(line.V5))
            {
                query = query.Where(p => p.V5 == line.V5);
            }
            return query;
        }
        #endregion
    }
}
