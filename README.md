# GithubLantern - Protecting the Master Branches in your Github Organization.

This is an ASP .NET Core web service that listen to the Webhook service of GitHub on an organization's event. Whenever a repository's master branch is created then the web service would turn on the master branch protection. Afer the update is completed, then the web service would post an issue on the same repository to notify a specific user in the organization.

For the listening part, I used the GitHub's WebHook service and Microsoft.AspNetCore.WebHooks.Receivers.GitHub library.
For the setting up master branch protection and mentioning through raising an issue on the repository, I used Octokit for .NET.

## Supported Platform
.NET Core 2.1 (WindowsOS/MacOS/Linux)

## Getting Started
First, you need to make a few modification on the appsettings.json file:

`Email` is the email that has admin right for this organization

`Pwd` is the password of the account above

`MetionAccount` is the account in the same organization which will be used for mentioning the protection completion.

`SecretKey`->`default` is the Secret key that will be used in the WebHook setting on GitHub.


Note that you will be using ngrok to public your localhost web service so you can link it to the GitHub WebHook's URL.

For more information, please refer to the *WebHooks in ASP .NET Core* in the *Reference* section

## Execution
Please use VSCode to run this project.

## Reference
Octokit for .NET

https://github.com/octokit/octokit.net

WebHooks in ASP .NET Core

https://dotnetthoughts.net/webhooks-in-aspnet-core/

