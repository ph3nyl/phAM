# phAM-bot

## description:
saves admin-provided credentials for an ace_auth sql server on a per-guild (discord server) basis  
can optionally restrict the maximum number of accounts that can be bound to a discord id  
allows for creation of new accounts that are automatically bound to the calling user  
allows displaying of all accounts that are bound to the calling user  
allows changing of passwords for any account that is bound to the calling user  

**todo**: *display how many characters are on each account?*

## settings:
- token
	- discord app bot token, from discord developer portal. you'll have to make one if you want to host this on your own.
- prefix
	- honestly a holdover from the project template i was using to get this up and running. i dont think this is used at all.
- debug
	- true/false to enable some debug printing in the console

## commands:
*note: bot responds to commands in ephemeral embedded messages so no other users should see anything*
- /help
	- displays frequent problems/solutions file
	
- /admin-setup \<db_addr> \<username> \<password> [db_port] [auth_db] [passwordhashworkfactor] [max_accts]
	- \<db_addr> ip of sql server
	- \<username>/\<password> to login to sql server. needs SELECT/INSERT/UPDATE on ace_auth or equivalent accounts db
	- [db_port] if different from ace default '3306'
	- [auth_db] if different from ace default 'ace_auth'
	- [passwordhashworkfactor] if different from ace default '8'
	- [max_accts] wont allow creation after this many accts. default '0' for infinite
	
- /new-acct \<username> \<password>
	- desired username/password combination for new account.
	- will fail if username is taken or if more than max_accts bound to calling user

- /fix-acct \<username> \<password>
	- updates password for specified account
	- will fail if account if unbound or bound to a user other than calling user

- /log-acct
	- displays all accounts bound to calling user

## special thanks:
- **hells**. you helped me make the unpleasant decision to start over and make a bot back when this project was a shitty website
