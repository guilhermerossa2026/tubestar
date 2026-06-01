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
                        "pack://application:,,,/TubeStarDesktop;component/Resources/avatar_mauricinho_1.png",
                        "pack://application:,,,/TubeStarDesktop;component/Resources/avatar_mauricinho_2.png",
                        "pack://application:,,,/TubeStarDesktop;component/Resources/avatar_mauricinho_3.png",
                        "pack://application:,,,/TubeStarDesktop;component/Resources/avatar_mauricinho_4.png",
                        "pack://application:,,,/TubeStarDesktop;component/Resources/avatar_gangstar_1.png",
                        "pack://application:,,,/TubeStarDesktop;component/Resources/avatar_gangstar_2.png",
                        "pack://application:,,,/TubeStarDesktop;component/Resources/avatar_cabelolongo_1.png",
                        "pack://application:,,,/TubeStarDesktop;component/Resources/avatar_cabelolongo_2.png",
                        "pack://application:,,,/TubeStarDesktop;component/Resources/avatar_cabelocurto_1.png",
                        "pack://application:,,,/TubeStarDesktop;component/Resources/avatar_cabelocurto_2.png",
                        "pack://application:,,,/TubeStarDesktop;component/Resources/avatar_trap_3.jpg",
                        "pack://application:,,,/TubeStarDesktop;component/Resources/avatar_casual_3.png",
                        "pack://application:,,,/TubeStarDesktop;component/Resources/avatar_casual_4.png",
                        "pack://application:,,,/TubeStarDesktop;component/Resources/avatar_casual_5.png",
                        "pack://application:,,,/TubeStarDesktop;component/Resources/avatar_casual_6.png"
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

            // Adicionar cada canal de fato
            if (Player.Current.Channels != null)
            {
                foreach (var channel in Player.Current.Channels)
                {
                    cbActiveChannelFilter.Items.Add(channel.Name);
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
    }
}