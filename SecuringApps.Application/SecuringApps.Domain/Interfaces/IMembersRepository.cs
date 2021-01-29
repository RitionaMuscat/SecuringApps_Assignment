using SecuringApps.Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace SecuringApps.Domain.Interfaces
{
    public interface IMembersRepository
    {
      public  void AddMember(Member m);
    }
}
