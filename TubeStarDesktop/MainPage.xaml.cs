using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Effects;
using System.Windows.Shapes;

namespace TubeStar
{
    public partial class MainPage : UserControl
    {
        public event Action GameDeath;
        public event Action GameLose;
        public event Action GameExit;

        private readonly string[] _avatarEmojis = new string[] { "🚀", "👾", "🎧", "🔥", "💻", "👑", "🎨", "🦄" };

        private readonly string[] _fanNames = new string[]
        {
            "CyberGamer", "NeonLegend", "PixelMaster", "SpeedyPro", "TechGuru",
            "ShadowPlay", "RetroStar", "VlogQueen", "SubHunter", "ByteSize",
            "GamerMom", "UnboxKing", "GlitchFix", "LevelUp", "AlphaStream"
        };

        private readonly string[] _fanComments = new string[]
        {
            "Nossa, a qualidade de edição desse canal tá subindo demais! 😱",
            "Melhor canal do momento! Me inscrevi correndo! 😍",
            "Amei o último vídeo, assisti 3 vezes seguidas! 🚀",
            "Aquele momento gamer no final foi épico demais kkkk 😂",
            "Esse canal merece 1 milhão de inscritos fácil! 🏆",
            "O áudio e iluminação estão perfeitos, que setup incrível! 🎧",
            "Faz collab com outros Tube Stars! Seria insano! 🤝",
            "Alguém assistindo isso em 2026? Com certeza o melhor! 🔥",
            "Que roteiro sensacional, muito original e bem pensado! 💡",
            "Comentando aqui antes do canal bombar e ficar gigante! 😎"
        };

        public MainPage()
        {
            // Garante que os singletons sejam inicializados na Thread de UI de forma síncrona
            var dummyRivals = Rivals.Current;
            var dummyPlayer = Player.Current;

            InitializeComponent();
            Translate();

            // Inicializar ou carregar a Bolsa de Valores
            StockMarketManager.InitializeOrLoad();
            
            // Wire player state update
            UpdateYoutuberProfile();
            UpdateSummaryStats();

            Player.Current.MoneyChanged += () =>
            {
                Dispatcher.BeginInvoke(new Action(delegate()
                {
                    txtMoney.Text = Player.Current.Money.ToCurrencyString();
                    UpdateSummaryStats();
                }));
            };

            dailyPlanner.NewDayCompleted += () =>
            {
                this.IsEnabled = true;
                progress.Visibility = System.Windows.Visibility.Collapsed;

                // Avançar o dia na Bolsa de Valores
                StockMarketManager.NextDay();

                if (videoManager.Visibility == System.Windows.Visibility.Visible)
                    videoManager.Update();

                if (rivalViewer.Visibility == System.Windows.Visibility.Visible)
                    rivalViewer.Update();

                txtNewDay.Text = String.Format("{0} {1}!", EnglishStrings.StartDay.Translate(), Player.Current.Iterations + 1);

                // Update customized profile days and stats
                UpdateYoutuberProfile();
                UpdateSummaryStats();
                GenerateDaySocialComments();

                if (Player.Current.Iterations > 1 && Player.Current.Iterations % 3 == 0)
                {
                    RandomEvents.RunEvent();
                    videoManager.Update();
                    dailyPlanner.Update();
                    rivalViewer.Update();
                }

                CheckTrophies();
            };

            dailyPlanner.GameExit += () =>
            {
                if (GameLose != null)
                    GameLose();
            };
            
            dailyPlanner.Death += () =>
            {
                if (GameDeath != null)
                    GameDeath();
            };

            // Set initial day text
            txtNewDay.Text = String.Format("{0} {1}!", EnglishStrings.StartDay.Translate(), Player.Current.Iterations + 1);
        }

        private void Translate()
        {
            gridHelp.ToolTip = EnglishStrings.Help.Translate();
            gridExit.ToolTip = EnglishStrings.Exit.Translate();
            imgSave.ToolTip = EnglishStrings.SaveGame.Translate();
            txtDailyPlanner.Text = EnglishStrings.DailyPlanner.Translate();
            txtVideoPlanner.Text = EnglishStrings.VideoManager.Translate();
            txtOnlineStore.Text = EnglishStrings.OnlineStore.Translate();
            txtRivals.Text = EnglishStrings.TopTubeStars.Translate();
            if (txtStockMarket != null)
                txtStockMarket.Text = "Bolsa de Valores";
        }

        private void UpdateYoutuberProfile()
        {
            try
            {
                txtProfileName.Text = string.IsNullOrEmpty(Player.Current.YoutuberName) ? "Gamer Pro" : Player.Current.YoutuberName;

                // 1. Atualizar Mini Avatar de Alta Definição
                if (imgProfileAvatarMini != null)
                {
                    int avatarIdx = Player.Current.YoutuberAvatarId;
                    if (avatarIdx < 0 || avatarIdx >= 15) avatarIdx = 0;

                    string[] avatarUris = new string[] {
                        "pack://application:,,,/TubeStar;component/Resources/avatar_mauricinho_1.png",
                        "pack://application:,,,/TubeStar;component/Resources/avatar_mauricinho_2.png",
                        "pack://application:,,,/TubeStar;component/Resources/avatar_mauricinho_3.png",
                        "pack://application:,,,/TubeStar;component/Resources/avatar_mauricinho_4.png",
                        "pack://application:,,,/TubeStar;component/Resources/avatar_gangstar_1.png",
                        "pack://application:,,,/TubeStar;component/Resources/avatar_gangstar_2.png",
                        "pack://application:,,,/TubeStar;component/Resources/avatar_cabelolongo_1.png",
                        "pack://application:,,,/TubeStar;component/Resources/avatar_cabelolongo_2.png",
                        "pack://application:,,,/TubeStar;component/Resources/avatar_cabelocurto_1.png",
                        "pack://application:,,,/TubeStar;component/Resources/avatar_cabelocurto_2.png",
                        "pack://application:,,,/TubeStar;component/Resources/avatar_trap_3.jpg",
                        "pack://application:,,,/TubeStar;component/Resources/avatar_casual_3.png",
                        "pack://application:,,,/TubeStar;component/Resources/avatar_casual_4.png",
                        "pack://application:,,,/TubeStar;component/Resources/avatar_casual_5.png",
                        "pack://application:,,,/TubeStar;component/Resources/avatar_casual_6.png"
                    };

                    imgProfileAvatarMini.Source = new BitmapImage(new Uri(avatarUris[avatarIdx], UriKind.Absolute));
                }

                // 2. Atualizar Aura de Brilho Dinâmica
                string hairColorHex = string.IsNullOrEmpty(Player.Current.YoutuberHairColor) ? "#FF2222" : Player.Current.YoutuberHairColor;
                var neonColor = ColorConverter.ConvertFromString(hairColorHex) as Color? ?? Colors.Red;
                var hairBrush = new SolidColorBrush(neonColor);

                if (profileAvatarGlow != null)
                {
                    profileAvatarGlow.BorderBrush = hairBrush;
                }
                
                if (profileAvatarGlowEffect != null)
                {
                    profileAvatarGlowEffect.Color = neonColor;
                }

                txtCareerDays.Text = "Dia " + (Player.Current.Iterations + 1);
                txtMoney.Text = Player.Current.Money.ToCurrencyString();
            }
            catch
            {
                // Fallback in case of conversion issues
            }
        }

        private void UpdateSummaryStats()
        {
            try
            {
                UpdateChannelFilterList();
                UpdateSummaryStatsFiltered();
            }
            catch
            {
            }
        }

        private void UpdateSummaryStatsFiltered()
        {
            try
            {
                if (cbActiveChannelFilter == null) return;

                string selectedChannelName = null;
                ComboBoxItem item = cbActiveChannelFilter.SelectedItem as ComboBoxItem;
                if (item != null)
                {
                    selectedChannelName = item.Content.ToString();
                }
                else if (cbActiveChannelFilter.SelectedItem != null)
                {
                    selectedChannelName = cbActiveChannelFilter.SelectedItem.ToString();
                }

                int totalSubs = 0;
                int totalVideos = 0;
                int totalViews = 0;

                if (string.IsNullOrEmpty(selectedChannelName) || selectedChannelName == "Todos os Canais")
                {
                    // Agregado
                    if (Player.Current.Channels != null)
                    {
                        foreach (var c in Player.Current.Channels)
                        {
                            totalSubs += c.Subscribers;
                            if (c.Videos != null)
                            {
                                foreach (var v in c.Videos)
                                {
                                    totalViews += v.Views;
                                }
                            }
                        }
                    }
                    totalVideos = Player.Current.Videos != null ? Player.Current.Videos.Count : 0;
                }
                else
                {
                    // Canal específico
                    Channel channel = null;
                    if (Player.Current.Channels != null)
                    {
                        foreach (var c in Player.Current.Channels)
                        {
                            if (c.Name == selectedChannelName)
                            {
                                channel = c;
                                break;
                            }
                        }
                    }

                    if (channel != null)
                    {
                        totalSubs = channel.Subscribers;
                        totalVideos = channel.Videos != null ? channel.Videos.Count : 0;
                        if (channel.Videos != null)
                        {
                            foreach (var v in channel.Videos)
                            {
                                totalViews += v.Views;
                            }
                        }
                    }
                }

                txtTotalSubscribers.Text = totalSubs.ToString("N0");
                txtTotalVideos.Text = totalVideos.ToString();
                txtTotalViews.Text = totalViews.ToString("N0");

                double costOfLiving = 50 + Player.Current.CostOfLivingExtra;
                txtCostOfLiving.Text = costOfLiving.ToCurrencyString();
            }
            catch
            {
            }
        }

        private void UpdateChannelFilterList()
        {
            if (cbActiveChannelFilter == null) return;

            // Salvar a seleção atual
            string selectedChannelName = null;
            ComboBoxItem currentItem = cbActiveChannelFilter.SelectedItem as ComboBoxItem;
            if (currentItem != null)
            {
                selectedChannelName = currentItem.Content.ToString();
            }
            else if (cbActiveChannelFilter.SelectedItem != null)
            {
                selectedChannelName = cbActiveChannelFilter.SelectedItem.ToString();
            }

            cbActiveChannelFilter.SelectionChanged -= cbActiveChannelFilter_SelectionChanged;
            cbActiveChannelFilter.Items.Clear();

            // Adicionar opção global
            cbActiveChannelFilter.Items.Add("Todos os Canais");

            // Adicionar cada canal de fato (ignorando o canal técnico de rascunhos)
            if (Player.Current.Channels != null)
            {
                foreach (var channel in Player.Current.Channels)
                {
                    if (channel != Channel.UnreleasedVideos)
                    {
                        cbActiveChannelFilter.Items.Add(channel.Name);
                    }
                }
            }

            // Restaurar seleção
            int indexToSelect = 0;
            if (!string.IsNullOrEmpty(selectedChannelName))
            {
                for (int i = 0; i < cbActiveChannelFilter.Items.Count; i++)
                {
                    if (cbActiveChannelFilter.Items[i].ToString() == selectedChannelName)
                    {
                        indexToSelect = i;
                        break;
                    }
                }
            }

            cbActiveChannelFilter.SelectedIndex = indexToSelect;
            cbActiveChannelFilter.SelectionChanged += cbActiveChannelFilter_SelectionChanged;
        }

        private void cbActiveChannelFilter_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateSummaryStatsFiltered();
        }

        private void GenerateDaySocialComments()
        {
            try
            {
                socialFeedContainer.Children.Clear();
                Random rand = new Random();

                // Generate 4 randomized recent feedback items
                for (int i = 0; i < 4; i++)
                {
                    string name = _fanNames[rand.Next(_fanNames.Length)];
                    string comment = _fanComments[rand.Next(_fanComments.Length)];
                    int minutesAgo = rand.Next(1, 60);

                    Border bubble = new Border
                    {
                        Background = new SolidColorBrush(ColorConverter.ConvertFromString("#181818") as Color? ?? Colors.DarkGray),
                        CornerRadius = new CornerRadius(8),
                        Padding = new Thickness(10),
                        Margin = new Thickness(0, 0, 0, 10),
                        BorderBrush = new SolidColorBrush(ColorConverter.ConvertFromString("#262626") as Color? ?? Colors.Gray),
                        BorderThickness = new Thickness(1)
                    };

                    StackPanel inner = new StackPanel();
                    
                    Grid header = new Grid();
                    header.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
                    header.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });

                    TextBlock txtUser = new TextBlock
                    {
                        Text = "👤 " + name + ":",
                        FontSize = 11,
                        FontWeight = FontWeights.Bold,
                        Foreground = new SolidColorBrush(ColorConverter.ConvertFromString("#FF2222") as Color? ?? Colors.Red),
                        Margin = new Thickness(0, 0, 5, 0)
                    };
                    Grid.SetColumn(txtUser, 0);
                    header.Children.Add(txtUser);

                    TextBlock txtTime = new TextBlock
                    {
                        Text = minutesAgo + " min atrás",
                        FontSize = 9,
                        Foreground = new SolidColorBrush(ColorConverter.ConvertFromString("#555555") as Color? ?? Colors.Gray),
                        HorizontalAlignment = HorizontalAlignment.Right
                    };
                    Grid.SetColumn(txtTime, 1);
                    header.Children.Add(txtTime);

                    inner.Children.Add(header);

                    TextBlock txtMsg = new TextBlock
                    {
                        Text = comment,
                        FontSize = 11,
                        Foreground = new SolidColorBrush(ColorConverter.ConvertFromString("#DDDDDD") as Color? ?? Colors.White),
                        Margin = new Thickness(0, 4, 0, 0),
                        TextWrapping = TextWrapping.Wrap
                    };
                    inner.Children.Add(txtMsg);

                    bubble.Child = inner;
                    socialFeedContainer.Children.Add(bubble);
                }
            }
            catch
            {
            }
        }

        private void ProfileCard_Click(object sender, MouseButtonEventArgs e)
        {
            // Open character edit dialog to customize aesthetic mid-game!
            Xceed.Wpf.Toolkit.ChildWindow dialog = new Xceed.Wpf.Toolkit.ChildWindow
            {
                Caption = "Personalizar Aparência Gamer",
                Width = 420,
                Height = 480,
                WindowStartupLocation = Xceed.Wpf.Toolkit.WindowStartupLocation.Center,
                Background = new SolidColorBrush(ColorConverter.ConvertFromString("#121212") as Color? ?? Colors.Black),
                BorderBrush = new SolidColorBrush(ColorConverter.ConvertFromString("#2E2E2E") as Color? ?? Colors.Gray),
                BorderThickness = new Thickness(1.5)
            };

            StackPanel panel = new StackPanel { Margin = new Thickness(15) };

            TextBlock lblTitle = new TextBlock { Text = "PERSONALIZAR MEU ESTILO", FontSize = 16, FontWeight = FontWeights.Bold, Foreground = new SolidColorBrush(Colors.Red), Margin = new Thickness(0, 0, 0, 10) };
            panel.Children.Add(lblTitle);

            // Cabelo
            // Seleção de Avatar
            TextBlock lblAvatar = new TextBlock { Text = "Selecione seu Avatar Gamer AAA", FontSize = 11, Foreground = new SolidColorBrush(Colors.Gray), Margin = new Thickness(0, 5, 0, 4) };
            panel.Children.Add(lblAvatar);

            ComboBox cbAvatar = new ComboBox { Height = 28, Margin = new Thickness(0, 0, 0, 15) };
            string[] avatarNames = new string[] {
                "Mauricinho Executivo",
                "Mauricinho Casual",
                "Empresário Jovem",
                "Empresário Maduro",
                "Trap Messy Pink",
                "Trap Hip-Hop",
                "Casual E-boy",
                "Casual Rocker",
                "Casual Cyberpunk",
                "Casual Contemporâneo",
                "Trap Platinado",
                "Casual Cachos",
                "Casual Bigode",
                "Casual Óculos",
                "Casual Jaqueta Couro"
            };

            for (int i = 0; i < avatarNames.Length; i++)
            {
                cbAvatar.Items.Add(avatarNames[i]);
            }

            cbAvatar.SelectedIndex = Math.Min(14, Math.Max(0, Player.Current.YoutuberAvatarId));
            panel.Children.Add(cbAvatar);

            Button btnSave = new Button { Content = "SALVAR ESTILO GAMER", Height = 35, Background = new SolidColorBrush(Colors.Red), Foreground = new SolidColorBrush(Colors.White), FontWeight = FontWeights.Bold };
            btnSave.Click += (s, ev) =>
            {
                int index = cbAvatar.SelectedIndex;
                if (index < 0 || index >= 15) index = 0;

                string[] avatarGlowColors = new string[] {
                    "#FFDD44", "#00FFFF", "#FF2222", "#FF5500", "#FF00FF", "#FF2222", "#00FF00", "#FFFFFF", "#00FF00", "#8A2BE2",
                    "#FFFFFF", "#FF00FF", "#00FFFF", "#FF5500", "#FFFFFF"
                };
                string[] avatarStyles = new string[] {
                    "Terno Premium Cinza", "Suéter de Lã Azul Premium", "Terno Azul de Fino Trato", "Camisa Preta de Sucesso",
                    "Jaqueta Puffer Preta de Grife", "Jaqueta Bomber Vermelha", "Jaqueta Jeans com Ovelha", "Jaqueta de Couro Rocker",
                    "Windbreaker Neon Militar", "Moletom Oversized Branco",
                    "Jaqueta Corta Vento & Correntes", "Camiseta Branca Básica de Grife", "Camiseta Algodão Egípcio Branca", "Camisa Flanela Xadrez Clássica", "Jaqueta de Couro Rocker Preta"
                };
                string[] avatarAccessories = new string[] {
                    "Óculos Escuros Italianos", "Colarinho de Lã Fina", "Gravata de Seda Fina", "Gola Aberta Confiante",
                    "Corrente de Ouro com Pingente $", "Correntes de Prata de Grife", "Cabelo Longo Despojado", "Cabelo Longo Platinado",
                    "Windbreaker Neon de Rua", "Corte Degradê Contemporâneo",
                    "Correntes de Prata Premium", "Fundo Rosa Alegre Contemporâneo", "Visual Despojado Confiante", "Óculos de Grau Intelectuais", "Camiseta Preta Básica"
                };
                string[] avatarTattoos = new string[] {
                    "Nenhuma (Barba Alinhada)", "Nenhuma (Cabelo Social)", "Nenhuma (Fade Degradê)", "Nenhuma (Barba Cerrada)",
                    "Trevo no Pescoço & Face", "Tranças com Dreads Dourados", "Colar de Couro (Estilo Indie)", "Cavanhaque Moderno",
                    "Riscos no Buzzcut Verde", "Barba de Linha Fina",
                    "Loiro Platinado & Tatuado", "Cachos Naturais & Aparelho", "Cabelo Ondulado & Bigode Retrô", "Cabelo Social & Barba Curta", "Expressão Confusa & Atitude"
                };

                Player.Current.YoutuberAvatarId = index;
                Player.Current.YoutuberHairColor = avatarGlowColors[index];
                Player.Current.YoutuberOutfit = avatarStyles[index];
                Player.Current.YoutuberAccessories = avatarAccessories[index];
                Player.Current.YoutuberTattoos = avatarTattoos[index];

                UpdateYoutuberProfile();
                dialog.Close();
            };
            panel.Children.Add(btnSave);

            dialog.Content = panel;
            dialog.ShowDialog();
        }

        private void CheckTrophies()
        {
            foreach (var channel in Player.Current.Channels)
            {
                int heelCount = 0;
                int rantCount = 0;
                int nerdCount = 0;
                foreach (var video in channel.Videos)
                {
                    if (!TrophyManager.HasTrophy(Trophy.InternetFamous) && video.Views >= 100000)
                        TrophyManager.UnlockTrophy(Trophy.InternetFamous);

                    if (!TrophyManager.HasTrophy(Trophy.PropDepartment) && video.Cost == 400)
                        TrophyManager.UnlockTrophy(Trophy.PropDepartment);

                    if (!TrophyManager.HasTrophy(Trophy.WellHeeld) && video.Category == VideoCategory.Hauls && video.Name.ToLower().Contains("heels"))
                        heelCount++;

                    if (!TrophyManager.HasTrophy(Trophy.RantMaster) && video.Category == VideoCategory.Vlog && video.Name.ToLower().Contains("rant"))
                        rantCount++;

                    if (!TrophyManager.HasTrophy(Trophy.Procrastinator) && video.Category == VideoCategory.Gaming && video.Name.ToLower().Contains("nerd"))
                        nerdCount++;
                }

                if (heelCount >= 5)
                    TrophyManager.UnlockTrophy(Trophy.WellHeeld);

                if (rantCount >= 5)
                    TrophyManager.UnlockTrophy(Trophy.RantMaster);

                if (nerdCount >= 3)
                    TrophyManager.UnlockTrophy(Trophy.Procrastinator);
            }

            if (!TrophyManager.HasTrophy(Trophy.AptPupil))
            {
                bool completed = true;
                foreach (var study in Studies.Current.All)
                {
                    if (!study.IsCompleted)
                    {
                        completed = false;
                        break;
                    }
                }

                if (completed)
                    TrophyManager.UnlockTrophy(Trophy.AptPupil);
            }
        }

        private void btnDailyPlanner_Click(object sender, RoutedEventArgs e)
        {
            btnDailyPlanner.IsChecked = true;
            btnVideoPlanner.IsChecked = false;
            btnOnlineStore.IsChecked = false;
            btnRivals.IsChecked = false;

            dailyPlanner.Visibility = Visibility.Visible;
            videoManager.Visibility = Visibility.Collapsed;
            onlineStore.Visibility = Visibility.Collapsed;
            rivalViewer.Visibility = Visibility.Collapsed;
        }

        private void btnVideoPlanner_Click(object sender, RoutedEventArgs e)
        {
            btnDailyPlanner.IsChecked = false;
            btnVideoPlanner.IsChecked = true;
            btnOnlineStore.IsChecked = false;
            btnRivals.IsChecked = false;

            dailyPlanner.Visibility = Visibility.Collapsed;
            videoManager.Visibility = Visibility.Visible;
            onlineStore.Visibility = Visibility.Collapsed;
            rivalViewer.Visibility = Visibility.Collapsed;
            videoManager.Update();
        }

        private void btnOnlineStore_Click(object sender, RoutedEventArgs e)
        {
            btnDailyPlanner.IsChecked = false;
            btnVideoPlanner.IsChecked = false;
            btnOnlineStore.IsChecked = true;
            btnRivals.IsChecked = false;

            dailyPlanner.Visibility = Visibility.Collapsed;
            videoManager.Visibility = Visibility.Collapsed;
            onlineStore.Visibility = Visibility.Visible;
            rivalViewer.Visibility = Visibility.Collapsed;
            onlineStore.Update();
        }

        private void btnRivals_Click(object sender, RoutedEventArgs e)
        {
            btnDailyPlanner.IsChecked = false;
            btnVideoPlanner.IsChecked = false;
            btnOnlineStore.IsChecked = false;
            btnRivals.IsChecked = true;

            dailyPlanner.Visibility = Visibility.Collapsed;
            videoManager.Visibility = Visibility.Collapsed;
            onlineStore.Visibility = Visibility.Collapsed;
            rivalViewer.Visibility = Visibility.Visible;
            rivalViewer.Update();
        }

        private void btnNewDay_Click(object sender, RoutedEventArgs e)
        {
            this.IsEnabled = false;
            progress.Visibility = System.Windows.Visibility.Visible;

            dailyPlanner.NewDay();
            rivalViewer.NewDay();
        }

        private void Tutorial_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (!TrophyManager.HasTrophy(Trophy.Pupil))
                TrophyManager.UnlockTrophy(Trophy.Pupil);

            System.Diagnostics.Process.Start("http://www.youtube.com/watch?v=oKfNSm1SLSQ");
        }

        private void Save_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (!Directory.Exists(SaveLoadHelper.SaveDirectory))
            {
                Directory.CreateDirectory(SaveLoadHelper.SaveDirectory);
            }

            if (File.Exists(SaveLoadHelper.SaveFile))
            {
                CustomMessageBox.ShowDialog(EnglishStrings.OverwriteSave.Translate(), EnglishStrings.SaveExists.Translate(), MessagePicture.Question, (result) =>
                    {
                        if (result == true)
                            DoSave();
                    });
            }
            else
            {
                DoSave();
            }
        }

        private void DoSave()
        {
            dailyPlanner.Appointments.ForEach(a => a.HoursPutIn--);
            SaveLoadHelper.Save(SaveLoadHelper.SaveFile);
            dailyPlanner.Appointments.ForEach(a => a.HoursPutIn++);
        }

        private void Exit_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            CustomMessageBox.ShowDialog(EnglishStrings.Confirm.Translate(), EnglishStrings.LeaveGame.Translate(), MessagePicture.Question, (result) =>
            {
                if (result == true)
                {
                    if (GameExit != null)
                        GameExit();
                }
            });
        }

        private void imgSave_MouseEnter(object sender, MouseEventArgs e)
        {
            imgSave.Source = new BitmapImage(new Uri("Resources/Disk_hover.png", UriKind.Relative));
        }

        private void imgSave_MouseLeave(object sender, MouseEventArgs e)
        {
            imgSave.Source = new BitmapImage(new Uri("Resources/Disk.png", UriKind.Relative));
        }

        private void gridHelp_MouseEnter(object sender, MouseEventArgs e)
        {
            gridHelp.Source = new BitmapImage(new Uri("Resources/Help_hover.png", UriKind.Relative));
        }

        private void gridHelp_MouseLeave(object sender, MouseEventArgs e)
        {
            gridHelp.Source = new BitmapImage(new Uri("Resources/Help.png", UriKind.Relative));
        }

        private void gridExit_MouseEnter(object sender, MouseEventArgs e)
        {
            gridExit.Source = new BitmapImage(new Uri("Resources/Exit_hover.png", UriKind.Relative));
        }

        private void gridExit_MouseLeave(object sender, MouseEventArgs e)
        {
            gridExit.Source = new BitmapImage(new Uri("Resources/Exit.png", UriKind.Relative));
        }

        private void btnStockMarket_Click(object sender, RoutedEventArgs e)
        {
            btnStockMarket.IsChecked = false; // Manter desmarcado de forma tátil
            OpenStockMarketDialog();
        }

        private void OpenStockMarketDialog()
        {
            // Garantir que a Bolsa esteja inicializada
            StockMarketManager.InitializeOrLoad();

            Xceed.Wpf.Toolkit.ChildWindow dialog = new Xceed.Wpf.Toolkit.ChildWindow
            {
                Caption = "Bolsa de Valores - Tube Star Index (TSI)",
                Width = 730,
                Height = 530,
                WindowStartupLocation = Xceed.Wpf.Toolkit.WindowStartupLocation.Center,
                Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#121212")),
                BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#2E2E2E")),
                BorderThickness = new Thickness(1.5)
            };

            Grid mainGrid = new Grid();
            mainGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(285) });
            mainGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });

            // COLUNA ESQUERDA: LISTA DE AÇÕES
            Border leftBorder = new Border
            {
                BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#222222")),
                BorderThickness = new Thickness(0, 0, 1, 0),
                Padding = new Thickness(10)
            };
            mainGrid.Children.Add(leftBorder);
            Grid.SetColumn(leftBorder, 0);

            StackPanel leftPanel = new StackPanel();
            leftBorder.Child = leftPanel;

            TextBlock lblBolsaTitle = new TextBlock
            {
                Text = "COTAÇÕES DO DIA",
                FontSize = 13,
                FontWeight = FontWeights.Black,
                Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF2222")),
                Margin = new Thickness(0, 0, 0, 12),
                HorizontalAlignment = HorizontalAlignment.Center
            };
            leftPanel.Children.Add(lblBolsaTitle);

            // Container para a lista de botões/cards das empresas
            StackPanel listContainer = new StackPanel();
            leftPanel.Children.Add(listContainer);

            // COLUNA DIREITA: DETALHE DA AÇÃO SELECIONADA
            Border rightBorder = new Border { Padding = new Thickness(15) };
            mainGrid.Children.Add(rightBorder);
            Grid.SetColumn(rightBorder, 1);

            Grid rightGrid = new Grid();
            rightBorder.Child = rightGrid;
            rightGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto }); // Header (Nome/Preço)
            rightGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto }); // Gráfico
            rightGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto }); // Notícias
            rightGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) }); // Transação

            // Header Detalhado
            StackPanel rightHeader = new StackPanel { Margin = new Thickness(0, 0, 0, 10) };
            rightGrid.Children.Add(rightHeader);
            Grid.SetRow(rightHeader, 0);

            TextBlock txtCompName = new TextBlock { FontSize = 15, FontWeight = FontWeights.Bold, Foreground = Brushes.White };
            TextBlock txtCompTickerPrice = new TextBlock { FontSize = 12, FontWeight = FontWeights.Bold, Margin = new Thickness(0, 2, 0, 0) };
            rightHeader.Children.Add(txtCompName);
            rightHeader.Children.Add(txtCompTickerPrice);

            // Gráfico Sparkline
            Border chartBorder = new Border
            {
                Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#0D0D0D")),
                BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#222222")),
                BorderThickness = new Thickness(1),
                CornerRadius = new CornerRadius(6),
                Height = 150,
                Margin = new Thickness(0, 0, 0, 12),
                Padding = new Thickness(5)
            };
            rightGrid.Children.Add(chartBorder);
            Grid.SetRow(chartBorder, 1);

            Canvas chartCanvas = new Canvas { ClipToBounds = true, Width = 370, Height = 140 };
            chartBorder.Child = chartCanvas;

            // Feed de Notícia
            Border newsBorder = new Border
            {
                Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#181818")),
                BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#2E2E2E")),
                BorderThickness = new Thickness(1),
                CornerRadius = new CornerRadius(6),
                Padding = new Thickness(10),
                Margin = new Thickness(0, 0, 0, 12)
            };
            rightGrid.Children.Add(newsBorder);
            Grid.SetRow(newsBorder, 2);

            StackPanel newsStack = new StackPanel();
            newsBorder.Child = newsStack;

            TextBlock lblNewsHeader = new TextBlock { Text = "📰 BOLSA NEWS", FontSize = 11, FontWeight = FontWeights.Bold, Foreground = Brushes.Red, Margin = new Thickness(0, 0, 0, 4) };
            TextBlock txtNewsContent = new TextBlock { FontSize = 11, Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#DDDDDD")), TextWrapping = TextWrapping.Wrap, FontStyle = FontStyles.Italic };
            newsStack.Children.Add(lblNewsHeader);
            newsStack.Children.Add(txtNewsContent);

            // Portfólio & Transações
            Border tradeBorder = new Border
            {
                Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#1A1A1A")),
                BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#2E2E2E")),
                BorderThickness = new Thickness(1),
                CornerRadius = new CornerRadius(8),
                Padding = new Thickness(12)
            };
            rightGrid.Children.Add(tradeBorder);
            Grid.SetRow(tradeBorder, 3);

            Grid tradeGrid = new Grid();
            tradeBorder.Child = tradeGrid;
            tradeGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto }); // Stats
            tradeGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto }); // Slider
            tradeGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) }); // Buttons

            // Stats
            Grid statsGrid = new Grid { Margin = new Thickness(0, 0, 0, 10) };
            statsGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            statsGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            tradeGrid.Children.Add(statsGrid);
            Grid.SetRow(statsGrid, 0);

            StackPanel pLeft = new StackPanel();
            TextBlock txtMyShares = new TextBlock { FontSize = 11, Foreground = Brushes.Gray, FontWeight = FontWeights.Bold };
            TextBlock txtMyAverage = new TextBlock { FontSize = 11, Foreground = Brushes.Gray, FontWeight = FontWeights.Bold, Margin = new Thickness(0, 3, 0, 0) };
            pLeft.Children.Add(txtMyShares);
            pLeft.Children.Add(txtMyAverage);
            statsGrid.Children.Add(pLeft);
            Grid.SetColumn(pLeft, 0);

            StackPanel pRight = new StackPanel { HorizontalAlignment = HorizontalAlignment.Right };
            TextBlock txtMyReturn = new TextBlock { FontSize = 11, Foreground = Brushes.Gray, FontWeight = FontWeights.Bold, HorizontalAlignment = HorizontalAlignment.Right };
            TextBlock txtMyMoney = new TextBlock { Text = "Saldo: " + Player.Current.Money.ToCurrencyString(), FontSize = 12, FontWeight = FontWeights.Bold, Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#00FF00")), Margin = new Thickness(0, 3, 0, 0), HorizontalAlignment = HorizontalAlignment.Right };
            pRight.Children.Add(txtMyReturn);
            pRight.Children.Add(txtMyMoney);
            statsGrid.Children.Add(pRight);
            Grid.SetColumn(pRight, 1);

            // Slider & Input de Quantidade
            Grid sliderGrid = new Grid { Margin = new Thickness(0, 0, 0, 12) };
            sliderGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            sliderGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
            tradeGrid.Children.Add(sliderGrid);
            Grid.SetRow(sliderGrid, 1);

            Slider tradeSlider = new Slider { Minimum = 1, Maximum = 10, Value = 1, IsSnapToTickEnabled = true, TickFrequency = 1, Margin = new Thickness(0, 0, 15, 0), VerticalAlignment = VerticalAlignment.Center };
            TextBlock txtTradeQty = new TextBlock { Text = "1 cota", FontSize = 14, FontWeight = FontWeights.Bold, Foreground = Brushes.White, HorizontalAlignment = HorizontalAlignment.Center, VerticalAlignment = VerticalAlignment.Center };
            sliderGrid.Children.Add(tradeSlider);
            sliderGrid.Children.Add(txtTradeQty);
            Grid.SetColumn(tradeSlider, 0);
            Grid.SetColumn(txtTradeQty, 1);

            // Botoes
            Grid btnGrid = new Grid();
            btnGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            btnGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            tradeGrid.Children.Add(btnGrid);
            Grid.SetRow(btnGrid, 2);

            Button btnBuy = new Button { Content = "COMPRAR", Margin = new Thickness(0, 0, 5, 0), Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#008800")), Foreground = Brushes.White, FontWeight = FontWeights.Bold, FontSize = 13, Height = 36 };
            Button btnSell = new Button { Content = "VENDER", Margin = new Thickness(5, 0, 0, 0), Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#AA0000")), Foreground = Brushes.White, FontWeight = FontWeights.Bold, FontSize = 13, Height = 36 };
            btnGrid.Children.Add(btnBuy);
            btnGrid.Children.Add(btnSell);
            Grid.SetColumn(btnBuy, 0);
            Grid.SetColumn(btnSell, 1);

            StockCompany selectedComp = StockMarketManager.Companies[0];

            // Método local para atualizar tudo na tela com base na empresa ativa selecionada
            Action updateUI = null;
            updateUI = () =>
            {
                // Dados do Portfólio
                int qty = 0;
                double avg = 0;
                if (selectedComp.Ticker == "STB") { qty = Player.Current.SharesSTB; avg = Player.Current.PricePaidSTB; }
                else if (selectedComp.Ticker == "PEAR") { qty = Player.Current.SharesPEAR; avg = Player.Current.PricePaidPEAR; }
                else if (selectedComp.Ticker == "RVG") { qty = Player.Current.SharesRVG; avg = Player.Current.PricePaidRVG; }
                else if (selectedComp.Ticker == "GDR") { qty = Player.Current.SharesGDR; avg = Player.Current.PricePaidGDR; }
                else if (selectedComp.Ticker == "WHP") { qty = Player.Current.SharesWHP; avg = Player.Current.PricePaidWHP; }

                txtCompName.Text = selectedComp.Name.ToUpper();
                bool compIsUp = selectedComp.ChangePercentage >= 0;
                string changeSign = compIsUp ? "+" : "";
                string changeHex = compIsUp ? "#00FF00" : "#FF2222";
                txtCompTickerPrice.Text = string.Format("{0}  |  PREÇO: {1}  ({2}{3}%)", 
                    selectedComp.Ticker, 
                    selectedComp.CurrentPrice.ToCurrencyString(), 
                    changeSign, 
                    selectedComp.ChangePercentage);
                txtCompTickerPrice.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString(changeHex));

                // Desenhar gráfico
                DrawStockChart(chartCanvas, selectedComp);

                // Notícias
                txtNewsContent.Text = selectedComp.DailyNews;

                // Portfólio
                txtMyShares.Text = "Cotas Possuídas: " + qty;
                txtMyAverage.Text = "Preço Médio Pago: " + (avg > 0 ? avg.ToCurrencyString() : "-");

                double returnVal = 0;
                if (qty > 0)
                {
                    returnVal = (selectedComp.CurrentPrice - avg) * qty;
                    txtMyReturn.Text = "Retorno Atual: " + (returnVal >= 0 ? "+" : "") + returnVal.ToCurrencyString();
                    txtMyReturn.Foreground = new SolidColorBrush(returnVal >= 0 ? (Color)ColorConverter.ConvertFromString("#00FF00") : (Color)ColorConverter.ConvertFromString("#FF2222"));
                }
                else
                {
                    txtMyReturn.Text = "Retorno Atual: -";
                    txtMyReturn.Foreground = Brushes.Gray;
                }

                txtMyMoney.Text = "Saldo Disponível: " + Player.Current.Money.ToCurrencyString();

                // Limites do Slider
                int maxBuy = (int)(Player.Current.Money / selectedComp.CurrentPrice);
                int maxSell = qty;

                int sliderMax = Math.Max(maxBuy, maxSell);
                if (sliderMax < 1) sliderMax = 1;

                tradeSlider.Maximum = sliderMax;
                if (tradeSlider.Value > tradeSlider.Maximum) tradeSlider.Value = tradeSlider.Maximum;
                if (tradeSlider.Value < 1) tradeSlider.Value = 1;
                txtTradeQty.Text = string.Format("{0} cota{1}", (int)tradeSlider.Value, (int)tradeSlider.Value > 1 ? "s" : "");

                // Habilitar / desabilitar botões
                int currentTradeQty = (int)tradeSlider.Value;
                btnBuy.IsEnabled = (Player.Current.Money >= (selectedComp.CurrentPrice * currentTradeQty));
                btnSell.IsEnabled = (qty >= currentTradeQty);
                btnBuy.Opacity = btnBuy.IsEnabled ? 1.0 : 0.4;
                btnSell.Opacity = btnSell.IsEnabled ? 1.0 : 0.4;
            };

            // Listener do Slider
            tradeSlider.ValueChanged += (s, ev) =>
            {
                int currentTradeQty = (int)tradeSlider.Value;
                txtTradeQty.Text = string.Format("{0} cota{1}", currentTradeQty, currentTradeQty > 1 ? "s" : "");
                
                int qty = 0;
                if (selectedComp.Ticker == "STB") qty = Player.Current.SharesSTB;
                else if (selectedComp.Ticker == "PEAR") qty = Player.Current.SharesPEAR;
                else if (selectedComp.Ticker == "RVG") qty = Player.Current.SharesRVG;
                else if (selectedComp.Ticker == "GDR") qty = Player.Current.SharesGDR;
                else if (selectedComp.Ticker == "WHP") qty = Player.Current.SharesWHP;

                btnBuy.IsEnabled = (Player.Current.Money >= (selectedComp.CurrentPrice * currentTradeQty));
                btnSell.IsEnabled = (qty >= currentTradeQty);
                btnBuy.Opacity = btnBuy.IsEnabled ? 1.0 : 0.4;
                btnSell.Opacity = btnSell.IsEnabled ? 1.0 : 0.4;
            };

            // Criar dinamicamente os cards de ações na esquerda
            Action populateCompaniesList = null;
            populateCompaniesList = () =>
            {
                listContainer.Children.Clear();
                foreach (var comp in StockMarketManager.Companies)
                {
                    Border card = new Border
                    {
                        Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#1A1A1A")),
                        BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString(comp == selectedComp ? "#FF2222" : "#2E2E2E")),
                        BorderThickness = new Thickness(comp == selectedComp ? 1.5 : 1),
                        CornerRadius = new CornerRadius(6),
                        Padding = new Thickness(8),
                        Margin = new Thickness(0, 3, 0, 3),
                        Cursor = Cursors.Hand
                    };

                    Grid cardGrid = new Grid();
                    cardGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
                    cardGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
                    card.Child = cardGrid;

                    StackPanel cLeft = new StackPanel();
                    TextBlock cTicker = new TextBlock { Text = comp.Ticker, FontSize = 12, FontWeight = FontWeights.Bold, Foreground = Brushes.White };
                    TextBlock cName = new TextBlock { Text = comp.Name, FontSize = 9, Foreground = Brushes.Gray };
                    cLeft.Children.Add(cTicker);
                    cLeft.Children.Add(cName);
                    cardGrid.Children.Add(cLeft);
                    Grid.SetColumn(cLeft, 0);

                    StackPanel cRight = new StackPanel { HorizontalAlignment = HorizontalAlignment.Right };
                    TextBlock cPrice = new TextBlock { Text = comp.CurrentPrice.ToCurrencyString(), FontSize = 11, FontWeight = FontWeights.Bold, Foreground = Brushes.White, HorizontalAlignment = HorizontalAlignment.Right };
                    bool compUp = comp.ChangePercentage >= 0;
                    string sign = compUp ? "+" : "";
                    string colorHex = compUp ? "#00FF00" : "#FF2222";
                    TextBlock cChange = new TextBlock
                    {
                        Text = string.Format("{0}{1}%", sign, comp.ChangePercentage),
                        FontSize = 10,
                        FontWeight = FontWeights.Bold,
                        Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString(colorHex)),
                        HorizontalAlignment = HorizontalAlignment.Right,
                        Margin = new Thickness(0, 1, 0, 0)
                    };
                    cRight.Children.Add(cPrice);
                    cRight.Children.Add(cChange);
                    cardGrid.Children.Add(cRight);
                    Grid.SetColumn(cRight, 1);

                    var currentComp = comp;
                    card.MouseLeftButtonDown += (s, ev) =>
                    {
                        selectedComp = currentComp;
                        updateUI();
                        populateCompaniesList();
                    };

                    listContainer.Children.Add(card);
                }
            };

            // Configurar Ações de Transações
            Action<int, double> saveTransaction = (newQty, newPricePaid) =>
            {
                if (selectedComp.Ticker == "STB") { Player.Current.SharesSTB = newQty; Player.Current.PricePaidSTB = newPricePaid; }
                else if (selectedComp.Ticker == "PEAR") { Player.Current.SharesPEAR = newQty; Player.Current.PricePaidPEAR = newPricePaid; }
                else if (selectedComp.Ticker == "RVG") { Player.Current.SharesRVG = newQty; Player.Current.PricePaidRVG = newPricePaid; }
                else if (selectedComp.Ticker == "GDR") { Player.Current.SharesGDR = newQty; Player.Current.PricePaidGDR = newPricePaid; }
                else if (selectedComp.Ticker == "WHP") { Player.Current.SharesWHP = newQty; Player.Current.PricePaidWHP = newPricePaid; }
                StockMarketManager.SaveState();
                UpdateSummaryStats();
            };

            btnBuy.Click += (s, ev) =>
            {
                int qtyToBuy = (int)tradeSlider.Value;
                double totalCost = qtyToBuy * selectedComp.CurrentPrice;
                if (Player.Current.Money >= totalCost)
                {
                    Player.Current.Money -= totalCost;

                    int qty = 0;
                    double avg = 0;
                    if (selectedComp.Ticker == "STB") { qty = Player.Current.SharesSTB; avg = Player.Current.PricePaidSTB; }
                    else if (selectedComp.Ticker == "PEAR") { qty = Player.Current.SharesPEAR; avg = Player.Current.PricePaidPEAR; }
                    else if (selectedComp.Ticker == "RVG") { qty = Player.Current.SharesRVG; avg = Player.Current.PricePaidRVG; }
                    else if (selectedComp.Ticker == "GDR") { qty = Player.Current.SharesGDR; avg = Player.Current.PricePaidGDR; }
                    else if (selectedComp.Ticker == "WHP") { qty = Player.Current.SharesWHP; avg = Player.Current.PricePaidWHP; }

                    double totalInvested = (qty * avg) + totalCost;
                    int newQty = qty + qtyToBuy;
                    double newAvg = Math.Round(totalInvested / newQty, 2);

                    saveTransaction(newQty, newAvg);
                    updateUI();
                    populateCompaniesList();
                }
            };

            btnSell.Click += (s, ev) =>
            {
                int qty = 0;
                double avg = 0;
                if (selectedComp.Ticker == "STB") { qty = Player.Current.SharesSTB; avg = Player.Current.PricePaidSTB; }
                else if (selectedComp.Ticker == "PEAR") { qty = Player.Current.SharesPEAR; avg = Player.Current.PricePaidPEAR; }
                else if (selectedComp.Ticker == "RVG") { qty = Player.Current.SharesRVG; avg = Player.Current.PricePaidRVG; }
                else if (selectedComp.Ticker == "GDR") { qty = Player.Current.SharesGDR; avg = Player.Current.PricePaidGDR; }
                else if (selectedComp.Ticker == "WHP") { qty = Player.Current.SharesWHP; avg = Player.Current.PricePaidWHP; }

                int qtyToSell = (int)tradeSlider.Value;
                if (qtyToSell > qty) qtyToSell = qty;

                if (qtyToSell > 0)
                {
                    double revenue = qtyToSell * selectedComp.CurrentPrice;
                    Player.Current.Money += revenue;

                    int newQty = qty - qtyToSell;
                    double newAvg = newQty > 0 ? avg : 0;

                    saveTransaction(newQty, newAvg);
                    updateUI();
                    populateCompaniesList();
                }
            };

            populateCompaniesList();
            updateUI();

            dialog.Content = mainGrid;
            dialog.ShowDialog();
        }

        private void DrawStockChart(Canvas canvas, StockCompany comp)
        {
            canvas.Children.Clear();
            if (comp.PriceHistory == null || comp.PriceHistory.Count < 2) return;

            double width = canvas.Width;
            double height = canvas.Height;

            if (width <= 0) width = 370;
            if (height <= 0) height = 140;

            // Desenhar linhas de grade horizontais
            for (int i = 1; i < 4; i++)
            {
                double gridY = (height / 4) * i;
                Line gridLine = new Line
                {
                    X1 = 0,
                    Y1 = gridY,
                    X2 = width,
                    Y2 = gridY,
                    Stroke = new SolidColorBrush(Color.FromArgb(20, 255, 255, 255)),
                    StrokeThickness = 1,
                    StrokeDashArray = new DoubleCollection { 4, 4 }
                };
                canvas.Children.Add(gridLine);
            }

            // Encontrar mínimo e máximo
            double min = double.MaxValue;
            double max = double.MinValue;
            foreach (var price in comp.PriceHistory)
            {
                if (price < min) min = price;
                if (price > max) max = price;
            }

            double delta = max - min;
            if (delta <= 0) delta = 1.0;

            // Criar a Polyline do gráfico
            Polyline polyline = new Polyline
            {
                StrokeThickness = 2.5,
                StrokeLineJoin = PenLineJoin.Round
            };

            bool isUp = comp.CurrentPrice >= comp.PriceHistory[0];
            Color chartColor = isUp ? (Color)ColorConverter.ConvertFromString("#00FF00") : (Color)ColorConverter.ConvertFromString("#FF2222");
            polyline.Stroke = new SolidColorBrush(chartColor);

            PointCollection points = new PointCollection();
            int count = comp.PriceHistory.Count;
            for (int i = 0; i < count; i++)
            {
                double price = comp.PriceHistory[i];
                double x = (width / (count - 1)) * i;
                double y = height - (((price - min) / delta) * (height - 30) + 15);
                points.Add(new Point(x, y));

                // Desenhar pequenos círculos
                Ellipse dot = new Ellipse
                {
                    Width = 6,
                    Height = 6,
                    Fill = new SolidColorBrush(chartColor),
                    Margin = new Thickness(x - 3, y - 3, 0, 0)
                };
                canvas.Children.Add(dot);
            }
            polyline.Points = points;
            canvas.Children.Add(polyline);

            // Rótulos de mínimo e máximo
            TextBlock lblMax = new TextBlock
            {
                Text = max.ToCurrencyString(),
                FontSize = 9,
                Foreground = new SolidColorBrush(Color.FromArgb(140, 255, 255, 255)),
                Margin = new Thickness(5, 5, 0, 0)
            };
            canvas.Children.Add(lblMax);

            TextBlock lblMin = new TextBlock
            {
                Text = min.ToCurrencyString(),
                FontSize = 9,
                Foreground = new SolidColorBrush(Color.FromArgb(140, 255, 255, 255)),
                Margin = new Thickness(5, height - 18, 0, 0)
            };
            canvas.Children.Add(lblMin);
        }
    }
}