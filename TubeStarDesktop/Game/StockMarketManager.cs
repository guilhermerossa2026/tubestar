using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace TubeStar
{
    public class StockCompany
    {
        public string Ticker { get; set; }
        public string Name { get; set; }
        public double CurrentPrice { get; set; }
        public double ChangePercentage { get; set; }
        public double BaseVolatility { get; set; }
        public double DividendYield { get; set; }
        public List<double> PriceHistory { get; set; }
        public string DailyNews { get; set; }

        public StockCompany()
        {
            PriceHistory = new List<double>();
        }
    }

    public static class StockMarketManager
    {
        public static List<StockCompany> Companies { get; set; }

        private static readonly Random _rnd = new Random();

        private static readonly string[] _positiveNewsSTB = new string[] {
            "StarTube bate recorde de criadores ativos, gerando forte otimismo dos anunciantes.",
            "Novo algoritmo de recomendação do StarTube aumenta engajamento de vídeos em 30%.",
            "Rumores indicam que a StarTube anunciará taxas de monetização mais atrativas em breve."
        };

        private static readonly string[] _negativeNewsSTB = new string[] {
            "Criadores protestam contra nova diretriz de direitos autorais do StarTube.",
            "Instabilidade global nos servidores do StarTube derruba acessos no horário de pico.",
            "Relatórios apontam queda de receita publicitária na plataforma da StarTube."
        };

        private static readonly string[] _positiveNewsPEAR = new string[] {
            "Pear Corp lança nova câmera DSLR 8K voltada a profissionais e YouTubers.",
            "Sucesso de vendas do novo computador Pear Pro impulsiona ações da empresa.",
            "Analistas recomendam compra forte de PEAR após resultados financeiros sólidos."
        };

        private static readonly string[] _negativeNewsPEAR = new string[] {
            "Falha de lote na nova linha de microfones da Pear Corp gera recall massivo.",
            "Pear Corp enfrenta atrasos na cadeia de suprimentos de seus computadores premium.",
            "Processo antitruste internacional faz ações da Pear Corp recuarem."
        };

        private static readonly string[] _positiveNewsRVG = new string[] {
            "Rivalry Games anuncia lançamento do aguardado RPG 'Legends of Tube' para o próximo mês.",
            "Novo jogo multiplayer da Rivalry Games bate 2 milhões de jogadores ativos simultâneos.",
            "Analistas preveem recorde de lucros para a Rivalry Games com vendas de fim de ano."
        };

        private static readonly string[] _negativeNewsRVG = new string[] {
            "Rivalry Games anuncia adiamento indefinido do seu jogo mais aguardado.",
            "Lançamento recente da Rivalry Games recebe críticas severas devido a bugs críticos.",
            "Jogadores boicotam Rivalry Games após anúncio de microtransações excessivas."
        };

        private static readonly string[] _positiveNewsGDR = new string[] {
            "GamerDrink fecha parceria de patrocínio com o maior streamer de eSports do mundo.",
            "Novo sabor de energético termogênico da GamerDrink se torna sucesso de vendas de rua.",
            "GamerDrink expande rede de distribuição internacional, acelerando crescimento de receita."
        };

        private static readonly string[] _negativeNewsGDR = new string[] {
            "Pesquisas de saúde questionam os efeitos colaterais dos novos estimulantes da GamerDrink.",
            "Vazamento em fábrica da GamerDrink interrompe produção temporariamente.",
            "Campanha publicitária polêmica da GamerDrink é criticada e gera recall de anúncios."
        };

        private static readonly string[] _positiveNewsWHP = new string[] {
            "WebHosting Pro fecha contrato milionário para hospedar servidores de rede corporativa.",
            "Nova tecnologia de servidores em nuvem da WebHosting Pro dobra velocidade de carregamento.",
            "WebHosting Pro registra crescimento constante e estável no número de assinantes pro."
        };

        private static readonly string[] _negativeNewsWHP = new string[] {
            "WebHosting Pro sofre ataque DDoS massivo, deixando portais fora do ar por 3 horas.",
            "Crescimento de custos operacionais com servidores da WebHosting Pro reduz margens líquidas.",
            "Aumento na concorrência de hospedagem barata desacelera planos da WebHosting Pro."
        };

        public static void InitializeOrLoad()
        {
            if (!string.IsNullOrEmpty(Player.Current.SerializeStockPrices))
            {
                try
                {
                    Companies = JsonConvert.DeserializeObject<List<StockCompany>>(Player.Current.SerializeStockPrices);
                    if (Companies != null && Companies.Count == 5)
                    {
                        return; // Loaded successfully!
                    }
                }
                catch
                {
                    // Fallback to fresh if corrupted
                }
            }

            // Create initial state
            Companies = new List<StockCompany>
            {
                new StockCompany { Ticker = "STB", Name = "StarTube Corp.", CurrentPrice = 100.0, BaseVolatility = 0.12, DividendYield = 0.015, DailyNews = "Mercado estável na plataforma de vídeos." },
                new StockCompany { Ticker = "PEAR", Name = "Pear Corp.", CurrentPrice = 150.0, BaseVolatility = 0.04, DividendYield = 0.02, DailyNews = "Pear Corp consolida liderança em computadores premium." },
                new StockCompany { Ticker = "RVG", Name = "Rivalry Games", CurrentPrice = 60.0, BaseVolatility = 0.10, DividendYield = 0.01, DailyNews = "Rivalry Games mantém ritmo estável no mercado." },
                new StockCompany { Ticker = "GDR", Name = "GamerDrink", CurrentPrice = 35.0, BaseVolatility = 0.07, DividendYield = 0.012, DailyNews = "GamerDrink consolida marca entre público jovem." },
                new StockCompany { Ticker = "WHP", Name = "WebHosting Pro", CurrentPrice = 80.0, BaseVolatility = 0.03, DividendYield = 0.025, DailyNews = "Hospedagem segura e estável para a Web gamer." }
            };

            // Pre-populate 7 days history retrospectively
            foreach (var comp in Companies)
            {
                double price = comp.CurrentPrice;
                comp.PriceHistory.Add(Math.Round(price * 0.92, 2));
                comp.PriceHistory.Add(Math.Round(price * 0.95, 2));
                comp.PriceHistory.Add(Math.Round(price * 0.93, 2));
                comp.PriceHistory.Add(Math.Round(price * 0.97, 2));
                comp.PriceHistory.Add(Math.Round(price * 0.96, 2));
                comp.PriceHistory.Add(Math.Round(price * 0.99, 2));
                comp.PriceHistory.Add(Math.Round(price, 2));
            }

            SaveState();
        }

        public static void SaveState()
        {
            if (Companies == null) return;
            try
            {
                Player.Current.SerializeStockPrices = JsonConvert.SerializeObject(Companies);
            }
            catch { }
        }

        public static void NextDay()
        {
            if (Companies == null)
            {
                InitializeOrLoad();
            }

            // Payout Dividends
            if (Player.Current != null)
            {
                double totalDividends = 0;
                foreach (var comp in Companies)
                {
                    int shares = 0;
                    if (comp.Ticker == "STB") shares = Player.Current.SharesSTB;
                    else if (comp.Ticker == "PEAR") shares = Player.Current.SharesPEAR;
                    else if (comp.Ticker == "RVG") shares = Player.Current.SharesRVG;
                    else if (comp.Ticker == "GDR") shares = Player.Current.SharesGDR;
                    else if (comp.Ticker == "WHP") shares = Player.Current.SharesWHP;

                    if (shares > 0)
                    {
                        double payout = Math.Round(shares * comp.CurrentPrice * comp.DividendYield, 2);
                        totalDividends += payout;
                    }
                }

                if (totalDividends > 0)
                {
                    Player.Current.Money += totalDividends;
                    CustomMessageBox.ShowDialog("Dividendos Pagos! 📈", string.Format("Seu portfólio de ações pagou um total de {0} em dividendos recorrentes hoje!", totalDividends.ToCurrencyString()), MessagePicture.Money);
                }
            }

            // Decrement remaining days
            if (Player.Current != null)
            {
                if (Player.Current.SponsorSTBDays > 0) Player.Current.SponsorSTBDays--;
                if (Player.Current.SponsorPEARDays > 0) Player.Current.SponsorPEARDays--;
                if (Player.Current.SponsorRVGDays > 0) Player.Current.SponsorRVGDays--;
                if (Player.Current.SponsorGDRDays > 0) Player.Current.SponsorGDRDays--;
                if (Player.Current.SponsorWHPDays > 0) Player.Current.SponsorWHPDays--;

                if (Player.Current.TrendSTBDays > 0) Player.Current.TrendSTBDays--;
                if (Player.Current.TrendPEARDays > 0) Player.Current.TrendPEARDays--;
                if (Player.Current.TrendRVGDays > 0) Player.Current.TrendRVGDays--;
                if (Player.Current.TrendGDRDays > 0) Player.Current.TrendGDRDays--;
                if (Player.Current.TrendWHPDays > 0) Player.Current.TrendWHPDays--;
            }

            // Selecionar aleatoriamente 1 empresa para sofrer um evento de notícia de mercado (45% de chance global)
            int targetEventCompIdx = -1;
            bool isPositiveEvent = false;
            if (_rnd.NextDouble() < 0.45)
            {
                targetEventCompIdx = _rnd.Next(0, 5);
                isPositiveEvent = _rnd.NextDouble() < 0.55; // leve tendência de alta nos eventos
            }

            for (int i = 0; i < Companies.Count; i++)
            {
                var comp = Companies[i];
                double trend = 0.005; // viés leve de crescimento a longo prazo
                double volatility = comp.BaseVolatility;
                string news = "O mercado segue sem anúncios expressivos para a empresa.";

                if (i == targetEventCompIdx)
                {
                    if (isPositiveEvent)
                    {
                        trend = _rnd.NextDouble() * 0.08 + 0.06; // alta forte (+6% a +14%)
                        volatility = comp.BaseVolatility * 0.5; // menos oscilação errática, viés direcionado
                        news = GetRandomNews(comp.Ticker, true);

                        if (Player.Current != null)
                        {
                            if (comp.Ticker == "STB") Player.Current.TrendSTBDays = 5;
                            else if (comp.Ticker == "PEAR") Player.Current.TrendPEARDays = 5;
                            else if (comp.Ticker == "RVG") Player.Current.TrendRVGDays = 5;
                            else if (comp.Ticker == "GDR") Player.Current.TrendGDRDays = 5;
                            else if (comp.Ticker == "WHP") Player.Current.TrendWHPDays = 5;
                        }
                    }
                    else
                    {
                        trend = -(_rnd.NextDouble() * 0.06 + 0.04); // queda forte (-4% a -10%)
                        volatility = comp.BaseVolatility * 0.5;
                        news = GetRandomNews(comp.Ticker, false);
                    }
                }

                // Random Walk model (Geometric Brownian Motion approximation)
                double rand = (_rnd.NextDouble() * 2.0 - 1.0) * volatility;
                double changePercent = trend + rand;
                double prevPrice = comp.CurrentPrice;
                double newPrice = prevPrice * (1.0 + changePercent);

                // Garantir preço mínimo de $1.00 para não quebrar ou zerar
                if (newPrice < 1.0) newPrice = 1.0;

                comp.CurrentPrice = Math.Round(newPrice, 2);
                comp.ChangePercentage = Math.Round(changePercent * 100.0, 2);
                comp.DailyNews = news;

                // Adicionar ao histórico de 7 dias e podar excessos
                comp.PriceHistory.Add(comp.CurrentPrice);
                while (comp.PriceHistory.Count > 7)
                {
                    comp.PriceHistory.RemoveAt(0);
                }
            }

            SaveState();
        }

        private static string GetRandomNews(string ticker, bool positive)
        {
            switch (ticker)
            {
                case "STB":
                    return positive ? _positiveNewsSTB[_rnd.Next(0, _positiveNewsSTB.Length)] : _negativeNewsSTB[_rnd.Next(0, _negativeNewsSTB.Length)];
                case "PEAR":
                    return positive ? _positiveNewsPEAR[_rnd.Next(0, _positiveNewsPEAR.Length)] : _negativeNewsPEAR[_rnd.Next(0, _negativeNewsPEAR.Length)];
                case "RVG":
                    return positive ? _positiveNewsRVG[_rnd.Next(0, _positiveNewsRVG.Length)] : _negativeNewsRVG[_rnd.Next(0, _negativeNewsRVG.Length)];
                case "GDR":
                    return positive ? _positiveNewsGDR[_rnd.Next(0, _positiveNewsGDR.Length)] : _negativeNewsGDR[_rnd.Next(0, _negativeNewsGDR.Length)];
                case "WHP":
                    return positive ? _positiveNewsWHP[_rnd.Next(0, _positiveNewsWHP.Length)] : _negativeNewsWHP[_rnd.Next(0, _negativeNewsWHP.Length)];
                default:
                    return "Mercado de cotações ativo e estável.";
            }
        }

        public static void GetSponsorRequirements(string ticker, out int requiredShares, out int requiredSubs, out double signBonus)
        {
            switch (ticker)
            {
                case "STB":
                    requiredShares = 10;
                    requiredSubs = 1000;
                    signBonus = 150;
                    break;
                case "WHP":
                    requiredShares = 20;
                    requiredSubs = 5000;
                    signBonus = 350;
                    break;
                case "GDR":
                    requiredShares = 30;
                    requiredSubs = 10000;
                    signBonus = 750;
                    break;
                case "RVG":
                    requiredShares = 40;
                    requiredSubs = 25000;
                    signBonus = 1500;
                    break;
                case "PEAR":
                    requiredShares = 50;
                    requiredSubs = 50000;
                    signBonus = 3000;
                    break;
                default:
                    requiredShares = 0;
                    requiredSubs = 0;
                    signBonus = 0;
                    break;
            }
        }

        public static int GetPlayerShares(string ticker)
        {
            if (Player.Current == null) return 0;
            switch (ticker)
            {
                case "STB": return Player.Current.SharesSTB;
                case "PEAR": return Player.Current.SharesPEAR;
                case "RVG": return Player.Current.SharesRVG;
                case "GDR": return Player.Current.SharesGDR;
                case "WHP": return Player.Current.SharesWHP;
                default: return 0;
            }
        }

        public static int GetPlayerSubscribers()
        {
            if (Player.Current == null || Player.Current.Channels == null) return 0;
            int total = 0;
            foreach (var channel in Player.Current.Channels)
            {
                if (channel != Channel.UnreleasedVideos)
                {
                    total += channel.Subscribers;
                }
            }
            return total;
        }

        public static int GetSponsorRemainingDays(string ticker)
        {
            if (Player.Current == null) return 0;
            switch (ticker)
            {
                case "STB": return Player.Current.SponsorSTBDays;
                case "PEAR": return Player.Current.SponsorPEARDays;
                case "RVG": return Player.Current.SponsorRVGDays;
                case "GDR": return Player.Current.SponsorGDRDays;
                case "WHP": return Player.Current.SponsorWHPDays;
                default: return 0;
            }
        }

        public static void SetSponsorRemainingDays(string ticker, int days)
        {
            if (Player.Current == null) return;
            switch (ticker)
            {
                case "STB": Player.Current.SponsorSTBDays = days; break;
                case "PEAR": Player.Current.SponsorPEARDays = days; break;
                case "RVG": Player.Current.SponsorRVGDays = days; break;
                case "GDR": Player.Current.SponsorGDRDays = days; break;
                case "WHP": Player.Current.SponsorWHPDays = days; break;
            }
        }

        public static int GetTrendRemainingDays(string ticker)
        {
            if (Player.Current == null) return 0;
            switch (ticker)
            {
                case "STB": return Player.Current.TrendSTBDays;
                case "PEAR": return Player.Current.TrendPEARDays;
                case "RVG": return Player.Current.TrendRVGDays;
                case "GDR": return Player.Current.TrendGDRDays;
                case "WHP": return Player.Current.TrendWHPDays;
                default: return 0;
            }
        }
    }
}
