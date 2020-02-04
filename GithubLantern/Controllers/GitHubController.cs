using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebHooks;
using Newtonsoft.Json.Linq;
using Octokit;
using System.Linq;
using GithubLantern.Models;
using Microsoft.Extensions.Options;

namespace GithubLantern.Controllers {
    public class GitHubController : ControllerBase {

        private readonly IOptions<WebHooks> _setting;
        

        public GitHubController(IOptions<WebHooks> setting)
                              
        {
            _setting = setting;
        }

        [GitHubWebHook]
        public IActionResult GitHubHandler (string id, string @event, JObject data) {
            if (!ModelState.IsValid) {
                return BadRequest (ModelState);
            }

            if (@event.Trim().Equals("create"))
            {

                // Intepreting the values acquiring from data and put into a dictionary type
                var result = JObject.FromObject (data).ToObject<Dictionary<string, object>>();

                if (result["ref"].ToString().Equals("master") 
                        &&  result["ref_type"].ToString().Equals("branch"))
                {
                    // get repository name
                    string orgNm = (JObject.FromObject(result["organization"]).ToObject<Dictionary<string, object>>())["login"].ToString();

                    // get repository ID
                    long repoID = long.Parse((JObject.FromObject (result["repository"]).ToObject<Dictionary<string, object>>())["id"].ToString());

                    ExecuteProtection(orgNm, repoID);
                }

            }

            return Ok();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="orgNm">Organization Name</param>
        /// <param name="id">Repository ID</param>
        private async void ExecuteProtection(string orgNm, long id)
        {

            // Login
            Credentials cre = new Credentials(_setting.Value.Email, _setting.Value.Pwd);
            GitHubClient ghc = new GitHubClient(new ProductHeaderValue(orgNm));
            ghc.Credentials = cre;

            // Check for branches amount
            var repoBranches = await ghc.Repository.Branch.GetAll(id);
            if (repoBranches.Count > 0)
            {
                // get the master branch
                var master = repoBranches.Where(br => br.Name.Equals("master")).FirstOrDefault();
                
                // set protection
                BranchProtectionRequiredReviewsUpdate bprru = new BranchProtectionRequiredReviewsUpdate(true, true, 1);
                BranchProtectionSettingsUpdate Setting = new BranchProtectionSettingsUpdate(bprru);
                await ghc.Repository.Branch.UpdateBranchProtection(id, "master", Setting);

                // mentioning oneself
                var ic = ghc.Issue;
                var newIssue = new NewIssue("Master Branch Protection Validation") { Body = "Hi @" + _setting.Value.MentionAccount + " , Please check if the master branch protection is valid!"};
                var issue = await ic.Create(id, newIssue);
            }
        }
    }
}