using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace TubeStar
{
    /// <summary>
    /// Interaction logic for DailyPlanner.xaml
    /// </summary>
    public partial class DailyPlanner : UserControl
    {
        public event Action GameExit;
        public event Action Death;
        public event Action NewDayCompleted;

        public List<Task> Appointments { get; private set; }
        private double _moneyAtStartOfDay;

        public DailyPlanner()
        {
            InitializeComponent();
            Translate();
            Appointments = new List<Task>();
            Dispatcher.BeginInvoke(new Action(NewDay), System.Windows.Threading.DispatcherPriority.Loaded);
        }

        private void Translate()
        {
            txtAddTask.Text = EnglishStrings.AddTask.Translate();
        }

        private void AppointmentBlock_Click(object sender, EventArgs e)
        {
            var appointmentBlock = sender as AppointmentBlock;
            if (appointmentBlock != null && appointmentBlock.Task != null)
            {
                //Special: quit job
                if (appointmentBlock.Task.TaskType == TaskType.Job)
                {
                    CustomMessageBox.ShowDialog(EnglishStrings.QuitJobHeader.Translate(), EnglishStrings.QuitJobText.Translate(), MessagePicture.Work, (result) =>
                    {
                        if (result == true)
                        {
                            Player.Current.CurrentJobId = null;
                            Player.Current.JobPerformance = 50.0;
                            Player.Current.JobEffortLevel = "Normal";
                            Player.Current.SalaryBonusMultiplier = 1.0;
                            Appointments.RemoveAll(a => a.TaskType == TaskType.Job);
                            Update();
                        }
                    });
                }
                else if (appointmentBlock.Task.TaskType == TaskType.BowToRobotRulers)
                {
                    double cost = 10000;
                    if (Player.Current.Money < cost)
                    {
                        CustomMessageBox.ShowDialog(EnglishStrings.RiseUp.Translate(), String.Format(EnglishStrings.RebellionCashRequired.Translate(), cost.ToCurrencyString()), MessagePicture.Robot);
                    }
                    else
                    {
                        int chance =  Player.Current.ChallengeMode ? 50 : 75;
                        CustomMessageBox.ShowDialog(EnglishStrings.RiseUp.Translate(), String.Format(EnglishStrings.RebellionStart.Translate(), chance), MessagePicture.Robot, (result) =>
                        {
                            if (result == true)
                            {
                                if (RandomHelpers.Chance(chance))
                                {
                                    if (!TrophyManager.HasTrophy(Trophy.RebelLeader))
                                        TrophyManager.UnlockTrophy(Trophy.RebelLeader);

                                    CustomMessageBox.ShowDialog(EnglishStrings.Freedom.Translate(), EnglishStrings.RebellionSuccess.Translate(), MessagePicture.Robot);
                                    Player.Current.Money -= 10000;
                                    Player.Current.RobotRulers = false;
                                    Appointments.RemoveAll(a => a.TaskType == TaskType.BowToRobotRulers);
                                    Update();
                                }
                                else
                                {
                                    if (Death != null)
                                        Death();
                                }
                            }
                        });
                    }
                }
                else if (appointmentBlock.Task != null)
                {
                    RemoveAppointment(appointmentBlock.Task, false);
                }
            }
        }

        private void AddTask_Click(object sender, RoutedEventArgs e)
        {
            AddTaskDialog dialog = new AddTaskDialog();
            dialog.TaskClick += (t) =>
            {
                AddTask(t);
            };
            dialog.ShowDialog();
        }

        private void AddTask(Task task)
        {
            foreach (var checkTask in Player.Current.TasksInProgress)
            {
                if (checkTask == task)
                    return;
            }

            Player.Current.TasksInProgress.Add(task);
            Update();
        }

        private void AddAppointment(Task task)
        {
            if (Appointments.Count < 15)
            {
                task.HoursPutIn++;
                Appointments.Add(task);
                Update();
            }
        }

        private void RemoveAppointment(Task task, bool removeTodoTask)
        {
            //Special case
            var shootVideoTask = task as ShootVideo;
            if (shootVideoTask != null)
            {
                foreach (var currentTask in Player.Current.TasksInProgress)
                {
                    var editVideoTask = currentTask as EditVideo;
                    if (editVideoTask != null && editVideoTask.Video == shootVideoTask.Video)
                    {
                        RemoveAppointment(currentTask, true);
                        break;
                    }
                }
            }

            if (removeTodoTask)
            {
                Appointments.RemoveAll(a => a == task);
                Player.Current.TasksInProgress.Remove(task);
            }
            else
            {
                Appointments.Remove(task);
                task.HoursPutIn--;
            }

            Update();
        }

        public void Update()
        {
            //Todo Tasks
            tasksPanel.Children.Clear();
            foreach (var task in Player.Current.TasksInProgress)
            {
                TodoTask appointmentTask = new TodoTask(task);
                appointmentTask.TaskClick += (s, ev) =>
                {
                    if (!task.IsCompleted)
                    {
                        AddAppointment(task);
                    }
                };
                appointmentTask.CancelTaskClick += (s, ev) =>
                {
                    bool isStudy = task is Study;
                    CustomMessageBox.ShowDialog(EnglishStrings.RemoveTask.Translate(), String.Format("{0} {1}", EnglishStrings.AreYouSure.Translate(), isStudy ? String.Format("\n{0}", EnglishStrings.NoRefunds.Translate()) : ""), MessagePicture.Question, (result) =>
                    {
                        if (result == true)
                        {
                            RemoveAppointment(task, true);
                        }
                    });
                };
                tasksPanel.Children.Add(appointmentTask);
                appointmentTask.UpdateText();
            }

            //Appointments
            foreach (AppointmentBlock appointment in appointmentPanel.Children)
            {
                appointment.Task = null;
            }

            for (int i = 0; i < Appointments.Count; i++)
            {
                (appointmentPanel.Children[i] as AppointmentBlock).Task = Appointments[i];
            }
        }

        private void CleanTodoList()
        {
            foreach (Task task in Player.Current.TasksInProgress)
            {
                if (task.IsCompleted)
                {
                    var studyTask = task as Study;
                    if (studyTask != null)
                    {
                        double skillGainBonusMultiplier = 1.0;
                        if (!string.IsNullOrEmpty(Player.Current.EnrolledUniversityId))
                        {
                            var uni = UniversityCatalog.GetUniversityById(Player.Current.EnrolledUniversityId);
                            if (uni != null)
                            {
                                skillGainBonusMultiplier = uni.SkillGainBonusMultiplier;
                            }
                        }

                        switch (studyTask.SkillModifierType)
                        {
                            case (SkillModifierType.Shooting):
                                Player.Current.ShootingSkill += (int)Math.Round(studyTask.SkillModifier * skillGainBonusMultiplier);
                                break;

                            case (SkillModifierType.PostProduction):
                                Player.Current.PostProductionSkill += (int)Math.Round(studyTask.SkillModifier * skillGainBonusMultiplier);
                                break;

                            case (SkillModifierType.VideoAttribute):
                                Player.Current.VideoAttributePoints += (int)Math.Round(studyTask.SkillModifier * skillGainBonusMultiplier);
                                break;

                            case (SkillModifierType.ViewQuality):
                                Player.Current.CanViewQualityBeforeUpload = true;
                                if (!string.IsNullOrEmpty(Player.Current.EnrolledUniversityId) && Player.Current.EnrolledUniversityId == "mit_insper")
                                {
                                    Player.Current.CanViewQualityBreakdown = true;
                                }
                                break;
                        }
                        
                        if (!string.IsNullOrEmpty(Player.Current.EnrolledUniversityId) && Player.Current.EnrolledUniversityId == "faculdade_ia" && !Player.Current.HasAIEnhancedTitles)
                        {
                            Player.Current.HasAIEnhancedTitles = true;
                            CustomMessageBox.ShowDialog("Formatura em IA! 🤖", "Você completou seu estudo na Faculdade de Inteligência Artificial! O bônus permanente de Automação de Títulos e SEO (+5% de CTR) foi ativado para todos os seus vídeos!", MessagePicture.Study);
                        }
                    }

                    var shootTask = task as ShootVideo;
                    if (shootTask != null)
                    {
                        Player.Current.Videos.Add(shootTask.Video);
                    }

                    var editTask = task as EditVideo;
                    if (editTask != null)
                    {
                        editTask.Video.HasBeenEdited = true;
                    }
                }
            }

            Player.Current.TasksInProgress.RemoveAll(t => t.IsCompleted);
        }

        public void NewDay()
        {
            _moneyAtStartOfDay = Player.Current.Money;

            // A1. Automação de Gravação (Gestor de Canal)
            if (Player.Current.TasksInProgress != null && Player.Current.Channels != null)
            {
                foreach (var task in Player.Current.TasksInProgress)
                {
                    var shootTask = task as ShootVideo;
                    if (shootTask != null && shootTask.Video != null && !string.IsNullOrEmpty(shootTask.Video.ChannelId))
                    {
                        var chan = Player.Current.Channels.FirstOrDefault(c => c.Id.ToString() == shootTask.Video.ChannelId);
                        if (chan != null && chan.HiredManager)
                        {
                            shootTask.HoursPutIn = shootTask.HoursToComplete + shootTask.ExtraHours;
                        }
                    }
                }
            }

            // A1.5 Eventos de Negociação de Equipe do Canal
            if (Player.Current.Channels != null)
            {
                foreach (var channel in Player.Current.Channels)
                {
                    if (channel != Channel.UnreleasedVideos && !channel.IsRivalChannel)
                    {
                        if (channel.HiredEditor && RandomHelpers.Chance(5))
                        {
                            string msgText = string.Format(
                                "Seu Editor do canal '{0}' está pedindo um aumento salarial devido ao volume de trabalho!\n\nSalário Base Atual: {1}\nProposta de Aumento: {2} (+15%)\n\nSe recusar, há 30% de chance de ele se demitir. Aceitar o aumento?",
                                channel.Name,
                                channel.EditorCurrentSalary.ToCurrencyString(),
                                (channel.EditorCurrentSalary * 1.15).ToCurrencyString()
                            );
                            CustomMessageBox.ShowDialog(
                                "Pedido de Aumento (Editor)",
                                msgText,
                                MessagePicture.Work,
                                (result) =>
                                {
                                    if (result == true)
                                    {
                                        channel.EditorBaseSalary = Math.Round(channel.EditorBaseSalary * 1.15, 2);
                                        CustomMessageBox.ShowDialog("Aumento Concedido!", "Seu editor agradece e continuará com excelente desempenho.", MessagePicture.Happy);
                                    }
                                    else
                                    {
                                        if (RandomHelpers.Chance(30))
                                        {
                                            channel.HiredEditor = false;
                                            CustomMessageBox.ShowDialog("Demissão!", "O seu Editor de Vídeo pediu demissão do canal " + channel.Name + "!", MessagePicture.Sad);
                                        }
                                        else
                                        {
                                            CustomMessageBox.ShowDialog("Aumento Negado", "O editor aceitou continuar trabalhando com o salário atual, mas ficou descontente.", MessagePicture.Sad);
                                        }
                                    }
                                }
                            );
                        }

                        if (channel.HiredManager && RandomHelpers.Chance(5))
                        {
                            string msgText = string.Format(
                                "Seu Gestor do canal '{0}' está pedindo um aumento salarial para gerenciar as gravações!\n\nSalário Base Atual: {1}\nProposta de Aumento: {2} (+15%)\n\nSe recusar, há 30% de chance de ele se demitir. Aceitar o aumento?",
                                channel.Name,
                                channel.ManagerCurrentSalary.ToCurrencyString(),
                                (channel.ManagerCurrentSalary * 1.15).ToCurrencyString()
                            );
                            CustomMessageBox.ShowDialog(
                                "Pedido de Aumento (Gestor)",
                                msgText,
                                MessagePicture.Work,
                                (result) =>
                                {
                                    if (result == true)
                                    {
                                        channel.ManagerBaseSalary = Math.Round(channel.ManagerBaseSalary * 1.15, 2);
                                        CustomMessageBox.ShowDialog("Aumento Concedido!", "Seu gestor agradece e continuará mantendo a gravação automatizada.", MessagePicture.Happy);
                                    }
                                    else
                                    {
                                        if (RandomHelpers.Chance(30))
                                        {
                                            channel.HiredManager = false;
                                            CustomMessageBox.ShowDialog("Demissão!", "O seu Gestor de Canal pediu demissão do canal " + channel.Name + "!", MessagePicture.Sad);
                                        }
                                        else
                                        {
                                            CustomMessageBox.ShowDialog("Aumento Negado", "O gestor aceitou continuar trabalhando com o salário atual, mas ficou descontente.", MessagePicture.Sad);
                                        }
                                    }
                                }
                            );
                        }
                    }
                }
            }

            // 1. Cobrar Imposto de Renda Pessoa Física (IRPF) com Sonegação e Deduções
            double yesterdayRev = Player.Current.YesterdayRevenue;
            
            // Deduções do Contador se contratado
            double accountantDeduction = 0;
            if (Player.Current.IsAccountantHired)
            {
                accountantDeduction += 30.0; // Depreciação de estúdio
                if (!string.IsNullOrEmpty(Player.Current.EnrolledUniversityId))
                {
                    var uni = UniversityCatalog.GetUniversityById(Player.Current.EnrolledUniversityId);
                    if (uni != null)
                    {
                        accountantDeduction += uni.DailyTuition * 0.5; // 50% da faculdade
                    }
                }
            }

            // Cálculo do imposto REAL (o que deveria pagar)
            double realTaxableRevenue = Math.Max(0, yesterdayRev - accountantDeduction);
            double fullTax = 0;
            if (realTaxableRevenue > 100)
            {
                if (realTaxableRevenue <= 500)
                    fullTax = (realTaxableRevenue - 100) * 0.10;
                else if (realTaxableRevenue <= 2000)
                    fullTax = (500 - 100) * 0.10 + (realTaxableRevenue - 500) * 0.20;
                else
                    fullTax = (500 - 100) * 0.10 + (2000 - 500) * 0.20 + (realTaxableRevenue - 2000) * 0.35;
            }

            // Cálculo do imposto DECLARADO (o que de fato vai pagar)
            double declaredRevenue = yesterdayRev * Player.Current.TaxDeclarationRate;
            double declaredTaxableRevenue = Math.Max(0, declaredRevenue - accountantDeduction);
            double paidTax = 0;
            if (declaredTaxableRevenue > 100)
            {
                if (declaredTaxableRevenue <= 500)
                    paidTax = (declaredTaxableRevenue - 100) * 0.10;
                else if (declaredTaxableRevenue <= 2000)
                    paidTax = (500 - 100) * 0.10 + (declaredTaxableRevenue - 500) * 0.20;
                else
                    paidTax = (500 - 100) * 0.10 + (2000 - 500) * 0.20 + (declaredTaxableRevenue - 2000) * 0.35;
            }

            // Pagar o imposto declarado
            if (paidTax > 0)
            {
                Player.Current.Money -= paidTax;
                CustomMessageBox.ShowDialog(
                    "Impostos Coletados (Receita Federal)",
                    string.Format("O governo recolheu {0} de imposto de renda (IRPF) sobre a receita declarada de {1} (Faturamento total de ontem: {2}).", paidTax.ToCurrencyString(), declaredRevenue.ToCurrencyString(), yesterdayRev.ToCurrencyString()),
                    MessagePicture.Money
                );
            }

            // Salvar para estatísticas
            Player.Current.YesterdayDeclaredRevenue = declaredRevenue;
            Player.Current.YesterdayTaxPaid = paidTax;

            // Acumular o que foi sonegado
            double evadedTaxToday = Math.Max(0, fullTax - paidTax);
            Player.Current.UnpaidEvadedTaxes += evadedTaxToday;

            Player.Current.YesterdayRevenue = 0; // Reset para o novo dia

            // 1.2 Auditoria Fiscal (Malha Fina)
            if (Player.Current.TaxDeclarationRate < 1.0 && Player.Current.UnpaidEvadedTaxes > 0)
            {
                double auditChance = (1.0 - Player.Current.TaxDeclarationRate) * 0.40;
                if (Player.Current.IsAccountantHired)
                {
                    auditChance /= 2.0;
                }

                if (RandomHelpers.Chance((int)Math.Round(auditChance * 100.0)))
                {
                    double unpaid = Player.Current.UnpaidEvadedTaxes;
                    double fineRate = Player.Current.IsTaxAttorneyHired ? 0.5 : 1.5;
                    double fine = unpaid * fineRate;
                    double totalDue = unpaid + fine;

                    if (Player.Current.Money >= totalDue)
                    {
                        Player.Current.Money -= totalDue;
                        Player.Current.UnpaidEvadedTaxes = 0.0;
                        CustomMessageBox.ShowDialog(
                            "🚨 CAIU NA MALHA FINA!",
                            string.Format("A Receita Federal auditou suas contas e detectou {0} em impostos não declarados. Você pagou os impostos pendentes mais uma multa de {1} (Total: {2}).", unpaid.ToCurrencyString(), fine.ToCurrencyString(), totalDue.ToCurrencyString()),
                            MessagePicture.Legal
                        );
                    }
                    else
                    {
                        double diff = totalDue - Player.Current.Money;
                        Player.Current.Money = 0;
                        Player.Current.TaxDebtAmount += diff;
                        Player.Current.UnpaidEvadedTaxes = 0.0;
                        CustomMessageBox.ShowDialog(
                            "🚨 MALHA FINA: DÍVIDA ATIVA!",
                            string.Format("A Receita Federal detectou sonegação fiscal! O valor total cobrado foi de {0}. Seu saldo foi zerado e o restante de {1} foi inscrito na DÍVIDA ATIVA. Regularize-se no Portal do Governo!", totalDue.ToCurrencyString(), diff.ToCurrencyString()),
                            MessagePicture.Axe
                        );
                    }
                }
            }

            // 1.3 Custos de Assessoria Diária
            if (Player.Current.IsAccountantHired)
            {
                Player.Current.Money -= 40.0;
            }
            if (Player.Current.IsTaxAttorneyHired)
            {
                Player.Current.Money -= 80.0;
            }

            // 1.4 Subsídios Ativos
            if (!string.IsNullOrEmpty(Player.Current.ActiveSubsidyId))
            {
                Player.Current.SubsidyDaysLeft--;
                if (Player.Current.ActiveSubsidyId == "edu")
                {
                    Player.Current.Money += 200.0;
                    if (Player.Current.SubsidyDaysLeft <= 0)
                    {
                        Player.Current.ActiveSubsidyId = null;
                        CustomMessageBox.ShowDialog(
                            "Subsídio de Educação Concluído!",
                            "Seu contrato de 30 dias de Subsídio de Educação chegou ao fim. Parabéns por ajudar na divulgação científica!",
                            MessagePicture.Study
                        );
                    }
                }
                else if (Player.Current.ActiveSubsidyId == "rouanet")
                {
                    if (Player.Current.SubsidyDaysLeft <= 0)
                    {
                        Player.Current.ActiveSubsidyId = null;
                        if (Player.Current.SubsidyVideosUploaded >= 3)
                        {
                            CustomMessageBox.ShowDialog(
                                "Fomento Rouanet Concluído!",
                                "Você atingiu a meta de 3 vídeos de alta qualidade dentro do prazo! O subsídio Rouanet foi finalizado com sucesso.",
                                MessagePicture.Study
                            );
                        }
                        else
                        {
                            // Fracassou! Devolver 15k
                            if (Player.Current.Money >= 15000)
                            {
                                Player.Current.Money -= 15000;
                                CustomMessageBox.ShowDialog(
                                    "Rouanet: Meta Não Cumprida!",
                                    "O prazo de 10 dias expirou e você não enviou os 3 vídeos com qualidade > 80. O governo recolheu os $15.000 da sua conta.",
                                    MessagePicture.Axe
                                );
                            }
                            else
                            {
                                double diff = 15000 - Player.Current.Money;
                                Player.Current.Money = 0;
                                Player.Current.TaxDebtAmount += diff;
                                CustomMessageBox.ShowDialog(
                                    "Rouanet: Multa e Nome Sujo!",
                                    string.Format("O prazo expirou sem cumprir a meta de vídeos. Como você não tinha $15.000 para reembolsar, o restante de {0} foi inscrito na Dívida Ativa!", diff.ToCurrencyString()),
                                    MessagePicture.Axe
                                );
                            }
                        }
                    }
                }
            }

            // 2. Mensalidade da Faculdade
            if (!string.IsNullOrEmpty(Player.Current.EnrolledUniversityId))
            {
                var uni = UniversityCatalog.GetUniversityById(Player.Current.EnrolledUniversityId);
                if (uni != null)
                {
                    if (Player.Current.Money >= uni.DailyTuition)
                    {
                        Player.Current.Money -= uni.DailyTuition;
                    }
                    else
                    {
                        Player.Current.EnrolledUniversityId = null;
                        Player.Current.TasksInProgress.RemoveAll(t => t is Study);
                        CustomMessageBox.ShowDialog(
                            "Matrícula Cancelada!",
                            string.Format("Você foi desligado da {0} por inadimplência! Para voltar a estudar lá, terá de pagar uma nova taxa de matrícula de {1}.", uni.Name, uni.EnrollmentFee.ToCurrencyString()),
                            MessagePicture.Money
                        );
                    }
                }
            }

            Player.Current.Money -= 50; //Living Expenses
            Player.Current.CostOfLivingExtra = Math.Min(150, Player.Current.CostOfLivingExtra);
            Player.Current.Money -= Math.Max(0, Player.Current.CostOfLivingExtra);

            // Logarithmic Server Costs
            double serverCost = 0;
            if (Player.Current.Videos != null && Player.Current.Videos.Count > 0)
            {
                long totalViews = Player.Current.Videos.Sum(v => (long)v.Views);
                if (totalViews > 0)
                {
                    serverCost = Math.Round(250.0 * Math.Log(totalViews / 10000.0 + 1.0), 2);
                }
            }

            if (serverCost > 0)
            {
                Player.Current.Money -= serverCost;
                CustomMessageBox.ShowDialog("Hospedagem & Servidores 🌐", string.Format("Seu canal consumiu {0} em infraestrutura de servidores e transferência de dados na nuvem hoje (Visualizações totais: {1}).", serverCost.ToCurrencyString(), Player.Current.Videos.Sum(v => v.Views).ToString("N0")), MessagePicture.Money);
            }

            // Calculate and process Real Estate and Vehicle taxes / rent
            double totalTax = 0;
            double totalRent = 0;

            if (Player.Current.OwnedRealEstate != null)
            {
                foreach (var id in Player.Current.OwnedRealEstate)
                {
                    var prop = AssetCatalog.RealEstate.FirstOrDefault(p => p.Id == id);
                    if (prop != null)
                    {
                        totalTax += prop.DailyTax;
                        totalRent += prop.DailyRent;
                    }
                }
            }

            if (Player.Current.OwnedVehicles != null)
            {
                foreach (var id in Player.Current.OwnedVehicles)
                {
                    var veh = AssetCatalog.Vehicles.FirstOrDefault(v => v.Id == id);
                    if (veh != null)
                    {
                        totalTax += veh.DailyTax;
                    }
                }
            }

            Player.Current.Money -= totalTax;
            Player.Current.Money += totalRent;

            // Process Companies Daily Loop
            if (Player.Current.OwnedCompanies != null)
            {
                foreach (var company in Player.Current.OwnedCompanies)
                {
                    // For backward compatibility: make sure Products is initialized
                    if (company.Products == null || company.Products.Count == 0)
                    {
                        company.Products = new System.Collections.Generic.List<Product>();
                        double stdPrice = 5.0;
                        if (company.Niche == "Alimentos") stdPrice = 5.0;
                        else if (company.Niche == "Merch") stdPrice = 25.0;
                        else if (company.Niche == "Gamer") stdPrice = 120.0;
                        else if (company.Niche == "Brinquedos") stdPrice = 40.0;
                        company.Products.Add(new Product(Guid.NewGuid().ToString(), "Produto Base " + company.Name, stdPrice, 50.0));
                    }

                    // Update market trend (15% chance to rotate)
                    if (RandomHelpers.Chance(15) || string.IsNullOrEmpty(company.MarketTrend))
                    {
                        int trendIdx = RandomHelpers.RandomInt(5);
                        if (trendIdx == 0) company.MarketTrend = "Estável";
                        else if (trendIdx == 1) company.MarketTrend = "Alta em Alimentos (+50%)";
                        else if (trendIdx == 2) company.MarketTrend = "Alta em Merch (+50%)";
                        else if (trendIdx == 3) company.MarketTrend = "Alta em Gamer (+50%)";
                        else company.MarketTrend = "Alta em Brinquedos (+50%)";
                    }

                    double dailyCosts = 0;
                    
                    // Maintenance
                    dailyCosts += company.InfrastructureLevel * 50;

                    // Salaries
                    if (company.HiredCEO) dailyCosts += 500;
                    if (company.HiredMarketingDirector) dailyCosts += 300;
                    if (company.HiredSalesManager) dailyCosts += 300;
                    if (company.HiredRDEngineer) dailyCosts += 250;

                    // Campaign costs
                    if (company.MarketingCampaign == "Baixa") dailyCosts += 100;
                    else if (company.MarketingCampaign == "Média") dailyCosts += 300;
                    else if (company.MarketingCampaign == "Agressiva") dailyCosts += 800;

                    // Debt interest (2% daily)
                    if (company.Balance < 0)
                    {
                        dailyCosts += Math.Abs(company.Balance) * 0.02;
                    }

                    // Marketing brand awareness updates
                    double awarenessDecay = company.HiredMarketingDirector ? 1.5 : 3.0;
                    double campaignGain = 0;
                    if (company.MarketingCampaign == "Baixa") campaignGain = company.HiredMarketingDirector ? 2.0 : 1.5;
                    else if (company.MarketingCampaign == "Média") campaignGain = company.HiredMarketingDirector ? 5.0 : 4.0;
                    else if (company.MarketingCampaign == "Agressiva") campaignGain = company.HiredMarketingDirector ? 11.0 : 9.0;

                    company.BrandAwareness = Math.Max(0.0, Math.Min(100.0, company.BrandAwareness - awarenessDecay + campaignGain));

                    // Process each product
                    double totalRevenue = 0;
                    double totalCOGS = 0;
                    int totalSales = 0;

                    foreach (var product in company.Products)
                    {
                        // R&D engineer reduces novelty decay
                        double noveltyDecay = company.HiredRDEngineer ? 3.0 : 5.0;
                        product.Novelty = Math.Max(0.0, product.Novelty - noveltyDecay);

                        // CEO Automates Version Update
                        if (company.HiredCEO && product.Novelty < 20.0 && company.Balance >= 15000)
                        {
                            company.Balance -= 15000;
                            product.Novelty = 100.0;
                            product.Quality = Math.Min(100.0, product.Quality + 5.0);
                        }

                        // Calculate demand
                        double demandFactor = (company.BrandAwareness * 0.4) + (product.Novelty * 0.3) + (product.Quality * 0.3);

                        // Niche specifications
                        double stdPrice = 5.0;
                        double stdCost = 2.50;
                        double baseDemandSize = 12.0;

                        switch (company.Niche)
                        {
                            case "Alimentos":
                                stdPrice = 5.0;
                                stdCost = 2.50;
                                baseDemandSize = 12.0;
                                break;
                            case "Merch":
                                stdPrice = 25.0;
                                stdCost = 12.00;
                                baseDemandSize = 2.5;
                                break;
                            case "Gamer":
                                stdPrice = 120.0;
                                stdCost = 70.00;
                                baseDemandSize = 0.8;
                                break;
                            case "Brinquedos":
                                stdPrice = 40.0;
                                stdCost = 20.00;
                                baseDemandSize = 1.6;
                                break;
                        }

                        // Price elasticity multiplier
                        double priceMultiplier = 1.0;
                        if (product.Price <= stdPrice)
                        {
                            priceMultiplier = 1.0 + ((stdPrice - product.Price) / stdPrice) * 0.8;
                        }
                        else
                        {
                            priceMultiplier = Math.Max(0.05, 1.0 - ((product.Price - stdPrice) / stdPrice) * 1.5);
                        }

                        // Infrastructure multiplier
                        double infraMultiplier = 1.0 + (company.InfrastructureLevel - 1) * 0.5;

                        // Staff multiplier
                        double staffMultiplier = 1.0;
                        if (company.HiredCEO) staffMultiplier += 0.15;
                        if (company.HiredSalesManager) staffMultiplier += 0.20;

                        // Trend multiplier
                        double trendMultiplier = 1.0;
                        if (!string.IsNullOrEmpty(company.MarketTrend) && company.MarketTrend.Contains(company.Niche))
                        {
                            trendMultiplier = 1.5;
                        }

                        // Calculate quantity sold
                        int salesQuantity = (int)(demandFactor * baseDemandSize * priceMultiplier * infraMultiplier * staffMultiplier * trendMultiplier);
                        salesQuantity = Math.Max(0, salesQuantity);

                        double productRevenue = salesQuantity * product.Price;
                        double productCOGS = salesQuantity * stdCost;

                        product.SalesYesterday = salesQuantity;
                        product.RevenueYesterday = productRevenue - productCOGS;

                        totalRevenue += productRevenue;
                        totalCOGS += productCOGS;
                        totalSales += salesQuantity;
                    }

                    // Apply corporate tax (IRPJ) of 20% on positive net profit
                    double netProfit = totalRevenue - totalCOGS - dailyCosts;
                    double corporateTax = 0;
                    if (netProfit > 0)
                    {
                        corporateTax = netProfit * 0.20;
                    }
                    company.Balance += (netProfit - corporateTax);

                    // Store metrics for yesterday
                    company.YesterdayRevenue = totalRevenue;
                    company.YesterdayCosts = totalCOGS + dailyCosts + corporateTax;
                    company.YesterdaySales = totalSales;
                }
            }

            if(StoreItems.Current.Loan.Purchased)
            {
                Player.Current.LoanPayOff -= StoreItems.Current.Loan.AdditionalCost;
                Player.Current.Money -= StoreItems.Current.Loan.AdditionalCost;

                if (Player.Current.LoanPayOff <= 0)
                {
                    StoreItems.Current.Loan.Purchased = false;
                }
            }

            Appointments = new List<Task>();
            if (Player.Current.RobotRulers)
            {
                var robots = new BowToRobotRulers();
                AddAppointment(robots);
                AddAppointment(robots);
                AddAppointment(robots);
            }
            if (!string.IsNullOrEmpty(Player.Current.CurrentJobId))
            {
                var jobDef = JobCatalog.GetJobById(Player.Current.CurrentJobId);
                if (jobDef != null)
                {
                    double salary = 0;
                    int hours = 0;
                    double perfChange = 0;

                    string effort = Player.Current.JobEffortLevel ?? "Normal";
                    if (effort == "Mínimo")
                    {
                        salary = jobDef.BaseSalary * 0.7 * Player.Current.SalaryBonusMultiplier;
                        hours = Math.Max(1, jobDef.BaseHours - 1);
                        perfChange = -3.0;
                    }
                    else if (effort == "Máximo")
                    {
                        salary = jobDef.BaseSalary * 1.3 * Player.Current.SalaryBonusMultiplier;
                        hours = jobDef.BaseHours + 2;
                        perfChange = 2.0;
                    }
                    else
                    {
                        salary = jobDef.BaseSalary * 1.0 * Player.Current.SalaryBonusMultiplier;
                        hours = jobDef.BaseHours;
                        perfChange = 0.5;
                    }

                    int numPromotions = (int)Math.Round((Player.Current.SalaryBonusMultiplier - 1.0) / 0.15);
                    if (perfChange > 0)
                    {
                        perfChange = perfChange / (1.0 + numPromotions * 0.5);
                    }

                    Player.Current.JobPerformance = Math.Max(0.0, Math.Min(100.0, Player.Current.JobPerformance + perfChange));
                    Player.Current.Money += salary;
                    Player.Current.YesterdayRevenue += salary;

                    var careerJob = new CareerJob(jobDef.Name);
                    for (int i = 0; i < hours; i++)
                    {
                        AddAppointment(careerJob);
                    }

                    if (Player.Current.JobPerformance >= 100.0)
                    {
                        Player.Current.SalaryBonusMultiplier += 0.15;
                        Player.Current.JobPerformance = 50.0;
                        CustomMessageBox.ShowDialog("Promoção Recebida!", string.Format("Seu bom desempenho no cargo de {0} rendeu uma promoção! Seu salário nessa vaga recebeu um bônus permanente de +15%.", jobDef.Name), MessagePicture.Work);
                    }
                    else if (Player.Current.JobPerformance <= 0.0)
                    {
                        Player.Current.CurrentJobId = null;
                        Player.Current.JobPerformance = 50.0;
                        Player.Current.JobEffortLevel = "Normal";
                        Player.Current.SalaryBonusMultiplier = 1.0;
                        CustomMessageBox.ShowDialog("Demissão!", string.Format("Devido ao seu baixo desempenho recente, você foi demitido do cargo de {0}.", jobDef.Name), MessagePicture.Axe);
                    }
                }
            }
            // A2. Automação de Edição (Editor de Vídeo)
            if (Player.Current.Videos != null && Player.Current.Channels != null)
            {
                foreach (var video in Player.Current.Videos.ToList())
                {
                    if (!video.HasBeenEdited && !string.IsNullOrEmpty(video.ChannelId))
                    {
                        var chan = Player.Current.Channels.FirstOrDefault(c => c.Id.ToString() == video.ChannelId);
                        if (chan != null && chan.HiredEditor)
                        {
                            video.HasBeenEdited = true;
                            // Nível do editor escala a qualidade da edição (Nível 1 = 58-69, Nível 10 = 85-96)
                            video.EditQuality = 55 + (chan.EditorLevel * 3) + RandomHelpers.RandomInt(11);
                            video.GenerateQuality();

                            chan.EditorXP += 10;
                        }
                    }
                }
            }

            CleanTodoList();
            Update();

            RunIterations();
        }

        private void RunIterations()
        {
            BackgroundWorker worker = new BackgroundWorker();
            worker.DoWork += (s, ea) =>
            {
                double totalChannelIncome = 0;
                foreach (var channel in Player.Current.Channels)
                {
                    if (channel != Channel.UnreleasedVideos)
                    {
                        double dailyIncome = 0;
                        double dailyExpenses = 0;
                        foreach (var video in channel.Videos)
                        {
                            VideoViewer.Iteration(channel, video, ref dailyIncome, ref dailyExpenses);
                            if (video.Iterations == 1)
                            {
                                dailyExpenses += video.Cost;
                                dailyExpenses += video.OnceOffCost;
                            }
                        }

                        // Custos Salariais Híbridos do Staff do Canal
                        double staffCost = 0;
                        if (channel.HiredEditor)
                        {
                            staffCost += channel.EditorCurrentSalary + (dailyIncome * 0.05);
                        }
                        if (channel.HiredManager)
                        {
                            staffCost += channel.ManagerCurrentSalary + (dailyIncome * 0.10);
                        }
                        if (staffCost > 0)
                        {
                            dailyExpenses += staffCost;
                        }

                        //Channel stats
                        channel.Income += dailyIncome;
                        channel.SubscribersOverTime.Add(channel.Subscribers);
                        channel.IncomeOverTime.Add(dailyIncome);
                        channel.ExpensesOverTime.Add(dailyExpenses);

                        //Player costs
                        Player.Current.Money += (dailyIncome - dailyExpenses);
                        totalChannelIncome += dailyIncome;
                    }
                }

                double rivalDailyIncome = 0;
                double refDailyExpenses = 0;
                foreach (var rival in Rivals.Current.All)
                {
                    if (rival.Channel != null && rival.Channel.Videos != null)
                    {
                        foreach (var video in rival.Channel.Videos.ToList())
                        {
                            VideoViewer.Iteration(rival.Channel, video, ref rivalDailyIncome, ref refDailyExpenses);
                        }
                    }
                }
                ea.Result = totalChannelIncome;
            };
            worker.RunWorkerCompleted += (s, ea) =>
            {
                if (ea.Error == null && ea.Result is double)
                {
                    Player.Current.YesterdayRevenue += (double)ea.Result;
                }

                if (Player.Current.Money < 0)
                {
                    if (GameExit != null)
                        GameExit();
                }

                if (Player.Current.Money - _moneyAtStartOfDay >= 1000)
                {
                    TrophyManager.UnlockTrophy(Trophy.Bunsen);
                }

                Player.Current.Iterations++;

                if (NewDayCompleted != null)
                    NewDayCompleted();
            };
            worker.RunWorkerAsync();
        }
    }
}