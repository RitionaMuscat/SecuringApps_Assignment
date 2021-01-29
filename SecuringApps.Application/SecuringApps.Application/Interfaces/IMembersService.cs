using SecuringApps.Application.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace SecuringApps.Application.Interfaces
{
   public interface IMembersService
    {
        void AddMember(MemberViewModel m);
    }
}
