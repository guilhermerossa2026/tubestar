using System;
using System.Collections.Generic;

namespace TubeStar
{
    public static class AssetCatalog
    {
        public static List<RealEstateItem> RealEstate { get; private set; }
        public static List<VehicleItem> Vehicles { get; private set; }

        static AssetCatalog()
        {
            RealEstate = new List<RealEstateItem>()
            {
                // Apartamentos
                new RealEstateItem("ap_estudantil", "Apartamento Estudantil", "Apartamento", 500, 5, 0),
                new RealEstateItem("ap_conforto", "Apartamento Conforto", "Apartamento", 2000, 15, 0),
                new RealEstateItem("ap_vista_mar", "Apartamento Vista Mar", "Apartamento", 8000, 50, 0),
                new RealEstateItem("ap_duplex_moderno", "Duplex Moderno", "Apartamento", 25000, 120, 0),
                new RealEstateItem("ap_cobertura_exec", "Cobertura Executiva", "Apartamento", 75000, 300, 0),
                new RealEstateItem("ap_triplex_premium", "Triplex Premium", "Apartamento", 200000, 700, 0),

                // Casas
                new RealEstateItem("casa_de_campo", "Casa de Campo", "Casa", 5000, 25, 0),
                new RealEstateItem("casa_familiar", "Casa Familiar", "Casa", 15000, 75, 0),
                new RealEstateItem("casa_sobrado_col", "Sobrado Colonial", "Casa", 40000, 180, 0),
                new RealEstateItem("casa_inteligente", "Casa Inteligente", "Casa", 100000, 450, 0),
                new RealEstateItem("casa_mansao_campo", "Mansão de Campo", "Casa", 250000, 950, 0),
                new RealEstateItem("casa_suspensa", "Casa Suspensa", "Casa", 600000, 2000, 0),

                // Mansões
                new RealEstateItem("mansao_modernista", "Mansão Modernista", "Mansão", 150000, 600, 0),
                new RealEstateItem("mansao_hollywood", "Mansão Hollywood", "Mansão", 400000, 1500, 0),
                new RealEstateItem("mansao_beira_lago", "Mansão à Beira-Lago", "Mansão", 900000, 3200, 0),
                new RealEstateItem("mansao_futurista", "Mansão Futurista", "Mansão", 2000000, 7000, 0),
                new RealEstateItem("mansao_palacio_clas", "Palácio Clássico", "Mansão", 5000000, 15000, 0),
                new RealEstateItem("mansao_youtuber", "Megamansão do Youtuber", "Mansão", 12000000, 35000, 0),

                // Ilhas
                new RealEstateItem("ilha_coral", "Ilhota de Coral", "Ilha", 500000, 1800, 0),
                new RealEstateItem("ilha_palmeiras", "Ilha das Palmeiras", "Ilha", 1200000, 4000, 0),
                new RealEstateItem("ilha_tropical", "Ilha Tropical", "Ilha", 3000000, 9500, 0),
                new RealEstateItem("ilha_atol_youtuber", "Atol do Youtuber", "Ilha", 7500000, 22000, 0),
                new RealEstateItem("ilha_fortificada", "Ilha Fortificada", "Ilha", 18000000, 50000, 0),
                new RealEstateItem("ilha_suprema", "Ilha Privada Suprema", "Ilha", 45000000, 120000, 0),

                // Salas Comerciais
                new RealEstateItem("sala_comercial_comp", "Sala Comercial Compacta", "Sala Comercial", 10000, 80, 150),
                new RealEstateItem("sala_escritorio_exec", "Escritório Executivo", "Sala Comercial", 30000, 220, 450),
                new RealEstateItem("sala_andar_corp", "Andar Corporativo", "Sala Comercial", 120000, 800, 1800),
                new RealEstateItem("sala_predio_com", "Prédio Comercial", "Sala Comercial", 500000, 3000, 7500),
                new RealEstateItem("sala_shopping", "Shopping Center", "Sala Comercial", 2500000, 12000, 35000),
                new RealEstateItem("sala_distrito_fin", "Distrito Financeiro", "Sala Comercial", 10000000, 45000, 140000),
            };

            Vehicles = new List<VehicleItem>()
            {
                // Carros
                new VehicleItem("car_popular", "Carro Popular Usado", "Carro", 300, 2),
                new VehicleItem("car_sedan", "Sedan Confortável", "Carro", 1500, 8),
                new VehicleItem("car_suv", "SUV Familiar", "Carro", 5000, 20),
                new VehicleItem("car_eletrico", "Esportivo Elétrico", "Carro", 18000, 60),
                new VehicleItem("car_superesportivo", "Superesportivo", "Carro", 85000, 250),
                new VehicleItem("car_formula", "Carro de Fórmula Neon", "Carro", 350000, 1000),

                // Aviões
                new VehicleItem("av_ultraleve", "Ultraleve Hélice", "Avião", 15000, 80),
                new VehicleItem("av_monomotor", "Monomotor Privado", "Avião", 50000, 200),
                new VehicleItem("av_helicoptero", "Helicóptero Ágil", "Avião", 180000, 650),
                new VehicleItem("av_jato_exec", "Jato Executivo", "Avião", 700000, 2200),
                new VehicleItem("av_jato_com", "Jato Comercial Customizado", "Avião", 2500000, 7500),
                new VehicleItem("av_supersonico", "Jato Supersônico", "Avião", 8000000, 22000),

                // Lanchas
                new VehicleItem("lan_bote", "Bote Motorizado", "Lancha", 1000, 6),
                new VehicleItem("lan_star", "Lancha Star", "Lancha", 4000, 20),
                new VehicleItem("lan_cabine", "Lancha Cabine", "Lancha", 12000, 50),
                new VehicleItem("lan_catamara", "Catamarã de Lazer", "Lancha", 45000, 180),
                new VehicleItem("lan_iate_med", "Iate de Luxo Médio", "Lancha", 250000, 900),
                new VehicleItem("lan_megaiate", "Megaiate de Luxo", "Lancha", 1500000, 5000),

                // Jetskis
                new VehicleItem("js_entrada", "Jet Ski de Entrada", "Jetski", 800, 5),
                new VehicleItem("js_esportivo", "Jet Ski Esportivo", "Jetski", 2200, 12),
                new VehicleItem("js_turbo", "Jet Ski Turbo", "Jetski", 6000, 30),
                new VehicleItem("js_racing", "Jet Ski Pro Racing", "Jetski", 15000, 70),
                new VehicleItem("js_elite", "Jet Ski de Elite", "Jetski", 35000, 150),
                new VehicleItem("js_anfibio", "Jet Ski Anfíbio Concept", "Jetski", 90000, 400),
            };
        }
    }
}
