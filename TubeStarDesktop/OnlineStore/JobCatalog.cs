using System;
using System.Collections.Generic;
using System.Linq;

namespace TubeStar
{
    public static class JobCatalog
    {
        private static List<JobDefinition> _jobs;

        public static List<JobDefinition> Jobs
        {
            get
            {
                if (_jobs == null)
                {
                    _jobs = new List<JobDefinition>
                    {
                        new JobDefinition(
                            "panfletos",
                            "Distribuidor de Panfletos",
                            30.0,
                            2,
                            0,
                            0,
                            null,
                            "Distribua panfletos na rua debaixo de sol. Trabalho simples, mas cansativo."
                        ),
                        new JobDefinition(
                            "lanchonete",
                            "Atendente de Lanchonete",
                            60.0,
                            4,
                            1,
                            1,
                            null,
                            "Atenda clientes e limpe mesas em um fast-food local."
                        ),
                        new JobDefinition(
                            "editor_freelance",
                            "Editor de Vídeo Freelancer",
                            120.0,
                            4,
                            0,
                            5,
                            null,
                            "Edite vídeos simples para canais menores na internet."
                        ),
                        new JobDefinition(
                            "suporte_ti",
                            "Suporte de TI Júnior",
                            200.0,
                            6,
                            2,
                            2,
                            "ProductionI",
                            "Ajude pessoas a reiniciarem roteadores e configurar redes básicas. Requer estudo de Gravação I."
                        ),
                        new JobDefinition(
                            "dev_junior",
                            "Desenvolvedor Júnior",
                            350.0,
                            6,
                            3,
                            3,
                            "PostProductionI",
                            "Escreva linhas de código e participe de reuniões diárias. Requer estudo de Edição I."
                        ),
                        new JobDefinition(
                            "gerente_ti",
                            "Gerente de TI",
                            600.0,
                            8,
                            5,
                            5,
                            "AudienceAnalysisI",
                            "Lidere equipes de tecnologia e faça a gestão de projetos corporativos. Requer estudo de Análise de Público I."
                        )
                    };
                }
                return _jobs;
            }
        }

        public static JobDefinition GetJobById(string id)
        {
            if (string.IsNullOrEmpty(id)) return null;
            return Jobs.FirstOrDefault(j => j.Id == id);
        }

        public static bool CheckPrerequisites(JobDefinition job, out string missingReason)
        {
            missingReason = "";
            if (job == null) return true;

            // Check skills
            if (Player.Current.ShootingSkill < job.RequiredShooting)
            {
                missingReason = string.Format("Requer habilidade de Gravação nível {0} (Atual: {1}).", job.RequiredShooting, Player.Current.ShootingSkill);
                return false;
            }

            if (Player.Current.PostProductionSkill < job.RequiredPost)
            {
                missingReason = string.Format("Requer habilidade de Edição nível {0} (Atual: {1}).", job.RequiredPost, Player.Current.PostProductionSkill);
                return false;
            }

            // Check studies
            if (!string.IsNullOrEmpty(job.RequiredStudyName))
            {
                bool studyCompleted = false;
                string studyFriendlyName = job.RequiredStudyName;

                if (job.RequiredStudyName == "ProductionI")
                {
                    studyCompleted = Studies.Current.ProductionI.IsCompleted;
                    studyFriendlyName = "Gravação I";
                }
                else if (job.RequiredStudyName == "PostProductionI")
                {
                    studyCompleted = Studies.Current.PostProductionI.IsCompleted;
                    studyFriendlyName = "Edição I";
                }
                else if (job.RequiredStudyName == "AudienceAnalysisI")
                {
                    studyCompleted = Studies.Current.AudienceAnalysisI.IsCompleted;
                    studyFriendlyName = "Análise de Público I";
                }

                if (!studyCompleted)
                {
                    missingReason = string.Format("Requer a conclusão do estudo: '{0}'.", studyFriendlyName);
                    return false;
                }
            }

            return true;
        }
    }
}
