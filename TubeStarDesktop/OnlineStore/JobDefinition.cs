using System;

namespace TubeStar
{
    [Serializable]
    public class JobDefinition
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public double BaseSalary { get; set; }
        public int BaseHours { get; set; }
        
        public int RequiredShooting { get; set; }
        public int RequiredPost { get; set; }
        public string RequiredStudyName { get; set; }
        public string Description { get; set; }

        public JobDefinition()
        {
        }

        public JobDefinition(string id, string name, double baseSalary, int baseHours, int reqShooting, int reqPost, string reqStudy, string desc)
        {
            Id = id;
            Name = name;
            BaseSalary = baseSalary;
            BaseHours = baseHours;
            RequiredShooting = reqShooting;
            RequiredPost = reqPost;
            RequiredStudyName = reqStudy;
            Description = desc;
        }
    }
}
