// Seed mock para smoke local da Transparência sem depender do Worker.
// Executado uma única vez quando o volume do mongo é criado.

const db = db.getSiblingDB("doacoes_db");

const idCampanhaA = UUID("11111111-1111-1111-1111-111111111111");
const idCampanhaB = UUID("22222222-2222-2222-2222-222222222222");
const agora = new Date();

db.painel_macro.drop();
db.painel_macro.insertOne({
    totalArrecadado: NumberDecimal("4250.00"),
    totalDoacoes: 12,
    totalCampanhasAtivas: 1,
    totalCampanhasConcluidas: 1,
    topDoadores: [
        { apelido: "Ana M.",     totalDoado: NumberDecimal("1200.00"), quantidadeDoacoes: 4 },
        { apelido: "Bruno S.",   totalDoado: NumberDecimal("950.00"),  quantidadeDoacoes: 3 },
        { apelido: "Carla R.",   totalDoado: NumberDecimal("700.00"),  quantidadeDoacoes: 2 }
    ],
    atualizadoEm: agora
});

db.lista_campanhas.drop();
db.lista_campanhas.insertMany([
    {
        idCampanha: idCampanhaA,
        titulo: "Campanha de Inverno 2026",
        metaFinanceira: NumberDecimal("5000.00"),
        valorArrecadado: NumberDecimal("3250.00"),
        status: "EmAndamento",
        dataInicio: new Date("2026-04-01T00:00:00Z"),
        dataFim:    new Date("2026-07-01T00:00:00Z"),
        dataEncerramento: null
    },
    {
        idCampanha: idCampanhaB,
        titulo: "Cestas Básicas - Páscoa",
        metaFinanceira: NumberDecimal("1000.00"),
        valorArrecadado: NumberDecimal("1000.00"),
        status: "Concluida",
        dataInicio: new Date("2026-03-01T00:00:00Z"),
        dataFim:    new Date("2026-04-01T00:00:00Z"),
        dataEncerramento: new Date("2026-04-01T00:00:00Z")
    }
]);

db.campanha_detalhe.drop();
db.campanha_detalhe.insertMany([
    {
        idCampanha: idCampanhaA,
        titulo: "Campanha de Inverno 2026",
        descricao: "Arrecadação de cobertores e agasalhos para população em situação de rua.",
        metaFinanceira: NumberDecimal("5000.00"),
        valorArrecadado: NumberDecimal("3250.00"),
        status: "EmAndamento",
        dataInicio: new Date("2026-04-01T00:00:00Z"),
        dataFim:    new Date("2026-07-01T00:00:00Z"),
        dataEncerramento: null,
        doacoes: [
            { apelidoDoador: "Ana M.",   valor: NumberDecimal("500.00"), data: new Date("2026-04-15T10:30:00Z") },
            { apelidoDoador: "Bruno S.", valor: NumberDecimal("250.00"), data: new Date("2026-04-18T16:00:00Z") },
            { apelidoDoador: "Carla R.", valor: NumberDecimal("200.00"), data: new Date("2026-04-22T09:15:00Z") }
        ]
    },
    {
        idCampanha: idCampanhaB,
        titulo: "Cestas Básicas - Páscoa",
        descricao: "Distribuição de cestas básicas para famílias acolhidas pela ONG.",
        metaFinanceira: NumberDecimal("1000.00"),
        valorArrecadado: NumberDecimal("1000.00"),
        status: "Concluida",
        dataInicio: new Date("2026-03-01T00:00:00Z"),
        dataFim:    new Date("2026-04-01T00:00:00Z"),
        dataEncerramento: new Date("2026-04-01T00:00:00Z"),
        doacoes: [
            { apelidoDoador: "Daniela P.", valor: NumberDecimal("500.00"), data: new Date("2026-03-15T12:00:00Z") },
            { apelidoDoador: "Eduardo L.", valor: NumberDecimal("500.00"), data: new Date("2026-03-25T15:00:00Z") }
        ]
    }
]);

print("✔ Seed de transparência aplicado em doacoes_db.");
