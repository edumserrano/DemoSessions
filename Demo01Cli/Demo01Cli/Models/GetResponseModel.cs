using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demo01Cli.Models
{

    public class GetResponseModel
    {
        public int id { get; set; }
        public string node_id { get; set; }
        public string name { get; set; }
        public int size_in_bytes { get; set; }
        public string url { get; set; }
        public string archive_download_url { get; set; }
        public bool expired { get; set; }
        public DateTime created_at { get; set; }
        public DateTime expires_at { get; set; }
        public DateTime updated_at { get; set; }
        public Workflow_Run2 workflow_run { get; set; }
    }

    public class Workflow_Run2
    {
        public int id { get; set; }
        public int repository_id { get; set; }
        public int head_repository_id { get; set; }
        public string head_branch { get; set; }
        public string head_sha { get; set; }
    }

}
