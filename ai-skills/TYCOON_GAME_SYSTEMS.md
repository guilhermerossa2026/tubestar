# TYCOON SYSTEMS DESIGNER

## MISSÃO
Projetar, balancear e refinar as mecânicas centrais de jogo (core loops), sistemas de upgrade e fórmulas matemáticas de pontuação/desempenho para criar uma experiência de simulação profunda, recompensadora e viciante.

---

## O LOOP CENTRAL DO TYCOON (CORE LOOP)
Qualquer nova mecânica no Tube Star deve encaixar-se no loop abaixo:
1. **Recurso/Entrada (Input)**: Jogador investe tempo (energia/turnos), dinheiro e pontos de atributo.
2. **Ação/Produção (Process)**: Gravação, edição e alocação de esforço em sliders (ex: focar mais em Edição do que em Roteiro).
3. **Avaliação/Feedback (Output)**: O algoritmo calcula a qualidade do vídeo e a reação do público.
4. **Recompensa (Reward)**: Ganho de inscritos, dinheiro e experiência (XP/Pontos).
5. **Reinvestimento (Upgrades)**: Compra de melhores câmeras, contratação de editores ou cursos de especialização para aumentar o potencial do próximo ciclo.

---

## DIRETRIZES DE DESIGN DE SISTEMAS

### 1. Sistema de Qualidade de Conteúdo (Rating Algorithm)
Inspirado por *Game Dev Tycoon* e *Youtubers Life*, a qualidade final de um vídeo ($Q_{total}$) não deve ser aleatória, mas calculada por fatores interligados:

$$Q_{total} = (\text{Habilidade do Jogador} \times W_{skill}) + (\text{Nível do Equipamento} \times W_{gear}) + (\text{Ajuste de Atributos} \times W_{sliders}) \pm \text{Bônus de Tendência}$$

- **Habilidade do Jogador**: `ShootingSkill` e `PostProductionSkill`.
- **Sliders (Atributos)**: Relação de trade-offs. Se o jogador coloca 100% de esforço em Humor, mas o tema do vídeo exige Roteiro, há penalidade.
- **Equipamentos**: Nivelam o teto máximo de qualidade que o jogador pode atingir.

### 2. Curvas de Custo e Progressão
Evite custos lineares. Utilize crescimento exponencial para upgrades e custos de manutenção para evitar que o jogador acumule dinheiro infinito sem esforço:

$$\text{Custo}(L) = \text{Custo Base} \times M^{L}$$

*Onde $L$ é o nível atual do upgrade e $M$ é o multiplicador de escala (geralmente entre $1.3$ e $1.8$).*

### 3. Decisões Significativas (Trade-offs)
Sistemas do tipo "sempre escolha a opção A" são ruins. Introduza dilemas:
- **Gravação Rápida**: Qualidade baixa, custo de energia baixo, risco de hate alto.
- **Gravação Cuidadosa**: Qualidade alta, consome 2 turnos, custo de energia alto.
- **Clickbait**: Dá muitos views a curto prazo, mas reduz a retenção e aumenta o churn (cancelamento) de inscritos a longo prazo.

---

## CHECKLIST DE CRIAÇÃO DE SISTEMAS

- [ ] **Alinhamento de Loop**: O sistema consome qual recurso e devolve qual tipo de poder ao jogador?
- [ ] **Fórmula de Escala**: A progressão foi testada matematicamente para o início, meio e fim de jogo (Late Game)?
- [ ] **Feedback Imediato**: O jogador sabe exatamente o motivo de ter tirado uma nota baixa ou alta?
- [ ] **Anti-Monotonia**: Existe variabilidade nas escolhas de slider para evitar que o jogador descubra uma "fórmula única e perfeita" e pare de pensar?
- [ ] **Sinergia**: O sistema interage com o Mercado de Ações, Impostos ou Redes Sociais do jogo?
