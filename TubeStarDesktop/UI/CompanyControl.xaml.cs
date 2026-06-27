using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

namespace TubeStar
{
    public partial class CompanyControl : UserControl
    {
        private Company _selectedCompany;
        private bool _isUpdating;
        private Product _selectedProduct;

        public CompanyControl()
        {
            InitializeComponent();
        }

        public void Update()
        {
            if (_isUpdating) return;
            _isUpdating = true;

            try
            {
                // Update Personal Money
                txtPersonalMoney.Text = Player.Current.Money.ToCurrencyString();

                // Get Total Subscribers
                int totalSubs = 0;
                if (Player.Current.Channels != null)
                {
                    totalSubs = Player.Current.Channels
                        .Where(c => c != Channel.UnreleasedVideos && !c.IsRivalChannel)
                        .Sum(c => c.Subscribers);
                }

                // Update Creation Panel requirement texts
                txtReqSubs.Text = string.Format("{0:N0} / 100.000", totalSubs);
                txtReqSubs.Foreground = totalSubs >= 100000 
                    ? System.Windows.Media.Brushes.Green 
                    : System.Windows.Media.Brushes.Red;

                txtReqMoney.Text = string.Format("{0} / $300.000", Player.Current.Money.ToCurrencyString());
                txtReqMoney.Foreground = Player.Current.Money >= 300000 
                    ? System.Windows.Media.Brushes.Green 
                    : System.Windows.Media.Brushes.Red;

                // Enable/Disable fund button
                btnFundCompany.IsEnabled = totalSubs >= 100000 && Player.Current.Money >= 300000 && Player.Current.TaxDebtAmount <= 0;
                if (Player.Current.TaxDebtAmount > 0)
                {
                    btnFundCompany.Content = "Nome Sujo na Dívida Ativa";
                }
                else
                {
                    btnFundCompany.Content = "Fundar Nova Empresa";
                }

                // Repopulate active company combo if list changed
                var ownedCount = Player.Current.OwnedCompanies != null ? Player.Current.OwnedCompanies.Count : 0;
                
                if (cmbActiveCompany.Items.Count != (ownedCount + 1))
                {
                    cmbActiveCompany.Items.Clear();
                    
                    if (Player.Current.OwnedCompanies != null)
                    {
                        foreach (var company in Player.Current.OwnedCompanies)
                        {
                            cmbActiveCompany.Items.Add(new ComboBoxItem 
                                { 
                                    Content = string.Format("{0} ({1})", company.Name, company.Niche), 
                                    Tag = company 
                                });
                        }
                    }
                    
                    cmbActiveCompany.Items.Add(new ComboBoxItem 
                        { 
                            Content = "+ Fundar Nova Empresa", 
                            Tag = null 
                        });

                    if (ownedCount > 0 && _selectedCompany == null)
                    {
                        _selectedCompany = Player.Current.OwnedCompanies[0];
                    }

                    if (_selectedCompany != null)
                    {
                        foreach (ComboBoxItem item in cmbActiveCompany.Items)
                        {
                            if (item.Tag == _selectedCompany)
                            {
                                cmbActiveCompany.SelectedItem = item;
                                break;
                            }
                        }
                    }
                    else
                    {
                        cmbActiveCompany.SelectedIndex = cmbActiveCompany.Items.Count - 1; // Fundar Nova Empresa
                    }
                }

                // Check selection
                var selectedItem = cmbActiveCompany.SelectedItem as ComboBoxItem;
                _selectedCompany = selectedItem != null ? selectedItem.Tag as Company : null;

                if (_selectedCompany == null)
                {
                    creationPanel.Visibility = Visibility.Visible;
                    dashboardPanel.Visibility = Visibility.Collapsed;
                }
                else
                {
                    creationPanel.Visibility = Visibility.Collapsed;
                    dashboardPanel.Visibility = Visibility.Visible;

                    // Bind company details
                    txtCompanyName.Text = _selectedCompany.Name.ToUpper();
                    txtCompanyNiche.Text = string.Format("Nicho de Mercado: {0}", _selectedCompany.Niche);
                    txtCompanyBalance.Text = _selectedCompany.Balance.ToCurrencyString();

                    if (_selectedCompany.Balance < 0)
                    {
                        txtCompanyBalance.Foreground = Brushes.Red;
                    }
                    else
                    {
                        txtCompanyBalance.Foreground = new SolidColorBrush(Color.FromRgb(0x2E, 0xCC, 0x71));
                    }

                    // Progress indicators
                    pbBrandAwareness.Value = _selectedCompany.BrandAwareness;
                    txtBrandAwarenessVal.Text = string.Format("{0:F1}%", _selectedCompany.BrandAwareness);

                    pbInfra.Value = _selectedCompany.InfrastructureLevel;
                    txtInfraVal.Text = string.Format("Nível {0} / 5", _selectedCompany.InfrastructureLevel);

                    // Update Yesterday's Report Tab Ops
                    txtYesterdaySales.Text = _selectedCompany.YesterdaySales.ToString("N0");
                    txtYesterdayRevenue.Text = _selectedCompany.YesterdayRevenue.ToCurrencyString();
                    txtYesterdayCosts.Text = _selectedCompany.YesterdayCosts.ToCurrencyString();

                    double netProfit = _selectedCompany.YesterdayRevenue - _selectedCompany.YesterdayCosts;
                    txtYesterdayNetProfit.Text = netProfit.ToCurrencyString();
                    txtYesterdayNetProfit.Foreground = netProfit >= 0 
                        ? new SolidColorBrush(Color.FromRgb(0x2E, 0xCC, 0x71)) 
                        : Brushes.Red;

                    txtMarketTrend.Text = _selectedCompany.MarketTrend ?? "Estável";

                    // Marketing campaign combo selection
                    foreach (ComboBoxItem mItem in cmbMarketingCampaign.Items)
                    {
                        if (mItem.Tag.ToString() == _selectedCompany.MarketingCampaign)
                        {
                            cmbMarketingCampaign.SelectedItem = mItem;
                            break;
                        }
                    }

                    // Staff buttons state
                    btnToggleCEO.Content = _selectedCompany.HiredCEO ? "Demitir" : "Contratar";
                    btnToggleCEO.Background = _selectedCompany.HiredCEO ? Brushes.Red : new SolidColorBrush(Color.FromRgb(0xFF, 0x22, 0x22));

                    btnToggleMarketing.Content = _selectedCompany.HiredMarketingDirector ? "Demitir" : "Contratar";
                    btnToggleMarketing.Background = _selectedCompany.HiredMarketingDirector ? Brushes.Red : new SolidColorBrush(Color.FromRgb(0xFF, 0x22, 0x22));

                    btnToggleSales.Content = _selectedCompany.HiredSalesManager ? "Demitir" : "Contratar";
                    btnToggleSales.Background = _selectedCompany.HiredSalesManager ? Brushes.Red : new SolidColorBrush(Color.FromRgb(0xFF, 0x22, 0x22));

                    btnToggleRD.Content = _selectedCompany.HiredRDEngineer ? "Demitir" : "Contratar";
                    btnToggleRD.Background = _selectedCompany.HiredRDEngineer ? Brushes.Red : new SolidColorBrush(Color.FromRgb(0xFF, 0x22, 0x22));

                    // Upgrade buttons
                    if (_selectedCompany.InfrastructureLevel >= 5)
                    {
                        btnUpgradeInfra.Content = "Nível Máximo";
                        btnUpgradeInfra.IsEnabled = false;
                        txtInfraUpgradeDesc.Text = "Sua fábrica está no nível máximo de eficiência (Nível 5).";
                    }
                    else
                    {
                        double upgradeCost = 50000 * _selectedCompany.InfrastructureLevel;
                        btnUpgradeInfra.Content = string.Format("Melhorar ({0})", upgradeCost.ToCurrencyString());
                        btnUpgradeInfra.IsEnabled = _selectedCompany.Balance >= upgradeCost;
                        txtInfraUpgradeDesc.Text = string.Format("Aumenta a margem e o limite máximo de volume de produção/vendas. Próxima melhoria custa {0}.", upgradeCost.ToCurrencyString());
                    }

                    // Products list binding
                    // Safe access to Products list
                    var currentProductsList = _selectedCompany.Products.ToList();
                    lstProducts.ItemsSource = currentProductsList;

                    // Update selected product view
                    if (_selectedProduct != null)
                    {
                        // Refresh from company's actual object
                        _selectedProduct = _selectedCompany.Products.FirstOrDefault(p => p.Id == _selectedProduct.Id);
                    }

                    if (_selectedProduct == null)
                    {
                        panelCreateProduct.Visibility = Visibility.Visible;
                        panelEditProduct.Visibility = Visibility.Collapsed;
                    }
                    else
                    {
                        panelCreateProduct.Visibility = Visibility.Collapsed;
                        panelEditProduct.Visibility = Visibility.Visible;

                        txtSelectedProductName.Text = _selectedProduct.Name.ToUpper();
                        txtSelectedProductQuality.Text = string.Format("{0:F0}%", _selectedProduct.Quality);
                        pbSelectedProductQuality.Value = _selectedProduct.Quality;

                        txtSelectedProductNovelty.Text = string.Format("{0:F0}%", _selectedProduct.Novelty);
                        pbSelectedProductNovelty.Value = _selectedProduct.Novelty;

                        txtEditProductPrice.Text = _selectedProduct.Price.ToString("F2");
                        UpdatePricePerceptionLabel(_selectedProduct.Price);

                        btnRelaunchProduct.IsEnabled = _selectedCompany.Balance >= 15000;
                    }
                }
            }
            finally
            {
                _isUpdating = false;
            }
        }

        private void LstProducts_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_isUpdating) return;
            _selectedProduct = lstProducts.SelectedItem as Product;
            Update();
        }

        private void UpdatePricePerceptionLabel(double price)
        {
            if (_selectedCompany == null) return;

            double stdPrice = 5.0;
            switch (_selectedCompany.Niche)
            {
                case "Alimentos": stdPrice = 5.0; break;
                case "Merch": stdPrice = 25.0; break;
                case "Gamer": stdPrice = 120.0; break;
                case "Brinquedos": stdPrice = 40.0; break;
            }

            if (price <= stdPrice * 0.5)
            {
                lblPricePerception.Text = "Preço Muito Baixo (Vende muito + / Margem muito baixa)";
                lblPricePerception.Foreground = new SolidColorBrush(Color.FromRgb(0x2E, 0xCC, 0x71));
            }
            else if (price <= stdPrice * 0.9)
            {
                lblPricePerception.Text = "Preço Barato (Vende + / Margem menor)";
                lblPricePerception.Foreground = new SolidColorBrush(Color.FromRgb(0x2E, 0xCC, 0x71));
            }
            else if (price <= stdPrice * 1.1)
            {
                lblPricePerception.Text = "Preço Justo (Mercado)";
                lblPricePerception.Foreground = Brushes.DeepSkyBlue;
            }
            else if (price <= stdPrice * 1.5)
            {
                lblPricePerception.Text = "Preço Caro (Vende - / Margem alta)";
                lblPricePerception.Foreground = Brushes.Orange;
            }
            else
            {
                lblPricePerception.Text = "Preço Abusivo (Vende muito - / Fãs reclamando!)";
                lblPricePerception.Foreground = Brushes.Red;
            }
        }

        private void TxtEditProductPrice_SelectionChanged(object sender, RoutedEventArgs e)
        {
            if (_selectedProduct == null || _isUpdating) return;

            double parsedPrice;
            if (double.TryParse(txtEditProductPrice.Text, out parsedPrice))
            {
                UpdatePricePerceptionLabel(parsedPrice);
            }
        }

        private void BtnSaveProductPrice_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedProduct == null || _selectedCompany == null) return;

            double parsedPrice;
            if (double.TryParse(txtEditProductPrice.Text, out parsedPrice) && parsedPrice >= 0)
            {
                _selectedProduct.Price = parsedPrice;
                CustomMessageBox.ShowDialog("Preço Salvo", string.Format("O preço do produto '{0}' foi atualizado para {1}.", _selectedProduct.Name, parsedPrice.ToCurrencyString()), MessagePicture.Money);
                Update();
            }
            else
            {
                CustomMessageBox.ShowDialog("Valor Inválido", "Por favor, insira um preço válido maior ou igual a zero.", MessagePicture.Sad);
            }
        }

        private void BtnDevelopProduct_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedCompany == null) return;

            if (_selectedCompany.Products.Count >= 3)
            {
                CustomMessageBox.ShowDialog("Limite Atingido", "Você pode ter no máximo 3 produtos ativos simultaneamente por empresa. Descontinue um antes.", MessagePicture.Puzzle);
                return;
            }

            string name = txtProductName.Text.Trim();
            if (string.IsNullOrEmpty(name))
            {
                CustomMessageBox.ShowDialog("Nome Inválido", "Por favor, insira um nome para o produto.", MessagePicture.Puzzle);
                return;
            }

            double price;
            if (!double.TryParse(txtProductPrice.Text, out price) || price < 0)
            {
                CustomMessageBox.ShowDialog("Preço Inválido", "Por favor, insira um preço de venda inicial válido.", MessagePicture.Money);
                return;
            }

            // Budget evaluation
            var selectedItem = cmbRDLevel.SelectedItem as ComboBoxItem;
            string rdTag = selectedItem != null ? selectedItem.Tag.ToString() : "Basic";

            double cost = 5000.0;
            double baseQuality = 45.0;

            if (rdTag == "Advanced")
            {
                cost = 15000.0;
                baseQuality = 70.0;
            }
            else if (rdTag == "Premium")
            {
                cost = 35000.0;
                baseQuality = 90.0;
            }

            if (_selectedCompany.Balance < cost)
            {
                CustomMessageBox.ShowDialog("Saldo Insuficiente", string.Format("A empresa não possui saldo em caixa ({0}) para custear esse P&D ({1}).", _selectedCompany.Balance.ToCurrencyString(), cost.ToCurrencyString()), MessagePicture.Money);
                return;
            }

            // Deduct
            _selectedCompany.Balance -= cost;

            // Generate quality
            double randQuality = baseQuality - 5.0 + RandomHelpers.RandomInt(11); // baseQuality +/- 5
            if (_selectedCompany.HiredRDEngineer)
            {
                randQuality += 10.0; // P&D engineer quality bonus
            }
            randQuality = Math.Max(0.0, Math.Min(100.0, randQuality));

            // Create product
            var newProd = new Product(Guid.NewGuid().ToString(), name, price, randQuality);
            _selectedCompany.Products.Add(newProd);

            CustomMessageBox.ShowDialog("Produto Lançado!", string.Format("O produto '{0}' com qualidade de {1:F0}% foi desenvolvido e lançado com sucesso!", name, randQuality), MessagePicture.Puzzle);

            txtProductName.Text = string.Empty;
            txtProductPrice.Text = "5.00";
            cmbRDLevel.SelectedIndex = 0;

            Update();
        }

        private void BtnRelaunchProduct_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedProduct == null || _selectedCompany == null) return;

            if (_selectedCompany.Balance < 15000)
            {
                CustomMessageBox.ShowDialog("Saldo Insuficiente", "A empresa não possui $15.000 em caixa para financiar o relançamento.", MessagePicture.Money);
                return;
            }

            _selectedCompany.Balance -= 15000;
            _selectedProduct.Novelty = 100.0;
            _selectedProduct.Quality = Math.Min(100.0, _selectedProduct.Quality + 5.0);

            CustomMessageBox.ShowDialog("Versão Atualizada", string.Format("Uma nova versão de '{0}' foi lançada! Fator de novidade restaurado para 100% e qualidade aumentada.", _selectedProduct.Name), MessagePicture.Puzzle);
            Update();
        }

        private void BtnDiscontinueProduct_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedProduct == null || _selectedCompany == null) return;

            CustomMessageBox.ShowDialog(
                "Descontinuar Produto",
                string.Format("Tem certeza de que deseja remover '{0}' de circulação permanentemente? As vendas desse produto serão zeradas.", _selectedProduct.Name),
                MessagePicture.Question,
                (result) =>
                {
                    if (result == true)
                    {
                        _selectedCompany.Products.Remove(_selectedProduct);
                        _selectedProduct = null;
                        Update();
                    }
                }
            );
        }

        private void BtnBackToCreate_Click(object sender, RoutedEventArgs e)
        {
            lstProducts.SelectedItem = null;
            _selectedProduct = null;
            Update();
        }

        private void TabBtn_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as ToggleButton;
            if (btn == null) return;

            // Reset selection states in tab bar
            tabBtnProducts.IsChecked = false;
            tabBtnOps.IsChecked = false;
            tabBtnStaff.IsChecked = false;

            btn.IsChecked = true;

            // Show/Hide Panels
            panelProducts.Visibility = btn == tabBtnProducts ? Visibility.Visible : Visibility.Collapsed;
            panelOps.Visibility = btn == tabBtnOps ? Visibility.Visible : Visibility.Collapsed;
            panelStaff.Visibility = btn == tabBtnStaff ? Visibility.Visible : Visibility.Collapsed;
        }

        private void CmbActiveCompany_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedItem = cmbActiveCompany.SelectedItem as ComboBoxItem;
            if (selectedItem != null)
            {
                _selectedCompany = selectedItem.Tag as Company;
            }
            _selectedProduct = null;
            Update();
        }

        private void BtnFundCompany_Click(object sender, RoutedEventArgs e)
        {
            if (Player.Current.TaxDebtAmount > 0)
            {
                CustomMessageBox.ShowDialog("Operação Bloqueada!", "Você possui pendências na Dívida Ativa governamental e não pode fundar empresas. Regularize seu CPF no Portal do Governo!", MessagePicture.Axe);
                return;
            }

            string name = txtNewCompanyName.Text.Trim();
            if (string.IsNullOrEmpty(name))
            {
                CustomMessageBox.ShowDialog("Valor Inválido", "Por favor, insira um nome para a sua empresa.", MessagePicture.Puzzle);
                return;
            }

            int totalSubs = 0;
            if (Player.Current.Channels != null)
            {
                totalSubs = Player.Current.Channels
                    .Where(c => c != Channel.UnreleasedVideos && !c.IsRivalChannel)
                    .Sum(c => c.Subscribers);
            }

            if (totalSubs < 100000 || Player.Current.Money < 300000)
            {
                CustomMessageBox.ShowDialog("Requisitos não atendidos", "Você ainda não possui os requisitos necessários para fundar uma empresa.", MessagePicture.Money);
                return;
            }

            // Deduct funds
            Player.Current.Money -= 300000;

            // Get selected niche
            var selectedNicheItem = cmbNewCompanyNiche.SelectedItem as ComboBoxItem;
            string niche = selectedNicheItem != null ? selectedNicheItem.Tag.ToString() : "Alimentos";

            // Create Company
            string id = Guid.NewGuid().ToString();
            var newCompany = new Company(id, name, niche, 50000.0); // Starts with $50,000 from the foundation capital

            if (Player.Current.OwnedCompanies == null)
            {
                Player.Current.OwnedCompanies = new List<Company>();
            }
            Player.Current.OwnedCompanies.Add(newCompany);

            _selectedCompany = newCompany;
            txtNewCompanyName.Text = string.Empty;

            cmbActiveCompany.Items.Clear();

            CustomMessageBox.ShowDialog("Parabéns!", string.Format("A empresa '{0}' no nicho de {1} foi fundada com sucesso!", name, niche), MessagePicture.Money);
            Update();
        }

        private void BtnInvest10k_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedCompany == null) return;

            if (Player.Current.TaxDebtAmount > 0)
            {
                CustomMessageBox.ShowDialog("Operação Bloqueada!", "Você possui pendências na Dívida Ativa governamental e não pode realizar investimentos pessoais. Regularize seu CPF no Portal do Governo!", MessagePicture.Axe);
                return;
            }

            if (Player.Current.Money >= 10000)
            {
                Player.Current.Money -= 10000;
                _selectedCompany.Balance += 10000;
                Update();
            }
            else
            {
                CustomMessageBox.ShowDialog("Sem Saldo", "Você não possui $10.000 para investir.", MessagePicture.Money);
            }
        }

        private void BtnInvest50k_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedCompany == null) return;

            if (Player.Current.TaxDebtAmount > 0)
            {
                CustomMessageBox.ShowDialog("Operação Bloqueada!", "Você possui pendências na Dívida Ativa governamental e não pode realizar investimentos pessoais. Regularize seu CPF no Portal do Governo!", MessagePicture.Axe);
                return;
            }

            if (Player.Current.Money >= 50000)
            {
                Player.Current.Money -= 50000;
                _selectedCompany.Balance += 50000;
                Update();
            }
            else
            {
                CustomMessageBox.ShowDialog("Sem Saldo", "Você não possui $50.000 para investir.", MessagePicture.Money);
            }
        }

        private void BtnWithdraw10k_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedCompany == null) return;
            if (_selectedCompany.Balance >= 10000)
            {
                _selectedCompany.Balance -= 10000;
                Player.Current.Money += 10000;
                Update();
            }
            else
            {
                CustomMessageBox.ShowDialog("Saldo Insuficiente", "A empresa não possui $10.000 em caixa.", MessagePicture.Money);
            }
        }

        private void BtnWithdrawAll_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedCompany == null) return;
            if (_selectedCompany.Balance > 0)
            {
                double amount = _selectedCompany.Balance;
                _selectedCompany.Balance = 0;
                Player.Current.Money += amount;
                Update();
            }
        }

        private void CmbMarketingCampaign_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_selectedCompany == null || _isUpdating) return;
            var selectedItem = cmbMarketingCampaign.SelectedItem as ComboBoxItem;
            if (selectedItem != null)
            {
                _selectedCompany.MarketingCampaign = selectedItem.Tag.ToString();
            }
        }

        private void BtnUpgradeInfra_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedCompany == null) return;
            if (_selectedCompany.InfrastructureLevel >= 5) return;

            double upgradeCost = 50000 * _selectedCompany.InfrastructureLevel;
            if (_selectedCompany.Balance >= upgradeCost)
            {
                _selectedCompany.Balance -= upgradeCost;
                _selectedCompany.InfrastructureLevel++;
                CustomMessageBox.ShowDialog("Fábrica Expandida", string.Format("Fábrica expandida para o Nível {0}!", _selectedCompany.InfrastructureLevel), MessagePicture.Work);
                Update();
            }
            else
            {
                CustomMessageBox.ShowDialog("Saldo Insuficiente", "A empresa não possui saldo suficiente para expandir a infraestrutura.", MessagePicture.Money);
            }
        }

        private void BtnToggleCEO_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedCompany == null) return;
            _selectedCompany.HiredCEO = !_selectedCompany.HiredCEO;
            CustomMessageBox.ShowDialog(
                _selectedCompany.HiredCEO ? "CEO Contratado" : "CEO Demitido",
                _selectedCompany.HiredCEO 
                    ? "Você contratou um CEO profissional por $500/dia. Ele relança seus produtos automaticamente e concede bônus de +15% de vendas."
                    : "Você demitiu o CEO.",
                MessagePicture.Work);
            Update();
        }

        private void BtnToggleMarketing_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedCompany == null) return;
            _selectedCompany.HiredMarketingDirector = !_selectedCompany.HiredMarketingDirector;
            CustomMessageBox.ShowDialog(
                _selectedCompany.HiredMarketingDirector ? "Diretor Contratado" : "Diretor Demitido",
                _selectedCompany.HiredMarketingDirector 
                    ? "Diretor de Marketing contratado por $300/dia. O decaimento da marca foi reduzido e a eficiência das campanhas de marketing aumentou em 25%."
                    : "Você demitiu o Diretor de Marketing.",
                MessagePicture.Work);
            Update();
        }

        private void BtnToggleSales_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedCompany == null) return;
            _selectedCompany.HiredSalesManager = !_selectedCompany.HiredSalesManager;
            CustomMessageBox.ShowDialog(
                _selectedCompany.HiredSalesManager ? "Gerente Contratado" : "Gerente Demitido",
                _selectedCompany.HiredSalesManager
                    ? "Gerente de Vendas contratado por $300/dia. Ele otimiza a conversão, gerando +20% no volume de vendas de todos os produtos."
                    : "Você demitiu o Gerente de Vendas.",
                MessagePicture.Work);
            Update();
        }

        private void BtnToggleRD_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedCompany == null) return;
            _selectedCompany.HiredRDEngineer = !_selectedCompany.HiredRDEngineer;
            CustomMessageBox.ShowDialog(
                _selectedCompany.HiredRDEngineer ? "Engenheiro Contratado" : "Engenheiro Demitido",
                _selectedCompany.HiredRDEngineer
                    ? "Engenheiro de P&D contratado por $250/dia. Novos produtos ganham +10 de qualidade inicial e o decaimento de novidade diário foi reduzido."
                    : "Você demitiu o Engenheiro de P&D.",
                MessagePicture.Work);
            Update();
        }
    }
}
