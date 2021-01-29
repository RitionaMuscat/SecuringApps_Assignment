using SecuringApps.Data.Context;
using SecuringApps.Domain.Interfaces;
using SecuringApps.Domain.Models;

using System;
using System.Collections.Generic;
using System.Text;

namespace SecuringApps.Data
{
    public class MembersRepository : IMembersRepository
    {
        SecuringAppsDBContext _context;
        public MembersRepository(SecuringAppsDBContext context)
        { _context = context; }
        public void AddMember(Member m)
        {
            _context.Members.Add(m);
            _context.SaveChanges();
        }
    }
}
