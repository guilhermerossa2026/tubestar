using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

namespace TubeStar
{
    public partial class CareerControl : UserControl
    {
        private bool _isUpdating;

        public CareerControl()
        {
            InitializeComponent();
        }

        public void Update()
        {
            if (_isUpdating) return;
            _isUpdating = true;

            try
            {
                // Personal Money
                txtPersonalMoney.Text = Player.Current.Money.ToCurrencyString();

                // Personal Skills
                if (txtShootingSkill != null)
                    txtShootingSkill.Text = Player.Current.ShootingSkill.ToString();
                if (txtPostProdSkill != null)
                    txtPostProdSkill.Text = Player.Current.PostProductionSkill.ToString();

                var currentJob = JobCatalog.GetJobById(Player.Current.CurrentJobId);
                if (currentJob == null)
                {
                    panelNoJob.Visibility = Visibility.Visible;
                    panelHasJob.Visibility = Visibility.Collapsed;
                }
                else
                {
                    panelNoJob.Visibility = Visibility.Collapsed;
                    panelHasJob.Visibility = Visibility.Visible;

                    // Set job texts
                    txtJobName.Text = currentJob.Name.ToUpper();
                    txtJobDesc.Text = currentJob.Description;

                    double bonusPercent = (Player.Current.SalaryBonusMultiplier - 1.0) * 100.0;
                    double currentSalary = currentJob.BaseSalary * Player.Current.SalaryBonusMultiplier;

                    txtJobSalary.Text = currentSalary.ToCurrencyString() + " / dia";
                    txtJobHours.Text = currentJob.BaseHours + " horas";

                    if (bonusPercent > 0)
                    {
                        gridSalaryBonus.Visibility = Visibility.Visible;
                        txtJobBonus.Text = string.Format("+{0:F0}% de aumento", bonusPercent);
                    }
                    else
                    {
                        gridSalaryBonus.Visibility = Visibility.Collapsed;
                    }

                    // Performance
                    pbPerformance.Value = Player.Current.JobPerformance;
                    txtPerformanceVal.Text = string.Format("{0:F0}%", Player.Current.JobPerformance);

                    // Effort Level buttons state
                    string effort = Player.Current.JobEffortLevel ?? "Normal";
                    btnEffortMin.IsChecked = effort == "Mínimo";
                    btnEffortNorm.IsChecked = effort == "Normal";
                    btnEffortMax.IsChecked = effort == "Máximo";

                    // Update effort description
                    if (effort == "Mínimo")
                    {
                        txtEffortDescription.Text = string.Format(
                            "Dedicação Mínima: Consome apenas {0} horas de trabalho/dia. Salário reduzido para {1} (-30%). Perda diária de -3.0% de desempenho.",
                            Math.Max(1, currentJob.BaseHours - 1),
                            (currentSalary * 0.7).ToCurrencyString()
                        );
                    }
                    else if (effort == "Máximo")
                    {
                        txtEffortDescription.Text = string.Format(
                            "Dedicação Máxima: Consome {0} horas de trabalho/dia. Salário aumentado para {1} (+30%). Ganho diário de +2.0% de desempenho corporativo (reduzido por promoções).",
                            currentJob.BaseHours + 2,
                            (currentSalary * 1.3).ToCurrencyString()
                        );
                    }
                    else
                    {
                        txtEffortDescription.Text = string.Format(
                            "Dedicação Normal: Consome as {0} horas padrão/dia. Salário de {1}. Ganho diário de +0.5% de desempenho corporativo (reduzido por promoções).",
                            currentJob.BaseHours,
                            currentSalary.ToCurrencyString()
                        );
                    }
                }

                // Render Job Catalog
                PopulateJobCatalog();
            }
            finally
            {
                _isUpdating = false;
            }
        }

        private void PopulateJobCatalog()
        {
            panelJobCatalog.Children.Clear();

            foreach (var job in JobCatalog.Jobs)
            {
                // Create card container
                var border = new Border
                {
                    Background = new SolidColorBrush(Color.FromRgb(0x22, 0x22, 0x22)),
                    BorderBrush = new SolidColorBrush(Color.FromRgb(0x2E, 0x2E, 0x2E)),
                    BorderThickness = new Thickness(1),
                    CornerRadius = new CornerRadius(6),
                    Padding = new Thickness(12),
                    Margin = new Thickness(0, 0, 0, 10)
                };

                var mainGrid = new Grid();
                mainGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
                mainGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(140) });

                // Job Details Stack
                var detailsStack = new StackPanel { VerticalAlignment = VerticalAlignment.Center };
                
                var nameText = new TextBlock
                {
                    Text = job.Name,
                    FontSize = 14,
                    FontWeight = FontWeights.Bold,
                    Foreground = Brushes.White
                };
                detailsStack.Children.Add(nameText);

                var specsText = new TextBlock
                {
                    Text = string.Format("Salário Base: {0}/dia  |  Carga Horária: {1}h", job.BaseSalary.ToCurrencyString(), job.BaseHours),
                    FontSize = 11,
                    Foreground = new SolidColorBrush(Color.FromRgb(0xAA, 0xAA, 0xAA)),
                    Margin = new Thickness(0, 2, 0, 4)
                };
                detailsStack.Children.Add(specsText);

                var descText = new TextBlock
                {
                    Text = job.Description,
                    FontSize = 11,
                    Foreground = new SolidColorBrush(Color.FromRgb(0x77, 0x77, 0x77)),
                    TextWrapping = TextWrapping.Wrap,
                    Margin = new Thickness(0, 0, 0, 6)
                };
                detailsStack.Children.Add(descText);

                // Check prerequisites
                string missingReason;
                bool isEligible = JobCatalog.CheckPrerequisites(job, out missingReason);

                if (!isEligible)
                {
                    var reqText = new TextBlock
                    {
                        Text = missingReason,
                        FontSize = 11,
                        FontWeight = FontWeights.Bold,
                        Foreground = new SolidColorBrush(Color.FromRgb(0xEA, 0x60, 0x60)),
                        TextWrapping = TextWrapping.Wrap
                    };
                    detailsStack.Children.Add(reqText);
                }
                else if (job.Id != Player.Current.CurrentJobId)
                {
                    var eligibleText = new TextBlock
                    {
                        Text = "Requisitos atendidos!",
                        FontSize = 11,
                        FontWeight = FontWeights.Bold,
                        Foreground = new SolidColorBrush(Color.FromRgb(0x2E, 0xCC, 0x71))
                    };
                    detailsStack.Children.Add(eligibleText);
                }

                Grid.SetColumn(detailsStack, 0);
                mainGrid.Children.Add(detailsStack);

                // Action Column
                var actionGrid = new Grid { VerticalAlignment = VerticalAlignment.Center };
                if (job.Id == Player.Current.CurrentJobId)
                {
                    var activeBadge = new Border
                    {
                        Background = new SolidColorBrush(Color.FromRgb(0x1E, 0x56, 0x31)),
                        BorderBrush = new SolidColorBrush(Color.FromRgb(0x2E, 0xCC, 0x71)),
                        BorderThickness = new Thickness(1),
                        CornerRadius = new CornerRadius(4),
                        Padding = new Thickness(8, 6, 8, 6),
                        HorizontalAlignment = HorizontalAlignment.Center
                    };
                    activeBadge.Child = new TextBlock
                    {
                        Text = "EMPREGO ATUAL",
                        FontSize = 11,
                        FontWeight = FontWeights.Bold,
                        Foreground = Brushes.White,
                        HorizontalAlignment = HorizontalAlignment.Center
                    };
                    actionGrid.Children.Add(activeBadge);
                }
                else
                {
                    var btnApply = new Button
                    {
                        Content = "Candidatar-se",
                        Height = 30,
                        IsEnabled = isEligible,
                        Tag = job.Id
                    };
                    btnApply.Click += BtnApply_Click;
                    actionGrid.Children.Add(btnApply);
                }

                Grid.SetColumn(actionGrid, 1);
                mainGrid.Children.Add(actionGrid);

                border.Child = mainGrid;
                panelJobCatalog.Children.Add(border);
            }
        }

        private void BtnApply_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            if (btn == null) return;

            string jobId = btn.Tag.ToString();
            var job = JobCatalog.GetJobById(jobId);
            if (job == null) return;

            // Apply for job
            Player.Current.CurrentJobId = jobId;
            Player.Current.JobPerformance = 50.0;
            Player.Current.JobEffortLevel = "Normal";
            Player.Current.SalaryBonusMultiplier = 1.0;

            CustomMessageBox.ShowDialog(
                "Contratado!",
                string.Format("Você foi contratado para a vaga de '{0}'! Seu esforço inicial está definido como Normal. Gerencie sua carreira no painel.", job.Name),
                MessagePicture.Work
            );

            Update();
        }

        private void BtnEffort_Click(object sender, RoutedEventArgs e)
        {
            var toggle = sender as ToggleButton;
            if (toggle == null || _isUpdating) return;

            string selectedEffort = toggle.Content.ToString();
            Player.Current.JobEffortLevel = selectedEffort;

            Update();
        }

        private void BtnResign_Click(object sender, RoutedEventArgs e)
        {
            var currentJob = JobCatalog.GetJobById(Player.Current.CurrentJobId);
            if (currentJob == null) return;

            CustomMessageBox.ShowDialog(
                "Pedir Demissão",
                string.Format("Tem certeza que deseja pedir demissão do cargo de '{0}'? Você ficará desempregado.", currentJob.Name),
                MessagePicture.Question,
                (result) =>
                {
                    if (result == true)
                    {
                        Player.Current.CurrentJobId = null;
                        Player.Current.JobPerformance = 50.0;
                        Player.Current.JobEffortLevel = "Normal";
                        Player.Current.SalaryBonusMultiplier = 1.0;
                        
                        // Clear scheduling appointments for Job if inside DailyPlanner
                        Update();
                    }
                }
            );
        }
    }
}
