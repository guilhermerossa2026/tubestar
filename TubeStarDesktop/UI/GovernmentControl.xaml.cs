using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace TubeStar
{
    /// <summary>
    /// Interaction logic for GovernmentControl.xaml
    /// </summary>
    public partial class GovernmentControl : UserControl
    {
        public GovernmentControl()
        {
            InitializeComponent();
        }

        public void Update()
        {
            if (Player.Current == null) return;

            // Money Balance
            if (txtMoneyBalance != null)
            {
                txtMoneyBalance.Text = Player.Current.Money.ToCurrencyString();
            }

            // Slider and declaration percent
            if (sliderDeclaration != null)
            {
                sliderDeclaration.Value = Player.Current.TaxDeclarationRate * 100.0;
                txtDeclarationPercent.Text = string.Format("{0}%", Math.Round(sliderDeclaration.Value));
            }

            // Yesterday's numbers
            if (txtYesterdayRevenue != null)
                txtYesterdayRevenue.Text = Player.Current.YesterdayRevenue.ToCurrencyString();

            if (txtYesterdayDeclared != null)
                txtYesterdayDeclared.Text = Player.Current.YesterdayDeclaredRevenue.ToCurrencyString();

            if (txtYesterdayTaxPaid != null)
                txtYesterdayTaxPaid.Text = Player.Current.YesterdayTaxPaid.ToCurrencyString();

            if (txtTotalEvaded != null)
                txtTotalEvaded.Text = Player.Current.UnpaidEvadedTaxes.ToCurrencyString();

            // Risco de auditoria
            double rawRisk = (1.0 - Player.Current.TaxDeclarationRate) * 0.40;
            if (Player.Current.IsAccountantHired)
            {
                rawRisk /= 2.0;
            }
            if (txtAuditRisk != null)
            {
                txtAuditRisk.Text = string.Format("{0:P1}", rawRisk);
                if (rawRisk == 0)
                    txtAuditRisk.Foreground = System.Windows.Media.Brushes.LightGreen;
                else if (rawRisk < 0.15)
                    txtAuditRisk.Foreground = System.Windows.Media.Brushes.Yellow;
                else
                    txtAuditRisk.Foreground = System.Windows.Media.Brushes.LightPink;
            }

            // Accountant status text in dashboard
            if (txtAccountantStatus != null)
            {
                if (Player.Current.IsAccountantHired)
                {
                    txtAccountantStatus.Text = "Sim (-50% risco + Deduções)";
                    txtAccountantStatus.Foreground = System.Windows.Media.Brushes.LightGreen;
                }
                else
                {
                    txtAccountantStatus.Text = "Não (-0% risco)";
                    txtAccountantStatus.Foreground = System.Windows.Media.Brushes.LightPink;
                }
            }

            // Dívida ativa warning
            if (borderDebtWarning != null && txtDebtWarningDescription != null)
            {
                if (Player.Current.TaxDebtAmount > 0)
                {
                    borderDebtWarning.Visibility = Visibility.Visible;
                    txtDebtWarningDescription.Text = string.Format(
                        "Você possui {0} pendentes com o governo de impostos/multas não pagos. Suas ações financeiras de compra de imóveis, veículos, ações ou abertura de novas empresas estão bloqueadas até a regularização!",
                        Player.Current.TaxDebtAmount.ToCurrencyString()
                    );
                }
                else
                {
                    borderDebtWarning.Visibility = Visibility.Collapsed;
                }
            }

            // Advisory Tab
            if (btnToggleAccountant != null)
            {
                if (Player.Current.IsAccountantHired)
                {
                    btnToggleAccountant.Content = "Demitir Contador";
                    btnToggleAccountant.BorderBrush = System.Windows.Media.Brushes.Red;
                }
                else
                {
                    btnToggleAccountant.Content = "Contratar Contador ($40/dia)";
                    btnToggleAccountant.BorderBrush = System.Windows.Media.Brushes.MediumSpringGreen;
                }
            }

            if (btnToggleAttorney != null)
            {
                if (Player.Current.IsTaxAttorneyHired)
                {
                    btnToggleAttorney.Content = "Demitir Advogado";
                    btnToggleAttorney.BorderBrush = System.Windows.Media.Brushes.Red;
                }
                else
                {
                    btnToggleAttorney.Content = "Contratar Advogado ($80/dia)";
                    btnToggleAttorney.BorderBrush = System.Windows.Media.Brushes.Orange;
                }
            }

            // Subsidies Tab
            if (borderActiveSubsidy != null && txtActiveSubsidyDetails != null)
            {
                if (!string.IsNullOrEmpty(Player.Current.ActiveSubsidyId))
                {
                    borderActiveSubsidy.Visibility = Visibility.Visible;
                    if (Player.Current.ActiveSubsidyId == "edu")
                    {
                        txtActiveSubsidyDetails.Text = string.Format(
                            "Subsídio de Educação: Você recebe +$200 por dia. Regra: Só poste vídeos Educativos ou Vlogs! Contrato expira em: {0} dias.",
                            Player.Current.SubsidyDaysLeft
                        );
                    }
                    else if (Player.Current.ActiveSubsidyId == "rouanet")
                    {
                        txtActiveSubsidyDetails.Text = string.Format(
                            "Lei Rouanet: Financiamento de $15.000 recebidos. Meta: Enviar 3 vídeos de qualidade > 80. Progresso: {0}/3 vídeos enviados. Dias restantes: {1} dias.",
                            Player.Current.SubsidyVideosUploaded,
                            Player.Current.SubsidyDaysLeft
                        );
                    }
                }
                else
                {
                    borderActiveSubsidy.Visibility = Visibility.Collapsed;
                }
            }

            // Enable / Disable Apply buttons
            if (btnApplyEdu != null && btnApplyRouanet != null)
            {
                bool hasActive = !string.IsNullOrEmpty(Player.Current.ActiveSubsidyId);
                bool hasDebt = Player.Current.TaxDebtAmount > 0;

                btnApplyEdu.IsEnabled = !hasActive && !hasDebt;
                btnApplyRouanet.IsEnabled = !hasActive && !hasDebt && !string.IsNullOrEmpty(Player.Current.EnrolledUniversityId);

                btnApplyEdu.Content = hasActive ? "Bloqueado (Contrato Ativo)" : (hasDebt ? "Bloqueado (Nome Sujo)" : "Inscrever-se");
                btnApplyRouanet.Content = hasActive ? "Bloqueado (Contrato Ativo)" : (hasDebt ? "Bloqueado (Nome Sujo)" : (string.IsNullOrEmpty(Player.Current.EnrolledUniversityId) ? "Requer Faculdade" : "Inscrever-se"));
            }
        }

        private void Tab_Click(object sender, RoutedEventArgs e)
        {
            var clickedButton = sender as ToggleButton;
            if (clickedButton == null) return;

            foreach (var child in tabGrid.Children)
            {
                var btn = child as ToggleButton;
                if (btn != null && btn != clickedButton)
                {
                    btn.IsChecked = false;
                }
            }

            clickedButton.IsChecked = true;

            // Toggle panels
            panelFiscal.Visibility = (clickedButton == tabFiscal) ? Visibility.Visible : Visibility.Collapsed;
            panelAdvisory.Visibility = (clickedButton == tabAdvisory) ? Visibility.Visible : Visibility.Collapsed;
            panelSubsidies.Visibility = (clickedButton == tabSubsidies) ? Visibility.Visible : Visibility.Collapsed;

            Update();
        }

        private void SliderDeclaration_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (Player.Current != null)
            {
                Player.Current.TaxDeclarationRate = e.NewValue / 100.0;
                if (txtDeclarationPercent != null)
                {
                    txtDeclarationPercent.Text = string.Format("{0}%", Math.Round(e.NewValue));
                }
                
                // Recalculate audit risk on UI
                double rawRisk = (1.0 - Player.Current.TaxDeclarationRate) * 0.40;
                if (Player.Current.IsAccountantHired)
                {
                    rawRisk /= 2.0;
                }
                if (txtAuditRisk != null)
                {
                    txtAuditRisk.Text = string.Format("{0:P1}", rawRisk);
                    if (rawRisk == 0)
                        txtAuditRisk.Foreground = System.Windows.Media.Brushes.LightGreen;
                    else if (rawRisk < 0.15)
                        txtAuditRisk.Foreground = System.Windows.Media.Brushes.Yellow;
                    else
                        txtAuditRisk.Foreground = System.Windows.Media.Brushes.LightPink;
                }
            }
        }

        private void ToggleAccountant_Click(object sender, RoutedEventArgs e)
        {
            if (Player.Current == null) return;
            Player.Current.IsAccountantHired = !Player.Current.IsAccountantHired;
            if (Player.Current.IsAccountantHired)
            {
                CustomMessageBox.ShowDialog("Contador Contratado!", "Você contratou um contador profissional. O custo diário de $40 será debitado na agenda. Seu risco de cair na malha fina foi reduzido pela metade!", MessagePicture.Study);
            }
            else
            {
                CustomMessageBox.ShowDialog("Contador Dispensado!", "Você encerrou a assessoria contábil.", MessagePicture.Axe);
            }
            Update();
        }

        private void ToggleAttorney_Click(object sender, RoutedEventArgs e)
        {
            if (Player.Current == null) return;
            Player.Current.IsTaxAttorneyHired = !Player.Current.IsTaxAttorneyHired;
            if (Player.Current.IsTaxAttorneyHired)
            {
                CustomMessageBox.ShowDialog("Advogado Contratado!", "Você contratou um advogado tributarista. O custo diário de $80 será debitado na agenda. Em caso de multas governamentais, o valor cobrado será reduzido em 66% (de 150% para 50%)!", MessagePicture.Legal);
            }
            else
            {
                CustomMessageBox.ShowDialog("Advogado Dispensado!", "Você encerrou a assessoria jurídica.", MessagePicture.Axe);
            }
            Update();
        }

        private void PayDebt_Click(object sender, RoutedEventArgs e)
        {
            if (Player.Current == null) return;
            if (Player.Current.Money >= Player.Current.TaxDebtAmount)
            {
                Player.Current.Money -= Player.Current.TaxDebtAmount;
                Player.Current.TaxDebtAmount = 0.0;
                CustomMessageBox.ShowDialog("Nome Regularizado!", "Sua dívida ativa foi quitada com sucesso. Seu nome está limpo e todas as restrições financeiras foram removidas!", MessagePicture.Money);
                Update();
            }
            else
            {
                CustomMessageBox.ShowDialog("Saldo Insuficiente!", "Você não possui dinheiro suficiente para quitar a dívida ativa tributária.", MessagePicture.Money);
            }
        }

        private void ApplyEdu_Click(object sender, RoutedEventArgs e)
        {
            if (Player.Current == null) return;
            Player.Current.ActiveSubsidyId = "edu";
            Player.Current.SubsidyDaysLeft = 30;
            Player.Current.SubsidyVideosUploaded = 0;
            CustomMessageBox.ShowDialog(
                "Inscrição Confirmada!",
                "Parabéns! Seu canal foi aprovado no Edital de Educação. Você receberá +$200/dia nos próximos 30 dias. Lembre-se: poste APENAS vídeos educativos ou vlogs. Outros temas resultarão em cancelamento e multa de $3.000!",
                MessagePicture.Study
            );
            Update();
        }

        private void ApplyRouanet_Click(object sender, RoutedEventArgs e)
        {
            if (Player.Current == null) return;
            Player.Current.ActiveSubsidyId = "rouanet";
            Player.Current.SubsidyDaysLeft = 10;
            Player.Current.SubsidyVideosUploaded = 0;
            Player.Current.Money += 15000;
            CustomMessageBox.ShowDialog(
                "Subsídio Concedido!",
                "O governo repassou $15.000 de fomento cultural para a sua conta! Você tem 10 dias para upar no mínimo 3 vídeos com pontuação de qualidade acima de 80. Caso falhe, deverá reembolsar os $15.000!",
                MessagePicture.Money
            );
            Update();
        }

        private void CancelSubsidy_Click(object sender, RoutedEventArgs e)
        {
            if (Player.Current == null) return;
            string subId = Player.Current.ActiveSubsidyId;
            if (subId == "edu")
            {
                Player.Current.ActiveSubsidyId = null;
                CustomMessageBox.ShowDialog("Contrato Rescindido!", "Você cancelou o subsídio de educação sem multas adicionais.", MessagePicture.Axe);
            }
            else if (subId == "rouanet")
            {
                Player.Current.ActiveSubsidyId = null;
                // Penalidade de devolução imediata
                if (Player.Current.Money >= 15000)
                {
                    Player.Current.Money -= 15000;
                    CustomMessageBox.ShowDialog("Contrato Rescindido!", "Você devolveu os $15.000 e cancelou o edital Rouanet.", MessagePicture.Money);
                }
                else
                {
                    double diff = 15000 - Player.Current.Money;
                    Player.Current.Money = 0;
                    Player.Current.TaxDebtAmount += diff;
                    CustomMessageBox.ShowDialog("Contrato Rescindido com Dívida!", string.Format("Seu saldo foi zerado e o restante de {0} foi inscrito na dívida ativa do seu nome sujo!", diff.ToCurrencyString()), MessagePicture.Axe);
                }
            }
            Update();
        }
    }
}
