using SecuringApps.Application.Interfaces;
using SecuringApps.Application.ViewModels;
using SecuringApps.Domain.Interfaces;
using SecuringApps.Domain.Models;

namespace SecuringApps.Application.Services
{
    public class MemberService : IMembersService
    {
        IMembersRepository _membersRepo;
        public MemberService(IMembersRepository repo)
        {

            _membersRepo = repo;
        }
        public void AddMember(MemberViewModel m)
        {
            Member member = new Member()
            {
                Email = m.Email,
                FirstName = m.FirstName,
                LastName = m.LastName

            };
            _membersRepo.AddMember(member);
        }
    }
}
