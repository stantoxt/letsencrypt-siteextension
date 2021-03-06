﻿using LetsEncrypt.Azure.Core;
using LetsEncrypt.Azure.Core.Models;
using LetsEncrypt.SiteExtension.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;

namespace LetsEncrypt.SiteExtension.Controllers.Api
{
    [ValidateApiVersion]
    public class CertificateController : ApiController
    {
        [HttpPost]
        [Route("api/certificates/renew")]    
        [ResponseType(typeof(List<CertificateInstallModel>))]    
        public async Task<IHttpActionResult> RenewExisting([FromUri(Name = "api-version")]string apiversion = null)
        {            
            Trace.TraceInformation("Renew certificate");
            var config = new AppSettingsAuthConfig();
            var res = await new CertificateManager(new AppSettingsAuthConfig()).RenewCertificate(renewXNumberOfDaysBeforeExpiration: config.RenewXNumberOfDaysBeforeExpiration);
            Trace.TraceInformation($"Completed renewal of '{res.Count()}' certificates");

            return Ok(res);
        }

        [HttpPost]
        [Route("api/certificates/provider/kudu/install")]
        [ResponseType(typeof(CertificateInstallModel))]
        public async Task<IHttpActionResult> GenerateAndInstall(GenerateAndInstallModel model, [FromUri(Name = "api-version")]string apiversion = null)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var mgr = new CertificateManager(model.AzureEnvironment, model.AcmeConfig, model.CertificateSettings, model.AuthorizationChallengeProviderConfig);

            return Ok(await mgr.AddCertificate());
        }
    }
}