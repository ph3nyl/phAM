using ACE.Database.Models.Auth;
using ACE.Server.Network;

namespace phAM
{
    public static class Discord
    {
        public static void CommandHandler(Session session, params string[] parameters)
        {
            if ((parameters?[0].ToLower() == "bind" || parameters?[0].ToLower() == "b") && parameters?[1].ToLower() != null)
            {
                Bind(session, parameters[1]);
            }
            else if (parameters?[0].ToLower() == "unbind" || parameters?[0].ToLower() == "u" || parameters?[0].ToLower() == "un" || parameters?[0].ToLower() == "unb")
            {
                Bind(session, "");
            }
        }

        private static bool Bind(Session session, string id)
        {
            using (var context = new AuthDbContext())
            {
                var account = context.Account
                    .First(r => r.AccountId == session.AccountId);

                if (account == null)
                    return false;

                //account.AccessLevel = (uint)accessLevel;
                account.EmailAddress = id;

                context.SaveChanges();
            }

            string message = $"You have successfully bound your account to a Discord ID. Hope it was the right one!";
            if (id == "") message = $"You have successfully unbound your account from a Discord ID. Cool I guess...";
            session.Player.SendMessage(message);

            return true;
        }
    }
}
