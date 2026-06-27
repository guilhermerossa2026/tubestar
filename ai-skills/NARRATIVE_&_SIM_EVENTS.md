# DESIGN NARRATIVO & EVENTOS DE SIMULAÇÃO

## MISSÃO
Criar eventos narrativos aleatórios, marcos de carreira e escolhas imersivas que quebrem a repetitividade do ciclo de jogo diário, gerando consequências reais a curto, médio e longo prazo na vida e reputação do jogador.

---

## ESTRUTURAÇÃO DE EVENTOS DE SIMULAÇÃO
Eventos no Tube Star devem forçar decisões táticas que impactam os recursos do `Player` (Dinheiro, Energia, Habilidades, Relações, Fisco).

### Modelo de Evento Base
Cada evento proposto deve seguir este esquema de escolhas:

* **Gatilho (Trigger)**: Ex: Alcançar 10.000 inscritos, ter mais de $5.000 sonegados (`UnpaidEvadedTaxes`), ou rodar no fim de cada turno com chance de 5%.
* **Situação**: Um texto imersivo que contextualiza o dilema (Ex: Uma grande marca de jogos suspeita oferece um patrocínio lucrativo, mas com reputação questionável).
* **Opções de Escolha**:
  1. **Opção Segura**: Pequeno bônus ou nenhuma mudança. Sem riscos.
  2. **Opção Ambiciosa (High Risk)**: Grande retorno financeiro, mas com risco de banimento de anúncios ou perda de inscritos fiéis.
  3. **Opção Criativa / Técnica**: Exige um nível específico em habilidades (ex: `PostProductionSkill >= 50`) para obter um desfecho excelente sem pontos negativos.

---

## EXEMPLOS DE EVENTOS PARA JOGOS CREATOR/TYCOON

### 1. Crise de Direitos Autorais (Copyright Strike)
- **Gatilho**: Vídeo postado usando músicas populares sem licença.
- **Opções**:
  * **Aceitar a Reclamação**: Doar 100% da receita deste vídeo para a gravadora. (Perda de receita).
  * **Disputar o Strike**: Chance de 50% de reaver a receita, 50% de chance de ter o vídeo excluído e o canal penalizado por 7 dias.
  * **Contratar Advogado de Mídia**: Custa $500, garante a resolução positiva do caso se o jogador tiver suporte corporativo.

### 2. A Oferta Indecente da Casa de Apostas (Shady Sponsor)
- **Gatilho**: Canal com mais de 50.000 inscritos.
- **Opções**:
  * **Aceitar**: Recebe $2.000 imediatamente, mas perde 5% de inscritos e o CPM do canal cai em 15% por 10 turnos (penalidade de marca).
  * **Recusar**: Ganha um bônus temporário de +10% de crescimento orgânico por lealdade do público.

### 3. A Visita da Receita Federal (Tax Audit)
- **Gatilho**: `UnpaidEvadedTaxes > $5,000` (Sonegação acumulada).
- **Opções**:
  * **Colaborar**: Pagar imediatamente a dívida com multa de 20%.
  * **Contratar Advogado Fiscal (`IsTaxAttorneyHired`)**: Reduz a multa pela metade, mas exige honorários fixos diários.
  * **Destruir Comprovantes (Obstrução)**: Chance de 20% de se safar completamente da dívida; 80% de chance de congelamento de ativos (bloqueio do saldo da conta por 5 turnos).

---

## CHECKLIST DE CRIAÇÃO DE EVENTOS

- [ ] **Gatilho Lógico**: O evento faz sentido com o estado atual do jogador (ex: não oferecer compra de escritórios para quem está trabalhando como distribuidor de panfletos)?
- [ ] **Variedade de Recursos**: As escolhas impactam algo além do saldo financeiro? (Energia, Seguidores no InstaFans, Performance no Trabalho)?
- [ ] **Feedback Narrativo**: O jogador recebe uma notificação clara explicando o resultado da sua escolha com tom jornalístico ou de simulação de rede social?
- [ ] **Decisões de Longo Prazo**: O evento pode desbloquear novos eventos no futuro (ex: aceitar um suborno de marca abre portas para chantagens futuras)?
