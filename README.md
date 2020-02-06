# GithubLantern
## Protecting the Master Branches in your Github Organization.

Protecting the master branch of a repository in your organization so that no irrevocable changes are made onto the master branch. Whenever a master branch is created on a repository, that master branch is vulnerable to these dangers. Therefore, I created a web service to automate the protection of the master branch whenever it is created on a repository of your organization. Then, notify a specific member in an issue with the corresponding repository.

## Supported Platform
.NET Core 2.1 (WindowsOS/MacOS/Linux). Make sure you install this before running.


## Getting Started
**First, you need to make a few modifications in the `GithubLatern\appsettings.json` file:**
Under `Webhooks`
*  `Email` is the email that has admin right for this organization. Ex: `admin@github.com`
*  `Pwd` is the password of the account above
*  `MentionAccount` is the account in the same organization which will be used for mentioning the protection completion. Ex: user123
*  `SecretKey`->`default` is the Secret key that will be used in the WebHook setting on GitHub. You can generate a secret key using this [free tool](https://www.freeformatter.com/hmac-generator.html)

**Running the program**
*  Open a Command-Prompt/Powershell on WindowsOS, or a Terminal on MacOs/Linux.
*  Change the directory to the folder where the Startup.cs file is included.
*  Run the following command: `dotnet run`
*  You can open the web service on the browser: http://localhost:5000/

**Using ngrok to host your web service**
*  Register a free account on ngrok and download the program [Here](https://ngrok.com/)
*  The installation is pretty straight forward.
*  Execute the following command: `ngrok.exe http localhost:5000`
*  Copy the link shows on the ngrok terminal. (Should be something like: http://ef948894.ngrok.io)

**Setting on your GitHub Organization page**
*  Navigate to Settings > Webhooks and click on **Add webhook**
*  On the Payload URL - simply put [the url from ngrok]/api/webhooks/incoming/github (e.g:http://ef948894.ngrok.io/api/webhooks/incoming/github)
*  Select Content type - application/json
*  Secret - The secret key that you set on `appsettings.json`
*  Events: check on the following options
1.   Branch or tag creation 
1.   Repositories 
*  Create the webhook and try out the deliveries.
For more information on the setup part, please refer to [WebHooks in ASP .NET Core](https://dotnetthoughts.net/webhooks-in-aspnet-core/)

You are ready to go!


## Reference
For accessing GitHub API with .NET or .NET Core: [Octokit for .NET](https://github.com/octokit/octokit.net)
