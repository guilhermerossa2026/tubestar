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
                        switch (studyTask.SkillModifierType)
                        {
                            case (SkillModifierType.Shooting):
                                Player.Current.ShootingSkill += studyTask.SkillModifier;
                                break;

                            case (SkillModifierType.PostProduction):
                                Player.Current.PostProductionSkill += studyTask.SkillModifier;
                                break;

                            case (SkillModifierType.VideoAttribute):
                                Player.Current.VideoAttributePoints += studyTask.SkillModifier;
                                break;

                            case (SkillModifierType.ViewQuality):
                                Player.Current.CanViewQualityBeforeUpload = true;
                                break;
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

            Player.Current.Money -= 50; //Living Expenses
            Player.Current.CostOfLivingExtra = Math.Min(150, Player.Current.CostOfLivingExtra);
            Player.Current.Money -= Math.Max(0, Player.Current.CostOfLivingExtra);

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

                    // Apply to company balance
                    company.Balance += (totalRevenue - totalCOGS - dailyCosts);

                    // Store metrics for yesterday
                    company.YesterdayRevenue = totalRevenue;
                    company.YesterdayCosts = totalCOGS + dailyCosts;
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
                        perfChange = -5.0;
                    }
                    else if (effort == "Máximo")
                    {
                        salary = jobDef.BaseSalary * 1.3 * Player.Current.SalaryBonusMultiplier;
                        hours = jobDef.BaseHours + 2;
                        perfChange = 6.0;
                    }
                    else
                    {
                        salary = jobDef.BaseSalary * 1.0 * Player.Current.SalaryBonusMultiplier;
                        hours = jobDef.BaseHours;
                        perfChange = 1.0;
                    }

                    Player.Current.JobPerformance = Math.Max(0.0, Math.Min(100.0, Player.Current.JobPerformance + perfChange));
                    Player.Current.Money += salary;

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
            CleanTodoList();
            Update();

            RunIterations();
        }

        private void RunIterations()
        {
            BackgroundWorker worker = new BackgroundWorker();
            worker.DoWork += (s, ea) =>
            {
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

                        //Channel stats
                        channel.Income += dailyIncome;
                        channel.SubscribersOverTime.Add(channel.Subscribers);
                        channel.IncomeOverTime.Add(dailyIncome);
                        channel.ExpensesOverTime.Add(dailyExpenses);

                        //Player costs
                        Player.Current.Money += (dailyIncome - dailyExpenses);
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
            };
            worker.RunWorkerCompleted += (s, ea) =>
            {
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