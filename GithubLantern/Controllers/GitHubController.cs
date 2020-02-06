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


        public string Index()
        {

            string GitHubLanternOath = @"

            In brightest day, in blackest night,
            No strange merging shall escape my sight.
            Let those who does pull request with no right
            Beware my power--Github Lantern's light!";

            return GitHubLanternOath;
        }

        [GitHubWebHook]
        public IActionResult GitHubHandler (string id, string @event, JObject data) {
            if (!ModelState.IsValid) {
                return BadRequest (ModelState);
            }

            var result = JObject.FromObject (data).ToObject<Dictionary<string, object>> ();

            if (@event.Trim().Equals("repository")) {
                // A repository was created

                if (result["action"].ToString().Equals("created"))
                {
                    ExecuteProtection(result);
                }


            }
            else if (@event.Trim().Equals("create"))
            {
                // When the master branch was created 

                if (result["ref"].ToString().Equals("master") 
                        &&  result["ref_type"].ToString().Equals("branch"))
                {
                    ExecuteProtection(result);
                }

            }

            return Ok();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="orgNm">Organization Name</param>
        /// <param name="id">Repository ID</param>
        private async void ExecuteProtection(Dictionary<string, object> result)
        {

            // get repository name
            string orgNm = (JObject.FromObject(result["organization"]).ToObject<Dictionary<string, object>>())["login"].ToString();

            // get repository ID
            long id = long.Parse((JObject.FromObject (result["repository"]).ToObject<Dictionary<string, object>>())["id"].ToString());

            // Login
            Credentials cre = new Credentials(_setting.Value.Email, _setting.Value.Pwd);
            GitHubClient ghc = new GitHubClient(new ProductHeaderValue(orgNm));
            ghc.Credentials = cre;

            // Check for branches amount
            var repoBranches = await ghc.Repository.Branch.GetAll(id);
            if (repoBranches.Count > 0) // Check if there is at least a branch
            {
                // get the master branch
                var master = repoBranches.Where(br => br.Name.Equals("master")).FirstOrDefault();
                
                if (!master.Protected)
                {
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
}