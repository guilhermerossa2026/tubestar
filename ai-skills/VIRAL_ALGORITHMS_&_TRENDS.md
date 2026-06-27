# ALGORITMOS DE VIRALIZAÇÃO E TENDÊNCIAS

## MISSÃO
Modelar as engrenagens ocultas da plataforma de vídeo virtual do jogo. Projetar como as tendências de conteúdo surgem, se espalham e desaparecem, bem como calcular o impacto de vídeos virais, taxas de clique (CTR) e comportamento de retenção de público.

---

## SISTEMA DE TENDÊNCIAS (TRENDS)
As tendências simulam a volatilidade da internet e forçam o jogador a mudar seus temas de vídeo com frequência.

### 1. Dinâmica e Decaimento de Hype
- Uma tendência (ex: Tecnologias STB, Humor PEAR, Games RVG) nasce aleatoriamente ou após eventos globais com um multiplicador de visibilidade inicial (ex: $2.5\times$).
- A cada turno/dia, a força da tendência decai usando um decaimento exponencial:
  $$\text{Multiplicador}_{t} = 1.0 + (\text{Multiplicador}_{t-1} - 1.0) \times (1.0 - R_{\text{decaimento}})$$
  *Onde $R_{\text{decaimento}}$ é a taxa de obsolescência (ex: $0.15$ ou 15% por dia).*
- Postar muitos vídeos do mesmo tema acelera a saturação do mercado, aumentando temporariamente o $R_{\text{decaimento}}$.

---

## FÓRMULA DE VIRALIZAÇÃO E CTR

### 1. Cálculo de Visualizações (Base vs. Viral)
A audiência total de um vídeo postado é a soma da audiência orgânica (inscritos atuais) e do tráfego viral (novos públicos recomendados pelo algoritmo):

$$\text{Views}_{\text{Total}} = (\text{Inscritos} \times \text{Taxa Engajamento} \times Q_{\text{Vídeo}}) + \text{Tráfego Viral}$$

### 2. O Algoritmo Viral (Viral Surge)
Para que um vídeo viralize (receba tráfego de fora de sua base), ele deve passar por uma checagem probabilística baseada em:
- **Alinhamento com Tendências**: Se o vídeo foi gravado em cima de uma tendência ativa (ex: `TrendSTBDays > 0`).
- **Pontuação de Qualidade**: Se a qualidade final supera o nível de habilidade média da concorrência.
- **Uso de Clickbait**: Aumenta o CTR em até 50%, mas impõe penalidades pesadas de retenção e pode irritar inscritos fiéis se a qualidade for baixa.

Se o gatilho viral disparar, o jogo aplica um multiplicador de tráfego de recomendação externa:

$$\text{Tráfego Viral} = \text{Inscritos Globais da Plataforma} \times (\text{Força Viral} \times \text{CTR}) \times \text{Fator Sorte}$$

---

## EVASÃO DE PÚBLICO (SUBSCRIBER CHURN)
Canais ruins perdem inscritos. O churn (taxa de cancelamento) deve ser calculado diariamente com base na satisfação do público:
- Se o jogador publica vídeos abaixo da expectativa de qualidade ou abusa de clickbaits, a lealdade cai.
- Se a Lealdade for baixa, a perda diária de inscritos é calculada por:
  $$\text{Perda de Inscritos} = \text{Inscritos Atuais} \times \text{Taxa Churn}$$

---

## CHECKLIST DO ALGORITMO

- [ ] **Volatilidade do Hype**: As tendências duram tempo suficiente para o jogador produzir um vídeo a tempo, mas não tanto a ponto de tornarem o jogo repetitivo?
- [ ] **Recompensa do Clickbait**: O clickbait apresenta um trade-off real de risco/recompensa em vez de ser uma ferramenta sempre lucrativa?
- [ ] **Feedback de Algoritmo**: O jogador consegue identificar se seu vídeo viralizou por causa de sorte, qualidade alta ou por seguir uma tendência?
- [ ] **Decaimento Orgânico**: O crescimento do canal desacelera se o jogador parar de postar vídeos por muitos turnos?
