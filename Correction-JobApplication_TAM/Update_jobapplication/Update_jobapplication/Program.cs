using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;

namespace Update_jobapplication
{
    class Program
    {
        static void Main(string[] args)
        {
            var token = "eyJhbGciOiJQUzUxMiIsInR5cCI6ImF0K2p3dCJ9.eyJuYmYiOjE2MTczNjYyMTMsImV4cCI6MTYxNzM2OTgxMywiaXNzIjoiaHR0cDovL2ludC5zc28ubXl0YWxlbnRwbHVnLmNvbSIsImNsaWVudF9pZCI6ImpvYi1sZWdhY3kiLCJzdWIiOiJtb2hhbWVkIiwiYXV0aF90aW1lIjoxNjE3MzY2MjEzLCJpZHAiOiJsb2NhbCIsImlkIjoiNTU3MiIsIm5pY2tuYW1lIjoibW9oYW1lZCIsIm5hbWUiOiJNb2hhbWVkIFJhc3NhYSIsImVtYWlsIjoibW9oYW1lZC5yYXNzYWFAdGFsZW50cGx1Zy5jb20iLCJDb21wYW55SWQiOiI2NCIsImdpdmVuX25hbWUiOiJNb2hhbWVkIiwiZmFtaWx5X25hbWUiOiJSYXNzYWEiLCJodHRwOi8vc2NoZW1hcy5taWNyb3NvZnQuY29tL3dzLzIwMDgvMDYvaWRlbnRpdHkvY2xhaW1zL3JvbGUiOlsiQ2xpZW50QWRtaW4iLCJUYW0iXSwianRpIjoiQjA1NzIzRUMxODBFQUMxRjY3RjdBNzU2RjU3RjQzMUYiLCJpYXQiOjE2MTczNjYyMTMsInNjb3BlIjpbIkNsaWVudEFwaSJdLCJhbXIiOlsicGFzc3dvcmQiXX0.OVM_voe3O7nBCGnVThBUmdq448EZGGfqAoTXmJvW1ugMgh12U049gX4fESpcsmsXoFx8cRAM7vl07XOL-y7IrJtQkHUiaGFUTMQzmzX1Ww2tBMHwesxN7irWFTGKcZNLlzuSmX6-oqgpiJRA5JE3Dlyd_cyoU2O0BBGs57BXCxxkomzPDwZJ1bl04lYPyV2YMXyjQw1s-5XAhtQ6ZxUXK0E88jrFxBUfD5ZkgQZjGYoyNAnnaufhhAI1e2SpscJzTf9UDAxZctaKEOgLGQeTw2-zWSxlTtj57yl4K3Idn6rfwza5VTFO-HicoaPDtxlDhTDuThAJJoqNz4eJpqLzhrZNeVGwCT9Ho-YvIol5YVMrcaeqMylmADGcz082IuNjAlrm-0AWsobwYG9Dc5MjM1KqR8a_-OEfSM2vpvFtTFCdyQc5DeUnVx7QB9R4kwP-6QuISHWILhA-ypYk7KfTbpATpQeQXD6MCfDD8q0LlFQ0CSmxwPI3qCGJPrICKRSsYyv_2oideWgstHYTyPwnph3gcP7okGF5ZP8W8Tt6aBm00MJ1P_7XZ1PiH6qqKblWypzuT-6vTfnNEjtiThec06pFT9oCFT1QzB3tl09napqbRhbvFhQdDoCvcMsKHmcGYtjwjkzuohookWbght-dZ895Dmmy6PsQAoQ4u_9H1-A";
            string urlGetAll = "https://int.jam.mytalentplug.com/api/jobapplication/getall";
            var jobApplications = GetAllJobApplications(urlGetAll, token);
            // Update Job application status 
            UpdateJobAplicationStatus(jobApplications.jobApplications, token);

            // Update job applicant Full name 
            //UpdateJobAplicatantName(jobApplications.jobApplications, token);
        }

        protected static JobApplications GetAllJobApplications(string url, string token)
        {
            HttpWebRequest webRequest = Client.CreateWebRequest(url, "GET", token);


            IAsyncResult asyncResult = webRequest.BeginGetResponse(null, null);

            asyncResult.AsyncWaitHandle.WaitOne();

            var jobApplications = new JobApplications();

            using (HttpWebResponse response = (HttpWebResponse)webRequest.EndGetResponse(asyncResult))
            {
                using (StreamReader rd = new StreamReader(response.GetResponseStream()))
                {
                    var results = JsonConvert.DeserializeObject(rd.ReadToEnd()).ToString();
                    jobApplications = JsonConvert.DeserializeObject<JobApplications>(results);


                }
            }
            return jobApplications;
        }
        protected static void UpdateJobAplicationStatus(List<JobApplication> jobAplications, string token)
        {
            var blackListStatus = new List<string>(){ "3","4" };
            var jobApplicationWithStepNotNull = jobAplications.Where(j => j.step != null && !blackListStatus.Contains(j.Status)).ToList();
            foreach (var item in jobApplicationWithStepNotNull)
            {

                var url = string.Format("https://int.jam.mytalentplug.com/api/recruitmentprocess/movetonextrecruitmentstep/{0}", item.id);
                Client.PutJobApplication(url, token, JsonConvert.SerializeObject(item.step).ToString());
            }


        }
        protected static void UpdateJobAplicatantName(List<JobApplication> jobAplication, string token)
        {
            foreach (var item in jobAplication)
            {
                var url = "https://int.jam.mytalentplug.com/api/jobapplication/modifyapplicantfullname";
                var applicantFullnameInformation = new ApplicantFullnameInformation()
                {
                    JobApplicationId = item.id,
                    ApplicantFullname = item.applicant.fullname

                };
                Client.PutJobApplication(url, token, JsonConvert.SerializeObject(applicantFullnameInformation).ToString());
            }

        }
    }
    public class JobApplication
    {

        [JsonProperty("id")]
        public string id { get; set; }
        [JsonProperty("applicant")]
        public applicant applicant { get; set; }
        [JsonProperty("step")]
        public step step { get; set; }
        [JsonProperty("Status")]
        public string Status { get; set; }
    }
    public class JobApplications
    {
        [JsonProperty("jobApplications")]
        public List<JobApplication> jobApplications { get; set; }

    }
    public class step
    {
        public string id { get; set; }
        public string Name { get; set; }
        public string Label { get; set; }
    }
    public class applicant
    {
        public string fullname { get; set; }
        public string email { get; set; }
    }
    public class ApplicantFullnameInformation
    {
        public string JobApplicationId { get; set; }
        public string ApplicantFullname { get; set; }
    }
}

