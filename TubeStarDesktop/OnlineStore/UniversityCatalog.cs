using System;
using System.Collections.Generic;
using System.Linq;

namespace TubeStar
{
    public static class UniversityCatalog
    {
        private static List<UniversityDefinition> _universities;

        public static List<UniversityDefinition> Universities
        {
            get
            {
                if (_universities == null)
                {
                    _universities = new List<UniversityDefinition>
                    {
                        new UniversityDefinition(
                            "ead_pop",
                            "EAD Cruzeiro do Sul (Pop)",
                            5000.0,
                            15.0,
                            0.0,
                            1.0,
                            "Curso de entrada básico e econômico. Matrícula de $5.000, mensalidade de $15/dia. Permite estudar apenas até nível II (Gravação II, Edição II, Análise de Público I). Não oferece acesso a análises avançadas.",
                            new List<string> {
                                "StudyProductionI", "StudyProductionII",
                                "StudyPostProductionI", "StudyPostProductionII",
                                "StudyAudienceAnalysisI"
                            }
                        ),
                        new UniversityDefinition(
                            "puc_rio",
                            "PUC-Rio (Privada de Prestígio)",
                            10000.0,
                            75.0,
                            0.20, // 20% faster
                            1.0,
                            "Excelente ensino presencial. Matrícula de $10.000, mensalidade de $75/dia. Cursos concluídos 20% mais rápido. Dá acesso a todos os estudos disponíveis, incluindo nível III e Análise de Qualidade.",
                            null // null means all studies allowed
                        ),
                        new UniversityDefinition(
                            "mit_insper",
                            "MIT / Insper (Elite Tecnológica)",
                            20000.0,
                            250.0,
                            0.40, // 40% faster
                            2.0, // 2x skill gain!
                            "A universidade mais renomada do mundo. Matrícula de $20.000, mensalidade de $250/dia. Cursos concluídos 40% mais rápido e com o dobro de pontos de habilidade ganhos! Libera automaticamente o detalhamento dos scores dos seus vídeos após se formar em Análise de Qualidade.",
                            null // null means all studies allowed
                        ),
                        new UniversityDefinition(
                            "faculdade_ia",
                            "Faculdade de Inteligência Artificial",
                            12000.0,
                            90.0,
                            0.10, // 10% faster
                            1.0,
                            "Matrícula de $12.000, mensalidade de $90/dia. Cursos concluídos 10% mais rápido. Formar-se em qualquer curso confere +5% de CTR permanente (Automação de Títulos e SEO).",
                            null // null means all studies allowed
                        )
                    };
                }
                return _universities;
            }
        }

        public static UniversityDefinition GetUniversityById(string id)
        {
            if (string.IsNullOrEmpty(id)) return null;
            return Universities.FirstOrDefault(u => u.Id == id);
        }
    }
}
