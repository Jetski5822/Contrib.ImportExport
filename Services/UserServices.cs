using System.Collections.Concurrent;
using Orchard;
using Orchard.Security;
using Orchard.Users.Models;

namespace Contrib.ExternalImportExport.Services {
    public interface IUserServices : IDependency {
        IUser GetUser(string userName, bool forceCreate);
        IUser GetUser(string userName, string emailAddress, bool forceCreate);
    }

    public class UserServices : IUserServices {
        private readonly IMembershipService _membershipService;

        private static readonly ConcurrentDictionary<string, IUser> Users = new ConcurrentDictionary<string, IUser>();

        public UserServices(IMembershipService membershipService) {
            _membershipService = membershipService;
        }

        public IUser GetUser(string userName, string emailAddress, bool forceCreate) {
            IUser owner = null;
            if (Users.TryGetValue(userName, out owner))
                return owner;

            owner = _membershipService.GetUser(userName);
            if (owner == null && forceCreate) {
                owner = _membershipService.CreateUser(new CreateUserParams(userName, "Password", emailAddress, string.Empty, string.Empty, true));
            } 
            else if (owner != null && (owner.Email == "dummy@wordpressimport.com" || owner.Email == "foo@bar.com ")) {
                var userPart = owner as UserPart;
                if (userPart != null) {
                    userPart.Email = emailAddress;
                }
                owner = userPart;
            }

            Users.AddOrUpdate(userName, owner, (s, user) => owner);

            return owner;
        }

        public IUser GetUser(string userName, bool forceCreate) {
            return GetUser(userName, "dummy@wordpressimport.com", forceCreate);
        }
    }
}