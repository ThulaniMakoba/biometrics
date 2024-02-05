using System.DirectoryServices;

namespace biometricService.Services
{
    public class LdapService
    {
        private readonly string _ldapPath;
        public LdapService(string ldapPath)
        {
            _ldapPath = ldapPath;
        }

        public bool IsUsernameInActiveDirectory(string username)
        {
            try
            {
                using (var entry = new DirectoryEntry(_ldapPath))
                {
                    using (var searcher = new DirectorySearcher(entry))
                    {
                        searcher.Filter = $"(&(objectClass=user)(sAMAccountName={username}))";
                        searcher.PropertiesToLoad.Add("displayName");

                        SearchResult result = searcher.FindOne();
                        return result != null;
                    }
                }
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
